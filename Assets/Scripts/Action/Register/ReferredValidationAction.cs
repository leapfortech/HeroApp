using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class ReferredValidationAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    InputField ifdCode = null;

    [Title("Action")]
    [SerializeField]
    Button btnValidate = null;

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
        btnValidate?.AddAction(Validate);
    }

    private void Validate()
    {
        if (!ElementHelper.Validate(ifdCode))
            return;

        ScreenDialog.Instance.Display();

        referredService.Validate(ifdCode.Text);
    }

    public void ApplyValidation(int response)
    {
        ifdCode.Clear();
        
        if (response == 0)
        {
            ChoiceDialog.Instance.Error("Código de referido", "El código de referido es incorrecto.");
            return;
        }

        PageManager.Instance.ChangePage(pagNext);
    }
}
