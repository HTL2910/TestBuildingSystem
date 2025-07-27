using UnityEngine;
using MultiplayerSystem.Core;
using MultiplayerSystem.Player;
using MultiplayerSystem.Enemy;
using MultiplayerSystem.UI;

namespace MultiplayerSystem.Core
{
    public class MultiplayerTestSetup : MonoBehaviour
    {
        [Header("Test Setup")]
        public bool autoSetupOnStart = true;
        public bool createTestPrefabs = true;
        
        [Header("Test Prefabs")]
        public GameObject testPlayerPrefab;
        public GameObject testEnemyPrefab;
        
        [Header("Test UI")]
        public Canvas testCanvas;
        
        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupMultiplayerSystem();
            }
        }
        
        public void SetupMultiplayerSystem()
        {
            Debug.Log("=== Multiplayer System Test Setup ===");
            
            // 1. Setup MultiplayerManager
            SetupMultiplayerManager();
            
            // 2. Create test prefabs if needed
            if (createTestPrefabs)
            {
                CreateTestPrefabs();
            }
            
            // 3. Setup test UI
            SetupTestUI();
            
            Debug.Log("=== Multiplayer System Setup Complete ===");
        }
        
        void SetupMultiplayerManager()
        {
            if (MultiplayerManager.Instance == null)
            {
                GameObject managerGO = new GameObject("MultiplayerManager");
                MultiplayerManager manager = managerGO.AddComponent<MultiplayerManager>();
                
                // Set default settings
                manager.gameVersion = "1.0";
                manager.maxPlayersPerRoom = 4;
                manager.autoConnect = false; // Don't auto connect for testing
                manager.defaultRoomName = "TestRoom";
                
                if (testPlayerPrefab != null)
                {
                    manager.playerPrefab = testPlayerPrefab;
                }
                
                Debug.Log("✓ MultiplayerManager created successfully");
            }
            else
            {
                Debug.Log("✓ MultiplayerManager already exists");
            }
        }
        
        void CreateTestPrefabs()
        {
            // Create test player prefab
            if (testPlayerPrefab == null)
            {
                testPlayerPrefab = CreatePlayerPrefab();
                Debug.Log("✓ Test Player Prefab created");
            }
            
            // Create test enemy prefab
            if (testEnemyPrefab == null)
            {
                testEnemyPrefab = CreateEnemyPrefab();
                Debug.Log("✓ Test Enemy Prefab created");
            }
        }
        
        GameObject CreatePlayerPrefab()
        {
            GameObject playerGO = new GameObject("TestPlayer");
            
            // Add required components
            PlayerPrefabSetup playerSetup = playerGO.AddComponent<PlayerPrefabSetup>();
            playerSetup.setupOnAwake = false; // Don't setup automatically
            playerSetup.SetupPlayerPrefab();
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.transform.SetParent(playerGO.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(1f, 2f, 1f);
            
            // Remove the collider from visual (we have one on parent)
            DestroyImmediate(visual.GetComponent<Collider>());
            
            // Add material
            Renderer renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.blue;
                renderer.material = mat;
            }
            
            return playerGO;
        }
        
        GameObject CreateEnemyPrefab()
        {
            GameObject enemyGO = new GameObject("TestEnemy");
            
            // Add required components
            EnemyPrefabSetup enemySetup = enemyGO.AddComponent<EnemyPrefabSetup>();
            enemySetup.setupOnAwake = false; // Don't setup automatically
            enemySetup.SetupEnemyPrefab();
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.transform.SetParent(enemyGO.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(1f, 1f, 1f);
            
            // Remove the collider from visual (we have one on parent)
            DestroyImmediate(visual.GetComponent<Collider>());
            
            // Add material
            Renderer renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.red;
                renderer.material = mat;
            }
            
            return enemyGO;
        }
        
        void SetupTestUI()
        {
            if (testCanvas == null)
            {
                // Create test canvas
                GameObject canvasGO = new GameObject("TestCanvas");
                testCanvas = canvasGO.AddComponent<Canvas>();
                testCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                // Create test UI manager
                MultiplayerUIManager uiManager = canvasGO.AddComponent<MultiplayerUIManager>();
                
                // Create simple test UI
                CreateTestUIElements(canvasGO);
                
                Debug.Log("✓ Test UI Canvas created");
            }
        }
        
        void CreateTestUIElements(GameObject canvasGO)
        {
            // Create main menu panel
            GameObject mainMenuPanel = new GameObject("MainMenuPanel");
            mainMenuPanel.transform.SetParent(canvasGO.transform, false);
            
            // Add RectTransform
            RectTransform mainMenuRect = mainMenuPanel.AddComponent<RectTransform>();
            mainMenuRect.anchorMin = Vector2.zero;
            mainMenuRect.anchorMax = Vector2.one;
            mainMenuRect.offsetMin = Vector2.zero;
            mainMenuRect.offsetMax = Vector2.zero;
            
            // Add background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(mainMenuPanel.transform, false);
            
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            UnityEngine.UI.Image bgImage = background.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.8f);
            
            // Create connect button
            GameObject connectButtonGO = new GameObject("ConnectButton");
            connectButtonGO.transform.SetParent(mainMenuPanel.transform, false);
            
            RectTransform buttonRect = connectButtonGO.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.4f, 0.4f);
            buttonRect.anchorMax = new Vector2(0.6f, 0.6f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            UnityEngine.UI.Image buttonImage = connectButtonGO.AddComponent<UnityEngine.UI.Image>();
            buttonImage.color = Color.green;
            
            UnityEngine.UI.Button connectButton = connectButtonGO.AddComponent<UnityEngine.UI.Button>();
            
            // Add text to button
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(connectButtonGO.transform, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            UnityEngine.UI.Text buttonText = textGO.AddComponent<UnityEngine.UI.Text>();
            buttonText.text = "Connect to Server";
            // Use default font instead of trying to load specific font
            buttonText.fontSize = 24;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            
            // Assign to UI manager
            MultiplayerUIManager uiManager = canvasGO.GetComponent<MultiplayerUIManager>();
            if (uiManager != null)
            {
                uiManager.mainMenuPanel = mainMenuPanel;
                uiManager.connectButton = connectButton;
            }
        }
        
        [ContextMenu("Test Multiplayer Setup")]
        public void TestSetup()
        {
            SetupMultiplayerSystem();
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
        
        [ContextMenu("Create Room")]
        public void TestCreateRoom()
        {
            if (MultiplayerManager.Instance != null && Photon.Pun.PhotonNetwork.IsConnected)
            {
                Debug.Log("Creating test room...");
                MultiplayerSystem.Data.RoomSettings settings = new MultiplayerSystem.Data.RoomSettings("TestRoom", 4);
                MultiplayerManager.Instance.CreateRoom("TestRoom", settings);
            }
            else
            {
                Debug.LogError("Not connected to server! Connect first.");
            }
        }
    }
} 