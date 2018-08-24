using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private BaseItem _item;
    private float slotWeight;
    private int currentStack;

    private RectTransform rect;

    public Text itemName;
    public Text itemAmount;
    public Sprite itemIcon;
    private Image img;
    public Sprite slotImage;
    public Sprite slotHoverImage;

    public BaseItem Item { get { return _item; } }

    private void Start()
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public int StackSize { get { return currentStack; } }

    public void SetItem(BaseItem item)
    {
        _item = item;
        currentStack++;
        slotWeight += _item.Weight;
        InventoryManager.instance.currentWeight += _item.Weight;
    }
    public void AddItem()
    {
        currentStack++;
        slotWeight += _item.Weight;
        InventoryManager.instance.currentWeight += _item.Weight;
    }
    public bool CanAddItem(BaseItem itemToAdd)
    {
        if (_item == null)
            return false;
        if (itemToAdd != _item)
            return false;
        if (currentStack >= _item.MaxStack)
            return false;
        if ((InventoryManager.instance.currentWeight + itemToAdd.Weight) > InventoryManager.instance.maxWeight)
            return false;

        return true;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public void RemoveItem()
    {
        if (currentStack > 1)
        {
            currentStack--;
            return;
        }

        Destroy();

    }
    public void UseItem(ViewController ply)
    {
        _item.Use(ply);
        RemoveItem();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.sprite = slotHoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.sprite = slotImage;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        img.sprite = slotImage;
        _item.Use(ViewController.instance);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        img.sprite = slotHoverImage;
    }
}
