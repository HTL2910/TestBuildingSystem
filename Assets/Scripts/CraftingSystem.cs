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

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("CrafButton").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });
    }

    private void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }
    void CraftAnyItem(BluePrint blueprintToCraft)
    {
        // Kiểm tra nguyên liệu trước
        if (!HasEnoughMaterials(blueprintToCraft))
        {
            Debug.Log("Not enough materials!");
            return;
        }

        // Thêm item mới
        InventorySystem.instance.AddToInventory(blueprintToCraft.itemName);

        // Xóa nguyên liệu
        if(blueprintToCraft.numOfRequirements == 1)
        {
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        }    
        else if(blueprintToCraft.numOfRequirements == 2)
        {
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
        }

        // Cập nhật inventory ngay lập tức
        InventorySystem.instance.ReCalculeList();
        RefreshNeededItems();
    }

    private bool HasEnoughMaterials(BluePrint blueprint)
    {
        int req1Count = 0;
        int req2Count = 0;
        
        foreach(string item in InventorySystem.instance.itemList)
        {
            if(item == blueprint.Req1) req1Count++;
            if(item == blueprint.Req2) req2Count++;
        }
        
        return req1Count >= blueprint.Req1amount && 
               (blueprint.numOfRequirements == 1 || req2Count >= blueprint.Req2amount);
    }

    // Update is called once per frame
    void Update()
    {
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
    public void RefreshNeededItems()
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
