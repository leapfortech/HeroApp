using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class ProjectService : MonoBehaviour
{
    [Serializable]
    public class ProjectLikeIdsEvent : UnityEvent<List<int>> { }


    [SerializeField]
    private ProjectLikeIdsEvent onProjectLikeIdsRetreived = null;

    [SerializeField]
    private UnityStringsEvent onImagesRetreived = null;

    [SerializeField]
    private UnityIntEvent onProjectLikeRegistered = null;


    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET

    // PROJECTS

    // PROJECT LIKE

    public void GetLikeIdsByAppUserId(int appUserId)
    {
        try
        {
            LikeIdsByAppUserIdGetOperation likeIdsGetOp = new LikeIdsByAppUserIdGetOperation();
            likeIdsGetOp.appUserId = appUserId;
            likeIdsGetOp["on-complete"] = (Action<LikeIdsByAppUserIdGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onProjectLikeIdsRetreived.Invoke(op.projectLikeIds);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            likeIdsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // IMAGES

    public void GetImagesById(int id)
    {
        ImagesByIdGetOperation imagesGetOp = new ImagesByIdGetOperation();
        try
        {
            imagesGetOp.id = id;
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

    public void RegisterProjectLike(ProjectLike projectLike)
    {
        LikeRegisterOperation likeRegisterOp = new LikeRegisterOperation();
        try
        {
            likeRegisterOp.projectLike = projectLike;
            likeRegisterOp["on-complete"] = (Action<LikeRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onProjectLikeRegistered.Invoke(Convert.ToInt32(op.projectLikeId));
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
