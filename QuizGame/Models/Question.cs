namespace QuizGame.Models;

public class Question
{
    public string Statement { get; set; }
    public List<string> Answers { get; set; }
    public string CorrectAnswer { get; set; }
}