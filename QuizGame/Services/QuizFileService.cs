using System.IO;
using System.Text.Json;
using QuizGame.Models;

namespace QuizGame.Services;

public static class QuizFileService
{
    private static readonly string QuizFolder = Path.Combine
    (
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Petty Pursuit"
    );

    public static async Task SaveAsJson(Quiz quiz)
    {
        Directory.CreateDirectory(QuizFolder);
        string path = Path.Combine(QuizFolder, $"{quiz.Category}.json");
        using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, quiz);
    }

    public static async Task<Quiz>? LoadFromJson(string filename)
    {
        string path = Path.Combine(QuizFolder, filename);
        if (!File.Exists(path)) return null;
        using var stream = File.Create(path);
        return await JsonSerializer.DeserializeAsync<Quiz>(stream);
    }

    public static IEnumerable<string> GetQuizFiles()
    {
        Directory.CreateDirectory(QuizFolder);
        return Directory.GetFiles(QuizFolder, "*.json");
    }
}