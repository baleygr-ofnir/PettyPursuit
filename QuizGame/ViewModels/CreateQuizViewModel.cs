using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using QuizGame.Helpers;
using QuizGame.Models;
using QuizGame.Services;

namespace QuizGame.ViewModels;

public class CreateQuizViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private string _quizCategory;
    private bool _isJsonFormat = true;
    private bool _isXmlFormat;
    private bool _isCsvFormat;

    public string QuizCategory
    {
        get => _quizCategory;
        set => SetProperty(ref _quizCategory, value);
    }
    public ObservableCollection<QuestionViewModel> Questions { get; set; }
    public bool IsJsonFormat
    {
        get => _isJsonFormat;
        set => SetProperty(ref _isJsonFormat, value);
    }
    public bool IsXmlFormat
    {
        get => _isXmlFormat;
        set => SetProperty(ref _isXmlFormat, value);
    }
    public bool IsCsvFormat
    {
        get => _isCsvFormat;
        set => SetProperty(ref _isCsvFormat, value);
    }
    
    // Commands
    public ICommand BackToMenuCommand { get; }
    public ICommand AddQuestionCommand { get; }
    public ICommand RemoveQuestionCommand { get; }
    public ICommand SaveQuizCommand { get; }

    public CreateQuizViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        Questions = new ObservableCollection<QuestionViewModel>();

        BackToMenuCommand = new RelayCommand(o => _mainViewModel.NavigateToMenuCommand.Execute(null));
        AddQuestionCommand = new RelayCommand(o => AddQuestion());
        RemoveQuestionCommand = new RelayCommand(RemoveQuestion);
        SaveQuizCommand = new RelayCommand(o => SaveQuiz(), o => CanSaveQuiz());
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

    private async Task SaveQuiz()
    {
        // Use QuizFileService to save in selected format (JSON/XML/CSV)
        // Save asynchronously as per specification requirements
        List<Question> modelQuestions = new List<Question>(Questions.Select(qvm => qvm.ToModel()));
        var quiz = new Quiz
        {
            Category = QuizCategory,
            Questions = modelQuestions
        };

        await QuizFileService.SaveAsJson(quiz);
    }
}