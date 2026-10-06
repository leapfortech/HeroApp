using System;

using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.UI.Page;

using Sirenix.OdinInspector;

[Serializable]
public class FeedDetailConfig
{
    [Serializable]
    public class ReactionEvent : UnityEvent<long, long, bool> { }

    [Space, SerializeField]
    public Page pagBack = null;

    [Title("Events")]
    [SerializeField]
    public ReactionEvent onReactionChanged = null;

    [SerializeField]
    public UnityBoolEvent onFavoriteChanged = null;
    [SerializeField]
    public UnityBoolEvent onSelectedChanged = null;
}
