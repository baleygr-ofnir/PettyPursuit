using System.Configuration;
using System.Data;
using System.Windows;
using QuizGame.Services;

namespace QuizGame;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void ApplicationStartup(object sender, StartupEventArgs e)
    {
        QuizFileService.InitializeStarterQuizzesAsync();
    }
}