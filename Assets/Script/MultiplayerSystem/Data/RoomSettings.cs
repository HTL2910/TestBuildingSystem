using UnityEngine;
using ExitGames.Client.Photon;

namespace MultiplayerSystem.Data
{
    [System.Serializable]
    public class RoomSettings
    {
        [Header("Basic Settings")]
        public string roomName = "GameRoom";
        public int maxPlayers = 4;
        public bool isVisible = true;
        public bool isOpen = true;
        
        [Header("Game Settings")]
        public string gameMode = "Deathmatch";
        public string mapName = "Default";
        public int timeLimit = 300; // seconds
        public int scoreLimit = 100;
        public bool friendlyFire = false;
        public bool autoRespawn = true;
        public float respawnTime = 3f;
        
        [Header("Team Settings")]
        public bool teamMode = false;
        public int maxTeams = 2;
        public int playersPerTeam = 2;
        public bool autoBalanceTeams = true;
        
        [Header("Advanced Settings")]
        public bool allowSpectators = false;
        public bool passwordProtected = false;
        public string password = "";
        public bool customSpawnPoints = false;
        public bool enableVoiceChat = false;
        public bool enableTextChat = true;
        
        [Header("Custom Properties")]
        public Hashtable customProperties = new Hashtable();
        public string[] propertiesForLobby = new string[0];
        
        public RoomSettings()
        {
            // Default constructor
        }
        
        public RoomSettings(string name, int maxPlayers)
        {
            this.roomName = name;
            this.maxPlayers = maxPlayers;
        }
        
        // Game mode presets
        public static RoomSettings CreateDeathmatch(int maxPlayers = 4)
        {
            RoomSettings settings = new RoomSettings("Deathmatch", maxPlayers);
            settings.gameMode = "Deathmatch";
            settings.timeLimit = 300;
            settings.scoreLimit = 100;
            settings.friendlyFire = false;
            return settings;
        }
        
        public static RoomSettings CreateTeamDeathmatch(int maxPlayers = 4)
        {
            RoomSettings settings = new RoomSettings("Team Deathmatch", maxPlayers);
            settings.gameMode = "TeamDeathmatch";
            settings.teamMode = true;
            settings.maxTeams = 2;
            settings.playersPerTeam = maxPlayers / 2;
            settings.timeLimit = 300;
            settings.scoreLimit = 100;
            settings.friendlyFire = false;
            return settings;
        }
        
        public static RoomSettings CreateCaptureTheFlag(int maxPlayers = 4)
        {
            RoomSettings settings = new RoomSettings("Capture The Flag", maxPlayers);
            settings.gameMode = "CaptureTheFlag";
            settings.teamMode = true;
            settings.maxTeams = 2;
            settings.playersPerTeam = maxPlayers / 2;
            settings.timeLimit = 600;
            settings.scoreLimit = 3;
            settings.friendlyFire = false;
            return settings;
        }
        
        public static RoomSettings CreateSurvival(int maxPlayers = 4)
        {
            RoomSettings settings = new RoomSettings("Survival", maxPlayers);
            settings.gameMode = "Survival";
            settings.timeLimit = 1800; // 30 minutes
            settings.scoreLimit = 0; // No score limit
            settings.friendlyFire = true;
            settings.autoRespawn = false;
            return settings;
        }
        
        // Utility methods
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(roomName))
                return false;
                
            if (maxPlayers < 1 || maxPlayers > 20)
                return false;
                
            if (teamMode)
            {
                if (maxTeams < 2)
                    return false;
                    
                if (maxPlayers % maxTeams != 0)
                    return false;
            }
            
