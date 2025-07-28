using UnityEngine;
using UnityEngine.SceneManagement;
using MultiplayerSystem.Core;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string lobbySceneName = "Lobby";
    public string gameSceneName = "Main";
    
    [Header("Loading UI")]
    public GameObject loadingPanel;
    public UnityEngine.UI.Slider loadingProgressBar;
    public TMPro.TextMeshProUGUI loadingText;
    
    [Header("Auto Setup")]
    public bool autoConnectOnStart = true;
    
    void Start()
    {
        loadingProgressBar.value = 0f;
        if (autoConnectOnStart)
        {
            StartCoroutine(AutoConnectSequence());
        }
    }
    
    IEnumerator AutoConnectSequence()
    {
        // Wait a frame to ensure everything is initialized
        yield return null;
        
        // Check if MultiplayerManager exists
        if (MultiplayerManager.Instance == null)
        {
            Debug.LogWarning("MultiplayerManager not found! Creating one...");
            CreateMultiplayerManager();
        }
        
        // Connect to server
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.ConnectToServer();
        }
    }
    
    void CreateMultiplayerManager()
    {
        GameObject managerGO = new GameObject("MultiplayerManager");
        MultiplayerManager manager = managerGO.AddComponent<MultiplayerManager>();
        
        // Setup default settings
        manager.gameVersion = "1.0";
        manager.maxPlayersPerRoom = 4;
        manager.autoConnect = false; // We'll connect manually
        manager.defaultRoomName = "GameRoom";
    }
    
    #region Scene Navigation
    
    public void LoadLobbyScene()
    {
        Debug.Log("Loading Lobby Scene");
        StartCoroutine(LoadSceneAsync(lobbySceneName, "Loading Lobby..."));
    }
    
    public void LoadGameScene()
    {
        Debug.Log("Loading Game Scene");
        StartCoroutine(LoadSceneAsync(gameSceneName, "Loading Game..."));
    }
    
    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu");
        StartCoroutine(LoadSceneAsync("Main", "Loading Main Menu..."));
    }
    
    IEnumerator LoadSceneAsync(string sceneName, string loadingMessage)
    {
        // Show loading UI
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        
        if (loadingText != null)
        {
            loadingText.text = loadingMessage;
        }
        
        // Start loading scene
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        // Update progress bar
        while (asyncLoad.progress < 0.9f)
        {
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = asyncLoad.progress;
            }
            
            yield return null;
        }
        
        // Complete loading
        if (loadingProgressBar != null)
        {
            loadingProgressBar.value = 1f;
        }
        
        yield return new WaitForSeconds(0.5f); // Brief pause to show 100%
        
        // Activate scene
        asyncLoad.allowSceneActivation = true;
    }
    
    #endregion
    
    #region Public Methods
    
    public void QuitGame()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.DisconnectFromServer();
        }
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void DisconnectAndReturnToMenu()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.DisconnectFromServer();
        }
        
        LoadMainMenu();
    }
    
    #endregion
} 