using System.Collections.Generic;

public class RegisterPuzzleRequest : RegisterPostRequest
{
    public Puzzle Puzzle { get; set; }
    public List<PuzzleAnswer> PuzzleAnswers { get; set; }

    public RegisterPuzzleRequest()
    {
    }

    public RegisterPuzzleRequest(Puzzle puzzle, List<PuzzleAnswer> puzzleAnswers)
    {
        Puzzle = puzzle;
        PuzzleAnswers = puzzleAnswers;
    }

    public RegisterPuzzleRequest(Post post, Puzzle puzzle, List<PuzzleAnswer> puzzleAnswers)
    {
        Post = post;

        Puzzle = puzzle;
        PuzzleAnswers = puzzleAnswers;
    }
}
