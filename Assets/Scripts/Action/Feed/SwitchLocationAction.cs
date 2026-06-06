using UnityEngine;

using Leap.UI.Elements;
using Leap.Core.Tools;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class SwitchLocationAction : MonoBehaviour
{
    [Space, Title("Details")]
    [SerializeField]
    public Text txtCountry = null;
    [SerializeField]
    public Text txtState = null;

    [Space, Title("Values")]
    [SerializeField]
    public ValueList vllCountry = null;
    [SerializeField]
    public ValueList vllState = null;

    [Space, Title("Event")]
    [SerializeField]
    private UnityIntEvent onLocationChanged = null;

    [Title("Action")]
    [SerializeField]
    private RectTransform switchTarget = null;
    [SerializeField]
    private float xCurrent = 0f;
    [SerializeField]
    private float xInterest = 60f;
    [SerializeField]
    private Button btnSwitch = null;

    private Locality interestLocality = null;
    private Locality currentLocality = null;

    private bool showingInterest = true;

    private void Start()
    {
        btnSwitch?.AddAction(Switch);

        if (switchTarget == null && btnSwitch != null)
            switchTarget = btnSwitch.GetComponent<RectTransform>();
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
        if (switchTarget == null)
            return;

        Vector2 pos = switchTarget.anchoredPosition;
        pos.x = interest ? xInterest : xCurrent;
        switchTarget.anchoredPosition = pos;
    }

    private void Display(long countryId, long stateId)
    {
        if (txtCountry != null)
            txtCountry.TextValue = vllCountry.FindRecordCellString(countryId, "Name");

        if (txtState != null)
            txtState.TextValue = vllState.FindRecordCellString(stateId, "Name");
    }
}
