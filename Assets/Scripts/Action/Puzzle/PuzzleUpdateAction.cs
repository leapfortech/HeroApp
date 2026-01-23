using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class PuzzleUpdateAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmPuzzle = null;
    [SerializeField]
    DataMapper dtmPuzzleAnswer = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    PuzzleService puzzleService = null;

    long postId = -1, puzzleId = -1;
    Post post = null;
    Puzzle puzzle = null;

    private void Awake()
    {
        puzzleService = GetComponent<PuzzleService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmPuzzle.ClearElements();
        dtmPuzzleAnswer.ClearElements();
    }

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        puzzleId = ids[1];
    }

    public void Populate()
    {
        PuzzleFull puzzleFull = null; // StateManager.Instance.GetPuzzleFullById(puzzleId);

        post = new Post(puzzleFull);
        dtmPost.PopulateClass<Post>(post);

        puzzle = new Puzzle(puzzleFull);
        dtmPuzzle.PopulateClass<Puzzle>(puzzle);

        List<PuzzleAnswer> puzzleAnswers = new List<PuzzleAnswer>();
        for (int i = 0; i < puzzleFull.PuzzleAnswerFulls.Count; i++)
            puzzleAnswers.Add(new PuzzleAnswer(puzzleFull.PuzzleAnswerFulls[i].Id, puzzleFull.Id, 
                                               puzzleFull.PuzzleAnswerFulls[i].Description,
                                               puzzleFull.PuzzleAnswerFulls[i].IsCorrect,
                                               puzzleFull.PuzzleAnswerFulls[i].Status));
        dtmPuzzleAnswer.PopulateClassList<PuzzleAnswer>(puzzleAnswers);
    }

    private void DoUpdate()
    {
        ScreenDialog.Instance.Display();

        List<PuzzleAnswer> puzzleAnswersNew = dtmPuzzleAnswer.BuildClassList<PuzzleAnswer>();

        if (puzzleAnswersNew == null || puzzleAnswersNew.Count == 0)
        {
            ChoiceDialog.Instance.Error("Error", "Debes ingresar al menos una respuesta.");
            return;
        }

        bool hasCorrect = puzzleAnswersNew.Exists(a => a.IsCorrect == 1);

        if (!hasCorrect)
        {
            ChoiceDialog.Instance.Error("Error", "Debes ingresar una respuesta correcta.");
            return;
        }

        Post postNew = dtmPost.BuildClass<Post>();
        post.Title = postNew.Title;
        post.Summary = postNew.Summary;
        post.Description = postNew.Description;

        Puzzle puzzleNew = dtmPuzzle.BuildClass<Puzzle>();
        puzzleNew.CountryId = StateManager.Instance.Identity.OriginCountryId;

        puzzle.PuzzleSubtypeId = puzzleNew.PuzzleSubtypeId;
        puzzle.CountryId = puzzleNew.CountryId;
        puzzle.Question = puzzleNew.Question;
        puzzle.Hint = puzzleNew.Hint;
        puzzle.Difficulty = puzzleNew.Difficulty;
        puzzle.Points = puzzleNew.Points;
        puzzle.PlayCount = puzzleNew.PlayCount;

        puzzleService.UpdatePuzzle(new RegisterPuzzleRequest(new RegisterPostRequest(post, null, null, null), 
                                                             puzzle, puzzleAnswersNew));
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
