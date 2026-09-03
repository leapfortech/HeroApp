using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;

//using Sirenix.OdinInspector;

namespace Leap.UI.Dialog
{
    public class InputFieldDialog : SingletonScreen<InputFieldDialog>
    {
        [SerializeField, Space]
        GameObject prefab = null;

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

        private void FillButtons(UnityAction<String> actionOK, UnityAction actionKO = null, String btnTitleOK = null, String btnTitleKO = null)
        {
            if (btnTitleOK != null)
                inputFieldItem.Buttons[0].Title = btnTitleOK;
            if (btnTitleKO != null)
                inputFieldItem.Buttons[1].Title = btnTitleKO;

            inputFieldItem.Buttons[0].SetAction(() => { String text = inputFieldItem.InputField.Text; Hide(true); actionOK?.Invoke(text); });
            inputFieldItem.Buttons[1].SetAction(() => { Hide(true); actionKO?.Invoke(); });
        }

        // InputField

        public void InputField(GameObject prefab, String title, String inputFieldText, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            Hide();

            inputFieldItem = Instantiate(prefab, pageAreaTrf).GetComponent<InputFieldDialogItem>();

            FillInputField(title, inputFieldText);
            FillButtons(actionOK, actionKO, btnTitleOK, btnTitleKO);

            Show();
            FillInputField(title, inputFieldText);

            inputFieldItem.InputField.Focus();
            inputFieldItem.InputField.CaretPosition = int.MaxValue;
        }

        public void InputField(GameObject prefab, String title, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            InputField(prefab, title, "", actionOK, actionKO, btnTitleOK, btnTitleKO);
        }

        public void InputField(String title, String inputFieldText, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            InputField(prefab, title, inputFieldText, actionOK, actionKO, btnTitleOK, btnTitleKO);
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
        }

        // Hide
        public void Hide()
        {
            Hide(false);
        }

        public void Hide(bool delayed)
        {
            ScreenDialog.Instance.Hide();

            if (delayed)
                StartCoroutine(DelayedHide());
            else
                MainHide();
        }

        private IEnumerator DelayedHide()
        {
            yield return null;

            if (!MainHide())
                yield break;
        }

        private bool MainHide()
        {
            if (inputFieldItem != null)
            {
                for (int i = 0; i < inputFieldItem.Buttons.Length; i++)
                    inputFieldItem.Buttons[i].ClearActions();

                inputFieldItem.transform.SetParent(null);
                Destroy(inputFieldItem.gameObject);
                inputFieldItem = null;
            }

            if (!bkgDialog.activeSelf)
                return false;

            PageManager.Instance.OnPageChanged -= Hide;

            bkgDialog.SetActive(false);
            return true;
        }
    }
}