using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.Data.Web;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class SupportAction : MonoBehaviour
{
    [Title("Element")]
    [SerializeField]
    Text txtPhone = null;

    
    public void RefreshSupport()
    {
        txtPhone.TextValue = AppManager.Instance.GetParamValue("SupportPhoneDisplay");
    }
}