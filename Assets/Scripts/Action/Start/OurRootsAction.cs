using System;
using UnityEngine;


using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class OurRootsAction : MonoBehaviour
{
    [Title("Actions")]
    [SerializeField]
    Button btnTreatment = null;

    public void RefreshOurRoots()
    {
        String treatmentsEnabled =  AppManager.Instance.GetParamValue("TreatmentsEnabled");
        btnTreatment.gameObject.SetActive(treatmentsEnabled == "1");
    }
}