using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;


public class ProductPriceAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Space, Title("Details")]
    [SerializeField]
    InputField ifdPrice = null;
    [SerializeField]
    InputField ifdDiscountPrice = null;

    [Title("Action")]
    [SerializeField]
    Button btnValidate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    private void Start()
    {
        btnValidate?.AddAction(Validate);
    }

    public void Validate()
    {
        double.TryParse(ifdPrice.Text, out double price);
        double.TryParse(ifdDiscountPrice.Text, out double discountPrice);

        if (discountPrice > 0 && discountPrice > price)
        {
            ChoiceDialog.Instance.Error("Precio de descuento", "El precio de descuento es mayor al precio regular.");
            return;
        }

        if (!ElementHelper.Validate(elementValues))
            return;

        PageManager.Instance.ChangePage(pagNext);
    }
}