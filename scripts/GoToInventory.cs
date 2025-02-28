using Godot;
using System;

public partial class GoToInventory : GridContainer
{
    public override void _Ready()
    {
        TextureButton Button = GetNode<TextureButton>("Button");
        Button.Pressed += OnPressed;
    }

    private void OnPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/inventory.tscn");
    }
}
