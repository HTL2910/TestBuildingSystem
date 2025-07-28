using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplayerSetupGuide : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(10, 20)]
    public string setupInstructions = @"
🎮 MULTIPLAYER SETUP GUIDE 🎮

📋 STEP 1: SCENE SETUP
=====================
1. Create 3 scenes: Main, Lobby, Game
2. Add scenes to Build Settings
3. Set Main as first scene

📋 STEP 2: MAIN SCENE SETUP
==========================
1. Create Canvas (UI > Canvas)
2. Add SceneManager component to empty GameObject
3. Create Loading UI:
   - Loading Panel (Panel)
   - Loading Progress Bar (Slider)
   - Loading Text (TextMeshPro)

📋 STEP 3: LOBBY SCENE SETUP
============================
1. Create Canvas with LobbyManager component
2. Create UI Panels:
   - Main Panel (Create Room, Join Random, Room List, Disconnect)
   - Create Room Panel (Room Name Input, Max Players Dropdown)
   - Room List Panel (ScrollView with RoomListItem prefabs)
   - Connecting Panel (Loading animation)

3. Create RoomListItem Prefab:
   - Background Image
   - Room Name Text
   - Player Count Text
   - Join Button
   - Add RoomListItem component

📋 STEP 4: GAME SCENE SETUP
===========================
1. Create Canvas with GameManager component
2. Create UI Panels:
   - Game UI Panel (Room info, game time, pause button)
   - Pause Panel (Resume, Leave Room, Quit)
   - Game Over Panel

📋 STEP 5: PLAYER PREFAB SETUP
==============================
1. Create Player GameObject
2. Add components:
   - PhotonView
   - NetworkPlayer (from MultiplayerSystem)
   - Rigidbody
   - Collider
3. Save as Prefab in Resources folder

📋 STEP 6: TESTING
==================
1. Build 2 instances
2. Run both
3. Test connection, room creation, joining
4. Test gameplay

