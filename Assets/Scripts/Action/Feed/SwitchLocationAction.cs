using UnityEngine;

using Leap.Core.Tools;
using Leap.Data.Collections;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class SwitchLocationAction : MonoBehaviour
{
    [Title("Switch")]
    [SerializeField]
    private Button btnSwitch = null;
    [SerializeField]
    private RectTransform rectIcon = null;

    [Title("Location")]
    [SerializeField]
    public Text txtCountry = null;
    [SerializeField]
    public Text txtState = null;
    [SerializeField]
    public Image imgCountryFlag = null;

    [Title("Values")]
    [SerializeField]
    public ValueList vllCountry = null;
    [SerializeField]
    public ValueList vllState = null;

    [Title("Event")]
    [SerializeField]
    private UnityBoolEvent onLocalityChanged = null;

    private Locality interestLocality = null;
    private Locality currentLocality = null;

    private bool showingInterest = true;

    private RectTransform rectButton = null;
    private float xButton, xIcon;
    private Vector2 posButton, posIcon;

    private void Start()
    {
        rectButton = btnSwitch.GetComponent<RectTransform>();
        btnSwitch?.AddAction(Switch);

        posButton = rectButton.anchoredPosition;
        xButton = posButton.x;
        posIcon = rectIcon.anchoredPosition;
        xIcon = posIcon.x;

        interestLocality = StateManager.Instance.InterestLocality;
        currentLocality = StateManager.Instance.CurrentLocality;

        showingInterest = true;

        Refresh(false);
    }

    public void Switch()
    {
        if (currentLocality == null || interestLocality == null)
            return;

        showingInterest = !showingInterest;

        Refresh();
    }

    private void Refresh(bool launchEvent = true)
    {
        Locality locality = showingInterest ? interestLocality : currentLocality;

        Display(locality.CountryId, locality.StateId);

        MoveSwitch(showingInterest);

        if (launchEvent)
            onLocalityChanged?.Invoke(showingInterest ? true : false);
    }

    private void MoveSwitch(bool interest)
    {
        if (rectButton == null)
            return;

        posButton.x = interest ? -xButton : xButton;
        rectButton.anchoredPosition = posButton;

        posIcon.x = interest ? -xIcon : xIcon;
        rectIcon.anchoredPosition = posIcon;
    }

    private void Display(long countryId, long stateId)
    {
        if (txtCountry != null)
        {
            txtCountry.TextValue = vllCountry.FindRecordCellString(countryId, "Name");
            imgCountryFlag.Sprite = vllCountry.FindRecordCellSprite(countryId, "Flag");
        }

        if (txtState != null)
            txtState.TextValue = vllState.FindRecordCellString(stateId, "Name");
    }
}
