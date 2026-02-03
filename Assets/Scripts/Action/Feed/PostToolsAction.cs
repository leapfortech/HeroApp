using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class PostToolsAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    InputField ifdComment = null;
    [SerializeField]
    ValueList vllReactionType = null;
    [SerializeField]
    ValueList vllPlaintType = null;
    [SerializeField]
    ValueList vllCommentPlaintType = null;

    [Title("Action")]
    [SerializeField]
    Button btnLike = null;
    [SerializeField]
    Button btnDislike = null;

    PostService postService;

    long postId = -1, appUserId = -1;

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }

    private void Start()
    {
        btnLike?.AddAction(() => RegisterLike(5));
        btnDislike?.AddAction(() => RegisterLike(0));
    }

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        appUserId = StateManager.Instance.appUserId;
    }

    // Share
    public void RegisterShare()
    {
        ScreenDialog.Instance.Display();
        postService.RegisterShare(new Share(postId, appUserId));
    }

    public void ApplyShare(long shareId)
    {
        ScreenDialog.Instance.Hide();
    }

    // Favorite
    public void RegisterFavorite()
    {
        ScreenDialog.Instance.Display();
        postService.RegisterFavorite(new Favorite(postId, appUserId));
    }

    // Comment
    public void RegisterComment()
    {
        ScreenDialog.Instance.Display();
        postService.RegisterComment(new Comment(postId, appUserId, ifdComment.Text));
    }

    public void ApplyComment(long commentId)
    {
        ifdComment.Clear();
        ScreenDialog.Instance.Hide();
    }

    public void RegisterCommentPlaint(int idx)
    {
        ScreenDialog.Instance.Display();

        long commentPlaintTypeId = vllCommentPlaintType.GetRecordId(idx);
        postService.RegisterCommenPlaint(new CommentPlaint(commentPlaintTypeId, postId, appUserId));
    }

    public void ApplyCommentPlaint(long commentPlaintId)
    {
        ScreenDialog.Instance.Hide();
    }

    public void ApplyFavorite(long favoriteId)
    {
        ScreenDialog.Instance.Hide();
    }

    // PostPlaint
    public void RegisterPostPlaint(int idx)
    {
        ScreenDialog.Instance.Display();

        long plaintTypeId = vllPlaintType.GetRecordId(idx);
        postService.RegisterPostPlaint(new PostPlaint(plaintTypeId, postId, appUserId));
    }

    public void ApplyPlaintType(long postPlaintId)
    {
        ScreenDialog.Instance.Hide();
    }

    // PostRead
    public void RegisterPostRead()
    {
        ScreenDialog.Instance.Display();
        postService.RegisterPostRead(new PostRead(postId, appUserId));
    }

    public void ApplyPostRead(long postReadId)
    {
        ScreenDialog.Instance.Hide();
    }

    // Reaction
    public void RegisterReaction(int idx)
    {
        ScreenDialog.Instance.Display();

        long reactionTypeId = vllReactionType.GetRecordId(idx);
        postService.RegisterReaction(new Reaction(reactionTypeId, postId, appUserId));
    }

    public void ApplyReaction(long reactionId)
    {
        ScreenDialog.Instance.Hide();
    }

    // Like
    public void RegisterLike(int rank)
    {
        ScreenDialog.Instance.Display();
        postService.RegisterLike(new Like(postId, appUserId, rank));
    }

    public void ApplyLike(long likeId)
    {
        ScreenDialog.Instance.Hide();
    }
}