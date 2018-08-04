using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public static List<BaseItem> itemList = new List<BaseItem>();
    public static Dictionary<int, Sprite> itemIcons = new Dictionary<int, Sprite>();
    private List<Slot> contents = new List<Slot>();
    public int maxWeight = 100;
    public float currentWeight;
    private int itemCount;
    public Transform container;

    public GameObject slotPrefab;
    public Text inventoryInfo;

    public Image testImage;

    public int ItemCount { get { return itemCount; } }

    private void Awake()
    {
        instance = this;    
        UpdateInfo();
    }

    public void AddItem(BaseItem item)
    {
        int i = FindItemIndex(item);

        if(i == -1)
        {
            contents.Add(CreateNewSlotWithItem(item));
            return;
        }
        
        if(contents[i].CanAddItem(item))
        {
            contents[i].AddItem();
        }
        else
        {
            contents.Add(CreateNewSlotWithItem(item));
        }

        UpdateInfo();
    }

    public static void RegisterItem(BaseItem item)
    {
        if(itemList.Contains(item) == false)
        {
            itemList.Add(item);
        }
    }
    public static BaseItem FetchItemByID(int id)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if(itemList[i].ID == id)
            {
                return itemList[i];
            }
        }

        return null;
    }

    private Slot CreateNewSlotWithItem(BaseItem item)
    {
        Slot slot = Instantiate(slotPrefab, container).GetComponent<Slot>();
        slot.SetItem(item);
        return slot;
    }

    private int FindItemIndex(BaseItem item)
    {
        for (int i = 0; i < contents.Count; i++)
        {
            if(contents[i].Item == item)
            {
                return i;
            }
        }

        return -1;
    }

    public void UpdateInfo()
    {
        if (inventoryInfo == null)
            return;

        inventoryInfo.text = "Weight: " + currentWeight + "/" + maxWeight + " Items: " + itemCount;
    }
}
