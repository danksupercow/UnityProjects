using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    private List<Slot> contents = new List<Slot>();
    public int maxWeight = 100;
    public int currentWeight;
    public Transform container;

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(BaseItem item)
    {
        
    }
}
