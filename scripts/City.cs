using Godot;
using System;

public partial class City : Node2D
{
    public override void _Ready()
    {
        TextureButton adventureButton = GetNode<TextureButton>("CanvasLayer/Adventure/Button");
        adventureButton.Pressed += (() => ChangeScene("adventure"));

        TextureButton casinoButton = GetNode<TextureButton>("CanvasLayer/Casino/Button");
        casinoButton.Pressed += (() => ChangeScene("casino"));
        
        TextureButton exitButton = GetNode<TextureButton>("CanvasLayer/Exit/Button");
        exitButton.Pressed += (() => GetTree().Quit());
        
        TextureButton marketButton = GetNode<TextureButton>("CanvasLayer/Market/Button");
        marketButton.Pressed += (() => ChangeScene("market"));
        
        TextureButton inventoryButton = GetNode<TextureButton>("CanvasLayer/Inventory/Button");
        inventoryButton.Pressed += (() => ChangeScene("inventory"));

        ProgressBar xpBar = GetNode<ProgressBar>("XPBar");
        xpBar.SetProgress(
            GameData.Instance.PlayerXP,
            (int) Math.Pow(1.1, GameData.Instance.PlayerLevel)
        );
    }

    private void ChangeScene(string path)
    {
        GetTree().ChangeSceneToFile($"res://scenes/{path}.tscn");
    }
}
