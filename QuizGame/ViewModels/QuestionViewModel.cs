using QuizGame.Helpers;
using QuizGame.Models;

namespace QuizGame.ViewModels;

// Helper ViewModel for individual questions
public class QuestionViewModel : ViewModelBase
{
    private int _questionNumber;
    private string _statement;
    private string _firstAnswer;
    private string _secondAnswer;
    private string _thirdAnswer;
    private string _fourthAnswer;
    private string _correctAnswer;

    public int QuestionNumber
    {
        get => _questionNumber;
        set => SetProperty(ref _questionNumber, value);
    }

    public string Statement
    {
        get => _statement;
        set => SetProperty(ref _statement, value);
    }

    public string FirstAnswer
    {
        get => _firstAnswer;
        set 
        {
            if (SetProperty(ref _firstAnswer, value)) OnPropertyChanged(nameof(AnswerOptions));
        }
    }

    public string SecondAnswer
    {
        get => _secondAnswer;
        set 
        {
            if (SetProperty(ref _secondAnswer, value)) OnPropertyChanged(nameof(AnswerOptions));
        }
    }

    public string ThirdAnswer
    {
        get => _thirdAnswer;
        set 
        {
            if (SetProperty(ref _thirdAnswer, value)) OnPropertyChanged(nameof(AnswerOptions));
        }
    }

    public string FourthAnswer
    {
        get => _fourthAnswer;
        set 
        {
            if (SetProperty(ref _fourthAnswer, value)) OnPropertyChanged(nameof(AnswerOptions));
        }
    }

    public string CorrectAnswer
    {
        get => _correctAnswer;
        set => SetProperty(ref _correctAnswer, value);
    }

    public IEnumerable<string?> AnswerOptions
    {
        get
        {
            return new[]
            {
                FirstAnswer?.Trim(),
                SecondAnswer?.Trim(),
                ThirdAnswer?.Trim(),
                FourthAnswer?.Trim()
            }.Where(a => !string.IsNullOrEmpty(a));
        }
    }
    public Question ToModel()
    {
        return new Question()
        {
            Statement = Statement,
            Answers = { FirstAnswer, SecondAnswer, ThirdAnswer, FourthAnswer },
            CorrectAnswer = CorrectAnswer
        };
    }
}