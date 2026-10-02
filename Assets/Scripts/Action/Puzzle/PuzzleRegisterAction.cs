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
    DataMapper dtmPuzzleFull = null;
    [SerializeField]
    DataMapper dtmPuzzleAnswerFull = null;

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
        dtmPuzzleFull.ClearElements();
        dtmPuzzleAnswerFull.ClearElements();
    }

    private void Register()
    {
        ScreenDialog.Instance.Display();

        List<PuzzleAnswerFull> puzzleAnswerFulls = dtmPuzzleAnswerFull.BuildClassList<PuzzleAnswerFull>();

        if (puzzleAnswerFulls == null || puzzleAnswerFulls.Count == 0)
        {
            ChoiceDialog.Instance.Error("Error", "Debes ingresar al menos una respuesta.");
            return;
        }

        if (!puzzleAnswerFulls.Exists(a => a.IsCorrect == 1))
        {
            ChoiceDialog.Instance.Error("Error", "Debes ingresar una respuesta correcta.");
            return;
        }

        PuzzleFull puzzleFull = dtmPuzzleFull.BuildClass<PuzzleFull>();
        puzzleFull.AppUserId = StateManager.Instance.AppUser.Id;
        puzzleFull.PuzzleAnswerFulls = puzzleAnswerFulls;

        //RM REVIEW
        puzzleFull.PostCountryId = StateManager.Instance.InterestLocality.CountryId;
        puzzleFull.PostStateId = StateManager.Instance.InterestLocality.StateId;

        puzzleFull.CountryId = StateManager.Instance.Identity.BirthCountryId;

        puzzleService.Register(puzzleFull);
    }

    public void ApplyPuzzle(long puzzleId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
