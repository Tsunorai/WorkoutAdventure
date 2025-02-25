using Godot;
using System;

public partial class GoToAdventure : GridContainer
{
    public override void _Ready()
    {
        TextureButton Button = GetNode<TextureButton>("Button");
        Button.Pressed += OnPressed;
    }

    private void OnPressed()
    {
        GD.Print("Called 1");
        SceneManager.Instance.ChangeSceneWithLoadingScreenAsync("res://scenes/adventure.tscn", "res://scenes/storygenerator.tscn");
    }
}
