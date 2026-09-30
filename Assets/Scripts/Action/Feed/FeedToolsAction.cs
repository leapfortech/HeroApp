using UnityEngine;

using Leap.UI.Dialog;


public class FeedToolsAction : MonoBehaviour
{
    PostService postService = null;

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }

    // Share
    public void RegisterShare(long postId)
    {
        ScreenDialog.Instance.Display();

        postService.RegisterShare(new Share(postId, StateManager.Instance.AppUser.Id));
    }

    public void ApplyShare(long shareId)
    {
        ScreenDialog.Instance.Hide();
    }

    // Favorite
    public void RegisterFavorite(long postId)
    {
        ScreenDialog.Instance.Display();

        postService.RegisterFavorite(new Favorite(postId, StateManager.Instance.AppUser.Id));
    }

    public void ApplyFavorite(long favoriteId)
    {
        ScreenDialog.Instance.Hide();
    }

    // PostRead
    public void RegisterPostRead(long postId)
    {
        ScreenDialog.Instance.Display();

        postService.RegisterPostRead(new PostRead(postId, StateManager.Instance.AppUser.Id));
    }

    public void ApplyPostRead(long postReadId)
    {
        ScreenDialog.Instance.Hide();
    }
}