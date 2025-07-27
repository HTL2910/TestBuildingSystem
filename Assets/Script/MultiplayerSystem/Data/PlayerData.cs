using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

namespace MultiplayerSystem.Data
{
    [System.Serializable]
    public class PlayerData
    {
        [Header("Basic Info")]
        public string playerName = "Player";
        public int playerId = -1;
        public bool isReady = false;
        public bool isAlive = true;
        
        [Header("Game Stats")]
        public int score = 0;
        public int kills = 0;
        public int deaths = 0;
        public int assists = 0;
        
        [Header("Health System")]
        public float currentHealth = 100f;
        public float maxHealth = 100f;
        public float currentShield = 0f;
        public float maxShield = 100f;
        
        [Header("Position")]
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        
        [Header("Custom Properties")]
        public string team = "None";
        public string characterClass = "Default";
        public int level = 1;
        public float experience = 0f;
        
        public PlayerData()
        {
            // Default constructor
        }
        
        public PlayerData(string name, int id)
        {
            playerName = name;
            playerId = id;
        }
        
        public PlayerData(PlayerData other)
        {
            // Copy constructor
            playerName = other.playerName;
            playerId = other.playerId;
            isReady = other.isReady;
            isAlive = other.isAlive;
            score = other.score;
            kills = other.kills;
            deaths = other.deaths;
            assists = other.assists;
            currentHealth = other.currentHealth;
            maxHealth = other.maxHealth;
            currentShield = other.currentShield;
            maxShield = other.maxShield;
            position = other.position;
            rotation = other.rotation;
            team = other.team;
            characterClass = other.characterClass;
            level = other.level;
            experience = other.experience;
        }
        
        // Health methods
        public void TakeDamage(float damage)
        {
            float remainingDamage = damage;
            
            // Damage shield first
            if (currentShield > 0)
            {
                if (currentShield >= remainingDamage)
                {
                    currentShield -= remainingDamage;
                    remainingDamage = 0;
                }
                else
                {
                    remainingDamage -= currentShield;
                    currentShield = 0;
                }
            }
            
            // Damage health
            if (remainingDamage > 0)
            {
                currentHealth -= remainingDamage;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    isAlive = false;
                }
            }
        }
        
        public void Heal(float amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        
        public void AddShield(float amount)
        {
            currentShield += amount;
            if (currentShield > maxShield)
            {
                currentShield = maxShield;
            }
        }
        
        public void Respawn()
        {
            currentHealth = maxHealth;
            currentShield = 0f;
            isAlive = true;
        }
        
        // Score methods
        public void AddScore(int points)
        {
            score += points;
        }
        
        public void AddKill()
        {
            kills++;
            AddScore(100);
        }
        
        public void AddDeath()
        {
            deaths++;
        }
        
        public void AddAssist()
        {
            assists++;
            AddScore(25);
        }
        
        // Experience methods
        public void AddExperience(float exp)
        {
            experience += exp;
            CheckLevelUp();
        }
        
        private void CheckLevelUp()
        {
            float expNeeded = level * 100f; // Simple level up formula
            if (experience >= expNeeded)
            {
                level++;
                experience -= expNeeded;
            }
        }
        
        // Utility methods
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0f;
        }
        
        public float GetShieldPercentage()
        {
            return maxShield > 0 ? currentShield / maxShield : 0f;
        }
        
        public bool IsFullHealth()
        {
            return currentHealth >= maxHealth;
        }
        
        public bool IsFullShield()
        {
            return currentShield >= maxShield;
        }
        
        // Serialization for Photon
        public Hashtable ToHashtable()
        {
            Hashtable hash = new Hashtable();
            hash["playerName"] = playerName;
            hash["playerId"] = playerId;
            hash["isReady"] = isReady;
            hash["isAlive"] = isAlive;
            hash["score"] = score;
            hash["kills"] = kills;
            hash["deaths"] = deaths;
            hash["assists"] = assists;
            hash["currentHealth"] = currentHealth;
            hash["maxHealth"] = maxHealth;
            hash["currentShield"] = currentShield;
            hash["maxShield"] = maxShield;
            hash["position"] = position;
            hash["rotation"] = rotation;
            hash["team"] = team;
            hash["characterClass"] = characterClass;
            hash["level"] = level;
            hash["experience"] = experience;
            return hash;
        }
        
        public static PlayerData FromHashtable(Hashtable hash)
        {
            PlayerData data = new PlayerData();
            if (hash.ContainsKey("playerName")) data.playerName = (string)hash["playerName"];
            if (hash.ContainsKey("playerId")) data.playerId = (int)hash["playerId"];
            if (hash.ContainsKey("isReady")) data.isReady = (bool)hash["isReady"];
            if (hash.ContainsKey("isAlive")) data.isAlive = (bool)hash["isAlive"];
            if (hash.ContainsKey("score")) data.score = (int)hash["score"];
            if (hash.ContainsKey("kills")) data.kills = (int)hash["kills"];
            if (hash.ContainsKey("deaths")) data.deaths = (int)hash["deaths"];
            if (hash.ContainsKey("assists")) data.assists = (int)hash["assists"];
            if (hash.ContainsKey("currentHealth")) data.currentHealth = (float)hash["currentHealth"];
            if (hash.ContainsKey("maxHealth")) data.maxHealth = (float)hash["maxHealth"];
            if (hash.ContainsKey("currentShield")) data.currentShield = (float)hash["currentShield"];
            if (hash.ContainsKey("maxShield")) data.maxShield = (float)hash["maxShield"];
            if (hash.ContainsKey("position")) data.position = (Vector3)hash["position"];
            if (hash.ContainsKey("rotation")) data.rotation = (Quaternion)hash["rotation"];
            if (hash.ContainsKey("team")) data.team = (string)hash["team"];
            if (hash.ContainsKey("characterClass")) data.characterClass = (string)hash["characterClass"];
            if (hash.ContainsKey("level")) data.level = (int)hash["level"];
            if (hash.ContainsKey("experience")) data.experience = (float)hash["experience"];
            return data;
        }
    }
} 