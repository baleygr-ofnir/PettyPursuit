using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
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
    private QuizViewState _quizViewState;
    
    // UI Elements Visibility
    private Visibility _isQuizSelectionVisible = Visibility.Visible;
    private Visibility _isQuestionVisible = Visibility.Collapsed;
    private Visibility _isResultsVisible = Visibility.Collapsed;

    public ObservableCollection<Quiz> AvailableQuizzes { get; set; } = new();
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
    //public string ScoreText => $"Score: {_correctAnswers}/{_totalAnswered}";
    public string ScoreText => $"Score: {_correctAnswers}/{_selectedQuiz.Questions.Count}";
    public string PercentageText
    {
        get
        {
            if (_totalAnswered == 0) return "0%";
            return $"{(_correctAnswers * 100.0 / _selectedQuiz.Questions.Count):F1}%";
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
    public ICommand BackCommand { get; }
    public ICommand ResetQuizCommand { get; }
    public ICommand StartQuizCommand { get; }
    public ICommand AnswerSelectedCommand { get; }

    public PlayQuizViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        // Init commands
        BackCommand = new RelayCommand(o => Back());
        ResetQuizCommand = new RelayCommand(o => ResetQuiz());
        StartQuizCommand = new RelayCommand(o => StartQuizAsync(), o => SelectedQuiz != null);
        AnswerSelectedCommand = new RelayCommand(AnswerSelected);
        
        // Load currently stored quizzes
        LoadAvailableQuizzesAsync();
    }

    private void Back()
    {
        if (_quizViewState == QuizViewState.Playing || _quizViewState == QuizViewState.Complete)
        {
            ResetQuizPlayState(QuizViewState.Selection);
        }
        else
        {
            _mainViewModel.NavigateToMenuCommand.Execute(null);
        }
    }

    private async void LoadAvailableQuizzesAsync()
    {
        AvailableQuizzes.Clear();
        foreach (var file in QuizFileService.GetQuizFiles())
        {
            var quiz = await QuizFileService.LoadFromJsonAsync(Path.GetFileName(file))!;
            if (quiz != null)
            {
                AvailableQuizzes.Add(quiz);
            }
        }
    }
    
    private void SetQuizViewState(QuizViewState state)
    {
        _quizViewState = state;
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
    
    private async Task StartQuizAsync()
    {
        if (SelectedQuiz == null) return;
        SelectedQuiz = await QuizFileService.LoadFromJsonAsync($"{SelectedQuiz.Category}.json");
        var random = new Random();
        // Shuffle question order
        SelectedQuiz.Questions = SelectedQuiz.Questions.OrderBy(question => random.Next()).ToList();
        // Shuffle order of question answers
        foreach (var question in SelectedQuiz.Questions)
        {
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(question.Answers));
        }
        ResetQuizPlayState(QuizViewState.Playing);
        LoadNextQuestion();
    }

    private void LoadNextQuestion()
    {
        if (SelectedQuiz == null || SelectedQuiz.Questions == null) return;
        
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
        string? answer = selectedAnswer as string;

        if (string.IsNullOrEmpty(answer)) return;
        
        // Check for a correct answer
        if
        (
            !string.IsNullOrEmpty(CurrentQuestion.CorrectAnswer) &&
            answer.ToLower() == CurrentQuestion.CorrectAnswer.ToLower()
        )
        {
            _correctAnswers++;
        }
        _totalAnswered++;
        _currentQuestionIndex++;
        
        // Move to next or show results
        LoadNextQuestion();
    }

    private void ResetQuizPlayState(QuizViewState targetState)
    {
        _currentQuestionIndex = 0;
        _correctAnswers = 0;
        _totalAnswered = 0;
        
        NotifyQuizStatProperties();
        OnPropertyChanged(nameof(QuizCategory));
        SetQuizViewState(targetState);
    }
    
    private void ResetQuiz()
    {
        SetQuizViewState(QuizViewState.Selection);
        // If reset means new selection is possible, ensure QuizCategory property is notified
        OnPropertyChanged(nameof(QuizCategory));
        NotifyQuizStatProperties(); // If relevant
    }
}
