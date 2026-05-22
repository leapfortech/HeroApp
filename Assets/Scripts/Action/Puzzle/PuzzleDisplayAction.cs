using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class PuzzleDisplayAction : MonoBehaviour
{
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    //[Title("Values")]
    //[SerializeField]
    //ValueList vllProjectDescriptionType = null;
    [Title("Event")]
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    PuzzleService puzzleService;

    long postId = -1, puzzleId = -1;

    private void Awake()
    {
        puzzleService = GetComponent<PuzzleService>();
    }

    public void Clear()
    {
        
    }

    // Display

    public void Display(long postId)
    {
        this.postId = postId;

        PuzzleFull puzzleFull = StateManager.Instance.GetPuzzleFullByPostId(postId);
        if (puzzleFull != null)
        {
            puzzleId = puzzleFull.Id;
            Display(puzzleFull);
            return;
        }

        ScreenDialog.Instance.Display();
        puzzleService.GetFullByPostId(postId, StateManager.Instance.AppUser.Id);
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

        txtTitle.TextValue = puzzleFull.Title;

        PageManager.Instance.ChangePage(pagDetail);

        onDisplayed.Invoke(new long[2] {puzzleFull.PostId, puzzleFull.Id});
    }
}