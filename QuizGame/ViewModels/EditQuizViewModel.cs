using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using QuizGame.Helpers;
using QuizGame.Models;
using QuizGame.Services;

namespace QuizGame.ViewModels;

public class EditQuizViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private Quiz _selectedQuiz;
    private QuestionViewModel _selectedQuestion;
    private string _quizCategory;
    private string _statusMessage;
    private Brush _statusColour;

    public ObservableCollection<Quiz> AvailableQuizzes { get; set; }
    public ObservableCollection<QuestionViewModel> Questions { get; set; }
    public Quiz SelectedQuiz
    {
        get => _selectedQuiz;
        set => SetProperty(ref _selectedQuiz, value);
    }
    public QuestionViewModel SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            SetProperty(ref _selectedQuestion, value);
            OnPropertyChanged(nameof(IsQuestionSelectedVisibility));
        }
    }
    public string QuizCategory
    {
        get => _quizCategory;
        set => SetProperty(ref _quizCategory, value);
    }
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
    public Visibility IsQuizLoadedVisibility => 
        Questions?.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility IsQuestionSelectedVisibility => 
        SelectedQuestion != null ? Visibility.Visible : Visibility.Collapsed;
    public ICommand BackToMenuCommand { get; }
    public ICommand LoadQuizCommand { get; }
    public ICommand SaveChangesCommand { get; }

    public EditQuizViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AvailableQuizzes = new ObservableCollection<Quiz>();
        Questions = new ObservableCollection<QuestionViewModel>();
        BackToMenuCommand = new RelayCommand(_ => _mainViewModel.NavigateToMenuCommand.Execute(null));
        LoadQuizCommand = new RelayCommand(_ => LoadQuizAsync(), o => SelectedQuiz != null);
        SaveChangesCommand = new RelayCommand(_ => SaveChangesAsync());

        LoadAvailableQuizzesAsync();
    }

    private async void LoadAvailableQuizzesAsync()
    {
        AvailableQuizzes.Clear();
        foreach (var file in QuizFileService.GetQuizFiles())
        {
            var quiz = await QuizFileService.LoadFromJsonAsync(Path.GetFileName(file));
            if (quiz != null) AvailableQuizzes.Add(quiz);
        }
    }

    private async void LoadQuizAsync()
    {
        if (SelectedQuiz == null) return;

        SelectedQuiz = await QuizFileService.LoadFromJsonAsync($"{SelectedQuiz.Category}.json");
        
        Questions.Clear();
        QuizCategory = SelectedQuiz.Category;

        if (SelectedQuiz.Questions != null)
        {
            foreach (var question in SelectedQuiz.Questions)
            {
                Questions.Add(new QuestionViewModel
                {
                    QuestionNumber = Questions.Count + 1,
                    Statement = question.Statement,
                    FirstAnswer = question.Answers.Count > 0 ? question.Answers[0] : "",
                    SecondAnswer = question.Answers.Count > 1 ? question.Answers[1] : "",
                    ThirdAnswer = question.Answers.Count > 2 ? question.Answers[2] : "",
                    FourthAnswer = question.Answers.Count > 3 ? question.Answers[3] : "",
                    CorrectAnswer = question.CorrectAnswer
                });
            }
        }

        OnPropertyChanged(nameof(IsQuizLoadedVisibility));
    }

    private async Task SaveChangesAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(QuizCategory))
            {
                MessageBox.Show("Quiz category is unavailable for save.", "Validation Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (Questions.Count == 0)
            {
                MessageBox.Show("Please add at least one question.", "Validation Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            
            var modelQuestions = Questions.Select(qvm => new Question()
            {
                Statement = qvm.Statement,
                Answers = new List<string>
                {
                    qvm.FirstAnswer,
                    qvm.FourthAnswer,
                    qvm.ThirdAnswer,
                    qvm.FourthAnswer
                },
                CorrectAnswer = qvm.CorrectAnswer
            }).ToList();
            var updatedQuiz = new Quiz
            {
                Category = QuizCategory,
                Questions = modelQuestions
            };
            
            await QuizFileService.SaveAsJsonAsync(updatedQuiz);
            
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