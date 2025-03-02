using Godot;
using System;

public partial class Inventory : Node2D
{
    ItemList items;
    public override void _Ready()
    {
        items = GetNode<ItemList>("ItemList");

        items.AddItem("Value   Rarity   Name");
        
        foreach (var item in GameData.Instance.Inventory)
        {
            items.AddItem($"{item.Value}   {item.Rarity}   {item.Name}");
        }

        Button button = GetNode<Button>("BackButton");
        button.Pressed += OnPressed;
    }

    public void OnPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/city.tscn");
    }
}