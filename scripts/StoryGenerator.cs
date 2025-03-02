using Godot;
using Godot.Collections;
using System;
using System.Text;
using System.Text.Json;
using WorkoutAdventure.scripts.model;
using System.Text.Json.Serialization;

public partial class StoryGenerator : Node
{
    [Export]
    private HttpRequest _httpRequest;

    [Signal]
    public delegate void LoadingEndedEventHandler(StoryPart[] storyData);

    string prompt = "Start prompt: You are a Story Generator for a fitness-themed adventure game. The objective is to create an engaging narrative that incorporates workout tasks into a storyline, ensuring that each workout is framed within the context of an adventure. The narrative should include elements of exploration, combat, and treasure hunting, all while maintaining a clear structure for parsing.  \n\nRequirements: \n\nStory Structure: The output should be a JSON array consisting of multiple objects. Each object must have a 'type' field that can be one of the following: Story, Workout, Fight, or Loot. \n\nObject Fields: \n\nFor type Story: \n\nInclude a ‘text’ field that contains the narrative description of the current scenario. \n\nFor type Workout: \n\nInclude a ‘text’ field that describes the specific workout task the adventurer must complete. Notice that the adventurer does not do the task. Just the player of the game. \n\nAdd an ‘xp’ field that indicates the experience points awarded for completing the task. XP should range from 10 to 200 based on the task's difficulty. \n\nFor type Fight or Loot: \n\nInclude a ‘text’ field that describes the context of the fight or loot scenario. Every fight and loot scenario must contain a workout task. So, add this to the end of the ‘text’ field. \n\nInclude an ‘xp’ field as described above. XP can only be earned trough a workout task. \n\nAdd a loot field that is an array of objects, which can include: \n\nItems and Coins \n\nEach item should have: \n\nname: The name of the item. \n\nrarity: One of the following values: Common, Uncommon, Rare, Epic, Legendary. \n\nvalue: The value of the item in coins. \n\nThe Coins: Represented as an item object with: \n\nname: Set to \"Coin\". \n\nvalue: The amount of coins awarded, which should range from 5 to 500 depending on the task. \n\nTask Relevance: Ensure that each workout task logically fits within the context of the ongoing adventure. For instance, if the adventurer is fighting an enemy, the workout should be related to strength. Do not use '. \n\nDo only return the ONE JSON array with no commentary. End prompt.";

    public override void _Ready()
    {
        _httpRequest = GetNode<HttpRequest>("HTTPRequest");
        _httpRequest.RequestCompleted += OnRequestCompleted;

        GenerateStory();
    }

    public void GenerateStory()
    {
        string modelName = "Qwen/Qwen2.5-72B-Instruct";
        string url = $"https://api-inference.huggingface.co/models/{modelName}";
        string apiKey = "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";

        string[] headers = new string[]
        {
            $"Authorization: Bearer {apiKey}",
            "Content-Type: application/json"
        };

        Dictionary data = new();
        data["inputs"] = prompt + " Ignore: " + System.DateTime.Now.Ticks + ".";
        Json json = new();

        string jsonData = Json.Stringify(data);

        GD.Print("RequestData: ", jsonData);
        Error err = _httpRequest.Request(url, headers, Godot.HttpClient.Method.Post, jsonData);
        if (err != Error.Ok)
        {
            GD.PrintErr("Error making request: " + err);
        }
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        GD.Print("ResponseCode: ", responseCode);
        if (responseCode == 200)
        {
            // Convert the response byte array into a UTF8 string.
            string responseText = Encoding.UTF8.GetString(body);
            FileAccess file = FileAccess.Open($"C:/dev/Godot/projects/workoutadventure/userFiles/success/{System.DateTime.Now.Ticks}.txt", FileAccess.ModeFlags.Write);
            file.StoreString(responseText);
            file.Close();
            GD.Print("Response written to file");
            Json json = new();
            Error jsonResult = json.Parse(responseText);
            if (jsonResult == Error.Ok)
            {
                Godot.Collections.Array parsedResult = Json.ParseString(responseText).AsGodotArray();

                if (parsedResult != null || parsedResult.Count == 0)
                {
                    string[] generatedStory = new string[parsedResult.Count];
                    foreach (Variant part in parsedResult)
                    {
                        var dataDict = (Dictionary)part;
                        if (dataDict != null && dataDict.ContainsKey("generated_text"))
                        {
                            string generatedText = (string)dataDict["generated_text"];

                            GetStoryArray(generatedText, false);
                            return;
                        }
                    }
                }
            }
            else
            {
                GD.PrintErr("JSON parse error: " + jsonResult);
            }
        }
        else
        {
            GD.Print("Request failed with response code: " + responseCode);
            FileAccess file = FileAccess.Open($"C:/dev/Godot/projects/workoutadventure/userFiles/errors/{System.DateTime.Now.Ticks}.txt", FileAccess.ModeFlags.Write);
            file.StoreBuffer(body);
            file.Close();

            GD.Print("Written Error in File");
        }

        LoadOldStory();
    }

    private void LoadOldStory()
    {
        string directoryPath = @"C:/dev/Godot/projects/workoutadventure/userFiles/story";

        string fileContent;
        if (System.IO.Directory.Exists(directoryPath))
        {
            string[] files = System.IO.Directory.GetFiles(directoryPath);
            if (files.Length > 0)
            {
                Random random = new();
                string randomFile = files[random.Next(files.Length)];
                fileContent = System.IO.File.ReadAllText(randomFile);

                GetStoryArray(fileContent, true);
            }
        }
    }

    private void GetStoryArray(string response, bool isOldStory)
    {
        if (!isOldStory)
        {
            int startIndex = response.IndexOf('[');
            int endIndex = response.LastIndexOf(']');
            if (startIndex == -1 || endIndex == -1 || endIndex < startIndex)
            {
                GD.Print("JSON array not found in the response.");
                return;
            }


            // Parse the JSON array using Godot's JSON parser
            Json json = new();
            Error parseResult = json.Parse(response);
            if (parseResult != Error.Ok)
            {
                GD.Print("Error parsing JSON: " + parseResult);
                return;
            }

            // Extract the substring that represents the JSON array
            response = response.Substring(startIndex, endIndex - startIndex + 1);
            FileAccess file = FileAccess.Open($"C:/dev/Godot/projects/workoutadventure/userFiles/story/{System.DateTime.Now.Ticks}.txt", FileAccess.ModeFlags.Write);
            file.StoreString(response);
            file.Close();
            GD.Print("Story saved");
        }
        string[] jsonArray = Json.ParseString(response).AsStringArray();


        StoryPart[] storyParts = ParseStories(jsonArray);

        // Output the parsed elements
        EmitSignal(SignalName.LoadingEnded, storyParts);
    }

    public StoryPart[] ParseStories(string[] jsonArray)
    {
        StoryPart[] stories = new StoryPart[jsonArray.Length];

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        for (int i = 0; i < jsonArray.Length; i++)
        {
            try
            {
                StoryPart part = JsonSerializer.Deserialize<StoryPart>(jsonArray[i], options);
                if (part != null)
                {
                    stories[i] = part;
                }
                else
                {
                    GD.PrintErr("\nFailed to deserialize JSON: " + jsonArray[i]);
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr("\nException while parsing JSON: ", ex.Message, "\n" + jsonArray[i]);
                break;
            }
        }
        GD.Print("Json converted");

        return stories;
    }
}
