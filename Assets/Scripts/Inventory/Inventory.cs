using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using System;

public enum Item_Slot_Category
{
    Backpack,
    Armor,
    Boots,
    Helmet,
    Necklace,
    Ring,
    Weapon
}


public class Inventory : MonoBehaviour
{
    private GameServerMock GameServerMock;
    private Item_Slot_Category Slot_Category;
    private List<Item_Data> Items;
    [SerializeField] private RectTransform Backpack_Inventory_Rect;
    [SerializeField] private GameObject Item_Slot;
    [SerializeField] private GameObject Item;
    [SerializeField] private Canvas Main_Canvas;
    [SerializeField] private GameObject Item_Draging_Parent;
    [SerializeField] private Player_Main Player_Main;
    [SerializeField] private List<Item_Data> Equiped_Items;
    [SerializeField] private TextMeshProUGUI DMG_Text;
    [SerializeField] private GameObject EQ_Loading;

    public static bool Data_Lodaed;

    private async void Start()
    {
        GameServerMock GameServerMock = new GameServerMock();
        CancellationTokenSource cts = new CancellationTokenSource();
        Data_Lodaed = false;

        try
        {
            Debug.Log("Pobieranie Danych");
            string json = await GameServerMock.GetItemsAsync(cts.Token);

            Debug.Log("Pobieranie Danych");


            JObject Parsed_Data = JObject.Parse(json);
            JArray Items_Array = (JArray)Parsed_Data["Items"];

            

            Debug.Log("Pobieranie Danych");



           // Items = await DeserializeItemsAsync(json);

            Items = Items_Array.ToObject<List<Item_Data>>();


            Equiped_Items = new List<Item_Data>();

            

            Create_Items_In_Chunks(Items);

            Vector2 current_Size = Backpack_Inventory_Rect.sizeDelta;
            Backpack_Inventory_Rect.sizeDelta = new Vector2(current_Size.x, (Items.Count / 10 + 1) * 160);

            Data_Lodaed = true;
            Debug.Log("All items loaded!");
            EQ_Loading.SetActive(false);

        }
        catch (OperationCanceledException)
        {
            Debug.Log("Item fetch was canceled.");
        }
    }

    private void Create_Items_In_Chunks(List<Item_Data> items)
    {
        int chunkSize = 10;
        int totalChunks = Mathf.CeilToInt((float)items.Count / chunkSize);

        for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            int start = chunkIndex * chunkSize;
            int end = Mathf.Min(start + chunkSize, items.Count);


            for (int i = start; i < end; i++)
            {
                Item_Data item_ref = items[i];


                GameObject item_slot = Instantiate(Item_Slot);
                item_slot.GetComponent<Item_Slot>().Inventory = this;
                GameObject _item = Instantiate(Item);
                item_slot.GetComponent<Item_Slot>().Item = _item;

                Item Item_Script = _item.GetComponent<Item>();
                Item_Script.Main_Canvas = Main_Canvas;
                Item_Script.Item_Draging_Parent = Item_Draging_Parent;
                Item_Script.Last_Item_Slot = item_slot.gameObject;
                Item_Script.item_data = item_ref;

                string Category = item_ref.Category;
                string prefix = item_ref.Name.Replace(Category, "");
                string assetPath = "Assets/Textures/Items" + "/" + Category + "/" + prefix + "" + Category + ".png";

                Load_Item_Sprite_Async(Item_Script, assetPath);

                item_slot.transform.SetParent(Backpack_Inventory_Rect.gameObject.transform, false);
                _item.transform.SetParent(item_slot.transform, false);
                _item.transform.localPosition = Vector3.zero;
            }

            Task.Yield();
        }
    }

    private void Load_Item_Sprite_Async(Item itemScript, string assetPath)
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(assetPath);

        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                itemScript.Item_Image.sprite = op.Result;
            }
            else
            {
                Debug.LogError("Item sprite was not found: " + assetPath);
            }

            // Release the handle after usage to prevent memory leaks
            Addressables.Release(op);
        };
    }
    public void Add_Item(Item_Data item_Data)
    {
        Equiped_Items.Add(item_Data);
        Update_Stats();
    }

    public void Subtract_Item(Item_Data item_Data)
    {
        Equiped_Items.Remove(item_Data);
        Update_Stats();
    }

    private void Update_Stats()
    {
        int DMG_Value = 5;

        foreach(var item in Equiped_Items)
        {
            DMG_Value += item.Damage;
        }

        DMG_Text.text = DMG_Value.ToString() + " DMG";
        Player_Main.Update_Stats(DMG_Value);
    }

}
public class Item_Data
{
    public string Name { get; set; }
    public string Category { get; set; }
    public int Rarity { get; set; }
    public int Damage { get; set; }
    public int Health_Points { get; set; }
    public int Defense { get; set; }
    public float Life_Steal { get; set; }
    public float Critical_Strike_Chance { get; set; }
    public float Attack_Speed { get; set; }
    public float Movement_Speed { get; set; }
    public float Luck { get; set; }

}