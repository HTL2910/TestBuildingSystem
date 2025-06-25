using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI hydrationCounter;
    public GameObject m_playerState;
    private float currentHydration,maxHydration;
    void Start()
    {
        slider=GetComponent<Slider>();
    }
    void Update()
    {
        currentHydration=m_playerState.GetComponent<PlayerStatus>().currentHydrationPercent;
        maxHydration=m_playerState.GetComponent<PlayerStatus>().maxHydrationPercent;

        float fillValue=currentHydration/maxHydration;
        slider.value=fillValue;
        hydrationCounter.text=$"{currentHydration}%";   
    }
}
