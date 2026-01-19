using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class PuzzleRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmPuzzle = null;

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
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();

        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = StateManager.Instance.Identity.OriginCountryId;
        post.StateId = StateManager.Instance.Identity.OriginStateId;

        Puzzle puzzle = dtmPuzzle.BuildClass<Puzzle>();

        // RM WIP Fill All Params
        List<PuzzleAnswer> puzzleAnswers = new List<PuzzleAnswer>();

        puzzleService.Register(new RegisterPuzzleRequest(new RegisterPostRequest(post, null, null, null), 
                                                         puzzle, puzzleAnswers));
    }

    public void ApplyPuzzle(long puzzleId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
