using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item_Slot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Item_Data item_data;
    public Item_Slot_Category Item_Slot_Category;
    public Inventory Inventory;
    public GameObject Item;

    public void OnDrop(PointerEventData Event_Data)
    {

        if (Item_Slot_Category != Item_Slot_Category.Backpack && Item_Slot_Category.ToString() != Event_Data.pointerDrag.GetComponent<Item>().item_data.Category)
        {
            Debug.Log("Item does not fit");
            return;
        }

        if (Item != null)
        {
            Debug.Log("Another item present");
            return;
        }
        if (Event_Data.pointerDrag.tag == "Item")
        {
            Event_Data.pointerDrag.GetComponent<Item>().Last_Item_Slot = gameObject;
            Event_Data.pointerDrag.transform.SetParent(transform);
            Event_Data.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }    
        if (item_data == null)
        {
            item_data = Event_Data.pointerDrag.GetComponent<Item>().item_data;
            Add_Item(item_data, Event_Data.pointerDrag);
        }
        
    }

    public void Add_Item(Item_Data data, GameObject item)
    {
        Item = item;
        if (Item_Slot_Category != Item_Slot_Category.Backpack)
        {
            item_data = data;
            Inventory.Add_Item(item_data);
        }
        
    }

    public void Subtract_Item()
    {
        Item = null;
        if (Item_Slot_Category != Item_Slot_Category.Backpack)
        {
            Inventory.Subtract_Item(item_data);
            item_data = null;
        }
    }
}
