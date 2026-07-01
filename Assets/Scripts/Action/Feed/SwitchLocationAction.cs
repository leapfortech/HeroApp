using UnityEngine;

using Leap.UI.Elements;
using Leap.Core.Tools;
using Leap.Data.Collections;

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
    private UnityIntEvent onLocationChanged = null;

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
    }

    public void Init(bool startWithInterest = true)
    {
        interestLocality = StateManager.Instance.InterestLocality;
        currentLocality = StateManager.Instance.CurrentLocality;

        showingInterest = startWithInterest;

        Refresh();
    }

    public void Switch()
    {
        if (currentLocality == null || interestLocality == null)
            return;

        showingInterest = !showingInterest;

        Refresh();
    }

    private void Refresh()
    {
        Locality locality = showingInterest ? interestLocality : currentLocality;

        Display(locality.CountryId, locality.StateId);

        MoveSwitch(showingInterest);

        onLocationChanged?.Invoke(showingInterest ? 1 : 0);
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
