using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MultiplayerSystem.Core;
using MultiplayerSystem.Player;
using MultiplayerSystem.Enemy;
using System.Collections.Generic;

namespace MultiplayerSystem.Core
{
    public class MultiplayerGameManager : MonoBehaviourPunCallbacks
    {
        [Header("Game Settings")]
        public float gameStartDelay = 3f;
        public float respawnDelay = 3f;
        public int maxEnemies = 10;
        public float enemySpawnInterval = 5f;
        
        [Header("Spawn Points")]
        public Transform[] playerSpawnPoints;
        public Transform[] enemySpawnPoints;
        
        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject enemyPrefab;
        
        [Header("Game State")]
        public bool gameStarted = false;
        public bool gameEnded = false;
        public float gameTime = 0f;
        public int maxGameTime = 300; // 5 minutes
        
        // Events
        public System.Action OnGameStart;
        public System.Action OnGameEnd;
        public System.Action<float> OnGameTimeChanged;
        
        // Private variables
        private float lastEnemySpawnTime;
        private List<NetworkEnemy> activeEnemies = new List<NetworkEnemy>();
        private Dictionary<int, NetworkPlayer> activePlayers = new Dictionary<int, NetworkPlayer>();
        
        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Master client starts the game after delay
                Invoke(nameof(StartGame), gameStartDelay);
            }
        }
        
        void Update()
        {
            if (gameStarted && !gameEnded)
            {
                UpdateGameTime();
                
                if (PhotonNetwork.IsMasterClient)
                {
                    UpdateEnemySpawning();
                }
            }
        }
        
        void UpdateGameTime()
        {
            gameTime += Time.deltaTime;
            OnGameTimeChanged?.Invoke(gameTime);
            
            // Check for game end
            if (gameTime >= maxGameTime)
            {
                EndGame();
            }
        }
        
        void UpdateEnemySpawning()
        {
            if (Time.time >= lastEnemySpawnTime + enemySpawnInterval && activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
                lastEnemySpawnTime = Time.time;
            }
        }
        
        public void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            photonView.RPC("StartGameRPC", RpcTarget.All);
        }
        
        [PunRPC]
        void StartGameRPC()
        {
            gameStarted = true;
            gameTime = 0f;
            OnGameStart?.Invoke();
            
            Debug.Log("Game started!");
        }
        
        public void EndGame()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            photonView.RPC("EndGameRPC", RpcTarget.All);
        }
        
        [PunRPC]
        void EndGameRPC()
        {
            gameEnded = true;
            OnGameEnd?.Invoke();
            
            Debug.Log("Game ended!");
        }
        
        public void SpawnPlayer(Photon.Realtime.Player player)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            Vector3 spawnPos = GetRandomPlayerSpawnPoint();
            GameObject playerGO = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
            
            NetworkPlayer networkPlayer = playerGO.GetComponent<NetworkPlayer>();
            if (networkPlayer != null)
            {
                activePlayers[player.ActorNumber] = networkPlayer;
            }
        }
        
        void SpawnEnemy()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            Vector3 spawnPos = GetRandomEnemySpawnPoint();
            GameObject enemyGO = PhotonNetwork.Instantiate(enemyPrefab.name, spawnPos, Quaternion.identity);
            
            NetworkEnemy networkEnemy = enemyGO.GetComponent<NetworkEnemy>();
            if (networkEnemy != null)
            {
                activeEnemies.Add(networkEnemy);
                networkEnemy.OnEnemyDeath += OnEnemyDeath;
            }
        }
        
        void OnEnemyDeath(NetworkEnemy enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }
        }
        
        Vector3 GetRandomPlayerSpawnPoint()
        {
            if (playerSpawnPoints != null && playerSpawnPoints.Length > 0)
            {
                return playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)].position;
            }
            return Vector3.zero;
        }
        
        Vector3 GetRandomEnemySpawnPoint()
        {
            if (enemySpawnPoints != null && enemySpawnPoints.Length > 0)
            {
                return enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)].position;
            }
            return Vector3.zero;
        }
        
        public void RespawnPlayer(NetworkPlayer player)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            Vector3 spawnPos = GetRandomPlayerSpawnPoint();
            player.transform.position = spawnPos;
            player.Respawn();
        }
        
        #region Photon Callbacks
        
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if (gameStarted && PhotonNetwork.IsMasterClient)
            {
                // Spawn player for new player
                SpawnPlayer(newPlayer);
            }
        }
        
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            if (activePlayers.ContainsKey(otherPlayer.ActorNumber))
            {
                activePlayers.Remove(otherPlayer.ActorNumber);
            }
        }
        
        #endregion
        
        #region Public Methods
        
        public bool IsGameStarted()
        {
            return gameStarted;
        }
        
        public bool IsGameEnded()
        {
            return gameEnded;
        }
        
        public float GetGameTime()
        {
            return gameTime;
        }
        
        public float GetGameTimeRemaining()
        {
            return Mathf.Max(0, maxGameTime - gameTime);
        }
        
        public int GetActivePlayerCount()
        {
            return activePlayers.Count;
        }
        
        public int GetActiveEnemyCount()
        {
            return activeEnemies.Count;
        }
        
        #endregion
    }
} 