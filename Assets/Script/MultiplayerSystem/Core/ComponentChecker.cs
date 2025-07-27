using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace MultiplayerSystem.Core
{
    public class ComponentChecker : MonoBehaviour
    {
        [Header("Component Check")]
        public bool checkOnStart = true;
        public bool showDetailedInfo = true;
        
        void Start()
        {
            if (checkOnStart)
            {
                CheckAllComponents();
            }
        }
        
        public void CheckAllComponents()
        {
            Debug.Log("=== MULTIPLAYER SYSTEM COMPONENT CHECK ===");
            
            // Check Core components
            CheckComponent("MultiplayerManager", typeof(MultiplayerManager));
            CheckComponent("MultiplayerManagerSetup", typeof(MultiplayerManagerSetup));
            CheckComponent("MultiplayerGameManager", typeof(MultiplayerGameManager));
            CheckComponent("MultiplayerTestSetup", typeof(MultiplayerTestSetup));
            
            // Check Player components
            CheckComponent("NetworkPlayer", typeof(MultiplayerSystem.Player.NetworkPlayer));
            CheckComponent("PlayerPrefabSetup", typeof(MultiplayerSystem.Player.PlayerPrefabSetup));
            
            // Check Enemy components
            CheckComponent("NetworkEnemy", typeof(MultiplayerSystem.Enemy.NetworkEnemy));
            CheckComponent("EnemyPrefabSetup", typeof(MultiplayerSystem.Enemy.EnemyPrefabSetup));
            
            // Check Data components
            CheckComponent("PlayerData", typeof(MultiplayerSystem.Data.PlayerData));
            CheckComponent("RoomSettings", typeof(MultiplayerSystem.Data.RoomSettings));
            
            // Check UI components
            CheckComponent("HealthBar", typeof(MultiplayerSystem.UI.HealthBar));
            CheckComponent("MultiplayerUIManager", typeof(MultiplayerSystem.UI.MultiplayerUIManager));
            
            // Check Photon components
            CheckPhotonComponents();
            
            Debug.Log("=== COMPONENT CHECK COMPLETE ===");
        }
        
        void CheckComponent(string componentName, System.Type componentType)
        {
            try
            {
                if (componentType != null)
                {
                    Debug.Log($"✓ {componentName}: FOUND");
                    
                    if (showDetailedInfo)
                    {
                        ShowComponentInfo(componentType);
                    }
                }
                else
                {
                    Debug.LogError($"✗ {componentName}: NOT FOUND");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"✗ {componentName}: ERROR - {e.Message}");
            }
        }
        
        void ShowComponentInfo(System.Type componentType)
        {
            // Show methods
            MethodInfo[] methods = componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (methods.Length > 0)
            {
                Debug.Log($"  Methods: {methods.Length}");
                foreach (var method in methods)
                {
                    if (method.IsPublic && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_"))
                    {
                        Debug.Log($"    - {method.Name}");
                    }
                }
            }
            
            // Show properties
            PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (properties.Length > 0)
            {
                Debug.Log($"  Properties: {properties.Length}");
                foreach (var prop in properties)
                {
                    Debug.Log($"    - {prop.Name} ({prop.PropertyType.Name})");
                }
            }
        }
        
        void CheckPhotonComponents()
        {
            Debug.Log("=== PHOTON COMPONENTS ===");
            
            // Check if Photon is available
            System.Type photonNetworkType = System.Type.GetType("Photon.Pun.PhotonNetwork, Assembly-CSharp");
            if (photonNetworkType != null)
            {
                Debug.Log("✓ PhotonNetwork: FOUND");
                
                // Check PhotonView
                System.Type photonViewType = System.Type.GetType("Photon.Pun.PhotonView, Assembly-CSharp");
                if (photonViewType != null)
                {
                    Debug.Log("✓ PhotonView: FOUND");
                }
                else
                {
                    Debug.LogError("✗ PhotonView: NOT FOUND");
                }
                
                // Check MonoBehaviourPunCallbacks
                System.Type punCallbacksType = System.Type.GetType("Photon.Pun.MonoBehaviourPunCallbacks, Assembly-CSharp");
                if (punCallbacksType != null)
                {
                    Debug.Log("✓ MonoBehaviourPunCallbacks: FOUND");
                }
                else
                {
                    Debug.LogError("✗ MonoBehaviourPunCallbacks: NOT FOUND");
                }
            }
            else
            {
                Debug.LogError("✗ PhotonNetwork: NOT FOUND - Photon PUN 2 may not be installed!");
            }
        }
        
        [ContextMenu("Check Components")]
        public void CheckFromContextMenu()
        {
            CheckAllComponents();
        }
        
        [ContextMenu("Test Multiplayer Setup")]
        public void TestSetup()
        {
            MultiplayerTestSetup testSetup = GetComponent<MultiplayerTestSetup>();
            if (testSetup != null)
            {
                testSetup.SetupMultiplayerSystem();
            }
            else
            {
                Debug.LogError("MultiplayerTestSetup component not found! Add it first.");
            }
        }
        
        [ContextMenu("Test Connection")]
        public void TestConnection()
        {
            if (MultiplayerManager.Instance != null)
            {
                Debug.Log("Testing connection to Photon server...");
                MultiplayerManager.Instance.ConnectToServer();
            }
            else
            {
                Debug.LogError("MultiplayerManager not found! Run setup first.");
            }
        }
        
        [ContextMenu("Show All Components in Scene")]
        public void ShowAllComponentsInScene()
        {
            Debug.Log("=== ALL MULTIPLAYER COMPONENTS IN SCENE ===");
            
            // Find all MultiplayerManager instances
            MultiplayerManager[] managers = FindObjectsOfType<MultiplayerManager>();
            Debug.Log($"MultiplayerManager instances: {managers.Length}");
            
            // Find all NetworkPlayer instances
            MultiplayerSystem.Player.NetworkPlayer[] players = FindObjectsOfType<MultiplayerSystem.Player.NetworkPlayer>();
            Debug.Log($"NetworkPlayer instances: {players.Length}");
            
            // Find all NetworkEnemy instances
            MultiplayerSystem.Enemy.NetworkEnemy[] enemies = FindObjectsOfType<MultiplayerSystem.Enemy.NetworkEnemy>();
            Debug.Log($"NetworkEnemy instances: {enemies.Length}");
            
            // Find all PhotonView instances
            Photon.Pun.PhotonView[] photonViews = FindObjectsOfType<Photon.Pun.PhotonView>();
            Debug.Log($"PhotonView instances: {photonViews.Length}");
            
            // Find all UI managers
            MultiplayerSystem.UI.MultiplayerUIManager[] uiManagers = FindObjectsOfType<MultiplayerSystem.UI.MultiplayerUIManager>();
            Debug.Log($"MultiplayerUIManager instances: {uiManagers.Length}");
        }
    }
} 