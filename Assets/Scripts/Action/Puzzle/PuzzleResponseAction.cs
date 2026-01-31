using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Dialog;
using Leap.Data.Collections;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class PuzzleAnswerAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    InputField ifdAnswer = null;
    [SerializeField]
    ToggleGroup tggIsCorrect = null;

    [SerializeField]
    int maxAnswer = 3;

    [Title("Data")]
    [SerializeField]
    ListScroller lstAnswer = null;

    [SerializeField]
    ValueList vllAnswer = null;

    public void Clear()
    {
        lstAnswer.Clear();
        vllAnswer.ClearRecords();
        ifdAnswer.Clear();
        tggIsCorrect.Clear();
    }

    public void AddRecord()
    {
        String response = ifdAnswer.Text;

        if (String.IsNullOrWhiteSpace(response))
        {
            ChoiceDialog.Instance.Error("Debes ingresar una respuesta.");
            return;
        }

        if (vllAnswer.RecordCount >= maxAnswer)
        {
            ChoiceDialog.Instance.Error("No se pueden ingresar más de " + maxAnswer + " respuestas.");
            return;
        }

        bool isCorrect = tggIsCorrect.Value == "1";

        if (isCorrect && HasCorrectAnswer())
        {
            ChoiceDialog.Instance.Error("Solo puede existir una respuesta correcta.");
            return;
        }

        vllAnswer.AddRecord(ifdAnswer.Text, tggIsCorrect.Value);

        Display();

        ifdAnswer.Clear();
        tggIsCorrect.Clear();
    }

    public void AddRecords(List<PuzzleAnswerFull> puzzleAnswerFulls)
    {
        if (puzzleAnswerFulls == null || puzzleAnswerFulls.Count == 0)
            return;

        if (vllAnswer == null)
            return;

        for (int i = 0; i < puzzleAnswerFulls.Count; i++)
            vllAnswer.AddRecord(puzzleAnswerFulls[i].Description, puzzleAnswerFulls[i].IsCorrect == 1 ? "1" : "0");

        Display();
    }

    public void RemoveRecord(int recordIdx)
    {
        vllAnswer.RemoveRecord(recordIdx);

        Display();
    }

    private bool HasCorrectAnswer()
    {
        for (int i = 0; i < vllAnswer.RecordCount; i++)
        {
            if (vllAnswer.GetRecordCellString(i, "IsCorrect") == "1")
                return true;
        }
        return false;
    }


    public void Display()
    {
        lstAnswer.Clear();

        for (int i = 0; i < vllAnswer.RecordCount; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, vllAnswer.GetRecordCellString(i, "Description"));
            scrollerValue.SetText(1, vllAnswer.GetRecordCellString(i, "IsCorrect") == "1" ? "Correcta" : "Incorrecta");

            lstAnswer.AddValue(scrollerValue);
        }

        lstAnswer.ApplyValues();
    }
}
