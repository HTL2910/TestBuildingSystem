using UnityEngine;
using UnityEngine.UI;
using MultiplayerSystem.Core;
using MultiplayerSystem.Data;

namespace MultiplayerSystem.UI
{
    public class MultiplayerUIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject mainMenuPanel;
        public GameObject connectingPanel;
        public GameObject lobbyPanel;
        public GameObject gamePanel;
        public GameObject settingsPanel;
        
        [Header("Main Menu UI")]
        public Button connectButton;
        public Button settingsButton;
        public Button quitButton;
        
        [Header("Connecting UI")]
        public Text connectingText;
        public Slider connectingProgress;
        
        [Header("Lobby UI")]
        public Button createRoomButton;
        public Button joinRandomButton;
        public Button refreshRoomsButton;
        public Transform roomListContent;
        public GameObject roomListItemPrefab;
        public InputField roomNameInput;
        public Dropdown maxPlayersDropdown;
        
        [Header("Game UI")]
        public Text playerCountText;
        public Text roomNameText;
        public Button leaveRoomButton;
        public Button startGameButton;
        
        [Header("Player Info")]
        public Transform playerListContent;
        public GameObject playerListItemPrefab;
        
        void Start()
        {
            SetupUI();
            SubscribeToEvents();
        }
        
        void SetupUI()
        {
            // Setup button listeners
            if (connectButton != null) connectButton.onClick.AddListener(OnConnectClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
            
            if (createRoomButton != null) createRoomButton.onClick.AddListener(OnCreateRoomClicked);
            if (joinRandomButton != null) joinRandomButton.onClick.AddListener(OnJoinRandomClicked);
            if (refreshRoomsButton != null) refreshRoomsButton.onClick.AddListener(OnRefreshRoomsClicked);
            
            if (leaveRoomButton != null) leaveRoomButton.onClick.AddListener(OnLeaveRoomClicked);
            if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameClicked);
            
            // Setup dropdown
            if (maxPlayersDropdown != null)
            {
                maxPlayersDropdown.ClearOptions();
                maxPlayersDropdown.AddOptions(new System.Collections.Generic.List<string> { "2", "4", "8", "16" });
                maxPlayersDropdown.value = 1; // Default to 4 players
            }
            
            // Show main menu initially
            ShowMainMenu();
        }
        
        void SubscribeToEvents()
        {
            if (MultiplayerManager.Instance != null)
            {
                MultiplayerManager.Instance.OnGameStateChanged += OnGameStateChanged;
                MultiplayerManager.Instance.OnPlayerJoined += OnPlayerJoined;
                MultiplayerManager.Instance.OnPlayerLeft += OnPlayerLeft;
                MultiplayerManager.Instance.OnRoomCreated += OnRoomCreated;
                MultiplayerManager.Instance.OnGameStarted += OnGameStarted;
                MultiplayerManager.Instance.OnGameEnded += OnGameEnded;
            }
        }
        
        void OnDestroy()
        {
            if (MultiplayerManager.Instance != null)
            {
                MultiplayerManager.Instance.OnGameStateChanged -= OnGameStateChanged;
                MultiplayerManager.Instance.OnPlayerJoined -= OnPlayerJoined;
                MultiplayerManager.Instance.OnPlayerLeft -= OnPlayerLeft;
                MultiplayerManager.Instance.OnRoomCreated -= OnRoomCreated;
                MultiplayerManager.Instance.OnGameStarted -= OnGameStarted;
                MultiplayerManager.Instance.OnGameEnded -= OnGameEnded;
            }
        }
        
        #region UI Event Handlers
        
        void OnConnectClicked()
        {
            if (MultiplayerManager.Instance != null)
            {
                MultiplayerManager.Instance.ConnectToServer();
            }
        }
        
        void OnSettingsClicked()
        {
            ShowSettings();
        }
        
        void OnQuitClicked()
        {
            Application.Quit();
        }
        
        void OnCreateRoomClicked()
        {
            if (MultiplayerManager.Instance != null)
            {
                string roomName = roomNameInput != null ? roomNameInput.text : "GameRoom";
                int maxPlayers = maxPlayersDropdown != null ? int.Parse(maxPlayersDropdown.options[maxPlayersDropdown.value].text) : 4;
                
                RoomSettings settings = new RoomSettings(roomName, maxPlayers);
                MultiplayerManager.Instance.CreateRoom(roomName, settings);
            }
        }
        
