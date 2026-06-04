using UnityEngine;

using Leap.UI.Dialog;


public class FeedToolsAction : MonoBehaviour
{
    FeedAction feedAction = null;
    PostService postService = null;

    const int LIKE = 5, DISLIKE = 0;

    private void Awake()
    {
        feedAction = GetComponent<FeedAction>();
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

    // Like/Dislike
    public void RegisterLike(long postId)
    {
        ScreenDialog.Instance.Display();

        postService.RegisterLike(new Like(postId, StateManager.Instance.AppUser.Id, LIKE));
    }

    public void RegisterDislike(long postId)
    {
        ScreenDialog.Instance.Display();

        postService.RegisterLike(new Like(postId, StateManager.Instance.AppUser.Id, DISLIKE));
    }

    public void ApplyLike(long likeId)
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