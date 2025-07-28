using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiplayerSystem.Core;

public class MultiplayerSetup : MonoBehaviour
{
    [Header("Setup UI")]
    public GameObject setupUI;
    public Button setupButton;
    public TextMeshProUGUI statusText;
    
    [Header("References")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    
    void Start()
    {
        InitializeSetup();
    }
    
    void InitializeSetup()
    {
        if (setupUI != null)
        {
            setupUI.SetActive(true);
            if (setupButton != null)
            {
                setupButton.onClick.AddListener(SetupMultiplayer);
            }
        }
        
        UpdateStatus("Ready to setup multiplayer");
    }
    
    public void SetupMultiplayer()
    {
        UpdateStatus("Setting up multiplayer...");
        
        // 1. Create MultiplayerManager
        CreateMultiplayerManager();
        
        // 2. Setup Player Prefab
        SetupPlayerPrefab();
        
        // 3. Setup Items
        SetupItems();
        
        // 4. Setup AI
        SetupAI();
        
        UpdateStatus("Multiplayer setup complete! Click 'Connect' to start.");
        
        if (setupUI != null)
        {
            setupUI.SetActive(false);
        }
    }
    
    void CreateMultiplayerManager()
    {
        // Check if MultiplayerManager already exists
        MultiplayerManager existingManager = FindObjectOfType<MultiplayerManager>();
        if (existingManager == null)
        {
            // Create MultiplayerManager
            GameObject managerGO = new GameObject("MultiplayerManager");
            MultiplayerManager manager = managerGO.AddComponent<MultiplayerManager>();
            
            // Setup basic settings
            manager.autoConnect = true;
            manager.maxPlayersPerRoom = 4;
            manager.gameVersion = "1.0";
            manager.playerPrefab = playerPrefab;
            // Note: spawnPoints array is not used by MultiplayerManager, it uses spawnPosition instead
        }
        
        UpdateStatus("MultiplayerManager created");
    }
    
    void SetupPlayerPrefab()
    {
        if (playerPrefab == null)
        {
            UpdateStatus("No player prefab assigned!");
            return;
        }
        
        // Add PhotonView if not exists
        Photon.Pun.PhotonView photonView = playerPrefab.GetComponent<Photon.Pun.PhotonView>();
        if (photonView == null)
        {
            photonView = playerPrefab.AddComponent<Photon.Pun.PhotonView>();
            photonView.ViewID = 1;
        }
        
        UpdateStatus("Player prefab setup complete");
    }
    
    void SetupItems()
    {
        // Find all items in scene
        InteractableObject[] items = FindObjectsOfType<InteractableObject>();
        
        foreach (var item in items)
        {
            // Add PhotonView if not exists
            Photon.Pun.PhotonView photonView = item.GetComponent<Photon.Pun.PhotonView>();
            if (photonView == null)
            {
                photonView = item.gameObject.AddComponent<Photon.Pun.PhotonView>();
                photonView.ViewID = Random.Range(100, 1000); // Random ViewID for items
            }
        }
        
        UpdateStatus($"Setup {items.Length} items");
    }
    
    void SetupAI()
    {
        // Find all AI in scene
        AIMovement[] aiObjects = FindObjectsOfType<AIMovement>();
        
        foreach (var ai in aiObjects)
        {
            // Add PhotonView if not exists
            Photon.Pun.PhotonView photonView = ai.GetComponent<Photon.Pun.PhotonView>();
            if (photonView == null)
            {
                photonView = ai.gameObject.AddComponent<Photon.Pun.PhotonView>();
                photonView.ViewID = Random.Range(2000, 3000); // Random ViewID for AI
            }
        }
        
        UpdateStatus($"Setup {aiObjects.Length} AI objects");
    }
    
    void UpdateStatus(string message)
    {
        Debug.Log("[MultiplayerSetup] " + message);
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
    
    // Public methods
    public bool IsSetupComplete()
    {
        return FindObjectOfType<MultiplayerManager>() != null;
    }
    
    public void ResetSetup()
    {
        MultiplayerManager manager = FindObjectOfType<MultiplayerManager>();
        if (manager != null)
        {
            DestroyImmediate(manager.gameObject);
        }
        
        InitializeSetup();
    }
} 