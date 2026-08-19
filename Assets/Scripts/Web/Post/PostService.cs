using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class PostService : MonoBehaviour
{
    [Serializable]
    public class PostFeedResponseEvent : UnityEvent<PostFeedResponse> { }

    [Serializable]
    public class CommentFeedResponseEvent : UnityEvent<CommentFeedResponse> { }

    [SerializeField]
    private PostFeedResponseEvent onFeedRetreived = null;

    [SerializeField]
    private CommentFeedResponseEvent onCommentFeedRetreived = null;

    [SerializeField]
    private UnityStringsEvent onImagesRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onFavoriteChanged = null;

    [SerializeField]
    private UnityLongEvent onLikeChanged = null;

    [SerializeField]
    private UnityBoolEvent onReactionChanged = null;

    [SerializeField]
    private UnityLongEvent onPlaintRegistered = null;

    //[SerializeField]
    //private UnityBoolEvent onDeleted = null;

    [Title("Errors")]
    [SerializeField]
    private UnityStringEvent onSendError = null;
    [SerializeField]
    private UnityStringEvent onResponseError = null;
    [SerializeField]
    private UnityStringEvent onTimeoutError = null;

    // GET
    public void GetImagesById(long id, String first = "true")
    {
        ImagesByIdGetOperation imagesGetOp = new ImagesByIdGetOperation();
        try
        {
            imagesGetOp.id = id;
            imagesGetOp.first = first;
            imagesGetOp["on-complete"] = (Action<ImagesByIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onImagesRetreived.Invoke(op.projectImages);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            imagesGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    // POST
    public void GetPostFeed(PostFeedRequest postFeedRequest)
    {
        PostFeedOperation postFeedOp = new PostFeedOperation();
        try
        {
            postFeedOp.postFeedRequest = postFeedRequest;
            postFeedOp["on-complete"] = (Action<PostFeedOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFeedRetreived.Invoke(op.postFeedResponse);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            postFeedOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    public void GetCommentFeed(CommentFeedRequest commentFeedRequest)
    {
        CommentFeedOperation commentFeedOp = new CommentFeedOperation();
        try
        {
            commentFeedOp.commentFeedRequest = commentFeedRequest;
            commentFeedOp["on-complete"] = (Action<CommentFeedOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onCommentFeedRetreived.Invoke(op.commentFeedResponse);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            commentFeedOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message, onSendError);
        }
    }

    // REGISTER
    public void RegisterShare(Share share)
    {
        ShareRegisterOperation shareRegisterOp = new ShareRegisterOperation();
        try
        {
            shareRegisterOp.share = share;
            shareRegisterOp["on-complete"] = (Action<ShareRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.shareId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            shareRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterFavorite(Favorite favorite)
    {
        FavoriteRegisterOperation favoriteRegisterOp = new FavoriteRegisterOperation();
        try
        {
            favoriteRegisterOp.favorite = favorite;
            favoriteRegisterOp["on-complete"] = (Action<FavoriteRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFavoriteChanged.Invoke(Convert.ToInt64(op.favoriteId) != -1);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            favoriteRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void DeleteFavorite(Favorite favorite)
    {
        FavoriteDeleteOperation favoriteDeleteOp = new FavoriteDeleteOperation();
        try
        {
            favoriteDeleteOp.favorite = favorite;
            favoriteDeleteOp["on-complete"] = (Action<FavoriteDeleteOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFavoriteChanged.Invoke(Convert.ToBoolean(op.done));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            favoriteDeleteOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterLike(Like like)
    {
        LikeRegisterOperation likeRegisterOp = new LikeRegisterOperation();
        try
        {
            likeRegisterOp.like = like;
            likeRegisterOp["on-complete"] = (Action<LikeRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onLikeChanged.Invoke(Convert.ToInt64(op.likeId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            likeRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void UpdateLike(Like like)
    {
        LikeUpdateOperation likeUpdateOp = new LikeUpdateOperation();
        try
        {
            likeUpdateOp.like = like;
            likeUpdateOp["on-complete"] = (Action<LikeUpdateOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onLikeChanged.Invoke(Convert.ToInt64(op.likeId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            likeUpdateOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void DeleteLike(Like like)
    {
        LikeDeleteOperation likeDeleteOp = new LikeDeleteOperation();
        try
        {
            likeDeleteOp.like = like;
            likeDeleteOp["on-complete"] = (Action<LikeDeleteOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onLikeChanged.Invoke(Convert.ToInt64(op.likeId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            likeDeleteOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterReaction(Reaction reaction)
    {
        ReactionRegisterOperation reactionRegisterOp = new ReactionRegisterOperation();
        try
        {
            reactionRegisterOp.reaction = reaction;
            reactionRegisterOp["on-complete"] = (Action<ReactionRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReactionChanged.Invoke(Convert.ToInt64(op.reactionId) != -1);
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            reactionRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void DeleteReaction(Reaction reaction)
    {
        ReactionDeleteOperation reactionDeleteOp = new ReactionDeleteOperation();
        try
        {
            reactionDeleteOp.reaction = reaction;
            reactionDeleteOp["on-complete"] = (Action<ReactionDeleteOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onReactionChanged.Invoke(Convert.ToBoolean(op.done));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            reactionDeleteOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterComment(Comment comment)
    {
        CommentRegisterOperation commentRegisterOp = new CommentRegisterOperation();
        try
        {
            commentRegisterOp.comment = comment;
            commentRegisterOp["on-complete"] = (Action<CommentRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.commentId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            commentRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterCommentPlaint(CommentPlaint commentPlaint)
    {
        CommentPlaintRegisterOperation commentPlaintRegisterOp = new CommentPlaintRegisterOperation();
        try
        {
            commentPlaintRegisterOp.commentPlaint = commentPlaint;
            commentPlaintRegisterOp["on-complete"] = (Action<CommentPlaintRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.commentPlaintId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            commentPlaintRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterPostPlaint(PostPlaint postPlaint)
    {
        PostPlaintRegisterOperation postPlaintRegisterOp = new PostPlaintRegisterOperation();
        try
        {
            postPlaintRegisterOp.postPlaint = postPlaint;
            postPlaintRegisterOp["on-complete"] = (Action<PostPlaintRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onPlaintRegistered.Invoke(Convert.ToInt64(op.postPlaintId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            postPlaintRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterPostRead(PostRead postRead)
    {
        PostReadRegisterOperation postReadRegisterOp = new PostReadRegisterOperation();
        try
        {
            postReadRegisterOp.postRead = postRead;
            postReadRegisterOp["on-complete"] = (Action<PostReadRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.postReadId));
                else
                    WebManager.Instance.OnResponseError(response, onResponseError, onTimeoutError);
            });
            postReadRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}