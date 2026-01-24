using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;



public class FeedAction : MonoBehaviour
{
    [Space]
    [Title("List")]
    [SerializeField]
    ListScroller lstFeed = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnNextPage = null;

    PostService postService;
    FeedState state;

    public int SelIdx { get; set; } = 0;

    Dictionary<long, long> indexes = new Dictionary<long, long>();
    long idx = 0;

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }


    private void Start()
    {
        btnNextPage?.AddAction(LoadNextPage);
    }

    public long GetSelectedPostId()
    {
        if (indexes.TryGetValue(SelIdx, out long postId))
            return postId;

        return -1;
    }

    // ONLY TEST
    private void Init()
    {
        if (state != null)
            return;

        state = new FeedState(1, 3, false, true, 1);

        StateManager.Instance.FeedTale = state;
    }

    public void LoadFirstPage()
    {
        Init();

        if (state.IsLoading)
            return;

        idx = 0;
        indexes.Clear();
        lstFeed.Clear();

        LoadPage();
    }

    // NEXT PAGE

    public void LoadNextPage()
    {
        if (state.IsLoading)
            return;

        if (!state.HasMore)
            return;

        state.Page++;
        LoadPage();
    }

    private void LoadPage()
    {
        ScreenDialog.Instance.Display();
        state.IsLoading = true;

        PostFeedRequest request = new PostFeedRequest(state.Page, state.PageSize, state.PostSubtypeId, 1);
        postService.GetPostFullsPaged(request);  
    }

    public void ApplyPage(PostFeedResponse response)
    {
        int startIndex = state.PostFulls.Count;

        state.Total = response.Total;

        for (int i = 0; i < response.PostFulls.Count; i++)
            state.PostFulls.Add(response.PostFulls[i]);

        state.HasMore = state.PostFulls.Count < state.Total;
        state.IsLoading = false;

        DisplayFrom(startIndex);

        ScreenDialog.Instance.Hide();
    }

    private void DisplayFrom(int startIdx)
    {
        for (int i = startIdx; i < state.PostFulls.Count; i++)
        {
            indexes[idx] = state.PostFulls[i].PostId;
            idx++;

            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, state.PostFulls[i].Description);
            scrollerValue.SetSprite(1, state.PostFulls[i].TitleSprite);

            lstFeed.AddValue(scrollerValue);
        }

        lstFeed.ApplyValues();
        txtEmpty.gameObject.SetActive(state.PostFulls.Count == 0);
    }
}