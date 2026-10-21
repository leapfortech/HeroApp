
using UnityEngine;

using Leap.Core.Tools;
using Leap.UI.Extensions;
using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class ContactAction : MonoBehaviour
{
    [Title("Toggles")]
    [SerializeField]
    Toggle tglHasContact = null;
    [SerializeField]
    Toggle tglHasPhone = null;
    [SerializeField]
    Toggle tglHasWhatsApp = null;
    [SerializeField]
    Toggle tglHasEmail = null;

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elmsContact = null;
    [Space, SerializeField]
    ElementValue[] elmsPhone = null;
    [Space, SerializeField]
    ElementValue[] elmsWhatsApp = null;
    [Space, SerializeField]
    ElementValue[] elmsEmail = null;

    [Title("Default Values")]
    [SerializeField]
    ComboAdapter cmbPhonePrefix = null;
    [SerializeField]
    ComboAdapter cmbWhatsAppPrefix = null;

    [Title("Interface")]
    [SerializeField]
    RectTransform trfContactDetails = null;
    [SerializeField]
    GameObject trfEmpty = null;
    [SerializeField]
    GameObject trfPhone = null;
    [SerializeField]
    GameObject trfWhatsApp = null;
    [SerializeField]
    GameObject trfEmail = null;

    [Title("Events")]
    [SerializeField]
    UnityFloatEvent onVisibilityChanged = null;

    float contactInitialHeight = 0f, phoneInitialHeight = 0f;
    bool initialized = false;


    private void Initialize()
    {
        if (initialized)
            return;

        contactInitialHeight = trfContactDetails.sizeDelta.y;
        phoneInitialHeight = trfPhone.GetComponent<RectTransform>().sizeDelta.y;

        initialized = true;
    }


    public void Clear()
    {
        Initialize();

        tglHasContact.Checked = false;
        tglHasPhone.Checked = false;
        tglHasWhatsApp.Checked = false;
        tglHasEmail.Checked = false;

        RefreshElements();
    }

    public void ContactRefresh()
    {
        if (!tglHasContact.Checked)
        {
            tglHasPhone.Checked = false;
            tglHasWhatsApp.Checked = false;
            tglHasEmail.Checked = false;
        }

        RefreshElements();
    }

    public void PhoneRefresh()
    {
        RefreshIndividual();
    }

    public void WhatsAppRefresh()
    {
        RefreshIndividual();
    }

    public void EmailRefresh()
    {
        RefreshIndividual();
    }

    private void RefreshIndividual()
    {
        RefreshElements();
    }

    public void RefreshElements()
    {
        bool hasContact = tglHasContact.Checked;

        SetElementsActive(elmsContact, hasContact);
        SetElementsActive(elmsPhone, hasContact && tglHasPhone.Checked);
        SetElementsActive(elmsWhatsApp, hasContact && tglHasWhatsApp.Checked);
        SetElementsActive(elmsEmail, hasContact && tglHasEmail.Checked);

        trfPhone.SetActive(hasContact && tglHasPhone.Checked);
        trfWhatsApp.SetActive(hasContact && tglHasWhatsApp.Checked);
        trfEmail.SetActive(hasContact && tglHasEmail.Checked);

        if (tglHasPhone.Checked && cmbPhonePrefix.GetSelectedId() == -1)
            cmbPhonePrefix.Select(StateManager.Instance.CurrentLocality.CountryId);

        if (tglHasWhatsApp.Checked && cmbWhatsAppPrefix.GetSelectedId() == -1)
            cmbWhatsAppPrefix.Select(StateManager.Instance.CurrentLocality.CountryId);

        RefreshInterface(hasContact);
    }

    private void RefreshInterface(bool hasContact)
    {
        int activeContacts = 0;
        float offset = contactInitialHeight;

        RectTransform rectPhone = trfPhone.GetComponent<RectTransform>();
        rectPhone.anchoredPosition = new Vector2(0f, -offset);
        if (trfPhone.activeSelf)
        {
            offset += phoneInitialHeight;
            activeContacts++;
        }

        RectTransform rectWhatsApp = trfWhatsApp.GetComponent<RectTransform>();
        rectWhatsApp.anchoredPosition = new Vector2(0f, -offset);
        if (trfWhatsApp.activeSelf)
        {
            offset += phoneInitialHeight;
            activeContacts++;
        }

        RectTransform rectEmail = trfEmail.GetComponent<RectTransform>();
        rectEmail.anchoredPosition = new Vector2(0f, -offset);
        if (trfEmail.activeSelf)
            activeContacts++;

        bool empty = hasContact && !tglHasPhone.Checked && !tglHasWhatsApp.Checked && !tglHasEmail.Checked;
        trfEmpty.SetActive(empty);

        if (empty)
            activeContacts++;

        float newHeight = tglHasContact.Checked ? contactInitialHeight + (activeContacts * phoneInitialHeight) : 0f;
        float deltaY = newHeight - trfContactDetails.sizeDelta.y;

        trfContactDetails.sizeDelta = new Vector2(trfContactDetails.sizeDelta.x, newHeight);

        onVisibilityChanged?.Invoke(deltaY);
    }

    //

    private void SetElementsActive(ElementValue[] elements, bool active)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].gameObject.SetActive(active);

            if (!active)
                elements[i].Clear();
        }
    }
}
