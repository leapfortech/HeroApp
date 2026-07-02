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

    private bool showingInterest = true;

    private RectTransform rectButton = null;
    private float xButton, xIcon;
    private Vector2 posButton, posIcon;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (rectButton != null)
            return;

        rectButton = btnSwitch.GetComponent<RectTransform>();
        btnSwitch?.AddAction(Switch);

        posButton = rectButton.anchoredPosition;
        xButton = posButton.x;
        posIcon = rectIcon.anchoredPosition;
        xIcon = posIcon.x;
    }

    public void Switch()
    {
        showingInterest = !showingInterest;

        Refresh();

        onLocalityChanged?.Invoke(showingInterest);
    }

    public void Refresh()
    {
        Initialize();
        
        Locality locality = showingInterest ? StateManager.Instance.InterestLocality : StateManager.Instance.CurrentLocality;

        DisplayNames(locality.CountryId, locality.StateId);

        MoveSwitch(showingInterest);
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

    private void DisplayNames(long countryId, long stateId)
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
