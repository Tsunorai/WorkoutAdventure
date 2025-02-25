using Godot;
using System;

public partial class City : CanvasLayer
{
    Label xPLabel;
    public override void _Ready()
    {
        xPLabel = GetNode<Label>("XP_label");
        xPLabel.Text = "XP: " + GameData.Instance.PlayerXP;
    }
}
