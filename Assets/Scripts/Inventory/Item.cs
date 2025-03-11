using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Item : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Canvas Main_Canvas;
    private RectTransform Rect_Transform;
    private CanvasGroup Canvas_Group;
    public GameObject Item_Draging_Parent;
    public GameObject Last_Item_Slot;
    public Item_Data item_data;
    public Image Item_Image;

    private void Awake()
    {
        Rect_Transform = GetComponent<RectTransform>();
        Canvas_Group = GetComponent<CanvasGroup>();
        Item_Image = GetComponent<Image>();

    }


    public void OnBeginDrag(PointerEventData Event_Data)
    {

        Last_Item_Slot.GetComponent<Item_Slot>().Subtract_Item();

        gameObject.transform.SetParent(Item_Draging_Parent.transform);

        Canvas_Group.blocksRaycasts = false;
        
    }

    public void OnDrag(PointerEventData Event_Data)
    {
        Rect_Transform.anchoredPosition += Event_Data.delta / Main_Canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData Event_Data)
    {
        Canvas_Group.blocksRaycasts = true;
        if (gameObject.transform.parent == Item_Draging_Parent.transform)
        {
            Last_Item_Slot.GetComponent<Item_Slot>().Add_Item(item_data, gameObject);




            gameObject.transform.SetParent(Last_Item_Slot.transform);
            Rect_Transform.anchoredPosition = Vector3.zero;


        }
    }
    public void OnPointerDown(PointerEventData Event_Data)
    {

    }

}
