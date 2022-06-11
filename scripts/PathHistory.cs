using Godot;
using System;
using System.Collections.Generic;

public class PathHistory : Reference
{
    private readonly List<string> _pathList = new List<string>();
    private int _currentIndex = 0;

    public PathHistory(string initialPath)
    {
        _pathList.Add(initialPath);
    }

    public void Append(string path)
    {
        // TODO if we are in the middle of the list and add a new entry after the current one
        // and delete all entries after that
        _pathList.Add(path);
        _currentIndex = _pathList.Count - 1;
    }

    public string Forward()
    {
        if (_currentIndex < _pathList.Count - 1)
        {
            _currentIndex++;
        }
        return _pathList[_currentIndex];
    }

    public string Back()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
        }
        return _pathList[_currentIndex];
    }

}
