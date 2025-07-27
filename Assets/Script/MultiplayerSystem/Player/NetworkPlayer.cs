using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using MultiplayerSystem.Core;
using MultiplayerSystem.Data;
using MultiplayerSystem.UI;
using MultiplayerSystem.Enemy;
using ExitGames.Client.Photon;

namespace MultiplayerSystem.Player
{
    public class NetworkPlayer : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Player Components")]
        public PhotonView photonView;
        public Rigidbody rb;
        public Collider playerCollider;
        
        [Header("Player Data")]
        public PlayerData playerData;
        
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float jumpForce = 5f;
        public bool isGrounded = true;
        
        [Header("Combat")]
        public float attackRange = 2f;
        public float attackDamage = 25f;
        public float attackCooldown = 1f;
        private float lastAttackTime;
        
        [Header("UI References")]
        public GameObject playerUI;
        public MultiplayerSystem.UI.HealthBar healthBar;
        
        // Events
        public System.Action<NetworkPlayer> OnPlayerDeath;
        public System.Action<NetworkPlayer> OnPlayerRespawn;
        public System.Action<float> OnHealthChanged;
        public System.Action<int> OnScoreChanged;
        
        // Private variables
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private bool isInitialized = false;
        
        void Awake()
        {
            // Get components
            if (photonView == null) photonView = GetComponent<PhotonView>();
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (playerCollider == null) playerCollider = GetComponent<Collider>();
            
            // Initialize player data
            playerData = new PlayerData();
            
            // Initialize network variables
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
        
        void Start()
        {
            InitializePlayer();
        }
        
        void Update()
        {
            if (photonView.IsMine)
            {
                HandleInput();
            }
            else
            {
                // Smooth interpolation for other players
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
                
                // Use Slerp for rotation to avoid Quaternion issues
                transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
            }
        }
        
        void InitializePlayer()
        {
            if (photonView.IsMine)
            {
                // Local player setup
                playerData.playerName = PhotonNetwork.LocalPlayer.NickName;
                playerData.playerId = PhotonNetwork.LocalPlayer.ActorNumber;
                
                // Set up UI
                if (playerUI != null) playerUI.SetActive(true);
                if (healthBar != null) healthBar.Initialize(playerData);
                
                // Register with MultiplayerManager
                MultiplayerManager.Instance.UpdatePlayerData(playerData.playerId, playerData);
                
                // Set spawn position
                Vector3 spawnPos = MultiplayerManager.Instance.GetSpawnPosition();
                transform.position = spawnPos;
                
                // Update player data position
                playerData.position = spawnPos;
                playerData.rotation = transform.rotation;
            }
            else
            {
                // Remote player setup
                if (playerUI != null) playerUI.SetActive(false);
                if (rb != null) rb.isKinematic = true;
            }
            
            isInitialized = true;
        }
        
        void HandleInput()
        {
            // Movement
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(horizontal, 0, vertical).normalized * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            
            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                if (rb != null) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            
            // Attack
            if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
            
            // Update player data
            playerData.position = transform.position;
            playerData.rotation = transform.rotation;
        }
        
        void Attack()
        {
            lastAttackTime = Time.time;
            
            // Call RPC to sync attack
            photonView.RPC("AttackRPC", RpcTarget.All);
        }
        
        [PunRPC]
        void AttackRPC()
        {
            // Attack logic
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider != playerCollider)
                {
                    NetworkPlayer targetPlayer = hitCollider.GetComponent<NetworkPlayer>();
                    if (targetPlayer != null && targetPlayer != this)
                    {
                        // Check if friendly fire is allowed
                        if (MultiplayerManager.Instance.currentGameState == GameState.InGame)
                        {
                            targetPlayer.TakeDamage(attackDamage, photonView.Owner);
                        }
                    }
                    
                    NetworkEnemy targetEnemy = hitCollider.GetComponent<NetworkEnemy>();
                    if (targetEnemy != null)
                    {
                        targetEnemy.TakeDamage(attackDamage, photonView.Owner);
                    }
                }
            }
        }
        
        public void TakeDamage(float damage, Photon.Realtime.Player attacker = null)
        {
            if (!photonView.IsMine) return;
            
            // Update health
            playerData.TakeDamage(damage);
            
            // Update UI
            if (healthBar != null) healthBar.UpdateHealth(playerData.currentHealth, playerData.maxHealth);
            OnHealthChanged?.Invoke(playerData.currentHealth);
            
            // Check if dead
            if (playerData.currentHealth <= 0 && playerData.isAlive)
            {
                Die(attacker);
            }
            
            // Update MultiplayerManager
            MultiplayerManager.Instance.UpdatePlayerData(playerData.playerId, playerData);
        }
        
        void Die(Photon.Realtime.Player attacker = null)
        {
            playerData.isAlive = false;
            playerData.AddDeath();
            
            // Give score to attacker
            if (attacker != null)
            {
                NetworkPlayer attackerPlayer = FindPlayerByPhotonPlayer(attacker);
                if (attackerPlayer != null)
                {
                    attackerPlayer.playerData.AddKill();
                    MultiplayerManager.Instance.UpdatePlayerData(attackerPlayer.playerData.playerId, attackerPlayer.playerData);
                }
            }
            
            OnPlayerDeath?.Invoke(this);
            
            // Handle respawn
            if (MultiplayerManager.Instance.currentGameState == GameState.InGame)
            {
                Invoke(nameof(Respawn), 3f);
            }
        }
        
        public void Respawn()
        {
            if (!photonView.IsMine) return;
            
            // Reset health
            playerData.Respawn();
            
            // Set new spawn position
            Vector3 spawnPos = MultiplayerManager.Instance.GetSpawnPosition();
            transform.position = spawnPos;
            playerData.position = spawnPos;
            
            // Update UI
            if (healthBar != null) healthBar.UpdateHealth(playerData.currentHealth, playerData.maxHealth);
            
            OnPlayerRespawn?.Invoke(this);
            MultiplayerManager.Instance.UpdatePlayerData(playerData.playerId, playerData);
        }
        
        NetworkPlayer FindPlayerByPhotonPlayer(Photon.Realtime.Player photonPlayer)
        {
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
            foreach (var player in players)
            {
                if (player.photonView.Owner == photonPlayer)
                {
                    return player;
                }
            }
            return null;
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send data
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(playerData.currentHealth);
                stream.SendNext(playerData.isAlive);
            }
            else
            {
                // Receive data
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
                playerData.currentHealth = (float)stream.ReceiveNext();
                playerData.isAlive = (bool)stream.ReceiveNext();
                
                // Update UI for remote players
                if (healthBar != null) healthBar.UpdateHealth(playerData.currentHealth, playerData.maxHealth);
            }
        }
        
        // Public methods for external access
        public void SetPlayerName(string name)
        {
            playerData.playerName = name;
            PhotonNetwork.LocalPlayer.NickName = name;
        }
        
        public void SetTeam(string team)
        {
            playerData.team = team;
            MultiplayerManager.Instance.UpdatePlayerData(playerData.playerId, playerData);
        }
        
        public void AddScore(int points)
        {
            playerData.AddScore(points);
            OnScoreChanged?.Invoke(playerData.score);
            MultiplayerManager.Instance.UpdatePlayerData(playerData.playerId, playerData);
        }
        
        public bool IsLocalPlayer()
        {
            return photonView.IsMine;
        }
        
        public bool IsAlive()
        {
            return playerData.isAlive;
        }
        
        public float GetHealthPercentage()
        {
            return playerData.GetHealthPercentage();
        }
    }
} 