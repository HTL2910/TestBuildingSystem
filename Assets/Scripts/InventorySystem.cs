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
            if(!CraftingSystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
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
        if (counter == 21)//Length of inventory
        {
            return true;
        }
        else
        {
            return false;
        }
    }    
    public void RemoveItem(string nameToRemove,int amountToRemove)
    {
        int counter = amountToRemove;
        Debug.Log("counter: " + counter);
        for (var i = slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(CLone)" && counter != 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter -= 1;
                }
            }
            
        }
    }
    public void ReCalculeList()
    {
        itemList.Clear();
        foreach(GameObject slot in slotList)
        {
            if( slot.transform.childCount>0)
            {
                string name=slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string result = name.Replace(str2,"");

                itemList.Add(result);
            }    
        }    
    }    
}
