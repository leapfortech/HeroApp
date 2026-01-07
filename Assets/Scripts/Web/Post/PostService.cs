using System;
using UnityEngine;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class PostService : MonoBehaviour
{
    [SerializeField]
    private UnityStringsEvent onImagesRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            imagesGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onRegistered.Invoke(Convert.ToInt64(op.favoriteId));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            favoriteRegisterOp.Send();
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            commentRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void RegisterCommenPlaintt(CommentPlaint commentPlaint)
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onRegistered.Invoke(Convert.ToInt64(op.postPlaintId));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
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
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            postReadRegisterOp.Send();
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
                    onRegistered.Invoke(Convert.ToInt64(op.reactionId));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            reactionRegisterOp.Send();
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
                    onRegistered.Invoke(Convert.ToInt64(op.likeId));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            likeRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}