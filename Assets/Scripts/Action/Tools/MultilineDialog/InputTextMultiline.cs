using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.UI.Elements;

namespace Leap.UI.Dialog
{
    [Serializable]
    public class InputTextMultiline : MonoBehaviour
    {
        [SerializeField, Space]
        public UnityStringEvent onTextChanged = null;

        private Text txtTitle = null;
        private Text txtMultiline = null;
        private Button btnMultiline = null;

        private void Awake()
        {
            Text[] texts = GetComponentsInChildren<Text>(true);
            txtTitle = texts[0];
            txtMultiline = texts[1];

            btnMultiline = GetComponentInChildren<Button>(true);
            btnMultiline.SetAction(Display);
        }

        private void Display()
        {
            MultilineDialog.Instance.Display(txtTitle.TextValue, txtMultiline.TextValue, Apply, null);
        }

        private void Apply(String text)
        {
            txtMultiline.TextValue = text;
            onTextChanged?.Invoke(text);
        }
    }
}