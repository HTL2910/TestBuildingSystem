using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CaloriesBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI caloriesCounter;
    public GameObject m_playerState;
    private float currentCalories,maxCalories;
    void Start()
    {
        slider=GetComponent<Slider>();
    }
    void Update()
    {
        currentCalories=m_playerState.GetComponent<PlayerStatus>().currentCalories;
        maxCalories=m_playerState.GetComponent<PlayerStatus>().maxCalories;

        float fillValue=currentCalories/maxCalories;
        slider.value=fillValue;
        caloriesCounter.text=$"{currentCalories}/{maxCalories}";   
    }
}

