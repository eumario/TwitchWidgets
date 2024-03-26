using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using SixLabors.ImageSharp;
using HttpClient = System.Net.Http.HttpClient;
using Image = Godot.Image;

namespace TwitchWidgetsApp.Library;

public static class Util
{
    private static readonly string[] ByteSizes = new string[] { "B", "KB", "MB", "GB", "TB" };
    
    public static string FormatSize(double bytes)
    {
        double len = bytes;
        int order = 0;
        while (len > 1024 && order < ByteSizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {ByteSizes[order]}";
    }

    private static async Task<Stream> GetFileStream(string url)
    {
        using var client = new HttpClient();
        try
        {
            var stream = await client.GetStreamAsync(url);
            return stream;
        }
        catch (Exception)
        {
            return Stream.Null;
        }
    }

    public static async Task<ImageTexture?> FetchImage(string? url, string subPath = "avatar", string fileName = "")
    {
        var path = Path.Combine(Path.GetFullPath(OS.GetUserDataDir()), "cache");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        path = Path.Combine(path, subPath);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (url is null) return null;

        var file = Path.Combine(path, string.IsNullOrEmpty(fileName) ? Path.GetFileName(url) : fileName).EnsureEndsWith(".png");

        if (!File.Exists(file))
        {
            await using var netStream = await GetFileStream(url);
            if (netStream == Stream.Null) return null;
            var slis = await SixLabors.ImageSharp.Image.LoadAsync(netStream);
            await slis.SaveAsPngAsync(file);
        }

        var image = Image.LoadFromFile(file);
        return ImageTexture.CreateFromImage(image);
    }
}