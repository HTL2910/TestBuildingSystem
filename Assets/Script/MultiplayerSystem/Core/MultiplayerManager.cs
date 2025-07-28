using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using MultiplayerSystem.Data;

namespace MultiplayerSystem.Core
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        [Header("Singleton")]
        public static MultiplayerManager Instance { get; private set; }
        
        [Header("Network Settings")]
        public string gameVersion = "1.0";
        public int maxPlayersPerRoom = 4;
        public bool autoConnect = true;
        
        [Header("Room Settings")]
        public string defaultRoomName = "GameRoom";
        public bool isRoomVisible = true;
        public bool isRoomOpen = true;
        
        [Header("Player Settings")]
        public GameObject playerPrefab;
        public Vector3 spawnPosition = Vector3.zero;
        
        [Header("Game State")]
        public GameState currentGameState = GameState.Disconnected;
        
        // Events
        public System.Action<GameState> OnGameStateChanged;
        public System.Action<Photon.Realtime.Player> OnPlayerJoined;
        public System.Action<Photon.Realtime.Player> OnPlayerLeft;
        public System.Action<string> OnRoomCreated;
        public System.Action OnGameStarted;
        public System.Action OnGameEnded;
        
        // Data
        private Dictionary<int, PlayerData> playerDataList = new Dictionary<int, PlayerData>();
        private RoomSettings currentRoomSettings;
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Initialize Photon settings
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        
        void Start()
        {
            if (autoConnect)
            {
                ConnectToServer();
            }
        }
        
        #region Connection Methods
        
        public void ConnectToServer()
        {
            if (!PhotonNetwork.IsConnected)
            {
                UpdateGameState(GameState.Connecting);
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        
        public void DisconnectFromServer()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }
        
        #endregion
        
        #region Room Methods
        
        public void CreateRoom(string roomName = null, RoomSettings settings = null)
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogWarning("Not connected to server!");
                return;
            }
            
            string roomNameToUse = roomName ?? defaultRoomName;
            currentRoomSettings = settings ?? new RoomSettings();
            
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = (byte)currentRoomSettings.maxPlayers,
                IsVisible = currentRoomSettings.isVisible,
                IsOpen = currentRoomSettings.isOpen,
                CustomRoomProperties = currentRoomSettings.customProperties,
                CustomRoomPropertiesForLobby = currentRoomSettings.propertiesForLobby
            };
            
            PhotonNetwork.CreateRoom(roomNameToUse, roomOptions);
        }
        
        public void JoinRoom(string roomName)
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogWarning("Not connected to server!");
                return;
            }
            
            PhotonNetwork.JoinRoom(roomName);
        }
        
        public void JoinRandomRoom()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogWarning("Not connected to server!");
                return;
            }
            
            PhotonNetwork.JoinRandomRoom();
        }
        
        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
        
        #endregion
        
        #region Player Methods
        
        public void SpawnPlayer()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && playerPrefab != null)
            {
                Vector3 spawnPos = GetSpawnPosition();
                PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
            }
        }
        
        public Vector3 GetSpawnPosition()
        {
            // Simple spawn position calculation
            Vector3 basePos = spawnPosition;
            basePos.x += Random.Range(-2f, 2f);
            basePos.z += Random.Range(-2f, 2f);
            return basePos;
        }
        
        public PlayerData GetPlayerData(int playerId)
        {
            if (playerDataList.ContainsKey(playerId))
            {
                return playerDataList[playerId];
            }
            return null;
        }
        
        public void UpdatePlayerData(int playerId, PlayerData data)
        {
            playerDataList[playerId] = data;
        }
        
        #endregion
        
        #region Game State Methods
        
        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Set room as not open (game in progress)
                PhotonNetwork.CurrentRoom.IsOpen = false;
                UpdateGameState(GameState.InGame);
                OnGameStarted?.Invoke();
            }
        }
        
        public void EndGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Set room as open (game ended)
                PhotonNetwork.CurrentRoom.IsOpen = true;
                UpdateGameState(GameState.InRoom);
                OnGameEnded?.Invoke();
            }
        }
        
        private void UpdateGameState(GameState newState)
        {
            currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
        
        #endregion
        
        #region Photon Callbacks
        
        public override void OnConnectedToMaster()
        {
            UpdateGameState(GameState.Connected);
            Debug.Log("Connected to Photon Master Server");
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            UpdateGameState(GameState.Disconnected);
            Debug.Log($"Disconnected from Photon: {cause}");
        }
        
        public override void OnJoinedRoom()
        {
            UpdateGameState(GameState.InRoom);
            OnRoomCreated?.Invoke(PhotonNetwork.CurrentRoom.Name);
            Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        }
        
        public override void OnLeftRoom()
        {
            UpdateGameState(GameState.Connected);
            playerDataList.Clear();
            Debug.Log("Left room");
        }
        
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            OnPlayerJoined?.Invoke(newPlayer);
            Debug.Log($"Player joined: {newPlayer.NickName}");
        }
        
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            OnPlayerLeft?.Invoke(otherPlayer);
            if (playerDataList.ContainsKey(otherPlayer.ActorNumber))
            {
                playerDataList.Remove(otherPlayer.ActorNumber);
            }
            Debug.Log($"Player left: {otherPlayer.NickName}");
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed to create room: {message}");
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed to join room: {message}");
        }
        
        #endregion
        
        #region Public Methods
        
        public int GetPlayerCount()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                return PhotonNetwork.CurrentRoom.PlayerCount;
            }
            return 0;
        }
        
        public string GetRoomName()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                return PhotonNetwork.CurrentRoom.Name;
            }
            return "No Room";
        }
        
        public bool IsConnected()
        {
            return PhotonNetwork.IsConnected;
        }
        
        public bool IsInRoom()
        {
            return PhotonNetwork.InRoom;
        }
        
        #endregion
    }
    
    public enum GameState
    {
        Disconnected,
        Connecting,
        Connected,
        InRoom,
        InGame
    }
} 