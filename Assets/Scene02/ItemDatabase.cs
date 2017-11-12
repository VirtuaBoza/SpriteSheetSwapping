using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Dictionary<int, Item> itemsDictionary;

    private JsonData itemData;

    void Start()
    {
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));

        itemsDictionary = new Dictionary<int, Item>();
        ConstructItemDatabase();
    }

    private void ConstructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            itemsDictionary.Add((int)itemData[i]["ID"],
                new Item((int)itemData[i]["ID"], 
                itemData[i]["SpriteSheetName"].ToString(), 
                itemData[i]["EquipType"].ToString()));
        }
    }
}