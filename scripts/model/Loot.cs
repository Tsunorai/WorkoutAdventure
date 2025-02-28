using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkoutAdventure.scripts.model
{
    public class Loot : Object
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("rarity")]
        public ItemRarity Rarity { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
