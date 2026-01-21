using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class PuzzleRegisterAction : MonoBehaviour
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
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    PuzzleService puzzleService = null;

    private void Awake()
    {
        puzzleService = GetComponent<PuzzleService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmPuzzle.ClearElements();
        dtmPuzzleAnswer.ClearElements();
    }

    private void Register()
    {
        ScreenDialog.Instance.Display();

        List<PuzzleAnswer> puzzleAnswers = dtmPuzzleAnswer.BuildClassList<PuzzleAnswer>();

        if (puzzleAnswers == null || puzzleAnswers.Count == 0)
        {
            ChoiceDialog.Instance.Error("Error", "Debes ingresar al menos una respuesta.");
            return;
        }

        bool hasCorrect = puzzleAnswers.Exists(a => a.IsCorrect == 1);

        if (!hasCorrect)
        {
            ChoiceDialog.Instance.Error("Error", "Debes ingresar una respuesta correcta.");
            return;
        }

        Post post = dtmPost.BuildClass<Post>();

        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = StateManager.Instance.Identity.OriginCountryId;
        post.StateId = StateManager.Instance.Identity.OriginStateId;

        Puzzle puzzle = dtmPuzzle.BuildClass<Puzzle>();
        puzzle.CountryId = StateManager.Instance.Identity.OriginCountryId;

        puzzleService.Register(new RegisterPuzzleRequest(new RegisterPostRequest(post, null, null, null), 
                                                         puzzle, puzzleAnswers));
    }

    public void ApplyPuzzle(long puzzleId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
