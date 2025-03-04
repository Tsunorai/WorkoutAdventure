using Godot;
using System;

public partial class ProgressBar : TextureProgressBar
{
	Label text;

	public override void _Ready()
	{
		text = GetNode<Label>("Label");
	}

	public void SetProgress(int value, int maxValue)
	{
		this.Value = value;
		this.MaxValue = maxValue;

		this.text.Text = $"{value}/{maxValue}";
	}
}
