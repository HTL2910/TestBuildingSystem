using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance{ get; set; }
    public bool onTarget;
    public GameObject interaction_info_UI;
    private TextMeshProUGUI interactionText;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }    
    }
    private void Start()
    {
        interactionText = interaction_info_UI.GetComponent<TextMeshProUGUI>();
        interaction_info_UI.gameObject.SetActive(false);
        onTarget = false;
    }
    private void Update()
    {
        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactableObject= selectionTransform.GetComponent<InteractableObject>();
            if (interactableObject && interactableObject.playerInRange)
            {
                interactionText.text = interactableObject.GetItemName();
                onTarget = true;
                interaction_info_UI.gameObject.SetActive(true);
            }
            else
            {
                onTarget = false;
                interaction_info_UI.gameObject.SetActive(false);
            }
        }
        else
        {
            onTarget = false;
        }
    }
}
