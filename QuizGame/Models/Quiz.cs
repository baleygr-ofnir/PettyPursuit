namespace QuizGame.Models;

public class Quiz
{
    /*private IEnumerable<Question> _questions;
    private string _title = string.Empty;

    public List<Question>? Questions
    {
        get => _questions as List<Question>;
        set => (_questions as List<Question>)?.AddRange(value);
    }
    public string Title => _title;
    public string Category { get; set; }

    public Quiz(List<Question> questions, string category)
    {
        _questions = new List<Question>();
        (_questions as List<Question>)?.AddRange(questions);
        Category = category;
    }*/
    public required string Category { get; set; }
    public List<Question> Questions { get; set; } = new ();
}