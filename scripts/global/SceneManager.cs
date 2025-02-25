using Godot;
using System;

public partial class SceneManager : Node
{

    public static SceneManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public async void ChangeSceneWithLoadingScreenAsync(string scenePath, string loadingScenePath)
    {
        PackedScene loadingPacked = (PackedScene)ResourceLoader.Load(loadingScenePath);
        Node loadingInstance = loadingPacked.Instantiate();
        AddChild(loadingInstance);

        await ToSignal(loadingInstance, "LoadingEnded");

        ChangeScene(scenePath);

        // Optionally remove the loading screen after the scene has switched.
        if (loadingInstance != null && loadingInstance.IsInsideTree())
        {
            loadingInstance.QueueFree();
        }
    }

    public void ChangeScene(string scenePath)
    {
        Error err = GetTree().ChangeSceneToFile(scenePath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Error loading scene {scenePath}: {err}");
        }
    }
}
