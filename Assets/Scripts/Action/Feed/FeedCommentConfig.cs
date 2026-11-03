using System;
using UnityEngine.Events;

using UnityEngine;
using Leap.Core.Tools;
using Leap.UI.Page;

using Sirenix.OdinInspector;

[Serializable]
public class FeedCommentConfig
{
    [Title("Events")]

    [SerializeField]
    public UnityIntEvent onCommentChanged = null;
    [SerializeField]
    public UnityEvent onPagBackActivated = null;
}
