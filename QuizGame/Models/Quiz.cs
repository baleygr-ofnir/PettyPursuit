namespace QuizGame.Models;

public class Quiz
{
    public required string Category { get; set; }
    public List<Question> Questions { get; set; } = new ();
}