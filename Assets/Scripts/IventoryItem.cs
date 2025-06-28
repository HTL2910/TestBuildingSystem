using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
public class IventoryItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    [Header("Trash able")]
    public bool isTrashable;
    [Header("Object item")]
    private GameObject itemInfoUI;
    [Header("Text Info Item")]
    private TextMeshProUGUI itemInfoUI_itemName;
    private TextMeshProUGUI itemInfoUI_itemDescription;
    private TextMeshProUGUI itemInfoUI_itemFunctionality;
    public string thisName,thisDescription,thisFunctionality;
    [Header("Consumption")]
    private GameObject itemPendingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float caloriesEffect;
    public float hydrationsEffect;

    void Start()
    {
        itemInfoUI=InventorySystem.instance.ItemInfoUI;
        itemInfoUI_itemName=itemInfoUI.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemDescription=itemInfoUI.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemFunctionality=itemInfoUI.transform.Find("ItemFunctionally").GetComponent<TextMeshProUGUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text=thisName;
        itemInfoUI_itemDescription.text=thisDescription;
        itemInfoUI_itemFunctionality.text=thisFunctionality;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button==PointerEventData.InputButton.Right)
        {
            if(isConsumable)
            {
                itemPendingConsumption=gameObject;
                ConsumingFunction(healthEffect,caloriesEffect,hydrationsEffect);
            }
        }
    }

    private void ConsumingFunction(float healthEffect, float caloriesEffect, float hydrationsEffect)
    {
        itemInfoUI.SetActive(false);
        healthEffectCalculation(healthEffect);
        caloriesEffectCalculation(caloriesEffect);
        hydrationsEffectCalculation(hydrationsEffect);
    }

    private void hydrationsEffectCalculation(float hydrationsEffect)
    {
        float hydrationsBeforeConsumption=PlayerStatus.Instance.currentHydrationPercent;
        float maxHydrations=PlayerStatus.Instance.maxHydrationPercent;
        if(hydrationsEffect!=0)
        {
            if((hydrationsBeforeConsumption+hydrationsEffect)>maxHydrations)
            {
                PlayerStatus.Instance.SetHydrations(maxHydrations);
            }
            else
            {
                PlayerStatus.Instance.SetHydrations(hydrationsBeforeConsumption+hydrationsEffect);
            }
        }
    }

    private void caloriesEffectCalculation(float caloriesEffect)
    {
        float caloriesBeforeConsumption=PlayerStatus.Instance.currentCalories;
        float maxCalories=PlayerStatus.Instance.maxCalories;
        if(caloriesEffect!=0)
        {
            if((caloriesBeforeConsumption+caloriesEffect)>maxCalories)
            {
                PlayerStatus.Instance.SetCalories(maxCalories);
            }
            else
            {
                PlayerStatus.Instance.SetCalories(caloriesBeforeConsumption+caloriesEffect);
            }
        }
    }

    private void healthEffectCalculation(float healthEffect)
    {
        float healthBeforeConsumption=PlayerStatus.Instance.currentHealth;
        float maxHealth=PlayerStatus.Instance.maxHealth;

        if(healthEffect!=0)
        {
            if((healthBeforeConsumption+healthEffect)>maxHealth)
            {
                PlayerStatus.Instance.SetHealth(maxHealth);
            }
            else
            {
                PlayerStatus.Instance.SetHealth(healthBeforeConsumption+healthEffect);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button ==PointerEventData.InputButton.Right)
        {
            if(isConsumable && itemPendingConsumption==gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.instance.ReCalculeList();
                CraftingSystem.instance.RefreshNeededItems();
            }
        }
    }
}
