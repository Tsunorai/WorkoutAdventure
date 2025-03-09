using System;
using System.IO;
using Godot;

public partial class DiskManager : Node
{
    public static DiskManager Instance { get; private set; }

    private string basePath = ProjectSettings.GlobalizePath("res://userFiles");

    public override void _Ready()
    {
        Instance = this;
        CheckOrCreateAbsolutePath(basePath);
    }

    public void SaveData(string fileName, string data)
    {
        // Create the full file path from the basePath and relative fileName.
        string filePath = Path.Join(basePath, fileName);
        // Ensure that the directory for the file exists.
        CheckOrCreateRelativePath(fileName);

        // Open the file in write mode using FileAccess.
        var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Write);
        file.StoreString(data);
        file.Close();

        GD.Print("Data saved to: " + filePath);
    }

    public void SaveDataBytes(string fileName, byte[] data)
    {
        string filePath = Path.Join(basePath, fileName);
        CheckOrCreateRelativePath(fileName);

        var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Write);
        
        file.StoreBuffer(data);
        file.Close();
        GD.Print("Binary data saved to: " + filePath);
    }

    public string LoadData(string fileName)
    {
        string filePath = Path.Join(basePath, fileName);
        if (Godot.FileAccess.FileExists(filePath))
        {
            var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
            string data = file.GetAsText();
            file.Close();
            GD.Print("Data loaded from: " + filePath);
            return data;
        }
        else
        {
            GD.Print("File not found: " + filePath);
            return null;
        }
    }
    
    public string[] LoadDataFromDir(string fileName)
    {
        string directoryPath = Path.Join(basePath, fileName);

        if (System.IO.Directory.Exists(directoryPath))
        {
            return System.IO.Directory.GetFiles(directoryPath);
           
        }
        else
        {
            GD.Print("Directory not found: " + directoryPath);
            return null;
        }
    }

    private void CheckOrCreateAbsolutePath(string path)
    {
        // Use DirAccess to check if the directory exists.
        DirAccess dir = DirAccess.Open(path);
        if (dir == null)
        {
            // Create the directory recursively if it does not exist.
            dir.MakeDirRecursive(path);
            GD.Print("Directory created at: " + path);
        }
        else
        {
            GD.Print("Directory already exists at: " + path);
        }
    }

    private void CheckOrCreateRelativePath(string relativePath)
    {
        // Compute the full path.
        string fullPath = Path.Join(basePath, relativePath);
        // Determine the directory portion by finding the last slash.
        int lastSlash = fullPath.LastIndexOf('/');
        string directoryPath = lastSlash >= 0 ? fullPath.Substring(0, lastSlash) : fullPath;

        // Check and create the directory if necessary.
        DirAccess dir = DirAccess.Open(directoryPath);
        if (dir == null)
        {
            dir.MakeDirRecursive(directoryPath);
            GD.Print("Directory created at: " + directoryPath);
        }
        else
        {
            GD.Print("Directory already exists at: " + directoryPath);
        }
    }
}