        void OnJoinRandomClicked()
        {
            if (MultiplayerManager.Instance != null)
            {
                MultiplayerManager.Instance.JoinRandomRoom();
            }
        }
        
        void OnRefreshRoomsClicked()
        {
            // TODO: Implement room list refresh
            Debug.Log("Refresh rooms clicked");
        }
        
        void OnLeaveRoomClicked()
        {
            if (MultiplayerManager.Instance != null)
            {
                MultiplayerManager.Instance.LeaveRoom();
            }
        }
        
        void OnStartGameClicked()
        {
            if (MultiplayerManager.Instance != null)
            {
                MultiplayerManager.Instance.StartGame();
            }
        }
        
        #endregion
        
        #region Event Callbacks
        
        void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Disconnected:
                    ShowMainMenu();
                    break;
                case GameState.Connecting:
                    ShowConnecting();
                    break;
                case GameState.Connected:
                    ShowLobby();
                    break;
                case GameState.InRoom:
                    ShowGame();
                    break;
                case GameState.InGame:
                    ShowGame();
                    break;
            }
        }
        
        void OnPlayerJoined(Photon.Realtime.Player player)
        {
            UpdatePlayerList();
            UpdatePlayerCount();
        }
        
        void OnPlayerLeft(Photon.Realtime.Player player)
        {
            UpdatePlayerList();
            UpdatePlayerCount();
        }
        
        void OnRoomCreated(string roomName)
        {
            if (roomNameText != null)
            {
                roomNameText.text = $"Room: {roomName}";
            }
        }
        
        void OnGameStarted()
        {
            if (startGameButton != null)
            {
                startGameButton.interactable = false;
            }
        }
        
        void OnGameEnded()
        {
            if (startGameButton != null)
            {
                startGameButton.interactable = true;
            }
        }
        
        #endregion
        
        #region UI Panel Management
        
        void ShowMainMenu()
        {
            SetActivePanel(mainMenuPanel);
        }
        
        void ShowConnecting()
        {
            SetActivePanel(connectingPanel);
            if (connectingText != null)
            {
                connectingText.text = "Connecting to server...";
            }
        }
        
        void ShowLobby()
        {
            SetActivePanel(lobbyPanel);
        }
        
        void ShowGame()
        {
            SetActivePanel(gamePanel);
            UpdatePlayerCount();
            UpdateRoomInfo();
        }
        
        void ShowSettings()
        {
            SetActivePanel(settingsPanel);
        }
        
        void SetActivePanel(GameObject activePanel)
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(mainMenuPanel == activePanel);
            if (connectingPanel != null) connectingPanel.SetActive(connectingPanel == activePanel);
            if (lobbyPanel != null) lobbyPanel.SetActive(lobbyPanel == activePanel);
            if (gamePanel != null) gamePanel.SetActive(gamePanel == activePanel);
            if (settingsPanel != null) settingsPanel.SetActive(settingsPanel == activePanel);
        }
        
        #endregion
        
        #region UI Updates
        
        void UpdatePlayerCount()
        {
            if (playerCountText != null && Photon.Pun.PhotonNetwork.InRoom)
            {
                playerCountText.text = $"Players: {Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount}/{Photon.Pun.PhotonNetwork.CurrentRoom.MaxPlayers}";
            }
        }
        
        void UpdateRoomInfo()
        {
            if (roomNameText != null && Photon.Pun.PhotonNetwork.InRoom)
            {
                roomNameText.text = $"Room: {Photon.Pun.PhotonNetwork.CurrentRoom.Name}";
            }
        }
        
        void UpdatePlayerList()
        {
            if (playerListContent == null || playerListItemPrefab == null) return;
            
            // Clear existing list
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }
            
            // Add current players
            if (Photon.Pun.PhotonNetwork.InRoom)
            {
                foreach (var player in Photon.Pun.PhotonNetwork.PlayerList)
                {
                    GameObject listItem = Instantiate(playerListItemPrefab, playerListContent);
                    Text playerText = listItem.GetComponentInChildren<Text>();
                    if (playerText != null)
                    {
                        playerText.text = player.NickName;
                        if (player.IsMasterClient)
                        {
                            playerText.text += " (Host)";
                        }
                    }
                }
            }
        }
        
        #endregion
    }
} 