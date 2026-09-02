using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;

using Sirenix.OdinInspector;

namespace Leap.UI.Dialog
{
    public class InputFieldDialog : SingletonScreen<InputFieldDialog>
    {
#if UNITY_EDITOR
        [Title("Prefab")]
#endif
        [SerializeField]
        GameObject prefab = null;

#if UNITY_EDITOR
        [Title("Buttons")]
#endif
        [SerializeField]
        String buttonOK = "Confirmar";

        [SerializeField]
        String buttonKO = "Regresar";

        GameObject bkgDialog;
        Transform pageAreaTrf;
        InputFieldDialogItem inputFieldItem;

        private void Awake()
        {
            bkgDialog = transform.GetChild(0).gameObject;
            pageAreaTrf = bkgDialog.transform.GetChild(1);
        }

        public bool IsVisible()
        {
            return bkgDialog.activeSelf;
        }

        // Preset Title
        String presetTitle = null;

        public void SetTitle(String title)
        {
            presetTitle = String.IsNullOrEmpty(title) ? null : title;
        }

        // Fill Item
        private void FillTitle(String title)
        {
            inputFieldItem.Title.TextValue = presetTitle ?? (!String.IsNullOrEmpty(title) ? title : PageManager.Instance.CurrentPage.HeaderTitle);
            presetTitle = null;
        }

        private void FillInputField(String title, String inputField)
        {
            FillTitle(title);
            inputFieldItem.InputField.Text = inputField;
        }

        private String CleanText(String text)
        {
            if (String.IsNullOrEmpty(text))
                return "";
            text = Regex.Unescape(text);
            if (text[0] == '"' && text[text.Length - 1] == '"')
                text = text.Substring(1, text.Length - 2);
            int idx = text.IndexOf('¶');
            if (idx != -1)
                text = text.Substring(idx + 1);
            return text;
        }

        private void FillButtons(UnityAction<String> actionOK, UnityAction actionKO = null, String btnTitleOK = null, String btnTitleKO = null)
        {
            inputFieldItem.Buttons[0].Title = btnTitleOK == null ? buttonOK : btnTitleOK;
            inputFieldItem.Buttons[1].Title = btnTitleKO == null ? buttonKO : btnTitleKO;

            inputFieldItem.Buttons[0].AddAction(() => { String text = inputFieldItem.InputField.Text; Hide(); actionOK?.Invoke(text); });
            inputFieldItem.Buttons[1].AddAction(() => { Hide(); actionKO?.Invoke(); });
        }

        // InputField

        public void InputField(GameObject prefab, String title, String inputField, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            Hide();

            inputFieldItem = Instantiate(prefab, pageAreaTrf).GetComponent<InputFieldDialogItem>();

            FillInputField(title, inputField);
            FillButtons(actionOK, actionKO, btnTitleOK, btnTitleKO);

            Show();
        }

        public void InputField(GameObject prefab, String title, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            InputField(prefab, title, "", actionOK, actionKO, btnTitleOK, btnTitleKO);
        }

        public void InputField(String title, String inputField, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            InputField(prefab, title, inputField, actionOK, actionKO, btnTitleOK, btnTitleKO);
        }

        public void InputField(String title, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            InputField(prefab, title, actionOK, actionKO, btnTitleOK, btnTitleKO);
        }

        // Show
        private void Show()
        {
            PageManager.Instance.OnPageChanged += Hide;
            bkgDialog.SetActive(true);

            ThemeManager.Instance.RefreshStylesInHierarchy(inputFieldItem.gameObject);

            if (inputFieldItem.StretchedTrf == null || inputFieldItem.Texts.Length == 0 || inputFieldItem.Texts[0] == null)
                return;

            Invoke(nameof(SetHeight), 0.025f);
        }

        private void SetHeight()
        {
            RectTransform rectDialog = inputFieldItem.GetComponent<RectTransform>();

            float yMin = rectDialog.sizeDelta.y + inputFieldItem.StretchedTrf.sizeDelta.y;
            float yLines = inputFieldItem.Texts[0].LinesHeight;

            if (yLines <= yMin)
                return;

            rectDialog.sizeDelta = new Vector2(rectDialog.sizeDelta.x, rectDialog.sizeDelta.y + yLines - yMin);
        }

        // Hide
        public void Hide()
        {
            ScreenDialog.Instance.Hide();

            if (inputFieldItem != null)
            {
                for (int i = 0; i < inputFieldItem.Buttons.Length; i++)
                    inputFieldItem.Buttons[i].ClearActions();

                inputFieldItem = null;
            }

            if (inputFieldItem != null)
            {
                inputFieldItem.transform.SetParent(null);
                Destroy(inputFieldItem.gameObject);
                inputFieldItem = null;
            }

            if (!bkgDialog.activeSelf)
                return;

            PageManager.Instance.OnPageChanged -= Hide;
            bkgDialog.SetActive(false);
        }
    }
}