using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int index;
    private Image itemImage;
    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        index = Int32.Parse(transform.gameObject.name.Substring(8));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("The cursor entered the selectable UI element.");
        inventory.ShowToolTip(eventData.position, index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("The cursor exited the selectable UI element.");
        inventory.HideToolTip();
    }

    public void SetSprite(string name)
    {
        //Debug.Log("Set Sprite begin");
        Sprite sp = Resources.Load<Sprite>(name);
        if(sp==null)
        {
            Debug.Log("Sprite not found");
        }
        itemImage = transform.Find("ItemImage").GetComponent<Image>();
        itemImage.sprite = sp;

        Color tmp = itemImage.color;
        tmp.a = 1f;
        itemImage.color = tmp;
    }

}
