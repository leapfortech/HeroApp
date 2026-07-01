using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class PuzzleUpdateAction : MonoBehaviour
{
    [Serializable]

    public class UnityPuzzleAnswerFullsEvent : UnityEvent<List<PuzzleAnswerFull>> { }

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

    [Title("Event")]
    [SerializeField]
    UnityPuzzleAnswerFullsEvent OnPopulated = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    PuzzleService puzzleService = null;

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

    public void Display(PuzzleFull puzzleFull)
    {
        Clear();

        post = new Post(puzzleFull);
        dtmPost.PopulateClass<Post>(post);

        puzzle = new Puzzle(puzzleFull);
        dtmPuzzle.PopulateClass<Puzzle>(puzzle);

        OnPopulated.Invoke(puzzleFull.PuzzleAnswerFulls);
    }

    private void DoUpdate()
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

        post.Update(dtmPost.BuildClass<Post>());

        puzzle.Update(dtmPuzzle.BuildClass<Puzzle>());
        puzzle.CountryId = StateManager.Instance.Identity.BirthCountryId;

        puzzleService.UpdatePuzzle(new RegisterPuzzleRequest(post, puzzle, puzzleAnswers));
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
