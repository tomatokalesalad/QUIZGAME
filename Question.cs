public class Question
{
    public string QuestionText { get; set; }
    public string OptionA { get; set; }
    public string OptionB { get; set; }
    public string OptionC { get; set; }
    public string OptionD { get; set; }
    public string CorrectAnswer { get; set; }
    public string Category { get; set; }
    public string Difficulty { get; set; }

    public override string ToString()
    {
        return $"{QuestionText} ({Category} - {Difficulty})";
    }
}
