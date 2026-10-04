using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class FlowControlAction : MonoBehaviour
{
    [Title("Data")]
    [SerializeField]
    DataMapper[] dtms = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagStart = null;
    [SerializeField]
    Page pagRegisterBack = null;
    [SerializeField]
    Page pagUpdateBack = null;

    [Title("Event")]
    [SerializeField]
    UnityEvent onClear = null;

    public bool ClearOnHide { get; set; } = true;

    public void ClearAll()
    {
        if (!ClearOnHide)
            return;

        for (int i = 0; i < dtms.Length; i++)
        {
            dtms[i].ClearElements();
            dtms[i].ClearRecords();
        }

        onClear.Invoke();
    }

    public void ActivateRegister()
    {
        ClearAll();

        btnRegister.gameObject.SetActive(true);
        btnUpdate.gameObject.SetActive(false);

        pagStart.HeaderPage = pagRegisterBack;
    }

    public void ActivateUpdate()
    {
        btnRegister.gameObject.SetActive(false);
        btnUpdate.gameObject.SetActive(true);

        pagStart.HeaderPage = pagUpdateBack;
    }
}