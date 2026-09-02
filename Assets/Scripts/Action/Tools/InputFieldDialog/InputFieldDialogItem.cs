using System;
using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

namespace Leap.UI.Dialog
{
    [Serializable]
    public class InputFieldDialogItem : MonoBehaviour
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
        public Text[] Texts = null;

#if UNITY_EDITOR
        [LabelText("        Stretched Transform"), LabelWidth(160f), ShowIf("@Texts.Length > 0 && Texts[0] != null")]
#endif
        [SerializeField]
        public RectTransform StretchedTrf = null;

#if UNITY_EDITOR
        [Space, ListDrawerSettings(DefaultExpandedState = true)]
#endif
        [SerializeField]
        public Button[] Buttons = null;
    }
}