using Godot;
using System;

public partial class City : Node2D
{
    Label xpLabel;
    public override void _Ready()
    {
        xpLabel = GetNode<Label>("CanvasLayer/XP_label");
        xpLabel.Text = "XP: " + GameData.Instance.PlayerXP;

        TextureButton exitButton = GetNode<TextureButton>("CanvasLayer/Exit/Button");
        exitButton.Pressed += (() => GetTree().Quit());

        TextureButton adventureButton = GetNode<TextureButton>("CanvasLayer/Adventure/Button");
        adventureButton.Pressed += (() => GetTree().ChangeSceneToFile("res://scenes/adventure.tscn"));

        TextureButton casinoButton = GetNode<TextureButton>("CanvasLayer/Casino/Button");
        casinoButton.Pressed += (() => GetTree().ChangeSceneToFile("res://scenes/casino.tscn"));

        TextureButton marketButton = GetNode<TextureButton>("CanvasLayer/Market/Button");
        marketButton.Pressed += (() => GetTree().ChangeSceneToFile("res://scenes/market.tscn"));

        TextureButton inventoryButton = GetNode<TextureButton>("CanvasLayer/Inventory/Button");
        inventoryButton.Pressed += (() => GetTree().ChangeSceneToFile("res://scenes/inventory.tscn"));
    }
}
