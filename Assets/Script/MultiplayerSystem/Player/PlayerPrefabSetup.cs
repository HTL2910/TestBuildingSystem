using UnityEngine;
using Photon.Pun;
using MultiplayerSystem.Player;
using MultiplayerSystem.UI;

namespace MultiplayerSystem.Player
{
    public class PlayerPrefabSetup : MonoBehaviour
    {
        [Header("Player Prefab Setup")]
        public bool setupOnAwake = true;
        
        [Header("Required Components")]
        public PhotonView photonView;
        public NetworkPlayer networkPlayer;
        public Rigidbody rb;
        public Collider playerCollider;
        
        [Header("UI Components")]
        public GameObject playerUI;
        public MultiplayerSystem.UI.HealthBar healthBar;
        
        void Awake()
        {
            if (setupOnAwake)
            {
                SetupPlayerPrefab();
            }
        }
        
        public void SetupPlayerPrefab()
        {
            // Get or add required components
            if (photonView == null) photonView = GetComponent<PhotonView>();
            if (photonView == null) photonView = gameObject.AddComponent<PhotonView>();
            
            if (networkPlayer == null) networkPlayer = GetComponent<NetworkPlayer>();
            if (networkPlayer == null) networkPlayer = gameObject.AddComponent<NetworkPlayer>();
            
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            
            if (playerCollider == null) playerCollider = GetComponent<Collider>();
            if (playerCollider == null) playerCollider = gameObject.AddComponent<CapsuleCollider>();
            
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
            if (playerCollider is CapsuleCollider capsuleCollider)
            {
                capsuleCollider.height = 2f;
                capsuleCollider.radius = 0.5f;
                capsuleCollider.center = new Vector3(0, 1f, 0);
            }
            
            // Setup PhotonView
            if (photonView != null)
            {
                photonView.ObservedComponents = new System.Collections.Generic.List<Component>();
                photonView.ObservedComponents.Add(networkPlayer);
                photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
            }
            
            // Setup NetworkPlayer references
            if (networkPlayer != null)
            {
                networkPlayer.photonView = photonView;
                networkPlayer.rb = rb;
                networkPlayer.playerCollider = playerCollider;
                networkPlayer.playerUI = playerUI;
                networkPlayer.healthBar = healthBar;
            }
            
            Debug.Log("Player prefab setup complete!");
        }
        
        [ContextMenu("Setup Player Prefab")]
        public void SetupFromContextMenu()
        {
            SetupPlayerPrefab();
        }
    }
} 