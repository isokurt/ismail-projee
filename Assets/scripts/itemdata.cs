using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class itemdata : ScriptableObject
{
    public bool isplank;
    public bool isBackpack;
    public GameObject itemprefab;
    public string itemName;
    public Sprite icon;
    public int itemID;
    
}

    

