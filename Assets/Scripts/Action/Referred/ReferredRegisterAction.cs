using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class ReferredRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;
    [Space]
    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmIdentity = null;

    [Title("Result")]
    [SerializeField]
    Text txtCode = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    ReferredService referredService = null;

    private void Awake()
    {
        referredService = GetComponent<ReferredService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        for (int i = 0; i < elementValues.Length; i++)
            elementValues[i].Clear();
    }

    public void SelectCountryId(int countryId)
    {
        cmbPhoneCountry.Select(countryId);
    }

    private void Register()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Identity identity = dtmIdentity.BuildClass<Identity>();
        long appUserId = StateManager.Instance.AppUser.Id;

        referredService.Register(new RegisterReferredRequest(appUserId, identity));
    }

    public void ApplyReferred(String referredIds)
    {
        Clear();

        txtCode.TextValue = referredIds.Split('|')[1].TrimEnd('"');
        StateManager.Instance.ReferredCount.Count += 1;
        PageManager.Instance.ChangePage(pagNext);
    }
}
