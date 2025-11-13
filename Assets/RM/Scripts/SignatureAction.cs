using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class SignatureAction : MonoBehaviour
{
    [Title("Fields")]
    [SerializeField]
    InputField ifdSignatureId = null;

    [SerializeField]
    Text txtSignature = null;

    [SerializeField]
    UILineDrawer ctrlUILine = null;

    [Title("Action")]
    [SerializeField]
    Button btnLoadSignature = null;

    [SerializeField]
    Button btnSaveSignature = null;

    IdentityService identityService = null;

    private void Awake()
    {
        identityService = GetComponent<IdentityService>();
    }

    private void Start()
    {
        btnLoadSignature?.AddAction(GetSignature);
        btnSaveSignature?.AddAction(SaveSignature);
    }

    private void GetSignature()
    {
        if (ifdSignatureId == null)
            return;

        if (!Int32.TryParse(ifdSignatureId.Text, out int signatureId))
        {
            ChoiceDialog.Instance.Error("<b>SignatureId</b> es necesario");
            return;
        }

        ScreenDialog.Instance.Display();
        //identityService.GetSignature(signatureId);
    }

    public void DisplaySignature(String strokes)
    {
        ctrlUILine.CreateLines(strokes);
        ScreenDialog.Instance.Hide();
    }

    private void SaveSignature()
    {
        if (txtSignature == null)
            return;

        txtSignature.TextValue = ctrlUILine.GetStrokes();
    }
}
