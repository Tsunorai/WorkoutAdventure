using Godot;
using System;

public partial class Adventure : Node2D
{
    String[] titles = {
        "Chapter 1: The Call to Adventure",
        "Chapter 2: The Enchanted Forest",
        "Chapter 3: The Ruins of Eldoria",
        "Chapter 4: The Final Battle",
        "Epilogue"
    };
    String[] text = {
        "Arin stood at the edge of the village, gazing at the steep mountain that marked the beginning of their quest. The elder's words echoed in their mind: \"To ascend the Mountain of Trials, you must first prove your strength and endurance.\"\r\n\r\nChallenge: Scale the Mountain\r\n\r\nExercise: Perform mountain climbers for 30 seconds.\r\nReward: 50 XP",
        "Arin dropped to the ground, hands and feet moving in a rapid rhythm, mimicking the climb they would soon undertake. Each movement brought them closer to the summit, their muscles burning with determination.\nWith the challenge complete, Arin felt a surge of energy and confidence. The path ahead was treacherous, but they knew each step would bring them closer to their destiny.",
        "The dense foliage of the Enchanted Forest whispered secrets as Arin ventured deeper. Suddenly, a locked treasure chest appeared, guarded by a mystical barrier. To unlock it, Arin needed to demonstrate their physical prowess.\r\n\r\nChallenge: Unlock the Treasure Chest\r\n\r\nExercise: Complete 15 push-ups.\r\nReward: 30 XP",
        "Arin dropped to the forest floor, pushing their body up and down with steady breaths. Each push-up felt like a key turning in the lock, and with the final repetition, the barrier dissolved.\nInside the chest, Arin found a magical amulet that would aid them in their journey. They slipped it around their neck, feeling its power resonate with their own.",
        "The ancient ruins loomed ahead, a labyrinth of stone and shadow. Arin knew that to navigate this maze, they would need both speed and agility.\r\n\r\nChallenge: Navigate the Ruins\r\n\r\nExercise: Sprint for 1 minute.\r\nReward: 40 XP",
        "Arin took off, their feet pounding against the ground as they weaved through the ruins. The wind rushed past, and the ruins blurred into a haze of motion. As the minute ended, Arin skidded to a halt, heart racing.\nThe ruins revealed a hidden passage, leading to the heart of Eldoria's magic. Arin pressed on, each step fueled by the knowledge that their physical efforts were unlocking the secrets of the realm.",
        "At last, Arin stood before the gates of the mystical castle, where the final challenge awaited. The guardian of the castle, a formidable foe, emerged from the shadows.\r\n\r\nChallenge: Defeat the Guardian\r\n\r\nExercise: Perform 20 burpees.\r\nReward: 60 XP",
        "Arin braced themselves, launching into a series of burpees. Each jump and push-up felt like a strike against the guardian, their body moving with precision and power. With the final burpee, the guardian fell, defeated.\nThe gates of the castle swung open, revealing the source of Eldoria's magic. Arin stepped inside, their journey complete. They had restored balance to the realm, their physical challenges transforming them into a true hero.",
        "Arin returned to Loria, greeted by cheers and celebrations. The village elder smiled, knowing that the prophecy had been fulfilled. Arin's journey had not only saved Eldoria but had also shown the power of physical strength and determination.\r\n\r\nAnd so, the legend of Arin, the hero who conquered Eldoria through fitness and courage, was passed down through generations, inspiring others to embark on their own quests of strength and adventure."
    };


    int currentIndex = -1;
    int storyIndex = 0;

    private Button nextBtn;
    private Button doneBtn;
    private RichTextLabel textLabel;
    private Label title;
    private Sprite2D background;

    public override void _Ready()
    {
        nextBtn = GetNode<Button>("NextBtn");
        nextBtn.Pressed += OnPressNext;

        doneBtn = GetNode<Button>("DoneBtn");
        doneBtn.Pressed += OnPressDone;
        doneBtn.Disabled = true;

        title = GetNode<Label>("Title");
        textLabel = GetNode<RichTextLabel>("StoryText");
        background = GetNode<Sprite2D>("Background");
    }

    private void OnPressDone()
    {
        doneBtn.Disabled = true;
        nextBtn.Disabled = false;

        storyIndex++;
        UpdateStory();
    }

    private void OnPressNext()
    {
        nextBtn.Disabled = true;
        doneBtn.Disabled = false;

        currentIndex++;

        if (currentIndex == titles.Length - 1)
        {
            nextBtn.Disabled = false;
            nextBtn.Text = "End Adventure";
            RemoveChild(doneBtn);
        }
        else if (currentIndex == titles.Length)
        {
            GameData.Instance.PlayerXP += 50;

            GetTree().ChangeSceneToFile("res://scenes/city.tscn");
        }

        title.Text = titles[currentIndex];
        UpdateStory();
    }

    private void UpdateStory()
    {
        textLabel.Text = text[currentIndex + storyIndex];
        background.Texture = GD.Load<Texture2D>("res://assets/adventure/img" + (currentIndex + storyIndex) + ".png");
    }
}
