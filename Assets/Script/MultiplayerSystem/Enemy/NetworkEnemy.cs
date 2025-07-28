using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using MultiplayerSystem.Core;
using MultiplayerSystem.Data;
using MultiplayerSystem.Player;
using MultiplayerSystem.UI;
using ExitGames.Client.Photon;

namespace MultiplayerSystem.Enemy
{
    public class NetworkEnemy : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Enemy Components")]
        public PhotonView photonView;
        public Rigidbody rb;
        public Collider enemyCollider;
        public Animator animator;
        
        [Header("Enemy Data")]
        public string enemyName = "Enemy";
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float moveSpeed = 3f;
        public float attackRange = 2f;
        public float attackDamage = 20f;
        public float attackCooldown = 2f;
        public int scoreValue = 50;
        
        [Header("AI Settings")]
        public float detectionRange = 10f;
        public float chaseRange = 8f;
        public Transform target;
        public Vector3 spawnPosition;
        
        [Header("UI References")]
        public GameObject enemyUI;
        public MultiplayerSystem.UI.HealthBar healthBar;
        
        // Events
        public System.Action<NetworkEnemy> OnEnemyDeath;
        public System.Action<NetworkEnemy> OnEnemySpawn;
        public System.Action<float> OnHealthChanged;
        
        // Private variables
        private float lastAttackTime;
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private bool isDead = false;
        private bool isInitialized = false;
        
        // AI States
        private enum AIState { Idle, Patrol, Chase, Attack, Dead }
        private AIState currentState = AIState.Idle;
        
        void Awake()
        {
            // Get components
            if (photonView == null) photonView = GetComponent<PhotonView>();
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (enemyCollider == null) enemyCollider = GetComponent<Collider>();
            if (animator == null) animator = GetComponent<Animator>();
            
            spawnPosition = transform.position;
            
            // Initialize network variables
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
        
        void Start()
        {
            InitializeEnemy();
        }
        
        void Update()
        {
            if (isDead) return;
            
            if (photonView.IsMine)
            {
                UpdateAI();
            }
            else
            {
                // Smooth interpolation for other clients
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
                
                // Use Slerp for rotation to avoid Quaternion issues
                transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
            }
        }
        
        void InitializeEnemy()
        {
            currentHealth = maxHealth;
            
            // Set up UI
            if (enemyUI != null) enemyUI.SetActive(true);
            if (healthBar != null) healthBar.Initialize(maxHealth, currentHealth);
            
            // Set spawn position
            transform.position = spawnPosition;
            
            isInitialized = true;
            OnEnemySpawn?.Invoke(this);
        }
        
        void UpdateAI()
        {
            // Find nearest player
            FindNearestPlayer();
            
            switch (currentState)
            {
                case AIState.Idle:
                    HandleIdleState();
                    break;
                case AIState.Patrol:
                    HandlePatrolState();
                    break;
                case AIState.Chase:
                    HandleChaseState();
                    break;
                case AIState.Attack:
                    HandleAttackState();
                    break;
            }
            
            // Update animations
            UpdateAnimations();
        }
        
        void FindNearestPlayer()
        {
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
            float nearestDistance = float.MaxValue;
            NetworkPlayer nearestPlayer = null;
            
            foreach (var player in players)
            {
                if (player.IsAlive())
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlayer = player;
                    }
                }
            }
            
            target = nearestPlayer?.transform;
            
            // Update AI state based on distance
            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                
                if (distanceToTarget <= attackRange)
                {
                    currentState = AIState.Attack;
                }
                else if (distanceToTarget <= chaseRange)
                {
                    currentState = AIState.Chase;
                }
                else if (distanceToTarget <= detectionRange)
                {
                    currentState = AIState.Patrol;
                }
                else
                {
                    currentState = AIState.Idle;
                }
            }
            else
            {
                currentState = AIState.Idle;
            }
        }
        
        void HandleIdleState()
        {
            // Stay in place, maybe look around
            if (Random.Range(0f, 1f) < 0.01f) // 1% chance to start patrolling
            {
                currentState = AIState.Patrol;
            }
        }
        
        void HandlePatrolState()
        {
            // Move towards target slowly
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * moveSpeed * 0.5f * Time.deltaTime;
                transform.LookAt(target.position);
            }
        }
        
        void HandleChaseState()
        {
            // Chase target at full speed
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.LookAt(target.position);
            }
        }
        
        void HandleAttackState()
        {
            // Attack if cooldown is ready
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
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
            if (target != null)
            {
                NetworkPlayer targetPlayer = target.GetComponent<NetworkPlayer>();
                if (targetPlayer != null)
                {
                    targetPlayer.TakeDamage(attackDamage);
                }
            }
            
            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
        
        void UpdateAnimations()
        {
            if (animator != null)
            {
                animator.SetBool("IsMoving", currentState == AIState.Chase || currentState == AIState.Patrol);
                animator.SetBool("IsAttacking", currentState == AIState.Attack);
                animator.SetBool("IsDead", isDead);
            }
        }
        
        public void TakeDamage(float damage, Photon.Realtime.Player attacker = null)
        {
            if (isDead) return;
            
            // Update health
            currentHealth -= damage;
            
            // Update UI
            if (healthBar != null) healthBar.UpdateHealth(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
            
            // Check if dead
            if (currentHealth <= 0)
            {
                Die(attacker);
            }
            else
            {
                // Play damage animation
                if (animator != null)
                {
                    animator.SetTrigger("TakeDamage");
                }
            }
        }
        
        void Die(Photon.Realtime.Player attacker = null)
        {
            isDead = true;
            currentState = AIState.Dead;
            
            // Give score to attacker
            if (attacker != null)
            {
                NetworkPlayer attackerPlayer = FindPlayerByPhotonPlayer(attacker);
                if (attackerPlayer != null)
                {
                    attackerPlayer.AddScore(scoreValue);
                }
            }
            
            OnEnemyDeath?.Invoke(this);
            
            // Play death animation
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            // Disable collider
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }
            
            // Respawn after delay (if master client)
            if (photonView.IsMine)
            {
                Invoke(nameof(Respawn), 10f);
            }
        }
        
        void Respawn()
        {
            if (!photonView.IsMine) return;
            
            // Reset health and position
            currentHealth = maxHealth;
            transform.position = spawnPosition;
            isDead = false;
            currentState = AIState.Idle;
            
            // Re-enable collider
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }
            
            // Update UI
            if (healthBar != null) healthBar.UpdateHealth(currentHealth, maxHealth);
            
            OnEnemySpawn?.Invoke(this);
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
                stream.SendNext(currentHealth);
                stream.SendNext(isDead);
                stream.SendNext((int)currentState);
            }
            else
            {
                // Receive data
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
                currentHealth = (float)stream.ReceiveNext();
                isDead = (bool)stream.ReceiveNext();
                currentState = (AIState)stream.ReceiveNext();
                
                // Update UI for remote clients
                if (healthBar != null) healthBar.UpdateHealth(currentHealth, maxHealth);
            }
        }
        
        // Public methods for external access
        public bool IsDead()
        {
            return isDead;
        }
        
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0f;
        }
        
        public void SetSpawnPosition(Vector3 position)
        {
            spawnPosition = position;
        }
        
        public void SetEnemyName(string name)
        {
            enemyName = name;
        }
        
        public void SetMaxHealth(float health)
        {
            maxHealth = health;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }
        
        public void SetAttackDamage(float damage)
        {
            attackDamage = damage;
        }
        
        public void SetScoreValue(int score)
        {
            scoreValue = score;
        }
    }
} 