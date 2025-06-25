using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI healthCounter;
    public GameObject m_playerState;
    private float currentHealth,maxHealth;
    void Start()
    {
        slider=GetComponent<Slider>();
    }
    void Update()
    {
        currentHealth=m_playerState.GetComponent<PlayerStatus>().currentHealth;
        maxHealth=m_playerState.GetComponent<PlayerStatus>().maxHealth;

        float fillValue=currentHealth/maxHealth;
        slider.value=fillValue;
        healthCounter.text=$"{currentHealth}/{maxHealth}";   
    }
}
