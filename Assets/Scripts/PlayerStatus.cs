using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance{get;set;}
    [Header("Health")]
    public float currentHealth;
    public float maxHealth;
    [Header("Calories")]
    public float currentCalories;
    public float maxCalories;
    float distanceTravelled=0;
    Vector3 lastPosition;
    public GameObject playerBody;

    [Header("Hydration")]
    public float currentHydrationPercent;
    public float maxHydrationPercent;
    public bool isHydrationActive;
    
    void Awake()
    {
        if(Instance!=null && Instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance=this;
        }
    }
    void Start()
    {
        currentHealth=maxHealth;
        currentCalories=maxCalories;
        currentHydrationPercent=maxHydrationPercent;
        if(isHydrationActive)
        {
            StartCoroutine(DecreaseHydration());
        }

        
    }
    IEnumerator DecreaseHydration()
    {
        while(true)
        {
            currentHydrationPercent-=1;
            yield return new WaitForSeconds(2f);
        }
    }
    void Update()
    {
        distanceTravelled+=Vector3.Distance(playerBody.transform.position,lastPosition);
        lastPosition=playerBody.transform.position;

        if(distanceTravelled>=5)
        {
            distanceTravelled=0;
            currentCalories-=1;
        }    
     
    }

    internal void SetHydrations(float maxHydrations)
    {
        currentHydrationPercent=maxHydrations;
    }

    internal void SetCalories(float maxCalories)
    {
        currentCalories=maxCalories;
    }

    internal void SetHealth(float maxHealth)
    {
        currentHealth=maxHealth;
    }
}
