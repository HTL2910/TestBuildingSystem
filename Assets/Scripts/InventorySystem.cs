using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class InventorySystem : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject ItemInfoUI;
    public static InventorySystem instance { get; set; }
    [Header("Inventory")]
    public GameObject inventoryScreenUI;
    public List<GameObject> slotList=new List<GameObject>();
    public List<string > itemList=new List<string>();
    public PhotonView photonView;

    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
    public bool isOpen;
    //public bool isFull;

    //Pickup Popup
    [Header("Pickup Popup")]
    public GameObject pickupALert;
    public TextMeshProUGUI pickupName;
    public Image pickupImage;
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
        
        // Get PhotonView component
        if (photonView == null) photonView = GetComponent<PhotonView>();
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
        if (photonView == null) return;
        if (!photonView.IsMine) return;
        
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
        if (photonView == null) return;
        if (!photonView.IsMine) return;
        
        // Call RPC to sync inventory across network
        photonView.RPC("AddToInventoryRPC", RpcTarget.All, itemName);
    }
    
    [PunRPC]
    void AddToInventoryRPC(string itemName)
    {
        whatSlotToEquip = FindNextEmptySlot();
        if (whatSlotToEquip != null)
        {
            itemToAdd = Instantiate(Resources.Load<GameObject>(itemName),
                whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
            itemToAdd.transform.SetParent(whatSlotToEquip.transform);

            itemList.Add(itemName);
            
            if (photonView.IsMine)
            {
                TriggerPickupPopup(itemName,itemToAdd.GetComponent<Image>().sprite);
            }

            ReCalculeList();
            if (CraftingSystem.instance != null)
            {
                CraftingSystem.instance.RefreshNeededItems();
            }
        }
    }
    private GameObject FindNextEmptySlot()
    {
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
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
    private void TriggerPickupPopup(string itemName,Sprite itemSprite)
    {
        pickupALert.SetActive(true);
        pickupName.text = itemName;
        pickupImage.sprite = itemSprite;
    }
    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        if (photonView == null) return;
        if (!photonView.IsMine) return;
        
        // Call RPC to sync item removal
        photonView.RPC("RemoveItemRPC", RpcTarget.All, nameToRemove, amountToRemove);
    }
    
    [PunRPC]
    void RemoveItemRPC(string nameToRemove, int amountToRemove)
    {
        int counter = amountToRemove;
        for (var i = slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
                {
                    DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
                    counter -= 1;
                    itemList.Remove(nameToRemove);
                }
            }
        }
        ReCalculeList();
        if (CraftingSystem.instance != null)
        {
            CraftingSystem.instance.RefreshNeededItems();
        }
    }
    public void ReCalculeList()
    {
        List<string> tempList = new List<string>();
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string result = name.Replace(str2,"");
                tempList.Add(result);
            }    
        }
        itemList.Clear();
        itemList.AddRange(tempList);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send inventory data
            stream.SendNext(itemList.Count);
            foreach (string item in itemList)
            {
                stream.SendNext(item);
            }
        }
        else
        {
            // Receive inventory data
            int itemCount = (int)stream.ReceiveNext();
            itemList.Clear();
            for (int i = 0; i < itemCount; i++)
            {
                string item = (string)stream.ReceiveNext();
                itemList.Add(item);
            }
        }
    }
}
