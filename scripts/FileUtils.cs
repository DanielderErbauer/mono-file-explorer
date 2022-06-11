using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// TODO Replace this with .NET std class
public static class FileUtils
{
    public const char PathSeparator = '/';

    /// <summary>
    /// Strips path of a trailing slash 
    /// </summary>
    /// <param name="path">the path to strip</param>
    /// <returns>the stripped path</returns>
    public static string Strip(string path)
    {
        if (path.Length == 1)
        {
            return path;
        }
        return path.EndsWith(PathSeparator.ToString()) ? path.Substring(0, path.Length - 1) : path;
    }

    /// <summary>
    /// Retrieves the parent directory path for the path
    /// </summary>
    /// <param name="path">the path to get the parent of</param>
    /// <returns>the parent of the path</returns>
    public static string GetParent(string path)
    {
        if (path.Length == 1)
        {
            return path;
        }
        path = Strip(path);
        
        var parentSlashIndex = path.LastIndexOf(PathSeparator);

        if (parentSlashIndex == path.Length - 1)
        {
            var secondLastSlash = parentSlashIndex > 0 ? path.LastIndexOf(PathSeparator, parentSlashIndex - 1) : -1;
            parentSlashIndex = secondLastSlash;
        }
        return parentSlashIndex == 0 ? PathSeparator.ToString() : path.Substring(0, parentSlashIndex);
    }

    public struct DirEntry
    {
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
    }

    /// <summary>
    /// Retrieves all entries in given path
    /// </summary>
    /// <param name="path">the path from which to scan entries</param>
    /// <param name="recursive">if the path should be scanned recursively</param>
    /// <returns>all entries in the path</returns>
    public static List<DirEntry> GetFiles(string path, bool recursive = false)
    {
        path = Strip(path);
        var entries = new List<DirEntry>();
        var directory = new Directory();
        directory.Open(path);

        directory.ListDirBegin();
        var file = directory.GetNext();
        while (file != "")
        {
            if (file == "." || file == "..")
            {
                file = directory.GetNext();
                continue;
            }
            var entry = new DirEntry{Name = file, IsDirectory = directory.CurrentIsDir()};
            entries.Add(entry);
            if (recursive && entry.IsDirectory)
            {
                entries.AddRange(GetFiles(path + PathSeparator + file));
            }
            file = directory.GetNext();
        }
        directory.ListDirEnd();
        return entries;
    }

}
