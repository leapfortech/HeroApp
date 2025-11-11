using System;
using UnityEngine;
using UnityEngine.Events;
using ULayoutElement = UnityEngine.UI.LayoutElement;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using MPUIKIT;

public class StepNavegationAction : MonoBehaviour
{
    [Header("Exit")]
    [SerializeField]
    String title = null;
    [SerializeField, TextArea(3, 6)]
    String message = null;
    [Space]
    [SerializeField]
    String btnTitleOK = null;
    [SerializeField]
    String btnTitleKO = null;

    [Space]
    [Header("Action")]
    [SerializeField]
    Button btnBack = null;
    [SerializeField]
    Button btnExit = null;

    [Space]
    [Header("Pages")]
    [SerializeField]
    Page pagBack = null;

    [Space]
    [Header("Event")]
    [SerializeField]
    UnityEvent onExit = null;

    private void Start()
    {
        btnBack?.AddAction(ChangeBackPage);
        btnExit?.AddAction(ShowChoiceDialog);
    }

    public void ChangeBackPage()
    {
        PageManager.Instance.ChangePage(pagBack);
    }

    public void ShowChoiceDialog()
    {
        ChoiceDialog.Instance.Warning(title, message, () => { onExit.Invoke(); }, null, btnTitleOK, btnTitleKO);
    }
}

