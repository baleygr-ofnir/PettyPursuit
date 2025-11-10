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

    public static async Task InitializeStarterQuizzesAsync()
    {
        try
        {
            string projectQuizzesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quizzes");
            
            Directory.CreateDirectory(QuizFolder);

            if (!Directory.Exists(projectQuizzesFolder))
            {
                System.Diagnostics.Debug.WriteLine($"Quizzes folder not found at: {projectQuizzesFolder}");
                return;
            }

            string[] quizFiles = Directory.GetFiles(projectQuizzesFolder, "*.json");

            foreach (var sourceFile in quizFiles)
            {
                string fileName = Path.GetFileName(sourceFile);
                string destinationFile = Path.Combine(QuizFolder, fileName);

                if (!File.Exists(destinationFile))
                {
                    File.Copy(sourceFile, destinationFile);
                    System.Diagnostics.Debug.WriteLine($"Copied starter quiz: {fileName}");
                }
            }
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing starter quizzes: {exception.Message}");
        }
        
        
        
    }
    
    public static async Task SaveAsJsonAsync(Quiz quiz)
    {
        Directory.CreateDirectory(QuizFolder);
        string path = Path.Combine(QuizFolder, $"{quiz.Category}.json");
        using var stream = File.Create(path);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        await JsonSerializer.SerializeAsync(stream, quiz, options);
    }

    public static async Task<Quiz>? LoadFromJsonAsync(string filename)
    {
        string path = Path.Combine(QuizFolder, filename);
        if (!File.Exists(path)) return null;
        using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<Quiz>(stream);
    }

    public static IEnumerable<string> GetQuizFiles()
    {
        //Directory.CreateDirectory(QuizFolder);
        return Directory.GetFiles(QuizFolder, "*.json");
    }
}