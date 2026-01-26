using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using System.Collections.Generic;
using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class PuzzleService : MonoBehaviour
{
    [Serializable]
    public class PuzzleFullEvent : UnityEvent<PuzzleFull> { }

    [Serializable]
    public class PuzzleFullsEvent : UnityEvent<List<PuzzleFull>> { }

    [SerializeField]
    private PuzzleFullEvent onFullRetreived = null;

    [SerializeField]
    private PuzzleFullsEvent onFullsRetreived = null;

    [SerializeField]
    private UnityLongEvent onRegistered = null;

    [SerializeField]
    private UnityBoolEvent onUpdated = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetFull(long id)
    {
        PuzzleGetFullOperation puzzleFullGetOp = new PuzzleGetFullOperation();
        try
        {
            puzzleFullGetOp.id = id;
            puzzleFullGetOp["on-complete"] = (Action<PuzzleGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.puzzleFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            puzzleFullGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullByPostId(long postId)
    {
        PuzzleFullByPostIdGetFullOperation puzzleFullByPostIdGetOp = new PuzzleFullByPostIdGetFullOperation();
        try
        {
            puzzleFullByPostIdGetOp.postId = postId;
            puzzleFullByPostIdGetOp["on-complete"] = (Action<PuzzleFullByPostIdGetFullOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullRetreived.Invoke(op.puzzleFull);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            puzzleFullByPostIdGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls(int status)
    {
        PuzzleGetFullsOperation puzzleFullsGetOp = new PuzzleGetFullsOperation();
        try
        {
            puzzleFullsGetOp.status = status;
            puzzleFullsGetOp["on-complete"] = (Action<PuzzleGetFullsOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onFullsRetreived.Invoke(op.puzzleFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            puzzleFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void Register(RegisterPuzzleRequest registerPuzzleRequest)
    {
        PuzzleRegisterOperation referredRegisterOp = new PuzzleRegisterOperation();
        try
        {
            referredRegisterOp.registerPuzzleRequest = registerPuzzleRequest;
            referredRegisterOp["on-complete"] = (Action<PuzzleRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt64(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            referredRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE
    public void UpdatePuzzle(RegisterPuzzleRequest registerPuzzleRequest)
    {
        PuzzlePutOperation referredPutOp = new PuzzlePutOperation();
        try
        {
            referredPutOp.registerPuzzleRequest = registerPuzzleRequest;
            referredPutOp["on-complete"] = (Action<PuzzlePutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(op.response);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            referredPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Accept(PostModerationRequest postModerationRequest)
    {
        PuzzleAcceptPutOperation acceptPutOp = new PuzzleAcceptPutOperation();
        try
        {
            acceptPutOp.postModerationRequest = postModerationRequest;
            acceptPutOp["on-complete"] = (Action<PuzzleAcceptPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(op.response);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            acceptPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void Reject(PostModerationRequest postModerationRequest)
    {
        PuzzleRejectPutOperation rejectPutOp = new PuzzleRejectPutOperation();
        try
        {
            rejectPutOp.postModerationRequest = postModerationRequest;
            rejectPutOp["on-complete"] = (Action<PuzzleRejectPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(op.response);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            rejectPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}