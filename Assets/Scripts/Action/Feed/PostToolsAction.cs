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

    PostService postService;

    long postId = -1, appUserId = -1;

    private void Awake()
    {
        postService = GetComponent<PostService>();
    }

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        appUserId = StateManager.Instance.AppUser.Id;
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
        postService.RegisterCommentPlaint(new CommentPlaint(commentPlaintTypeId, postId, appUserId));
    }

    public void ApplyCommentPlaint(long commentPlaintId)
    {
        ScreenDialog.Instance.Hide();
    }
}