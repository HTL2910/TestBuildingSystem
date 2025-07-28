using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI maxPlayersText;
    public Button joinButton;
    public Image backgroundImage;
    
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color selectedColor = new Color(0.8f, 0.8f, 1f, 1f);
    
    // Private variables
    private RoomInfo roomInfo;
    private LobbyManager lobbyManager;
    private bool isHovered = false;
    private bool isSelected = false;
    
    void Start()
    {
        SetupUI();
    }
    
    void SetupUI()
    {
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(JoinRoom);
        }
        
        // Setup hover effects
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
    
    public void SetupRoom(RoomInfo room, LobbyManager manager)
    {
        roomInfo = room;
        lobbyManager = manager;
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (roomInfo == null) return;
        
        // Update room name
        if (roomNameText != null)
        {
            roomNameText.text = roomInfo.Name;
        }
        
        // Update player count
        if (playerCountText != null)
        {
            playerCountText.text = roomInfo.PlayerCount.ToString();
        }
        
        // Update max players
        if (maxPlayersText != null)
        {
            maxPlayersText.text = "/ " + roomInfo.MaxPlayers.ToString();
        }
        
        // Update join button state
        if (joinButton != null)
        {
            bool canJoin = roomInfo.PlayerCount < roomInfo.MaxPlayers && roomInfo.IsOpen;
            joinButton.interactable = canJoin;
            
            // Update button text based on state
            TextMeshProUGUI buttonText = joinButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (canJoin)
                {
                    buttonText.text = "Join";
                }
                else if (!roomInfo.IsOpen)
                {
                    buttonText.text = "Full";
                }
                else
                {
                    buttonText.text = "Full";
                }
            }
        }
        
        // Update background color based on room state
        UpdateBackgroundColor();
    }
    
    void UpdateBackgroundColor()
    {
        if (backgroundImage == null) return;
        
        Color targetColor = normalColor;
        
        if (isSelected)
        {
            targetColor = selectedColor;
        }
        else if (isHovered)
        {
            targetColor = hoverColor;
        }
        else if (roomInfo != null)
        {
            // Color based on room state
            if (!roomInfo.IsOpen)
            {
                targetColor = new Color(0.8f, 0.6f, 0.6f, 1f); // Reddish for closed rooms
            }
            else if (roomInfo.PlayerCount >= roomInfo.MaxPlayers)
            {
                targetColor = new Color(0.8f, 0.8f, 0.6f, 1f); // Yellowish for full rooms
            }
        }
        
        backgroundImage.color = targetColor;
    }
    
    public void JoinRoom()
    {
        if (roomInfo == null || lobbyManager == null) return;
        
        // Check if room is still joinable
        if (roomInfo.PlayerCount >= roomInfo.MaxPlayers || !roomInfo.IsOpen)
        {
            Debug.LogWarning("Cannot join room: " + roomInfo.Name);
            return;
        }
        
        lobbyManager.JoinRoom(roomInfo.Name);
    }
    
    #region UI Event Handlers
    
    public void OnPointerEnter()
    {
        isHovered = true;
        UpdateBackgroundColor();
    }
    
    public void OnPointerExit()
    {
        isHovered = false;
        UpdateBackgroundColor();
    }
    
    public void OnPointerClick()
    {
        isSelected = true;
        UpdateBackgroundColor();
        
        // Auto-join after selection
        JoinRoom();
    }
    
    #endregion
    
    #region Public Methods
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBackgroundColor();
    }
    
    public RoomInfo GetRoomInfo()
    {
        return roomInfo;
    }
    
    public bool IsRoomFull()
    {
        return roomInfo != null && (roomInfo.PlayerCount >= roomInfo.MaxPlayers || !roomInfo.IsOpen);
    }
    
    #endregion
} 