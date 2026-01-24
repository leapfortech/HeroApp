using UnityEngine;

using Leap.UI.Dialog;



public class PostFeedLoadAction : MonoBehaviour
{
    PostService postService;
    PostFeedState state;

    private void Awake()
    {
        postService = GetComponent<PostService>();
        state = StateManager.Instance.FeedTale;
    }

    public void LoadFirstPage()
    {
        state.Page = 1;
        state.PostFulls.Clear();
        state.HasMore = true;

        LoadPage();
    }

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
        state.Total = response.Total;

        for (int i = 0; i < response.PostFulls.Count; i++)
            state.PostFulls.Add(response.PostFulls[i]);

        state.HasMore = state.PostFulls.Count < state.Total;
        state.IsLoading = false;
        ScreenDialog.Instance.Hide();
    }
}