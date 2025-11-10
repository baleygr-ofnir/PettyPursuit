using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using QuizGame.Helpers;
using QuizGame.Models;
using QuizGame.Services;

namespace QuizGame.ViewModels;

public class CreateQuizViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private string _quizCategory;
    private string _statusMessage;
    private Brush _statusColour;

    public string QuizCategory
    {
        get => _quizCategory;
        set => SetProperty(ref _quizCategory, value);
    }
    public ObservableCollection<QuestionViewModel> Questions { get; set; }
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    public Brush StatusColour
    {
        get => _statusColour;
        set => SetProperty(ref _statusColour, value);
    }
    
    // Commands
    public ICommand BackToMenuCommand { get; set; }
    public ICommand AddQuestionCommand { get; set; }
    public ICommand RemoveQuestionCommand { get; set; }
    public ICommand SaveQuizCommand { get; set; }

    public CreateQuizViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        Questions = new ObservableCollection<QuestionViewModel>();

        BackToMenuCommand = new RelayCommand(o => _mainViewModel.NavigateToMenuCommand.Execute(null));
        AddQuestionCommand = new RelayCommand(o => AddQuestion());
        RemoveQuestionCommand = new RelayCommand(RemoveQuestion);
        SaveQuizCommand = new RelayCommand(o => SaveQuizAsync(), o => CanSaveQuiz());
    }

    private void AddQuestion()
    {
        Questions.Add(new QuestionViewModel
        {
            QuestionNumber = Questions.Count + 1
        });
    }

    private void RemoveQuestion(object parameter)
    {
        if (parameter is QuestionViewModel question)
        {
            Questions.Remove(question);
            // Renumber questions
            for (int i = 0; i < Questions.Count; i++)
            {
                Questions[i].QuestionNumber = i + 1;
            }
        }
    }

    private bool CanSaveQuiz()
    {
        return !string.IsNullOrWhiteSpace(QuizCategory) && Questions.Count > 0;
    }

    private async Task SaveQuizAsync()
    {
        // Use QuizFileService to save in JSON
        try
        {
            if (string.IsNullOrEmpty(QuizCategory))
            {
                MessageBox.Show("Quiz category unavailable for save", "Validation Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (Questions.Count == 0)
            {
                MessageBox.Show("Please add at least one question.", "Validation Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var quiz = new Quiz
            {
                Category = QuizCategory,
                Questions = Questions.Select(qvm => new Question
                {
                    Statement = qvm.Statement,
                    Answers = new List<string>()
                    {
                        qvm.FirstAnswer,
                        qvm.SecondAnswer,
                        qvm.ThirdAnswer,
                        qvm.FourthAnswer
                    },
                    CorrectAnswer = qvm.CorrectAnswer
                }).ToList()
            };
            await QuizFileService.SaveAsJsonAsync(quiz);
            
            StatusMessage = "Quiz saved successfully! Returning to menu...";
            StatusColour = new SolidColorBrush(Colors.Green);
        
            await Task.Delay(1500);
            _mainViewModel.NavigateToMenuCommand.Execute(null);
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Failed to save quiz: {exception.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            throw;
        }
    }
}