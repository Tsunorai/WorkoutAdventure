using Godot;
using System;

public partial class Casino : GridContainer
{
    [Export]
    PackedScene NotImplemented;

    Timer ShowError = new();
    Node Instance;

    public override void _Ready()
    {
        TextureButton Button = GetNode<TextureButton>("Button");
        Button.Pressed += OnPressed;
        NotImplemented = GD.Load<PackedScene>("res://scenes/notImplemented.tscn");
    }

    private void OnPressed()
    {
        Instance = NotImplemented.Instantiate();
        AddChild(Instance);
        ShowError.WaitTime = 2;
        AddChild(ShowError);
        ShowError.Start();
        ShowError.Timeout += OnTimeout;
    }

    private void OnTimeout()
    {
        RemoveChild(Instance);
        RemoveChild(ShowError);
    }
}
