using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;

namespace Leap.UI.Dialog
{
    public class MultilineDialog : SingletonScreen<MultilineDialog>
    {
        [SerializeField, Space]
        GameObject multilinePrefab = null;

        GameObject bkgDialog;
        Transform pageAreaTrf;
        MultilineDialogItem multilineItem;

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
            multilineItem.Title.TextValue = presetTitle ?? (!String.IsNullOrEmpty(title) ? title : PageManager.Instance.CurrentPage.HeaderTitle);
            presetTitle = null;
        }

        private void FillInputField(String title, String inputField)
        {
            FillTitle(title);
            multilineItem.InputField.Text = inputField;
        }

        private void FillButtons(UnityAction<String> actionOK, UnityAction actionKO = null, String btnTitleOK = null, String btnTitleKO = null)
        {
            if (btnTitleOK != null)
                multilineItem.Buttons[0].Title = btnTitleOK;
            if (btnTitleKO != null)
                multilineItem.Buttons[1].Title = btnTitleKO;

            multilineItem.Buttons[0].SetAction(() => { String text = multilineItem.InputField.Text; Hide(true); actionOK?.Invoke(text); });
            multilineItem.Buttons[1].SetAction(() => { Hide(true); actionKO?.Invoke(); });
        }

        // InputField

        public void Display(GameObject prefab, String title, String inputFieldText, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            Hide();

            multilineItem = Instantiate(prefab, pageAreaTrf).GetComponent<MultilineDialogItem>();

            FillInputField(title, inputFieldText);
            FillButtons(actionOK, actionKO, btnTitleOK, btnTitleKO);

            Show();
            FillInputField(title, inputFieldText);

            multilineItem.InputField.Focus();
            multilineItem.InputField.CaretPosition = int.MaxValue;
        }

        public void Display(GameObject prefab, String title, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            Display(prefab, title, "", actionOK, actionKO, btnTitleOK, btnTitleKO);
        }

        public void Display(String title, String inputFieldText, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            Display(multilinePrefab, title, inputFieldText, actionOK, actionKO, btnTitleOK, btnTitleKO);
        }

        public void Display(String title, UnityAction<String> actionOK, UnityAction actionKO, String btnTitleOK = null, String btnTitleKO = null)
        {
            Display(multilinePrefab, title, actionOK, actionKO, btnTitleOK, btnTitleKO);
        }

        // Show
        private void Show()
        {
            PageManager.Instance.OnPageChanged += Hide;
            bkgDialog.SetActive(true);

            ThemeManager.Instance.RefreshStylesInHierarchy(multilineItem.gameObject);
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
            if (multilineItem != null)
            {
                for (int i = 0; i < multilineItem.Buttons.Length; i++)
                    multilineItem.Buttons[i].ClearActions();

                multilineItem.transform.SetParent(null);
                Destroy(multilineItem.gameObject);
                multilineItem = null;
            }

            if (!bkgDialog.activeSelf)
                return false;

            PageManager.Instance.OnPageChanged -= Hide;

            bkgDialog.SetActive(false);
            return true;
        }
    }
}