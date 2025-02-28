using Godot;
using Godot.Collections;
using System.Collections.Generic;
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

    string prompt = "You are a Storygenerator for a game. The game is about a workout. But because a sole workout can be boring, it should be embedded in a story and should have a warm-up and exclude meditation exercises. The story should consist of adventure, fight, explore, and loot tasks. This means a workout task can involve fighting enemies, traveling to a location, or opening and looting something.\n\nThe story has to be an array of parsable JSON objects. Each object has a type which is one of the following: Story, Workout, Fight, Loot.\n\nIf the type is Story, include the text in the field 'text'.\nIf the type is Workout, the task should be in the field 'text'. It should also have a field 'xp' which contains a number indicating how much XP the task gives.\nIf the type is Fight or Loot, do the same thing as in Workout and add an additional array of objects 'loot'. The Loot object can be Items or Coins. It contains the 'name', 'rarity', and 'value'. If it is coins, write 'Coin' in the 'name' field and the amount of coins in the 'value' field. The value of the items should be calculated in coins.\nThe rarity of the loot items should be one of: Common, Uncommon, Rare, Epic, Legendary.\nXP can range from 10 to 200 depending on the task. \nCoins can range from 5 to 500 depending on the task.\n\nDO NOT INCLUDE ANY NOT MENTIONED FIELD OR TYPE.";

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
            "Content-Type: application/json",
            "Cache-Control: no-store"
        };

        Dictionary data = new();
        data["inputs"] = prompt + " " + System.DateTime.Now.Ticks;
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
            Json json = new();
            Error jsonResult = json.Parse(responseText);
            if (jsonResult == Error.Ok)
            {
                Godot.Collections.Array parsedResult = Json.ParseString(Encoding.UTF8.GetString(body)).AsGodotArray();

                if (parsedResult != null || parsedResult.Count == 0)
                {
                    string[] generatedStory = new string[parsedResult.Count];
                    foreach (Variant part in parsedResult)
                    {
                        var dataDict = (Dictionary)part;
                        if (dataDict != null && dataDict.ContainsKey("generated_text"))
                        {
                            string generatedText = (string)dataDict["generated_text"];
                            GetStoryArray(generatedText);
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
        }
    }

    private void GetStoryArray(string response)
    {
        int startIndex = response.IndexOf('[');
        int endIndex = response.LastIndexOf(']');
        if (startIndex == -1 || endIndex == -1 || endIndex < startIndex)
        {
            GD.Print("JSON array not found in the response.");
            return;
        }

        // Extract the substring that represents the JSON array
        string jsonArrayString = response.Substring(startIndex, endIndex - startIndex + 1);

        // Parse the JSON array using Godot's JSON parser
        Json json = new();
        Error parseResult = json.Parse(jsonArrayString);
        if (parseResult != Error.Ok)
        {
            GD.Print("Error parsing JSON: " + parseResult);
            return;
        }

        // The parsed result is expected to be a Godot.Collections.Array
        string[] jsonArray = Json.ParseString(jsonArrayString).AsStringArray();

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
                GD.PrintErr("\nException while parsing JSON: ", ex.Message);
            }
        }
        GD.Print("Json converted");

        return stories;
    }
}
