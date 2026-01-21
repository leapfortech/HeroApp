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

    public RegisterPuzzleRequest(RegisterPostRequest registerPostRequest, Puzzle puzzle,
                                 List<PuzzleAnswer> puzzleAnswers)
    {
        Post = registerPostRequest.Post;
        Contact = registerPostRequest.Contact;
        Links = registerPostRequest.Links;
        Images = registerPostRequest.Images;

        Puzzle = puzzle;
        PuzzleAnswers = puzzleAnswers;
    }
}
