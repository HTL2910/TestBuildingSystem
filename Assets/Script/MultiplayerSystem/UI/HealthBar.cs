using UnityEngine;
using UnityEngine.UI;
using MultiplayerSystem.Data;

namespace MultiplayerSystem.UI
{
    public class HealthBar : MonoBehaviour
    {
        [Header("UI Components")]
        public Slider healthSlider;
        public Image fillImage;
        public Text healthText;
        public Canvas healthCanvas;
        
        [Header("Health Bar Settings")]
        public bool showHealthText = true;
        public bool showPercentage = false;
        public bool smoothAnimation = true;
        public float animationSpeed = 5f;
        
        [Header("Color Settings")]
        public Color fullHealthColor = Color.green;
        public Color mediumHealthColor = Color.yellow;
        public Color lowHealthColor = Color.red;
        public Color shieldColor = Color.blue;
        
        [Header("Billboard Settings")]
        public bool enableBillboard = true;
        public bool lockYAxis = true;
        public Vector3 offset = new Vector3(0, 2f, 0);
        
        // Private variables
        private float currentHealth;
        private float maxHealth;
        private float currentShield;
        private float maxShield;
        private float targetHealthRatio;
        private Transform targetTransform;
        private Camera mainCamera;
        
        // Events
        public System.Action<float> OnHealthChanged;
        public System.Action<float> OnShieldChanged;
        
        void Start()
        {
            InitializeHealthBar();
        }
        
        void Update()
        {
            if (enableBillboard)
            {
                UpdateBillboard();
            }
            
            if (smoothAnimation)
            {
                UpdateSmoothAnimation();
            }
        }
        
        void InitializeHealthBar()
        {
            if (healthSlider != null)
            {
                healthSlider.minValue = 0f;
                healthSlider.maxValue = 1f;
                healthSlider.value = 1f;
            }
            
            if (healthCanvas != null)
            {
                healthCanvas.renderMode = RenderMode.WorldSpace;
            }
            
            mainCamera = Camera.main;
            
            // Find target transform (parent of health bar)
            targetTransform = transform.parent;
        }
        
        void UpdateBillboard()
        {
            if (mainCamera == null || targetTransform == null) return;
            
            // Update position
            transform.position = targetTransform.position + offset;
            
            // Update rotation to face camera
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            
            if (lockYAxis)
            {
                directionToCamera.y = 0;
            }
            
            if (directionToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-directionToCamera);
            }
        }
        
        void UpdateSmoothAnimation()
        {
            if (healthSlider != null)
            {
                float currentRatio = healthSlider.value;
                float newRatio = Mathf.Lerp(currentRatio, targetHealthRatio, Time.deltaTime * animationSpeed);
                healthSlider.value = newRatio;
            }
        }
        
        // Public methods
        public void Initialize(float maxHealth, float currentHealth = -1)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth >= 0 ? currentHealth : maxHealth;
            this.currentShield = 0f;
            this.maxShield = 0f;
            
            targetHealthRatio = this.currentHealth / this.maxHealth;
            
            UpdateHealthDisplay();
            UpdateHealthBarColor();
        }
        
        public void Initialize(PlayerData playerData)
        {
            this.maxHealth = playerData.maxHealth;
            this.currentHealth = playerData.currentHealth;
            this.maxShield = playerData.maxShield;
            this.currentShield = playerData.currentShield;
            
            targetHealthRatio = this.currentHealth / this.maxHealth;
            
            UpdateHealthDisplay();
            UpdateHealthBarColor();
        }
        
        public void UpdateHealth(float current, float maximum)
        {
            currentHealth = current;
            maxHealth = maximum;
            targetHealthRatio = currentHealth / maxHealth;
            
            if (!smoothAnimation)
            {
                if (healthSlider != null)
                {
                    healthSlider.value = targetHealthRatio;
                }
            }
            
            UpdateHealthDisplay();
            UpdateHealthBarColor();
            
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        public void UpdateShield(float current, float maximum)
        {
            currentShield = current;
            maxShield = maximum;
            
            UpdateHealthDisplay();
            UpdateHealthBarColor();
            
            OnShieldChanged?.Invoke(currentShield);
        }
        
        void UpdateHealthDisplay()
        {
            // Update slider
            if (healthSlider != null)
            {
                if (!smoothAnimation)
                {
                    healthSlider.value = targetHealthRatio;
                }
            }
            
            // Update text
            if (healthText != null && showHealthText)
            {
                if (showPercentage)
                {
                    float percentage = maxHealth > 0 ? (currentHealth / maxHealth) * 100f : 0f;
                    healthText.text = $"{percentage:F0}%";
                }
                else
                {
                    if (maxShield > 0)
                    {
                        healthText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)} + {Mathf.CeilToInt(currentShield)}";
                    }
                    else
                    {
                        healthText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
                    }
                }
            }
        }
        
        void UpdateHealthBarColor()
        {
            if (fillImage == null) return;
            
            float healthRatio = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            
            // Determine color based on health percentage
            Color targetColor;
            if (healthRatio > 0.6f)
            {
                targetColor = fullHealthColor;
            }
            else if (healthRatio > 0.3f)
            {
                targetColor = mediumHealthColor;
            }
            else
            {
                targetColor = lowHealthColor;
            }
            
            // Blend with shield color if shield is active
            if (currentShield > 0 && maxShield > 0)
            {
                float shieldRatio = currentShield / maxShield;
                targetColor = Color.Lerp(targetColor, shieldColor, shieldRatio * 0.3f);
            }
            
            fillImage.color = targetColor;
        }
        
        // Utility methods
        public void SetHealthBarVisible(bool visible)
        {
            if (healthCanvas != null)
            {
                healthCanvas.enabled = visible;
            }
        }
        
        public void SetShowHealthText(bool show)
        {
            showHealthText = show;
            if (healthText != null)
            {
                healthText.gameObject.SetActive(show);
            }
        }
        
        public void SetShowPercentage(bool show)
        {
            showPercentage = show;
            UpdateHealthDisplay();
        }
        
        public void SetSmoothAnimation(bool smooth)
        {
            smoothAnimation = smooth;
        }
        
        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
        }
        
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
        
        public void SetBillboardEnabled(bool enabled)
        {
            enableBillboard = enabled;
        }
        
        public void SetLockYAxis(bool lockY)
        {
            lockYAxis = lockY;
        }
        
        // Color setters
        public void SetFullHealthColor(Color color)
        {
            fullHealthColor = color;
            UpdateHealthBarColor();
        }
        
        public void SetMediumHealthColor(Color color)
        {
            mediumHealthColor = color;
            UpdateHealthBarColor();
        }
        
        public void SetLowHealthColor(Color color)
        {
            lowHealthColor = color;
            UpdateHealthBarColor();
        }
        
        public void SetShieldColor(Color color)
        {
            shieldColor = color;
            UpdateHealthBarColor();
        }
        
        // Getters
        public float GetCurrentHealth()
        {
            return currentHealth;
        }
        
        public float GetMaxHealth()
        {
            return maxHealth;
        }
        
        public float GetCurrentShield()
        {
            return currentShield;
        }
        
        public float GetMaxShield()
        {
            return maxShield;
        }
        
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
        
        // Debug methods
        [ContextMenu("Test Health Bar")]
        public void TestHealthBar()
        {
            float randomHealth = Random.Range(0f, maxHealth);
            UpdateHealth(randomHealth, maxHealth);
        }
        
        [ContextMenu("Reset Health Bar")]
        public void ResetHealthBar()
        {
            UpdateHealth(maxHealth, maxHealth);
        }
    }
} 