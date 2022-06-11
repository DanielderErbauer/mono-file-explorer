using System.Collections.Generic;
using Godot;

public class MonoExplorer : Control
{
    private Tree _tree;
    private GridContainer _grid;
    private LineEdit _path;

    private TreeItem _root;

    private string _currentPath;

    private PathHistory _pathHistory;

    public override void _Ready()
    {
        GetNode<Button>("VBoxContainer/TopBar/Up").Connect("pressed", this, nameof(OnUpPressed));
        GetNode<Button>("VBoxContainer/TopBar/Forward").Connect("pressed", this, nameof(OnForwardPressed));
        GetNode<Button>("VBoxContainer/TopBar/Back").Connect("pressed", this, nameof(OnBackPressed));
        GetNode<Button>("VBoxContainer/TopBar/Home").Connect("pressed", this, nameof(OnHomePressed));
        
        _path = GetNode<LineEdit>("VBoxContainer/TopBar/Path");
        _path.Connect("text_entered", this, nameof(OnPathChanged));
        _tree = GetNode<Tree>("VBoxContainer/HBoxContainer/Tree");
        _tree.Connect("item_collapsed", this, nameof(OnTreeCollapsed));
        _grid = GetNode<GridContainer>("VBoxContainer/HBoxContainer/ScrollContainer/GridContainer");

        _currentPath = OS.GetExecutablePath().GetBaseDir();
        _path.Text = _currentPath;
        _pathHistory = new PathHistory(_currentPath);
        LoadFiles();

        _root = _tree.CreateItem();
        _root.SetText(0, "/");
        UpdateTree("/");
        _root.Collapsed = true;

    }

    private void OnTreeCollapsed(TreeItem item)
    {
        GD.Print($"Collapsed: {item.Collapsed}, Label: {item.GetText(0)}");
    }

    private void OnIconPressed(string label)
    {
        if (label.EndsWith("/"))
        {
            string path;
            if (_currentPath == "/")
            {
                path = _currentPath + label.Substring(0, label.Length - 1);
            }
            else
            {
                path = _currentPath + "/" + label.Substring(0, label.Length - 1);
            }
            ChangePath(path);
        }
    }

    private void OnPathChanged(string newPath)
    {
        GD.Print(newPath);
    }

    private void OnHomePressed()
    {
#if GODOT_X11
        var username = OS.GetEnvironment("USER");
        ChangePath($"/home/{username}");
#else
        GD.Print("Platform not supported!");
#endif
    }

    private void OnUpPressed()
    {
        var parent = FileUtils.GetParent(_currentPath);
        ChangePath(parent);
    }

    private void OnForwardPressed()
    {
        UpdateWindow(_pathHistory.Forward());
    }

    private void OnBackPressed()
    {
        UpdateWindow(_pathHistory.Back());
    }
    
    private void UpdateTree(string basePath)
    {
        var entries = FileUtils.GetFiles(basePath);
        foreach (var entry in entries)
        {
            if (entry.IsDirectory)
            {
                var item = _tree.CreateItem(_root);
                item.SetText(0, entry.Name); // 428
            }
        }
    }

    private void ChangePath(string newPath)
    {
        _pathHistory.Append(newPath);
        UpdateWindow(newPath);
    }

    private void UpdateWindow(string newPath)
    {
        ClearFiles();
        _currentPath = newPath;
        _path.Text = _currentPath;
        LoadFiles();
    }

    private void ClearFiles()
    {
        var children = _grid.GetChildren();
        foreach (var child in children)
        {
            if (child is Node node)
            {
                _grid.RemoveChild(node);
                node.QueueFree();
            }
        }
    }

    private void LoadFiles()
    {
        var entries = FileUtils.GetFiles(_currentPath);
        foreach (var entry in entries)
        {
            var button = new Button();
            button.Text = entry.Name;
            if (entry.IsDirectory)
            {
                button.Text = $"{button.Text}/";
            }

            button.Connect("pressed", this, nameof(OnIconPressed), new Godot.Collections.Array() {button.Text});
            
            _grid.AddChild(button);
        }
    }
}
