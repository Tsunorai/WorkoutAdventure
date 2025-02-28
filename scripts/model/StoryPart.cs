using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkoutAdventure.scripts.model
{
    public partial class StoryPart : Godot.GodotObject
    {
        [JsonPropertyName("type")]
        public StoryType Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("xp")]
        public int XP { get; set; } = 0;

        [JsonPropertyName("loot")]
        public Loot[] Loot { get; set; } = Array.Empty<Loot>();
    }
}
