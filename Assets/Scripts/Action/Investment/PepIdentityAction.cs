using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;

public class PepIdentityAction : MonoBehaviour
{
    [Serializable]
    public class PepIdentityRequestsEvent : UnityEvent<List<PepIdentityRequest>> { }

    [Space]
    [Title("Params")]
    [SerializeField]
    int maxCount = 10;

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;
    [Space]
    [SerializeField]
    ToggleGroup tggPepIdentity = null;

    [Space]
    [Title("Lists")]
    [SerializeField]
    ListScroller lstPepIdentity = null;
    [SerializeField]
    Text txtEmpty;

    [Space]
    [Title("Data")]
    [SerializeField]
    DataMapper dtmPepIdentity = null;
    [SerializeField]
    DataMapper dtmIdentity = null;

    [Title("Action")]
    [SerializeField]
    Button btnNew = null;
    [SerializeField]
    Button btnAddEdit = null;

    [Title("Event")]
    [SerializeField]
    private PepIdentityRequestsEvent onPepIdentityRequestsAdded = null;

    [Title("Page")]
    [SerializeField]
    Page pagPepIdentity = null;
    [SerializeField]
    Page pagAddUpdate = null;
    [SerializeField]
    Page pagNext = null;

    int idx = -1;
    bool isAdd = false;
    List<PepIdentityRequest> pepIdentityRequests = new List<PepIdentityRequest>();


    private void Start()
    {
        btnNew?.AddAction(New);
        btnAddEdit?.AddAction(AddEdit);
    }

    public void Clear()
    {
        pepIdentityRequests.Clear();
        ClearNew();
    }
    
    public void ClearNew()
    {
        dtmPepIdentity.ClearElements();
        dtmIdentity.ClearElements();
    }

    public void SetPepIdentityRequest(PepIdentityRequest[] pepIdentityRequests)
    {
        this.pepIdentityRequests.Clear();

        if (pepIdentityRequests != null)
        {
            this.pepIdentityRequests.AddRange(pepIdentityRequests);
            Refresh();
        }
        else
            lstPepIdentity.Clear();

    }

    public void Select(int idx)
    {
        ClearNew();

        isAdd = false;

        this.idx = idx;

        dtmPepIdentity.ClearElements();
        dtmPepIdentity.PopulateClass<PepIdentity>(pepIdentityRequests[idx].PepIdentity);
        dtmIdentity.PopulateClass<Identity>(pepIdentityRequests[idx].Identity);

        PageManager.Instance.ChangePage(pagAddUpdate);
    }

    private void New()
    {
        ClearNew();

        isAdd = true;

        PageManager.Instance.ChangePage(pagAddUpdate);
    }

    private void AddEdit()
    {
        if (isAdd)
            Add();
        else
            Edit();
    }

    private void Add()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        PepIdentity pepIdentity = dtmPepIdentity.BuildClass<PepIdentity>();
        Identity identity = dtmIdentity.BuildClass<Identity>();

        identity.BirthDate = new DateTime(1753, 1, 1);
        identity.DpiIssueDate = new DateTime(1753, 1, 1);
        identity.DpiDueDate = new DateTime(1753, 1, 1);

        identity.AppUserId = -1;
        identity.BirthCountryId = -1;
        identity.BirthStateId = -1;
        identity.BirthCityId = -1;
        identity.MaritalStatusId = -1;
        identity.PhoneCountryId = -1;

        pepIdentityRequests.Add(new PepIdentityRequest(pepIdentity, identity));

        dtmPepIdentity.ClearElements();
        dtmIdentity.ClearElements();

        Refresh();

        PageManager.Instance.ChangePage(pagPepIdentity);
    }

    private void Edit()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        PepIdentity pepIdentity = dtmPepIdentity.BuildClass<PepIdentity>();
        Identity identity = dtmIdentity.BuildClass<Identity>();

        identity.BirthDate = new DateTime(1753, 1, 1);
        identity.DpiIssueDate = new DateTime(1753, 1, 1);
        identity.DpiDueDate = new DateTime(1753, 1, 1);

        identity.AppUserId = -1;
        identity.BirthCountryId = -1;
        identity.BirthStateId = -1;
        identity.BirthCityId = -1;
        identity.MaritalStatusId = -1;
        identity.PhoneCountryId = -1;

        pepIdentityRequests[idx] = new PepIdentityRequest(pepIdentity, identity);

        Refresh();

        PageManager.Instance.ChangePage(pagPepIdentity);
    }

    public void Remove(int idx)
    {
        pepIdentityRequests.RemoveAt(idx);

        Refresh();

        PageManager.Instance.ChangePage(pagPepIdentity);
    }

    public void Refresh()
    {
        if (tggPepIdentity.Value == "0")
        {
            lstPepIdentity.gameObject.SetActive(false);
            txtEmpty.gameObject.SetActive(false);
            btnNew.Interactable = false;
            return;
        }

        lstPepIdentity.gameObject.SetActive(true);
        lstPepIdentity.Clear();

        for (int i = 0; i < pepIdentityRequests.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, pepIdentityRequests[i].Identity.FirstName1 + " " + pepIdentityRequests[i].Identity.LastName1);
            scrollerValue.SetText(1, pepIdentityRequests[i].PepIdentity.InstitutionName);
            lstPepIdentity.ApplyAddValue(scrollerValue);
        }

        if (pepIdentityRequests.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (pepIdentityRequests.Count < maxCount)
            btnNew.Interactable = true;
        else
            btnNew.Interactable = false;
    }

    public bool Done()
    {
        if (tggPepIdentity.Value == "1")
            onPepIdentityRequestsAdded.Invoke(pepIdentityRequests);
        
        PageManager.Instance.ChangePage(pagNext);

        return false;
    }
}
