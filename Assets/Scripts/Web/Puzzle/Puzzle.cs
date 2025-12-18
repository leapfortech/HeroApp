using System;

public class Puzzle
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long PuzzleSubtypeId { get; set; }
    public long CountryId { get; set; }
    public String Question { get; set; }
    public String Hint { get; set; }
    public int Difficulty { get; set; }
    public int Points { get; set; }
    public int PlayCount { get; set; }
    public int Status { get; set; }

    public Puzzle() { }

    public Puzzle(long id, long postId, long puzzleSubtypeId, long countryId, String question, String hint,
                    int difficulty, int points, int playCount, int status)
    {
        Id = id;
        PostId = postId;
        PuzzleSubtypeId = puzzleSubtypeId;
        CountryId = countryId;
        Question = question;
        Hint = hint;
        Difficulty = difficulty;
        Points = points;
        PlayCount = playCount;
        Status = status;
    }
}
