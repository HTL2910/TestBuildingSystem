using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiplayerSystem.Core;
using MultiplayerSystem.Data;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject createRoomPanel;
    public GameObject roomListPanel;
    public GameObject connectingPanel;
    
    [Header("Main Panel")]
    public Button createRoomButton;
    public Button joinRandomButton;
    public Button refreshRoomsButton;
    public Button disconnectButton;
    public TextMeshProUGUI statusText;
    
    [Header("Create Room Panel")]
    public InputField roomNameInput;
    public Dropdown maxPlayersDropdown;
    public Button createButton;
    public Button cancelCreateButton;
    
    [Header("Room List Panel")]
    public Transform roomListContent;
    public GameObject roomListItemPrefab;
    public Button refreshButton;
    public Button backButton;
    
    [Header("Connecting Panel")]
    public TextMeshProUGUI connectingText;
    public Slider connectingProgress;
    
    [Header("Settings")]
    public float refreshInterval = 2f;
    
    // Private variables
    private List<RoomInfo> roomList = new List<RoomInfo>();
    private Coroutine refreshCoroutine;
    private bool isConnecting = false;
    
    void Start()
    {
        SetupUI();
        SubscribeToEvents();
        
        // Start auto-refresh
        StartAutoRefresh();
    }
    
    void SetupUI()
    {
        // Setup button listeners
        if (createRoomButton != null) createRoomButton.onClick.AddListener(ShowCreateRoomPanel);
        if (joinRandomButton != null) joinRandomButton.onClick.AddListener(JoinRandomRoom);
        if (refreshRoomsButton != null) refreshRoomsButton.onClick.AddListener(ShowRoomList);
        if (disconnectButton != null) disconnectButton.onClick.AddListener(Disconnect);
        
        if (createButton != null) createButton.onClick.AddListener(CreateRoom);
        if (cancelCreateButton != null) cancelCreateButton.onClick.AddListener(ShowMainPanel);
        
        if (refreshButton != null) refreshButton.onClick.AddListener(RefreshRoomList);
        if (backButton != null) backButton.onClick.AddListener(ShowMainPanel);
        
        // Setup dropdown
        if (maxPlayersDropdown != null)
        {
            maxPlayersDropdown.ClearOptions();
            maxPlayersDropdown.AddOptions(new List<string> { "2", "4", "6", "8", "10" });
            maxPlayersDropdown.value = 1; // Default to 4 players
        }
        
        // Show main panel initially
        ShowMainPanel();
    }
    
    void SubscribeToEvents()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.OnGameStateChanged += OnGameStateChanged;
            MultiplayerManager.Instance.OnPlayerJoined += OnPlayerJoined;
            MultiplayerManager.Instance.OnPlayerLeft += OnPlayerLeft;
            MultiplayerManager.Instance.OnRoomCreated += OnRoomCreated;
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
        }
        
        StopAutoRefresh();
    }
    
    #region UI Panel Management
    
    void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        roomListPanel.SetActive(false);
        connectingPanel.SetActive(false);
        
        UpdateStatus();
    }
    
    void ShowCreateRoomPanel()
    {
        mainPanel.SetActive(false);
        createRoomPanel.SetActive(true);
        roomListPanel.SetActive(false);
        connectingPanel.SetActive(false);
        
        // Set default room name
        if (roomNameInput != null)
        {
            roomNameInput.text = "Room_" + Random.Range(1000, 9999);
        }
    }
    
    void ShowRoomList()
    {
        mainPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomListPanel.SetActive(true);
        connectingPanel.SetActive(false);
        
        RefreshRoomList();
    }
    
    void ShowConnecting()
    {
        mainPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomListPanel.SetActive(false);
        connectingPanel.SetActive(true);
        
        isConnecting = true;
        StartCoroutine(ConnectingAnimation());
    }
    
    IEnumerator ConnectingAnimation()
    {
        string[] dots = { "", ".", "..", "..." };
        int dotIndex = 0;
        
        while (isConnecting)
        {
            if (connectingText != null)
            {
                connectingText.text = "Connecting to server" + dots[dotIndex];
            }
            
            if (connectingProgress != null)
            {
                connectingProgress.value = Mathf.PingPong(Time.time * 0.5f, 1f);
            }
            
            dotIndex = (dotIndex + 1) % dots.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    #endregion
    
    #region Room Management
    
    void CreateRoom()
    {
        if (MultiplayerManager.Instance == null) return;
        
        string roomName = roomNameInput != null ? roomNameInput.text : "GameRoom";
        int maxPlayers = maxPlayersDropdown != null ? int.Parse(maxPlayersDropdown.options[maxPlayersDropdown.value].text) : 4;
        
        if (string.IsNullOrEmpty(roomName))
        {
            UpdateStatus("Room name cannot be empty!");
            return;
        }
        
        RoomSettings settings = new RoomSettings(roomName, maxPlayers);
        MultiplayerManager.Instance.CreateRoom(roomName, settings);
        
        UpdateStatus("Creating room: " + roomName);
    }
    
    void JoinRandomRoom()
    {
        if (MultiplayerManager.Instance == null) return;
        
        MultiplayerManager.Instance.JoinRandomRoom();
        UpdateStatus("Joining random room...");
    }
    
    public void JoinRoom(string roomName)
    {
        if (MultiplayerManager.Instance == null) return;
        
        MultiplayerManager.Instance.JoinRoom(roomName);
        UpdateStatus("Joining room: " + roomName);
    }
    
    void RefreshRoomList()
    {
        if (MultiplayerManager.Instance == null) return;
        
        // Clear existing room list
        if (roomListContent != null)
        {
            foreach (Transform child in roomListContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        // Get room list from Photon
        if (Photon.Pun.PhotonNetwork.InLobby)
        {
            // Room list will be updated via callbacks
            UpdateStatus("Refreshing room list...");
        }
        else
        {
            UpdateStatus("Not in lobby. Connecting...");
            MultiplayerManager.Instance.ConnectToServer();
        }
    }
    
    void UpdateRoomListUI()
    {
        if (roomListContent == null || roomListItemPrefab == null) return;
        
        // Clear existing items
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }
        
        // Create room list items
        foreach (var room in roomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            RoomListItem item = roomItem.GetComponent<RoomListItem>();
            
            if (item != null)
            {
                item.SetupRoom(room, this);
            }
        }
        
        UpdateStatus($"Found {roomList.Count} rooms");
    }
    
    #endregion
    
    #region Auto Refresh
    
    void StartAutoRefresh()
    {
        if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
        }
        
        refreshCoroutine = StartCoroutine(AutoRefreshRooms());
    }
    
    void StopAutoRefresh()
    {
        if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
            refreshCoroutine = null;
        }
    }
    
    IEnumerator AutoRefreshRooms()
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshInterval);
            
            if (roomListPanel.activeInHierarchy && MultiplayerManager.Instance != null)
            {
                RefreshRoomList();
            }
        }
    }
    
    #endregion
    
    #region Event Callbacks
    
    void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Disconnected:
                isConnecting = false;
                ShowMainPanel();
                UpdateStatus("Disconnected from server");
                break;
                
            case GameState.Connecting:
                ShowConnecting();
                break;
                
            case GameState.Connected:
                isConnecting = false;
                ShowMainPanel();
                UpdateStatus("Connected to server");
                break;
                
            case GameState.InRoom:
                isConnecting = false;
                UpdateStatus("Joined room: " + MultiplayerManager.Instance.GetRoomName());
                // Load game scene after a short delay
                StartCoroutine(LoadGameSceneAfterDelay(2f));
                break;
        }
    }
    
    IEnumerator LoadGameSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        if (sceneManager != null)
        {
            sceneManager.LoadGameScene();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
    
    void OnPlayerJoined(Photon.Realtime.Player player)
    {
        UpdateStatus($"Player joined: {player.NickName}");
    }
    
    void OnPlayerLeft(Photon.Realtime.Player player)
    {
        UpdateStatus($"Player left: {player.NickName}");
    }
    
    void OnRoomCreated(string roomName)
    {
        UpdateStatus($"Room created: {roomName}");
    }
    
    #endregion
    
    #region Public Methods
    
    public void Disconnect()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.DisconnectFromServer();
        }
        
        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        if (sceneManager != null)
        {
            sceneManager.DisconnectAndReturnToMenu();
        }
    }
    
    void UpdateStatus(string message = "")
    {
        if (statusText == null) return;
        
        if (!string.IsNullOrEmpty(message))
        {
            statusText.text = message;
        }
        else
        {
            if (MultiplayerManager.Instance != null)
            {
                if (MultiplayerManager.Instance.IsConnected())
                {
                    statusText.text = "Connected to server";
                }
                else
                {
                    statusText.text = "Disconnected";
                }
            }
            else
            {
                statusText.text = "MultiplayerManager not found";
            }
        }
    }
    
    #endregion
} 