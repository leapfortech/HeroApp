using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;
using UnityEngine.Events;

public class ModerationAction : MonoBehaviour
{
    [Serializable]
    public class ModerationEvent : UnityEvent<PostModerationRequest> { }

    [Title("Action")]
    [SerializeField]
    Button btnAccept = null;
    [SerializeField]
    Button btnReject = null;

    [Space]
    [Title("Event")]
    [SerializeField]
    ModerationEvent onAccepted = null;
    [SerializeField]
    ModerationEvent onRejected = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    PostModerationRequest postModerationRequest = null;

    private void Start()
    {
        btnAccept?.AddAction(Accept);
        btnReject?.AddAction(Reject);
    }

    public void SetIds(long[] ids)
    {
        if (postModerationRequest == null)
            postModerationRequest = new PostModerationRequest();

        postModerationRequest.PostId = ids[0];      // PostId
        postModerationRequest.SubtypeId = ids[1];   // SubTypeId
    }

    private void Accept()
    {
        ScreenDialog.Instance.Display();

        onAccepted.Invoke(postModerationRequest);
    }

    private void Reject()
    {
        ScreenDialog.Instance.Display();

        onRejected.Invoke(postModerationRequest);
    }

    public void ApplyModeration(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        PageManager.Instance.ChangePage(pagNext);
    }
}
