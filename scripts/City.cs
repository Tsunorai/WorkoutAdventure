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

        CalculateLevel();
    }

    private void CalculateLevel()
    {
        ProgressBar xpBar = GetNode<ProgressBar>("XPBar");

        GameData gameData = GameData.Instance;

        int maxXP = 1000 * (int)Math.Pow(0.1, gameData.PlayerLevel - 1);

        while (gameData.PlayerXP >= maxXP)
        {
            gameData.PlayerLevel++;
            gameData.PlayerXP -= maxXP;
            maxXP = (int)(1000 * Math.Pow(1.1, gameData.PlayerLevel - 1));
        }
        xpBar.SetProgress(gameData.PlayerXP, maxXP);

        Label xpLabel = GetNode<Label>("XPBar/XPLabel");
        xpLabel.Text = $"Lvl: {gameData.PlayerLevel}";
    }

    private void ChangeScene(string path)
    {
        GetTree().ChangeSceneToFile($"res://scenes/{path}.tscn");
    }
}
