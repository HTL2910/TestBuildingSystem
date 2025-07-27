using UnityEngine;
using MultiplayerSystem.Core;

namespace MultiplayerSystem.Core
{
    public class MultiplayerManagerSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public bool autoSetupOnStart = true;
        public GameObject playerPrefab;
        
        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupMultiplayerManager();
            }
        }
        
        public void SetupMultiplayerManager()
        {
            // Check if MultiplayerManager already exists
            if (MultiplayerManager.Instance != null)
            {
                Debug.Log("MultiplayerManager already exists!");
                return;
            }
            
            // Create MultiplayerManager GameObject
            GameObject managerGO = new GameObject("MultiplayerManager");
            MultiplayerManager manager = managerGO.AddComponent<MultiplayerManager>();
            
            // Set up default settings
            if (playerPrefab != null)
            {
                manager.playerPrefab = playerPrefab;
            }
            
            Debug.Log("MultiplayerManager setup complete!");
        }
        
        [ContextMenu("Setup MultiplayerManager")]
        public void SetupFromContextMenu()
        {
            SetupMultiplayerManager();
        }
    }
} 