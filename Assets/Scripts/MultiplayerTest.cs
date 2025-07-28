using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiplayerSystem.Core;

public class MultiplayerTest : MonoBehaviour
{
    [Header("Test UI")]
    public GameObject testUI;
    public Button connectButton;
    public Button disconnectButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI playerCountText;
    
    void Start()
    {
        SetupUI();
    }
    
    void Update()
    {
        UpdateStatus();
    }
    
    void SetupUI()
    {
        if (testUI != null)
        {
            testUI.SetActive(true);
            
            if (connectButton != null)
            {
                connectButton.onClick.AddListener(() => {
                    MultiplayerManager.Instance?.ConnectToServer();
                });
            }
            
            if (disconnectButton != null)
            {
                disconnectButton.onClick.AddListener(() => {
                    MultiplayerManager.Instance?.DisconnectFromServer();
                });
            }
        }
    }
    
    void UpdateStatus()
    {
        if (statusText == null) return;
        
        if (MultiplayerManager.Instance == null)
        {
            statusText.text = "MultiplayerManager not found!";
            return;
        }
        
        string status = "";
        
        if (MultiplayerManager.Instance.IsConnected())
        {
            status += "✅ Connected to Photon\n";
            
            if (MultiplayerManager.Instance.IsInRoom())
            {
                status += $"✅ In Room: {MultiplayerManager.Instance.GetRoomName()}\n";
                status += $"👥 Players: {MultiplayerManager.Instance.GetPlayerCount()}\n";
            }
            else
            {
                status += "⏳ Joining room...\n";
            }
        }
        else
        {
            status += "❌ Disconnected\n";
        }
        
        statusText.text = status;
        
        if (playerCountText != null)
        {
            playerCountText.text = $"Players: {MultiplayerManager.Instance.GetPlayerCount()}";
        }
    }
    
    // Public methods for testing
    public void TestCreateRoom()
    {
        MultiplayerManager.Instance?.CreateRoom("TestRoom");
    }
    
    public void TestJoinRandomRoom()
    {
        MultiplayerManager.Instance?.JoinRandomRoom();
    }
    
    public void TestLeaveRoom()
    {
        MultiplayerManager.Instance?.LeaveRoom();
    }
    
    public void TestSpawnPlayer()
    {
        MultiplayerManager.Instance?.SpawnPlayer();
    }
} 