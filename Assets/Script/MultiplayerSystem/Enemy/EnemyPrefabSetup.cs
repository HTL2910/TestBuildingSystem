using UnityEngine;
using Photon.Pun;
using MultiplayerSystem.Enemy;
using MultiplayerSystem.UI;

namespace MultiplayerSystem.Enemy
{
    public class EnemyPrefabSetup : MonoBehaviour
    {
        [Header("Enemy Prefab Setup")]
        public bool setupOnAwake = true;
        
        [Header("Required Components")]
        public PhotonView photonView;
        public NetworkEnemy networkEnemy;
        public Rigidbody rb;
        public Collider enemyCollider;
        public Animator animator;
        
        [Header("UI Components")]
        public GameObject enemyUI;
        public MultiplayerSystem.UI.HealthBar healthBar;
        
        void Awake()
        {
            if (setupOnAwake)
            {
                SetupEnemyPrefab();
            }
        }
        
        public void SetupEnemyPrefab()
        {
            // Get or add required components
            if (photonView == null) photonView = GetComponent<PhotonView>();
            if (photonView == null) photonView = gameObject.AddComponent<PhotonView>();
            
            if (networkEnemy == null) networkEnemy = GetComponent<NetworkEnemy>();
            if (networkEnemy == null) networkEnemy = gameObject.AddComponent<NetworkEnemy>();
            
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            
            if (enemyCollider == null) enemyCollider = GetComponent<Collider>();
            if (enemyCollider == null) enemyCollider = gameObject.AddComponent<CapsuleCollider>();
            
            if (animator == null) animator = GetComponent<Animator>();
            
            // Setup Rigidbody
            if (rb != null)
            {
                rb.mass = 1f;
                rb.linearDamping = 0f;
                rb.angularDamping = 0.05f;
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            
            // Setup Collider
            if (enemyCollider is CapsuleCollider capsuleCollider)
            {
                capsuleCollider.height = 2f;
                capsuleCollider.radius = 0.5f;
                capsuleCollider.center = new Vector3(0, 1f, 0);
            }
            
            // Setup PhotonView
            if (photonView != null)
            {
                photonView.ObservedComponents = new System.Collections.Generic.List<Component>();
                photonView.ObservedComponents.Add(networkEnemy);
                photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
            }
            
            // Setup NetworkEnemy references
            if (networkEnemy != null)
            {
                networkEnemy.photonView = photonView;
                networkEnemy.rb = rb;
                networkEnemy.enemyCollider = enemyCollider;
                networkEnemy.animator = animator;
                networkEnemy.enemyUI = enemyUI;
                networkEnemy.healthBar = healthBar;
            }
            
            Debug.Log("Enemy prefab setup complete!");
        }
        
        [ContextMenu("Setup Enemy Prefab")]
        public void SetupFromContextMenu()
        {
            SetupEnemyPrefab();
        }
    }
} 