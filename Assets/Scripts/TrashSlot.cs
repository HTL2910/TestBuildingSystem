using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TrashSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject trashAlertUI;
    private TextMeshProUGUI textToModify;
    public Sprite trash_closedSprite;
    public Sprite trash_openSprite;
    private Image imageComponent;
    Button YesBtn, NoBtn;

    GameObject draggableItem
    {
        get
        {
            return DragDrop.itemBeingDragged;
        }
    }
    GameObject itemToBeDeleted;

    public string itemName
    {
        get
        {
            string name = itemToBeDeleted.name;
            string toRemove = "(Clone)";
            string result = name.Replace(toRemove, "");
            return result;
        }
    }
    private void Start()
    {
        imageComponent = transform.Find("background").GetComponent<Image>();
        textToModify = trashAlertUI.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        YesBtn = trashAlertUI.transform.Find("YesButton").GetComponent<Button>();
        NoBtn = trashAlertUI.transform.Find("NoButton").GetComponent<Button>();
        YesBtn.onClick.AddListener(delegate { DeleteItem(); });
        NoBtn.onClick.AddListener(delegate { CancelDelete(); });
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggableItem.GetComponent<IventoryItem>().isTrashable == true && draggableItem!=null)
        {
            itemToBeDeleted = draggableItem.gameObject;
            StartCoroutine(NotifyBeforeDelete());
        }
    }
    IEnumerator NotifyBeforeDelete()
    {
        trashAlertUI.SetActive(true);
        textToModify.text = "Are you sure you want to delete " + itemName + "?";
        yield return new WaitForSeconds(1f);
    }
    private void CancelDelete()
    {
        imageComponent.sprite = trash_closedSprite;
        trashAlertUI.SetActive(false);
    }
    private void DeleteItem()
    {
        imageComponent.sprite = trash_closedSprite;
        DestroyImmediate(itemToBeDeleted.gameObject);
        InventorySystem.instance.ReCalculeList();
        CraftingSystem.instance.RefreshNeededItems();
        trashAlertUI.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (draggableItem.GetComponent<IventoryItem>().isTrashable == true && draggableItem!=null)
        {
            imageComponent.sprite = trash_openSprite;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (draggableItem.GetComponent<IventoryItem>().isTrashable == true && draggableItem != null)
        {
            imageComponent.sprite = trash_closedSprite;
        }
    }
}
