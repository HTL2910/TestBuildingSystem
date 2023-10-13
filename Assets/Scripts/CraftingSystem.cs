using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;

    private List<string> inventoryItemList = new List<string>();
    // Category buttons
    Button toolsBTN;
    //craft Buttons
    Button craftAxeBTN;

    //Requrement Text
    Text AxeReq1, AxeReq2;
    public bool isOpen;
    //All Blueprint
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

        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<Text>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<Text>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(); });
    }

    private void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }
    void CraftAnyItem()
    {

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
            Cursor.lockState = CursorLockMode.Locked;
            isOpen = false;
        }
    }
}
