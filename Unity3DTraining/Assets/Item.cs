using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public string itemName;
    public string description;
    public string effects;
    public bool obtained = false;

    public Item(string _itemName, string _description, string _effects, Sprite _sprite)
    {
        itemName = _itemName;
        description = _description;
        effects = _effects;
        sprite = _sprite;
    }
}
