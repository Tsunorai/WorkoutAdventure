using Godot;
using System.Collections.Generic;
using WorkoutAdventure.scripts.model;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }

    public int PlayerXP = 0;
    public List<Loot> Inventory = new();
    public override void _Ready()
    {
        Instance = this;
    }
}
