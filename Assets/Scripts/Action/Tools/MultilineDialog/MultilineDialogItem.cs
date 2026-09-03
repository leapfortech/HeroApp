using System;
using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

namespace Leap.UI.Dialog
{
    [Serializable]
    public class MultilineDialogItem : MonoBehaviour
    {
#if UNITY_EDITOR
        [Space, LabelWidth(80f)]
#endif
        [SerializeField]
        public Text Title = null;

#if UNITY_EDITOR
        [Space, LabelWidth(80f)]
#endif
        [SerializeField]
        public InputField InputField = null;

#if UNITY_EDITOR
        [Space, ListDrawerSettings(DefaultExpandedState = true)]
#endif
        [SerializeField]
        public Button[] Buttons = null;
    }
}