using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;

    private List<string> inventoryItemList = new List<string>();
    // Category buttons

    public Button toolsBTN;
    //craft Buttons
    public Button craftAxeBTN;

    //Requrement Text
    public TextMeshProUGUI AxeReq1, AxeReq2;
    public bool isOpen;
    //All Blueprint
    private BluePrint AxeBLP=new BluePrint("Axe",2,"Rock",3,"Stick",3);
    public static CraftingSystem instance { get; set; }

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

    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TextMeshProUGUI>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });
    }

    private void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }
    void CraftAnyItem(BluePrint blueprintToCraft)
    {
        Debug.Log("Craft any Item");
        InventorySystem.instance.AddToInventory(blueprintToCraft.itemName);

        if(blueprintToCraft.numOfRequirements==1)
        {
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        }    
        else if(blueprintToCraft.numOfRequirements==2)
        {
            Debug.Log("Have" + blueprintToCraft.numOfRequirements);
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
        }
        //replace Stone
        StartCoroutine(calculate());

        RefreshNeededItems();

    }   

    public IEnumerator calculate()
    {
        yield return new WaitForSeconds(1f);
        InventorySystem.instance.ReCalculeList();
    }    
    // Update is called once per frame
    void Update()
    {
        RefreshNeededItems();
        if (Input.GetKeyDown(KeyCode.C)&& !isOpen)
        {
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.C)&&isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false );
            if(!InventorySystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            isOpen = false;
        }
    }
    private void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count=0;
        inventoryItemList = InventorySystem.instance.itemList;
        foreach(string itemName in inventoryItemList)
        {
            switch(itemName)
            {
                case "Rock":
                    stone_count+=1;
                    break;
                case "Stick":
                    stick_count+=1; 
                    break;
            }
        }
        AxeReq1.text = "3 Rock[" + stone_count + "]";
        AxeReq2.text = "3 Stick[" + stick_count + "]";
        if(stone_count>=3 && stick_count>=3)
        {
            craftAxeBTN.gameObject.SetActive(true);
        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false);
        }    
    }    

}
