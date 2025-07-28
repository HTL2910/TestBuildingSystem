using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiplayerSystem.Core;
using Photon.Realtime;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game UI")]
    public GameObject gameUIPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    
    [Header("Game Info")]
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI statusText;
    
    [Header("Game Controls")]
    public Button pauseButton;
    public Button resumeButton;
    public Button leaveRoomButton;
    public Button quitGameButton;
    
    [Header("Game Settings")]
    public float gameStartDelay = 3f;
    public bool autoStartGame = true;
    
    // Private variables
    private bool isGamePaused = false;
    private bool isGameStarted = false;
    private float gameTime = 0f;
    private Coroutine gameTimeCoroutine;
    
    void Start()
    {
        SetupUI();
        SubscribeToEvents();
        
        if (autoStartGame)
        {
            StartCoroutine(StartGameAfterDelay());
        }
    }
    
    void SetupUI()
    {
        // Setup button listeners
        if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (leaveRoomButton != null) leaveRoomButton.onClick.AddListener(LeaveRoom);
        if (quitGameButton != null) quitGameButton.onClick.AddListener(QuitGame);
        
        // Show game UI initially
        ShowGameUI();
    }
    
    void SubscribeToEvents()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.OnGameStateChanged += OnGameStateChanged;
            MultiplayerManager.Instance.OnPlayerJoined += OnPlayerJoined;
            MultiplayerManager.Instance.OnPlayerLeft += OnPlayerLeft;
            MultiplayerManager.Instance.OnGameStarted += OnGameStarted;
            MultiplayerManager.Instance.OnGameEnded += OnGameEnded;
        }
    }
    
    void OnDestroy()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            MultiplayerManager.Instance.OnPlayerJoined -= OnPlayerJoined;
            MultiplayerManager.Instance.OnPlayerLeft -= OnPlayerLeft;
            MultiplayerManager.Instance.OnGameStarted -= OnGameStarted;
            MultiplayerManager.Instance.OnGameEnded -= OnGameEnded;
        }
        
        StopGameTime();
    }
    
    #region Game Flow
    
    IEnumerator StartGameAfterDelay()
    {
        UpdateStatus($"Game starting in {gameStartDelay} seconds...");
        
        for (int i = (int)gameStartDelay; i > 0; i--)
        {
            UpdateStatus($"Game starting in {i} seconds...");
            yield return new WaitForSeconds(1f);
        }
        
        StartGame();
    }
    
    void StartGame()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.StartGame();
        }
        
        isGameStarted = true;
        StartGameTime();
        UpdateStatus("Game started!");
    }
    
    void StartGameTime()
    {
        if (gameTimeCoroutine != null)
        {
            StopCoroutine(gameTimeCoroutine);
        }
        
        gameTimeCoroutine = StartCoroutine(GameTimeCoroutine());
    }
    
    void StopGameTime()
    {
        if (gameTimeCoroutine != null)
        {
            StopCoroutine(gameTimeCoroutine);
            gameTimeCoroutine = null;
        }
    }
    
    IEnumerator GameTimeCoroutine()
    {
        gameTime = 0f;
        
        while (isGameStarted)
        {
            gameTime += Time.deltaTime;
            UpdateGameTimeUI();
            yield return null;
        }
    }
    
    #endregion
    
    #region UI Management
    
    void ShowGameUI()
    {
        if (gameUIPanel != null) gameUIPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        UpdateGameInfo();
    }
    
    void ShowPauseUI()
    {
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }
    
    void ShowGameOverUI()
    {
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
    
    void UpdateGameInfo()
    {
        if (MultiplayerManager.Instance == null) return;
        
        // Update room name
        if (roomNameText != null)
        {
            roomNameText.text = "Room: " + MultiplayerManager.Instance.GetRoomName();
        }
        
        // Update player count
        if (playerCountText != null)
        {
            playerCountText.text = "Players: " + MultiplayerManager.Instance.GetPlayerCount();
        }
    }
    
    void UpdateGameTimeUI()
    {
        if (gameTimeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
                if (MultiplayerManager.Instance.IsInRoom())
                {
                    statusText.text = "In Room";
                }
                else
                {
                    statusText.text = "Not in Room";
                }
            }
            else
            {
                statusText.text = "MultiplayerManager not found";
            }
        }
    }
    
    #endregion
    
    #region Game Controls
    
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        ShowPauseUI();
        UpdateStatus("Game Paused");
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        ShowGameUI();
        UpdateStatus("Game Resumed");
    }
    
    public void LeaveRoom()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.LeaveRoom();
        }
        
        // Load lobby scene
        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        if (sceneManager != null)
        {
            sceneManager.LoadLobbyScene();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }
    }
    
    public void QuitGame()
    {
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.DisconnectFromServer();
        }
        
        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        if (sceneManager != null)
        {
            sceneManager.QuitGame();
        }
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    #endregion
    
    #region Event Callbacks
    
    void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Disconnected:
                UpdateStatus("Disconnected from server");
                break;
                
            case GameState.Connected:
                UpdateStatus("Connected to server");
                break;
                
            case GameState.InRoom:
                UpdateStatus("In Room");
                UpdateGameInfo();
                break;
                
            case GameState.InGame:
                UpdateStatus("Game in progress");
                break;
        }
    }
    
    void OnPlayerJoined(Photon.Realtime.Player player)
    {
        UpdateStatus($"Player joined: {player.NickName}");
        UpdateGameInfo();
    }
    
    void OnPlayerLeft(Photon.Realtime.Player player)
    {
        UpdateStatus($"Player left: {player.NickName}");
        UpdateGameInfo();
    }
    
    void OnGameStarted()
    {
        isGameStarted = true;
        StartGameTime();
        UpdateStatus("Game started!");
    }
    
    void OnGameEnded()
    {
        isGameStarted = false;
        StopGameTime();
        ShowGameOverUI();
        UpdateStatus("Game ended!");
    }
    
    #endregion
    
    #region Public Methods
    
    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    public bool IsGameStarted()
    {
        return isGameStarted;
    }
    
    public float GetGameTime()
    {
        return gameTime;
    }
    
    #endregion
} 