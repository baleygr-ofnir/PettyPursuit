using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using QuizGame.Helpers;
using QuizGame.Models;
using QuizGame.Services;

namespace QuizGame.ViewModels;

public class PlayQuizViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private Quiz _selectedQuiz;
    private Question _currentQuestion;
    private int _currentQuestionIndex;
    private int _correctAnswers;
    private int _totalAnswered;
    
    // UI Elements Visibility
    private Visibility _isQuizSelectionVisible = Visibility.Visible;
    private Visibility _isQuestionVisible = Visibility.Collapsed;
    private Visibility _isResultsVisible = Visibility.Collapsed;
    
    public ObservableCollection<Quiz> AvailableQuizzes { get; set; } = new ObservableCollection<Quiz>();
    public Quiz SelectedQuiz
    {
        get => _selectedQuiz;
        set
        {
            if (SetProperty(ref _selectedQuiz, value)) OnPropertyChanged(nameof(QuizCategory));
        }
    }
    public Question CurrentQuestion
    {
        get => _currentQuestion;
        set => SetProperty(ref _currentQuestion, value);
    }
    public string QuizCategory => _selectedQuiz?.Category ?? "Select a Quiz";
    public string ScoreText => $"Score: {_correctAnswers}/{_totalAnswered}";
    public string PercentageText
    {
        get
        {
            if (_totalAnswered == 0) return "0%";
            return $"{(_correctAnswers * 100.0 / _totalAnswered):F1}%";
        }
    }
    public string QuestionCounterText => $"{_currentQuestionIndex + 1} of {_selectedQuiz?.Questions?.Count ?? 0}";
    public string FinalScoreText => $"Final Score: {_correctAnswers}/{_totalAnswered}";
    public string FinalPercentageText => $"{(_correctAnswers * 100.0 / _totalAnswered):F1}%";

    // Visibility property bindings
    private enum QuizViewState { Selection, Playing, Complete }
    public Visibility IsQuizSelectionVisible
    {
        get => _isQuizSelectionVisible;
        set => SetProperty(ref _isQuizSelectionVisible, value);
    }
    public Visibility IsQuestionVisible
    {
        get => _isQuestionVisible;
        set => SetProperty(ref _isQuestionVisible, value);
    }
    public Visibility IsResultsVisible
    {
        get => _isResultsVisible;
        set => SetProperty(ref _isResultsVisible, value);
    }

    // Commands
    public ICommand BackToMenuCommand { get; }
    public ICommand ResetQuizCommand { get; }
    public ICommand StartQuizCommand { get; }
    public ICommand AnswerSelectedCommand { get; }

    public PlayQuizViewModel(MainViewModel mainViewModel)
    {
        var mainViewModel1 = mainViewModel;

        // Init commands
        BackToMenuCommand = new RelayCommand(o => mainViewModel1.NavigateToMenuCommand.Execute(null));
        ResetQuizCommand = new RelayCommand(o => ResetQuiz());
        StartQuizCommand = new RelayCommand(o => StartQuiz(), o => SelectedQuiz != null);
        AnswerSelectedCommand = new RelayCommand(AnswerSelected);
        
        // Load currently stored quizzes
        LoadAvailableQuizzes();
    }

    private async void LoadAvailableQuizzes()
    {
        AvailableQuizzes.Clear();
        foreach (var file in QuizFileService.GetQuizFiles())
        {
            var quiz = await QuizFileService.LoadFromJson(Path.GetFileName(file))!;
            if (quiz != null) AvailableQuizzes.Add(quiz);
        }
    }
    
    private void SetQuizViewState(QuizViewState state)
    {
        IsQuizSelectionVisible = state == QuizViewState.Selection ? Visibility.Visible : Visibility.Collapsed;
        IsQuestionVisible = state == QuizViewState.Playing ? Visibility.Visible : Visibility.Collapsed;
        IsResultsVisible = state == QuizViewState.Complete ? Visibility.Visible : Visibility.Collapsed;
    }

    private void NotifyQuizStatProperties()
    {
        OnPropertyChanged(nameof(ScoreText));
        OnPropertyChanged(nameof(PercentageText));
        OnPropertyChanged(nameof(QuestionCounterText));
        OnPropertyChanged(nameof(FinalScoreText));
        OnPropertyChanged(nameof(FinalPercentageText));
    }
    private async Task StartQuiz()
    {
        if (SelectedQuiz == null) return;
        SelectedQuiz = await QuizFileService.LoadFromJson($"{SelectedQuiz.Category}.json");
        var random = new Random();
        SelectedQuiz.Questions = SelectedQuiz.Questions.OrderBy(question => random.Next()).ToList();
        
        _currentQuestionIndex = 0;
        _correctAnswers = 0;
        _totalAnswered = 0;
        
        SetQuizViewState(QuizViewState.Playing);
        NotifyQuizStatProperties();
        OnPropertyChanged(nameof(QuizCategory));
    }

    private void LoadNextQuestion()
    {
        if (_currentQuestionIndex < SelectedQuiz.Questions.Count)
        {
            CurrentQuestion = SelectedQuiz.Questions[_currentQuestionIndex];
            SetQuizViewState(QuizViewState.Playing);
        }
        else
        {
            SetQuizViewState(QuizViewState.Complete);
        }
        NotifyQuizStatProperties();
    }

    private void AnswerSelected(object selectedAnswer)
    {
        string answer = selectedAnswer as string;
        
        // Check for a correct answer
        if (answer?.ToLower() == CurrentQuestion.CorrectAnswer.ToLower()) _correctAnswers++;
        _totalAnswered++;
        _currentQuestionIndex++;
        
        // Move to next or show results
        if (_currentQuestionIndex < SelectedQuiz.Questions.Count) LoadNextQuestion();
    }

    private void ResetQuiz()
    {
        SetQuizViewState(QuizViewState.Selection);
        // If reset means new selection is possible, ensure QuizCategory property is notified
        OnPropertyChanged(nameof(QuizCategory));
        NotifyQuizStatProperties(); // If relevant
    }
}
