using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class OnboardingUpdateAction : MonoBehaviour
{
    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Stages")]
    [SerializeField]
    Text txtStage = null;

    [PropertySpace(5f)]
    [SerializeField, TextArea(2, 5)]
    String[] stages = null;

    [Title("Pages")]
    [SerializeField]
    Page pagDpiFront = null;

    [SerializeField]
    Page pagDpiBack = null;

    [SerializeField]
    Page pagDpiExpired = null;

    [SerializeField]
    Page pagAddress = null;

    [Title("Event")]
    [SerializeField]
    UnityEvent onPagPortrait = null;
    [SerializeField]
    UnityEvent onAddressStarted = null;


    [Title("Flow")]
    [SerializeField]
    PageFlow flowDpiExpired = null;

    [SerializeField]
    PageFlow flowAddress = null;

    private Dictionary<int, int> stageTexts = new Dictionary<int, int>() { {  1 << 7,       0 }, { (1 << 7) + 1, 1 }, { 2 << 7,  2 }, { (2 << 7) + 1,  3 }, { 3 << 7, 4 },
                                                                           { (3 << 7) + 1,  0 }, {  4 << 7,      5 }, { 5 << 7,  6 }, { (5 << 7) + 1,  7 } };

    private void Start()
    {
        btnUpdate?.AddAction(ChangeRequestPage);
    }

    public void InitializeStage()
    {
        txtStage.TextValue = stages[stageTexts[StateManager.Instance.OnboardingStage]];
    }

    private void ChangeRequestPage()
    {
        switch (StateManager.Instance.OnboardingStage)
        {
            case 1 << 7:            // Bad Quality DpiFront
                PageManager.Instance.ChangePage(pagDpiFront);
                break;
            case (1 << 7) + 1:      // Dpi Error
                flowDpiExpired.Clear();
                PageManager.Instance.ChangePage(pagDpiExpired);
                break;
            case 2 << 7:            // Bad Quality DpiBack
                PageManager.Instance.ChangePage(pagDpiBack);
                break;
            case (2 << 7) + 1:      // Dpi Expired
                flowDpiExpired.Clear();
                PageManager.Instance.ChangePage(pagDpiExpired);
                break;
            case 3 << 7:            // Bad Quality Portrait
                onPagPortrait.Invoke();
                break;
            case (3 << 7) + 1:      // Bad Quality DpiFront
                PageManager.Instance.ChangePage(pagDpiFront);
                break;
            case 4 << 7:            // Bad Quality Bill
                flowAddress.Clear();
                onAddressStarted.Invoke();
                PageManager.Instance.ChangePage(pagAddress);
                break;
        }
    }
}
