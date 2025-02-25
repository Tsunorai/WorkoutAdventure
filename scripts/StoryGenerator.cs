using Godot;
using Godot.Collections;
using System.Text;

public partial class StoryGenerator : Node
{
    [Export]
    private HttpRequest _httpRequest;

    [Signal]
    public delegate void LoadingEndedEventHandler(string[] storyData);

    public override void _Ready()
    {
        _httpRequest = GetNode<HttpRequest>("HTTPRequest");
        _httpRequest.RequestCompleted += OnRequestCompleted;
        GD.Print("Called 2");

        GenerateStory("Generate a Story with a Hero named Ardoin. And add some fitting workout tasks to the story but they shoudn't happen in the story just fit to the story. They are for the player of my game, because the idea behind the game is that the user exercises. Write 'Story: ' before each story block and 'Workout' before each workout block. Format it like a Json String Array that i can parse it as json.");
    }

    public void GenerateStory(string prompt)
    {
        string modelName = "Qwen/Qwen2.5-72B-Instruct";
        string url = $"https://api-inference.huggingface.co/models/{modelName}";
        string apiKey = "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";

        string[] headers = new string[] {
            $"Authorization: Bearer {apiKey}",
            "Content-Type: application/json"
        };

        Dictionary data = new();
        data["inputs"] = prompt;
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
                Array parsedResult = Json.ParseString(Encoding.UTF8.GetString(body)).AsGodotArray();

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
        Array jsonArray = Json.ParseString(jsonArrayString).AsGodotArray();

        // Output the parsed elements
        EmitSignal(SignalName.LoadingEnded, jsonArray);
    }
}