🔧 TROUBLESHOOTING
=================
- Check Photon AppID in PhotonServerSettings
- Ensure all scenes are in Build Settings
- Verify prefabs are in Resources folder
- Check console for error messages
";

    [Header("UI Templates")]
    public GameObject mainMenuTemplate;
    public GameObject lobbyTemplate;
    public GameObject gameTemplate;
    
    [Header("Auto Setup")]
    public bool autoCreateTemplates = false;
    
    void Start()
    {
        if (autoCreateTemplates)
        {
            CreateUITemplates();
        }
    }
    
    void CreateUITemplates()
    {
        Debug.Log("Creating UI templates...");
        
        // Create Main Menu Template
        CreateMainMenuTemplate();
        
        // Create Lobby Template
        CreateLobbyTemplate();
        
        // Create Game Template
        CreateGameTemplate();
        
        Debug.Log("UI templates created! Check the scene for new GameObjects.");
    }
    
    void CreateMainMenuTemplate()
    {
        // Create Canvas
        GameObject canvas = new GameObject("MainMenuCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // Create Loading Panel
        GameObject loadingPanel = CreatePanel(canvas, "LoadingPanel");
        loadingPanel.SetActive(false);
        
        // Create Loading Progress Bar
        GameObject progressBar = CreateSlider(loadingPanel, "LoadingProgressBar");
        
        // Create Loading Text
        GameObject loadingText = CreateText(loadingPanel, "LoadingText", "Loading...");
        
        // Add SceneManager
        GameObject sceneManager = new GameObject("SceneManager");
        SceneManager sceneManagerComponent = sceneManager.AddComponent<SceneManager>();
        sceneManagerComponent.loadingPanel = loadingPanel;
        sceneManagerComponent.loadingProgressBar = progressBar.GetComponent<Slider>();
        sceneManagerComponent.loadingText = loadingText.GetComponent<TextMeshProUGUI>();
        
        Debug.Log("Main Menu template created!");
    }
    
    void CreateLobbyTemplate()
    {
        // Create Canvas
        GameObject canvas = new GameObject("LobbyCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // Create Main Panel
        GameObject mainPanel = CreatePanel(canvas, "MainPanel");
        
        // Create buttons
        CreateButton(mainPanel, "CreateRoomButton", "Create Room");
        CreateButton(mainPanel, "JoinRandomButton", "Join Random");
        CreateButton(mainPanel, "RoomListButton", "Room List");
        CreateButton(mainPanel, "DisconnectButton", "Disconnect");
        
        // Create status text
        CreateText(mainPanel, "StatusText", "Ready to connect");
        
        // Create Create Room Panel
        GameObject createPanel = CreatePanel(canvas, "CreateRoomPanel");
        createPanel.SetActive(false);
        
        CreateInputField(createPanel, "RoomNameInput", "Room Name");
        CreateDropdown(createPanel, "MaxPlayersDropdown");
        CreateButton(createPanel, "CreateButton", "Create");
        CreateButton(createPanel, "CancelButton", "Cancel");
        
        // Create Room List Panel
        GameObject roomListPanel = CreatePanel(canvas, "RoomListPanel");
        roomListPanel.SetActive(false);
        
        GameObject scrollView = CreateScrollView(roomListPanel, "RoomListScrollView");
        CreateButton(roomListPanel, "RefreshButton", "Refresh");
        CreateButton(roomListPanel, "BackButton", "Back");
        
        // Create Connecting Panel
        GameObject connectingPanel = CreatePanel(canvas, "ConnectingPanel");
        connectingPanel.SetActive(false);
        
        CreateText(connectingPanel, "ConnectingText", "Connecting...");
        CreateSlider(connectingPanel, "ConnectingProgress");
        
        // Add LobbyManager
        GameObject lobbyManager = new GameObject("LobbyManager");
        LobbyManager lobbyManagerComponent = lobbyManager.AddComponent<LobbyManager>();
        
        // Assign references
        lobbyManagerComponent.mainPanel = mainPanel;
        lobbyManagerComponent.createRoomPanel = createPanel;
        lobbyManagerComponent.roomListPanel = roomListPanel;
        lobbyManagerComponent.connectingPanel = connectingPanel;
        
        Debug.Log("Lobby template created!");
    }
    
    void CreateGameTemplate()
    {
        // Create Canvas
        GameObject canvas = new GameObject("GameCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // Create Game UI Panel
        GameObject gamePanel = CreatePanel(canvas, "GameUIPanel");
        
        CreateText(gamePanel, "RoomNameText", "Room: None");
        CreateText(gamePanel, "PlayerCountText", "Players: 0");
        CreateText(gamePanel, "GameTimeText", "00:00");
        CreateText(gamePanel, "StatusText", "In Game");
        CreateButton(gamePanel, "PauseButton", "Pause");
        
        // Create Pause Panel
        GameObject pausePanel = CreatePanel(canvas, "PausePanel");
        pausePanel.SetActive(false);
        
        CreateButton(pausePanel, "ResumeButton", "Resume");
        CreateButton(pausePanel, "LeaveRoomButton", "Leave Room");
        CreateButton(pausePanel, "QuitButton", "Quit Game");
        
        // Create Game Over Panel
        GameObject gameOverPanel = CreatePanel(canvas, "GameOverPanel");
        gameOverPanel.SetActive(false);
        
        CreateText(gameOverPanel, "GameOverText", "Game Over!");
        
        // Add GameManager
        GameObject gameManager = new GameObject("GameManager");
        GameManager gameManagerComponent = gameManager.AddComponent<GameManager>();
        
        // Assign references
        gameManagerComponent.gameUIPanel = gamePanel;
        gameManagerComponent.pausePanel = pausePanel;
        gameManagerComponent.gameOverPanel = gameOverPanel;
        
        Debug.Log("Game template created!");
    }
    
    #region UI Creation Helpers
    
    GameObject CreatePanel(GameObject parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
        return panel;
    }
    
    GameObject CreateButton(GameObject parent, string name, string text)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = button.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        
        Image image = button.AddComponent<Image>();
        Button buttonComponent = button.AddComponent<Button>();
        
        GameObject textObj = CreateText(button, "Text", text);
        
        return button;
    }
    
    GameObject CreateText(GameObject parent, string name, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 16;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        return textObj;
    }
    
    GameObject CreateInputField(GameObject parent, string name, string placeholder)
    {
        GameObject inputField = new GameObject(name);
        inputField.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = inputField.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        
        Image image = inputField.AddComponent<Image>();
        InputField inputFieldComponent = inputField.AddComponent<InputField>();
        
        GameObject textObj = CreateText(inputField, "TextMeshProUGUI", "");
        GameObject placeholderObj = CreateText(inputField, "Placeholder", placeholder);
        
        inputFieldComponent.textComponent = textObj.GetComponent<Text>();
        inputFieldComponent.placeholder = placeholderObj.GetComponent<TextMeshProUGUI>();
        
        return inputField;
    }
    
    GameObject CreateDropdown(GameObject parent, string name)
    {
        GameObject dropdown = new GameObject(name);
        dropdown.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = dropdown.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        
        Image image = dropdown.AddComponent<Image>();
        Dropdown dropdownComponent = dropdown.AddComponent<Dropdown>();
        
        return dropdown;
    }
    
    GameObject CreateSlider(GameObject parent, string name)
    {
        GameObject slider = new GameObject(name);
        slider.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = slider.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 20);
        
        Slider sliderComponent = slider.AddComponent<Slider>();
        
        return slider;
    }
    
    GameObject CreateScrollView(GameObject parent, string name)
    {
        GameObject scrollView = new GameObject(name);
        scrollView.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = scrollView.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 300);
        
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = content.GetComponent<RectTransform>();
        
        return scrollView;
    }
    
    #endregion
    
    [ContextMenu("Show Setup Instructions")]
    void ShowInstructions()
    {
        Debug.Log(setupInstructions);
    }
    
    [ContextMenu("Create UI Templates")]
    void CreateTemplatesFromMenu()
    {
        CreateUITemplates();
    }
} 