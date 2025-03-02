using Godot;
using System;

public partial class Exit : GridContainer
{
	public override void _Ready()
	{
        TextureButton Button = GetNode<TextureButton>("Button");
        Button.Pressed += OnPressed;
    }

    private void OnPressed()
    {
        GetTree().Quit();
    }
}
