using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public enum ItemIdx
    {
        Amulet,
        //MicrohardIDCard,
        NumOfItems
    };

    private ItemSlot[] itemSlots = new ItemSlot[numItemSlots];
    private List<Item> items = new List<Item>();

    private Sprite[] sprites = new Sprite[(int)ItemIdx.NumOfItems];

    public string[] descriptions =
    {
        "An amulet that could reduce bugs in your code.",
        "Microhard company ID card"
    };

    public string[] effects =
    {
        "Slightly improves (?) your programming skills.",
        "Grant you access to Microhard company building."
    };

    public const int numItemSlots = 20;

    //private int ObtainedItem = 0;
    private GameObject tooltip;

    private void Start()
    {
        InitializeItems();

        Debug.Log("item count before add:" + items.Count);
        AddItem(ItemIdx.Amulet);
        Debug.Log("item count after add:" + items.Count);
    }

    private void InitializeItems()
    {
        ItemIdx idx;
        //Create all sprites
        for(int i=0;i<(int)ItemIdx.NumOfItems;i++)
        {
            idx = (ItemIdx)i;
            sprites[i] = Resources.Load<Sprite>(idx.ToString());
        //    items.Add(new Item(idx.ToString(), descriptions[i], effects[i],sp));
        }
    }

    public void InitializeItemSlots()
    {
        GameObject CanvasItem = transform.Find("CanvasItem").gameObject;
        GameObject ItemsGroup = CanvasItem.transform.Find("ItemsGroup").gameObject;
        for(int i=0;i<numItemSlots;i++)
        {
            string name = "ItemSlot" + i;
            itemSlots[i] = ItemsGroup.transform.Find(name).GetComponent<ItemSlot>();
        }
    }

    public void ShowItemSprites()
    {
        for(int i=0;i<items.Count;i++)
        {
            ItemIdx idx = (ItemIdx)i;
            itemSlots[i].SetSprite(idx.ToString());
        }
    }

    public void AddItem(ItemIdx itemToAdd)
    {
        if(CheckItem(itemToAdd))
        {
            Debug.Log("Item already obtained!");
        }
        //Debug.Log("item To Add:" + itemToAdd.ToString());
        //Sprite sp = Resources.Load<Sprite>(name);
        //itemSlots[ObtainedItem].SetSprite(Enum.GetName(typeof(ItemIdx), itemToAdd));
        //items[(int)itemToAdd].obtained = true;
        items.Add(new Item(itemToAdd.ToString(), descriptions[(int)itemToAdd], effects[(int)itemToAdd], sprites[(int)itemToAdd]));
    }

    public bool CheckItem(ItemIdx itemToCheck)
    {
        //Debug.Log("item To Check:" + itemToCheck.ToString());
        //Debug.Log("item count:" + items.Count);
        for (int i=0;i<items.Count;i++)
        {
            if(items[i].name == itemToCheck.ToString())
            {
                return true;
            }
        }
        return false;
    }

    /*
    public void RemoveItem (Item itemToRemove)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = null;
                itemImages[i].sprite = null;
                itemImages[i].enabled = false;
                return;
            }
        }
    }
    */

    public Item GetItem(int idx)
    {
        if(idx>=0&&idx<items.Count)
        {
            return items[idx];
        }
        return null;
    }
}
