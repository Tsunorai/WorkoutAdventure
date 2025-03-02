using Godot;
using System;
using System.Collections.Generic;
using WorkoutAdventure.scripts.model;

public partial class Adventure : Node2D
{
    StoryPart lastStoryPart;
    StoryPart[] storyParts;
    int storyIndex = 0;

    private Button nextBtn;
    private RichTextLabel textLabel;
    private Sprite2D background;
    private Label xpCount;
    private Label itemCount;

    StoryGenerator storyGenerator;

    int collectedXP = 0;
    List<Loot> collectedLoot = new();

    public override void _Ready()
    {
        PackedScene storyGeneratorScene = ResourceLoader.Load<PackedScene>("res://scenes/storygenerator.tscn");
        storyGenerator = storyGeneratorScene.Instantiate<StoryGenerator>();
        AddChild(storyGenerator);
        storyGenerator.LoadingEnded += StartAdventure;

        xpCount = GetNode<Label>("XPCount");
        itemCount = GetNode<Label>("ItemCount");

        nextBtn = GetNode<Button>("NextBtn");

        textLabel = GetNode<RichTextLabel>("StoryText");
        background = GetNode<Sprite2D>("Background");
    }

    private void StartAdventure(StoryPart[] data)
    {
        RemoveChild(storyGenerator);

        storyParts = data;

        nextBtn.Pressed += OnPressNext;
        OnPressNext();
    }

    private void OnPressNext()
    {
        if (storyIndex == storyParts.Length)
        {
            GameData.Instance.PlayerXP += collectedXP;
            GameData.Instance.AddItems(collectedLoot);

            GetTree().ChangeSceneToFile("res://scenes/city.tscn");
        }
        else if (storyIndex == storyParts.Length - 1)
        {
            nextBtn.Text = "End Adventure";
        }


        ManageStoryPart(storyParts[storyIndex]);
    }

    private void ManageStoryPart(StoryPart part)
    {
        storyIndex++;

        if (lastStoryPart != null)
        {
            collectedXP += lastStoryPart.XP;
            collectedLoot.AddRange(lastStoryPart.Loot);
        }

        lastStoryPart = part;
        textLabel.Text = part.Text;

        xpCount.Text = $"XP: {collectedXP}";
        itemCount.Text = $"Its: {collectedLoot.Count}";
    }
}
