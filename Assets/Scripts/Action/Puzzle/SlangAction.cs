using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;


public class SlangAction : MonoBehaviour
{
    [Title("Action")]
    [SerializeField]
    Button btnStart = null;
    [SerializeField]
    Button btnNext = null;

    [Title("Page")]
    [SerializeField]
    Page pagSlang;

    PuzzleService puzzleService;

    long puzzleId = -1;

    private void Awake()
    {
        puzzleService = GetComponent<PuzzleService>();
    }

    private void Start()
    {
        btnStart?.AddAction(NextGame);
        btnNext?.AddAction(NextGame);
    }

    public void Clear()
    {
        
    }

    // Display
    public void NextGame()
    {
        ScreenDialog.Instance.Display();

        int difficulty = 1;

        puzzleService.GetNextPuzzle(new PuzzleNextRequest(StateManager.Instance.Player.Id, 1, StateManager.Instance.InterestLocality.CountryId, difficulty));
    }

    public void ApplyFull(PuzzleFull puzzleFull)
    {
        puzzleId = puzzleFull.Id;
        StateManager.Instance.AddPuzzleFull(puzzleFull);
        Display(puzzleFull);
    }

    private void Display(PuzzleFull puzzleFull)
    {       
        if (puzzleFull == null)
            return;

        PageManager.Instance.ChangePage(pagSlang);
    }
}