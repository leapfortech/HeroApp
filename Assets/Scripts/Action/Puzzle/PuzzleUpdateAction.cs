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
    DataMapper dtmPuzzleFull = null;
    [SerializeField]
    DataMapper dtmPuzzleAnswerFull = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Event")]
    [SerializeField]
    UnityPuzzleAnswerFullsEvent onPopulated = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    PuzzleService puzzleService = null;

    PuzzleFull puzzleFull = null;

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
        dtmPuzzleFull.ClearElements();
        dtmPuzzleAnswerFull.ClearElements();
    }

    public void Display(PuzzleFull puzzleFull)
    {
        Clear();

        this.puzzleFull = puzzleFull;

        dtmPuzzleFull.PopulateClass<PuzzleFull>(puzzleFull);

        onPopulated.Invoke(puzzleFull.PuzzleAnswerFulls);
    }

    private void DoUpdate()
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

        puzzleFull.Update(dtmPuzzleFull.BuildClass<PuzzleFull>());
        puzzleFull.PuzzleAnswerFulls = puzzleAnswerFulls;

        puzzleFull.CountryId = StateManager.Instance.Identity.BirthCountryId;

        puzzleService.UpdatePuzzle(puzzleFull);
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
