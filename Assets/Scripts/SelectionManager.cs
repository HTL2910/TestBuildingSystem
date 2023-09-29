using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectionManager : MonoBehaviour
{
    public GameObject interaction_info_UI;
    private TextMeshProUGUI interactionText;

    private void Start()
    {
        interactionText = interaction_info_UI.GetComponent<TextMeshProUGUI>();
        interaction_info_UI.gameObject.SetActive(false);
    }
    private void Update()
    {
        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit) )
        {
            var selectionTransform = hit.transform;
            if (selectionTransform.GetComponent<InteractableObject>())
            {
                interactionText.text= selectionTransform.GetComponent<InteractableObject>().GetItemName();
                interaction_info_UI.gameObject.SetActive(true);
            }
            else
            {
                interaction_info_UI.gameObject.SetActive(false);
            }
        }    
    }
}
