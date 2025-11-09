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
        set => SetProperty(ref _firstAnswer, value);
    }

    public string SecondAnswer
    {
        get => _secondAnswer;
        set => SetProperty(ref _secondAnswer, value);
    }

    public string ThirdAnswer
    {
        get => _thirdAnswer;
        set => SetProperty(ref _thirdAnswer, value);
    }

    public string FourthAnswer
    {
        get => _fourthAnswer;
        set => SetProperty(ref _fourthAnswer, value);
    }

    public string CorrectAnswer
    {
        get => _correctAnswer;
        set => SetProperty(ref _correctAnswer, value);
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