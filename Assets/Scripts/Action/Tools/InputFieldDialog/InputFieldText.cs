using System;
using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

namespace Leap.UI.Dialog
{
    [Serializable]
    public class InputFieldText : MonoBehaviour
    {
        [SerializeField, Space]
        public String title;

        [SerializeField, Space]
        public Text txtInputField = null;

        [SerializeField, Space]
        public Button btnInputField = null;

        private void Awake()
        {
            btnInputField.SetAction(Display);
        }

        private void Display()
        {
            InputFieldDialog.Instance.InputField(title, txtInputField.TextValue, Apply, null);
        }

        private void Apply(String text)
        {
            txtInputField.TextValue = text;
        }
    }
}