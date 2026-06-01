using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class ClueAction : MonoBehaviour
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
    Text txtTimer = null;
    [SerializeField]
    PercentBar pcbTimer = null;
    [SerializeField]
    Text txtWinPoints = null;
    [SerializeField]
    Text txtCorrectAnswer = null;

    [Title("Action")]
    [SerializeField]
    Button btnExit = null;

    [Title("Event")]
    [SerializeField]
    UnityStringEvent OnStart = null;

    [Title("Page")]
    [SerializeField]
    Page pagHome = null;
    [SerializeField]
    Page pagClue = null;
    [SerializeField]
    Page pagCorrect = null;
    [SerializeField]
    Page pagIncorrect = null;

    PuzzleService puzzleService;

    PuzzleFull puzzleFull = null;
    PuzzleFull nextPuzzleFull = null;
    List<PuzzleAnswerFull> currentAnswers = new List<PuzzleAnswerFull>();

    Coroutine timerCoroutine = null;
    float remainingTime = 0f, startTime = 0f;


    private void Awake()
    {
        puzzleService = GetComponent<PuzzleService>();
    }

    private void Start()
    {
        btnExit?.AddAction(Exit);

        btnAnswer1?.AddAction(() => SaveResult(0));
        btnAnswer2?.AddAction(() => SaveResult(1));
        btnAnswer3?.AddAction(() => SaveResult(2));
    }

    public void Clear()
    {
        
    }

    private void Exit()
    {
        ChoiceDialog.Instance.Warning("Salir del reto", "¿Estás seguro que deseas salir?", () => PageManager.Instance.ChangePage(pagHome), null, "Sí", "No");
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

        puzzleService.GetNextPuzzle(new PuzzleNextRequest(StateManager.Instance.Player.Id, 2, StateManager.Instance.InterestLocality.CountryId, difficulty));
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

        OnStart.Invoke("Test");

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

        PageManager.Instance.ChangePage(pagClue);

        StartTimer();
    }

    private void StartTimer()
    {
        StopTimer();

        startTime = Time.time;
        remainingTime = puzzleFull.Delay;

        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        while (remainingTime > 0)
        {
            txtTimer.TextValue = Mathf.CeilToInt(remainingTime) + " s";
            pcbTimer.Pourcent = remainingTime / puzzleFull.Delay;

            yield return new WaitForSeconds(1f);

            remainingTime -= 1f;
        }

        txtTimer.TextValue = "0s";
        
        SaveResult(-1);
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    public void DisplayHelp()
    {
        ChoiceDialog.Instance.Info("Ayuda", puzzleFull.Hint);
    }

    // Result
    private void SaveResult(int index)
    {
        StopTimer();

        int elapsedSeconds = Mathf.RoundToInt(Time.time - startTime);

        ScreenDialog.Instance.Display();

        long answerId = -1;

        if (index >= 0 && index < currentAnswers.Count)
            answerId = currentAnswers[index].Id;

        puzzleService.SaveResult(new PuzzleResultRequest(StateManager.Instance.Player.Id, puzzleFull.Id, answerId, elapsedSeconds));
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
        {
            txtCorrectAnswer.TextValue = puzzleResultResponse.CorrectAnswer;
            PageManager.Instance.ChangePage(pagIncorrect);
        }
    }
}