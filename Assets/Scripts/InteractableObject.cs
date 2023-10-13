using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string itemName;
    public bool playerInRange;
    public string GetItemName(){ return itemName; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selectedGameObject==gameObject)
        {
            //if the inventory is not full
            if(!InventorySystem.instance.CheckIfFull())
            {
                InventorySystem.instance.AddToInventory(itemName);
                Destroy(gameObject);
            }   
            else
            {
                Debug.Log("inventory is full");
            }    
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }    
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
