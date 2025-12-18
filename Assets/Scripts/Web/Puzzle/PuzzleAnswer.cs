using System;

public class PuzzleAnswer
{
    public long Id { get; set; }
    public long PuzzleId { get; set; }
    public String Description { get; set; }
    public int IsCorrect { get; set; }

    public PuzzleAnswer() { }

    public PuzzleAnswer(long id, long puzzleId, String description, int isCorrect)
    {
        Id = id;
        PuzzleId = puzzleId;
        Description = description;
        IsCorrect = isCorrect;
    }
}
