using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance { get; set; }

    public GameObject inventoryScreenUI;
    public List<GameObject> slotList=new List<GameObject>();
    public List<string > itemList=new List<string>();

    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
    public bool isOpen;
    //public bool isFull;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        { 
            instance = this;
        }
    }
    private void Start()
    {
        isOpen = false;
        //isFull = false;
        PopulateSlotList(); 
    }
    private void PopulateSlotList()
    {
        foreach(Transform child in inventoryScreenUI.transform)
        {
            if(child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }    
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            Debug.Log("i is pressd");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;

        }
        else if(Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            isOpen = false;
        }
    }

    public void AddToInventory(string itemName)
    {

        whatSlotToEquip = FindNextEmptySlot();
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName),
            whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        itemList.Add(itemName);

    }
    private GameObject FindNextEmptySlot()
    {
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount==0)
            {
                return slot;
            }
            
        }
        return new GameObject();
    }
    public bool CheckIfFull()
    {
        int counter = 0;
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount>0)
            {
                counter += 1;
               
            }  
        }
        if (counter == 12)//Length of inventory
        {
            return true;
        }
        else
        {
            return false;
        }
    }    
}
