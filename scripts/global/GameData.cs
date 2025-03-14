using Godot;
using System.Collections.Generic;
using System.Linq;
using WorkoutAdventure.scripts.model;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }


    public int PlayerXP = 0;
    public int PlayerLevel = 1;

    public List<Loot> Inventory = new();
    public override void _Ready()
    {
        Instance = this;
    }

    public void AddItems(List<Loot> newItems)
    {
        foreach (var newItem in newItems)
        {
            if (newItem.Name.Equals("Coin"))
            {
                Loot coinItem = Inventory.FirstOrDefault(item => item.Name.Equals("Coin"));
                if (coinItem != null)
                {
                    coinItem.Value += newItem.Value;
                }
                else
                {
                    Inventory.Add(newItem);
                }
            }
            else
            {
                Inventory.Add(newItem);
            }
        }
    }
}
