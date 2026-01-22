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

    [Title("Event")]
    [SerializeField]
    UnityEvent onSelected = null;

    [Title("Page")]
    [SerializeField]
    Page pagStart = null;
    [SerializeField]
    Page pagRegisterBack = null;
    [SerializeField]
    Page pagUpdateBack = null;

    bool registerInitialized = false;
    bool updateInitialized = false;

    void ClearAll()
    {
        for (int i = 0; i < dtms.Length; i++)
            dtms[i].ClearElements();
    }

    public void ActivateRegister()
    {
        if (!registerInitialized)
        {
            ClearAll();
            onSelected?.Invoke();
            registerInitialized = true;
        }

        btnRegister.gameObject.SetActive(true);
        btnUpdate.gameObject.SetActive(false);

        pagStart.HeaderPage = pagRegisterBack;
        updateInitialized = false;
    }

    public void ActivateUpdate()
    {
        if (!updateInitialized)
        {
            ClearAll();
            onSelected?.Invoke();
            updateInitialized = true;
        }

        btnRegister.gameObject.SetActive(false);
        btnUpdate.gameObject.SetActive(true);

        pagStart.HeaderPage = pagUpdateBack;
        registerInitialized = false;
    }
}