            return true;
        }
        
        public string GetValidationError()
        {
            if (string.IsNullOrEmpty(roomName))
                return "Room name cannot be empty";
                
            if (maxPlayers < 1)
                return "Max players must be at least 1";
                
            if (maxPlayers > 20)
                return "Max players cannot exceed 20";
                
            if (teamMode && maxTeams < 2)
                return "Team mode requires at least 2 teams";
                
            if (teamMode && maxPlayers % maxTeams != 0)
                return "Players must be evenly distributed among teams";
                
            return null;
        }
        
        public void AddCustomProperty(string key, object value)
        {
            customProperties[key] = value;
        }
        
        public object GetCustomProperty(string key)
        {
            if (customProperties.ContainsKey(key))
            {
                return customProperties[key];
            }
            return null;
        }
        
        public void RemoveCustomProperty(string key)
        {
            if (customProperties.ContainsKey(key))
            {
                customProperties.Remove(key);
            }
        }
        
        public void AddLobbyProperty(string propertyName)
        {
            string[] newArray = new string[propertiesForLobby.Length + 1];
            propertiesForLobby.CopyTo(newArray, 0);
            newArray[propertiesForLobby.Length] = propertyName;
            propertiesForLobby = newArray;
        }
        
        // Serialization
        public Hashtable ToHashtable()
        {
            Hashtable hash = new Hashtable();
            hash["roomName"] = roomName;
            hash["maxPlayers"] = maxPlayers;
            hash["isVisible"] = isVisible;
            hash["isOpen"] = isOpen;
            hash["gameMode"] = gameMode;
            hash["mapName"] = mapName;
            hash["timeLimit"] = timeLimit;
            hash["scoreLimit"] = scoreLimit;
            hash["friendlyFire"] = friendlyFire;
            hash["autoRespawn"] = autoRespawn;
            hash["respawnTime"] = respawnTime;
            hash["teamMode"] = teamMode;
            hash["maxTeams"] = maxTeams;
            hash["playersPerTeam"] = playersPerTeam;
            hash["autoBalanceTeams"] = autoBalanceTeams;
            hash["allowSpectators"] = allowSpectators;
            hash["passwordProtected"] = passwordProtected;
            hash["password"] = password;
            hash["customSpawnPoints"] = customSpawnPoints;
            hash["enableVoiceChat"] = enableVoiceChat;
            hash["enableTextChat"] = enableTextChat;
            hash["customProperties"] = customProperties;
            hash["propertiesForLobby"] = propertiesForLobby;
            return hash;
        }
        
        public static RoomSettings FromHashtable(Hashtable hash)
        {
            RoomSettings settings = new RoomSettings();
            if (hash.ContainsKey("roomName")) settings.roomName = (string)hash["roomName"];
            if (hash.ContainsKey("maxPlayers")) settings.maxPlayers = (int)hash["maxPlayers"];
            if (hash.ContainsKey("isVisible")) settings.isVisible = (bool)hash["isVisible"];
            if (hash.ContainsKey("isOpen")) settings.isOpen = (bool)hash["isOpen"];
            if (hash.ContainsKey("gameMode")) settings.gameMode = (string)hash["gameMode"];
            if (hash.ContainsKey("mapName")) settings.mapName = (string)hash["mapName"];
            if (hash.ContainsKey("timeLimit")) settings.timeLimit = (int)hash["timeLimit"];
            if (hash.ContainsKey("scoreLimit")) settings.scoreLimit = (int)hash["scoreLimit"];
            if (hash.ContainsKey("friendlyFire")) settings.friendlyFire = (bool)hash["friendlyFire"];
            if (hash.ContainsKey("autoRespawn")) settings.autoRespawn = (bool)hash["autoRespawn"];
            if (hash.ContainsKey("respawnTime")) settings.respawnTime = (float)hash["respawnTime"];
            if (hash.ContainsKey("teamMode")) settings.teamMode = (bool)hash["teamMode"];
            if (hash.ContainsKey("maxTeams")) settings.maxTeams = (int)hash["maxTeams"];
            if (hash.ContainsKey("playersPerTeam")) settings.playersPerTeam = (int)hash["playersPerTeam"];
            if (hash.ContainsKey("autoBalanceTeams")) settings.autoBalanceTeams = (bool)hash["autoBalanceTeams"];
            if (hash.ContainsKey("allowSpectators")) settings.allowSpectators = (bool)hash["allowSpectators"];
            if (hash.ContainsKey("passwordProtected")) settings.passwordProtected = (bool)hash["passwordProtected"];
            if (hash.ContainsKey("password")) settings.password = (string)hash["password"];
            if (hash.ContainsKey("customSpawnPoints")) settings.customSpawnPoints = (bool)hash["customSpawnPoints"];
            if (hash.ContainsKey("enableVoiceChat")) settings.enableVoiceChat = (bool)hash["enableVoiceChat"];
            if (hash.ContainsKey("enableTextChat")) settings.enableTextChat = (bool)hash["enableTextChat"];
            if (hash.ContainsKey("customProperties")) settings.customProperties = (Hashtable)hash["customProperties"];
            if (hash.ContainsKey("propertiesForLobby")) settings.propertiesForLobby = (string[])hash["propertiesForLobby"];
            return settings;
        }
    }
} 