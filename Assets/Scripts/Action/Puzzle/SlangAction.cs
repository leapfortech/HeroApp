using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;


public class SlangAction : MonoBehaviour
{
    [Title("Element")]
    [SerializeField]
    Text txtPoints = null;
    [SerializeField]
    Text txtQuestion = null;
    [SerializeField]
    Button btnAnswer1 = null;
    [SerializeField]
    Button btnAnswer2 = null;
    [SerializeField]
    Button btnAnswer3 = null;
    [SerializeField]
    Text txtWinPoints = null;

    [Title("Action")]

    [Title("Page")]
    [SerializeField]
    Page pagSlang = null;
    [SerializeField]
    Page pagCorrect = null;
    [SerializeField]
    Page pagIncorrect = null;

    PuzzleService puzzleService;

    PuzzleFull puzzleFull = null;
    PuzzleFull nextPuzzleFull = null;
    List<PuzzleAnswerFull> currentAnswers = new List<PuzzleAnswerFull>();


    private void Awake()
    {
        puzzleService = GetComponent<PuzzleService>();
    }

    private void Start()
    {
        btnAnswer1?.AddAction(() => SaveResult(0));
        btnAnswer2?.AddAction(() => SaveResult(1));
        btnAnswer3?.AddAction(() => SaveResult(2));
    }

    public void Clear()
    {
        
    }

    // Display
    public void NextGame()
    {
        if (nextPuzzleFull != null)
        {
            ApplyFull(nextPuzzleFull);
            nextPuzzleFull = null;
            return;
        }

        ScreenDialog.Instance.Display();

        int difficulty = 1;

        puzzleService.GetNextPuzzle(new PuzzleNextRequest(StateManager.Instance.Player.Id, 1, StateManager.Instance.InterestLocality.CountryId, difficulty));
    }

    public void ApplyFull(PuzzleFull puzzleFull)
    {
        if (puzzleFull == null || puzzleFull.Id == 0 || puzzleFull.Id == -1)
        {
            ChoiceDialog.Instance.Info("Retos" , "Por el momento no hay nuevos retos.");
            return;
        }

        this.puzzleFull = puzzleFull;

        StateManager.Instance.AddPuzzleFull(puzzleFull);
        Display(puzzleFull);
    }

    private void Display(PuzzleFull puzzleFull)
    {
        if (puzzleFull == null)
            return;

        txtPoints.TextValue = "Puntos: " + puzzleFull.Points.ToString();
        txtQuestion.TextValue = puzzleFull.Question;

        currentAnswers.Clear();

        for (int i = 0; i < puzzleFull.PuzzleAnswerFulls.Count; i++)
            currentAnswers.Add(puzzleFull.PuzzleAnswerFulls[i]);

        // Fisher-Yates Shuffle
        for (int i = currentAnswers.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            PuzzleAnswerFull temp = currentAnswers[i];
            currentAnswers[i] = currentAnswers[randomIndex];
            currentAnswers[randomIndex] = temp;
        }

        btnAnswer1.Title = currentAnswers[0].Description;
        btnAnswer2.Title = currentAnswers[1].Description;
        btnAnswer3.Title = currentAnswers[2].Description;

        PageManager.Instance.ChangePage(pagSlang);
    }

    public void DisplayHelp()
    {
        ChoiceDialog.Instance.Info("Ayuda", puzzleFull.Hint);
    }

    // Result
    private void SaveResult(int index)
    {
        if (index < 0 || index >= currentAnswers.Count)
            return;

        PuzzleAnswerFull answer = currentAnswers[index];

        ScreenDialog.Instance.Display();

        puzzleService.SaveResult(new PuzzleResultRequest(StateManager.Instance.Player.Id, puzzleFull.Id, answer.Id, 30));
    }

    public void ApplySaveResult(PuzzleResultResponse puzzleResultResponse)
    {
        StateManager.Instance.UpdatePuzzleResultSummary(puzzleFull.PuzzleGameId, puzzleResultResponse.Points, puzzleResultResponse.NewMedals,
                                                        puzzleResultResponse.NewCups);

        if (puzzleResultResponse.Correct == 1)
        {
            txtWinPoints.TextValue = puzzleResultResponse.Points.ToString() + " Puntos";
            PageManager.Instance.ChangePage(pagCorrect);
        }
        else
            PageManager.Instance.ChangePage(pagIncorrect);
    }
}