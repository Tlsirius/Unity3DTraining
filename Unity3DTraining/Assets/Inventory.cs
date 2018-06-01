using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public enum ItemIdx
    {
        Amulet,
        NumOfItems
    };

    private ItemSlot[] itemSlots = new ItemSlot[numItemSlots];
    private List<Item> items = new List<Item>();

    private Sprite[] sprites = new Sprite[(int)ItemIdx.NumOfItems];

    public string[] descriptions =
    {
        "An amulet that brings good luck.",
    };

    public string[] effects =
    {
        "Slightly improves your furtune.",
    };

    public const int numItemSlots = 4;

    //private int ObtainedItem = 0;
    private GameObject tooltip;
    private Text tooltipName;
    private Text tooltipDescription;
    private Text tooltipEffect;

    private Canvas canvasItem;
    private float width;
    private float height;

    private int delay = 0;

    private void Start()
    {
        InitializeItems();
        canvasItem = GameObject.Find("CanvasItem").GetComponent<Canvas>();

        tooltip = GameObject.Find("Tooltip");
        tooltipName = tooltip.transform.Find("Name").GetComponent<Text>();
        tooltipDescription = tooltip.transform.Find("Description").GetComponent<Text>();
        tooltipEffect = tooltip.transform.Find("Effect").GetComponent<Text>();

        RectTransform rect = tooltip.GetComponent<RectTransform>();
        width = 0.5f * rect.rect.width * rect.localScale.x;
        height = 0.5f * rect.rect.height * rect.localScale.y;
        Debug.Log("width:" + width.ToString());
        Debug.Log("height:" + height.ToString());

        InitializeItemSlots();
        HideToolTip();
        canvasItem.enabled = false;

        AddItem(ItemIdx.Amulet);
    }

    void Update()
    {
        if(delay>0)
        {
            delay--;
        }
        else if (Input.anyKey)
        {
            if (Input.GetKey("i") && canvasItem.enabled == false)
            {
                canvasItem.enabled = true;
                delay=10;
            }
            else if (Input.GetKey("i") && canvasItem.enabled == true)
            {
                canvasItem.enabled = false;
                delay = 10;
            }
        }

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
        if(CanvasItem==null)
        {
            Debug.Log("CanvasItem not found!");
        }
        for(int i=0;i<numItemSlots;i++)
        {
            string name = "ItemSlot" + i;
            itemSlots[i] = CanvasItem.transform.Find(name).GetComponent<ItemSlot>();
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
    
    public Item GetItem(int idx)
    {
        if(idx>=0&&idx<items.Count)
        {
            return items[idx];
        }
        return null;
    }


    public void ShowToolTip(Vector3 position, int index)
    {
        if(index>=(int)ItemIdx.NumOfItems)
        {
            return;
        }
        tooltip.SetActive(true);
        //Debug.Log("Before:" + position.ToString());
        Vector3 leftTop = position;
        leftTop.x = leftTop.x + 1.1f*width;
        leftTop.y = leftTop.y - 1.1f*height;
        tooltip.transform.position = leftTop;
        tooltipName.text = items[index].itemName;
        tooltipDescription.text = items[index].description;
        tooltipEffect.text = items[index].effects;
        //Debug.Log("After:" + leftTop.ToString());

    }

    public void HideToolTip()
    {
        tooltip.SetActive(false);
    }
}
