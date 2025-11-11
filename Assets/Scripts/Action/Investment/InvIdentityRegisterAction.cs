using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Data.Collections;
using Leap.Core.Tools;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class InvIdentityRegisterAction : MonoBehaviour
{
    [Serializable]
    public class PepIdentityRequestsEvent : UnityEvent<PepIdentityRequest[]> { }
    [Space]
    [Title("Params")]
    [SerializeField]
    bool isSigner = true;
    [SerializeField]
    int maxCount = 4;

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;
    [Space]
    [SerializeField]
    InputField ifdRelatioship = null;
    [SerializeField]
    ComboAdapter cmbPercentage = null;
    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;
    [Space]
    [SerializeField]
    ToggleGroup tggPep = null;
    [SerializeField]
    ToggleGroup tggPepIdentity = null;
    [SerializeField]
    ToggleGroup tggCpe = null;

    [Space]
    [Title("Lists")]
    [SerializeField]
    ListScroller lstNationality = null;
    [SerializeField]
    ListScroller lstInvIdentity = null;
    [SerializeField]
    Text txtEmpty;

    [Space]
    [Title("Data")]
    [SerializeField]
    ValueList vllNationality = null;
    [SerializeField]
    ValueList vllPercentage = null;
    [SerializeField]
    DataMapper dtmIdentity = null;
    [SerializeField]
    DataMapper dtmAddress = null;
    [SerializeField]
    DataMapper dtmPep = null;
    [SerializeField]
    DataMapper dtmCpe = null;

    [Title("Action")]
    [SerializeField]
    Button btnNew = null;

    [Title("Event")]
    [SerializeField]
    private UnityStringsEvent onNationalitiesLoaded = null;
    [SerializeField]
    private PepIdentityRequestsEvent onPepIdentityRequestsAdded = null;

    [Title("Page")]
    [SerializeField]
    Page pagIdentityDpi = null;
    [SerializeField]
    Page pagName = null;

    [SerializeField]
    Page pagBirth = null;
    [SerializeField]
    Page pagDpi = null;

    [SerializeField]
    Page pagInvIdentity = null;
    [SerializeField]
    Page pagNext = null;
    [SerializeField]
    Page pagInvestment = null;

    [Title("Message")]
    [SerializeField]
    String askRegisteredTitle = "Guardar";
    [SerializeField, TextArea(2, 4)]
    String askRegisteredMessage = "¿Estás seguro de guardar los datos?";
    [SerializeField]
    String invIdentityRegisteredTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String invIdentityRegisteredMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";

    [SerializeField]
    String dateErrorTitle = "Error de fecha";
    [SerializeField, TextArea(2, 4)]
    String birthDateErrorMessage = "La fecha de nacimiento es incorrecta. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 4)]
    String issueDateErrorMessage = "La fecha de emisión es incorrecta. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 4)]
    String dueDateErrorMessage = "La fecha de vencimiento es incorrecta. Revisa e intenta de nuevo.";
    [SerializeField, TextArea(2, 4)]
    String dpiDatesErrorMessage = "La fecha de emisión o vencimiento son incorrectas. Revisa e intenta de nuevo.";

    List<InvestmentIdentityRequest> invIdentitiesRequest = new List<InvestmentIdentityRequest>();
    InvestmentService investmentService = null;

    int investmentId = -1, idx = -1;
    bool isUpdate = false;

    bool isAdd = false;
    double currPercentage = 1.0f;

    List<PepIdentityRequest> pepIdentityRequests = new List<PepIdentityRequest>();

    private void Awake()
    {
        investmentService = GetComponent<InvestmentService>();
    }

    private void Start()
    {
        btnNew?.AddAction(New);
    }

    public void Clear()
    {
        investmentId = -1;
        isUpdate = false;
        lstInvIdentity.Clear();
        ClearNew();

        currPercentage = 1.0f;
        vllPercentage.MaxValue = Convert.ToInt32(currPercentage) * 100;
        vllPercentage.RefreshRecords();
    }

    public void ClearNew()
    {
        lstNationality.Clear();
        vllNationality.ClearRecords();
        ifdRelatioship.Clear();
        cmbPercentage.Clear();

        dtmIdentity.ClearElements();
        dtmAddress.ClearElements();
        dtmPep.ClearElements();
        dtmCpe.ClearElements();
    }

    public void SetInvestmentRequest((int investmentId, bool isUpdate) investmentRequest)
    {
        this.investmentId = investmentRequest.investmentId;
        this.isUpdate = investmentRequest.isUpdate;
    }

    public void SetPepIdentityRequest(List<PepIdentityRequest> pepIdentityRequests)
    {
        this.pepIdentityRequests.Clear();

        this.pepIdentityRequests.AddRange(pepIdentityRequests);

        Refresh();
    }

    public void Select(int idx)
    {
        ClearNew();

        isAdd = false;

        this.idx = idx;

        dtmIdentity.PopulateClass<Identity>(invIdentitiesRequest[idx].Identity);
        dtmAddress.PopulateClass<Address>(invIdentitiesRequest[idx].Address);
        
        if (invIdentitiesRequest[idx].Identity.IsPep == 1)
            dtmPep.PopulateClass<Pep>(invIdentitiesRequest[idx].Pep);

        onPepIdentityRequestsAdded.Invoke(invIdentitiesRequest[idx].PepIdentityRequests);

        if (invIdentitiesRequest[idx].Identity.IsCpe == 1)
            dtmCpe.PopulateClass<Cpe>(invIdentitiesRequest[idx].Cpe);

        ifdRelatioship.Text = invIdentitiesRequest[idx].InvestmentIdentity.Relationship;
        ifdRelatioship.Revalidate(true);

        cmbPercentage.gameObject.SetActive(!isSigner);

        if (!isSigner)
        {
            vllPercentage.MaxValue = Convert.ToInt32(invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage * 100.0f) + Convert.ToInt32(currPercentage * 100.0f);
            vllPercentage.RefreshRecords();

            cmbPercentage.SelectIndex(Convert.ToInt32(invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage * 100f) - 1);
        }


        String[] nationalities = invIdentitiesRequest[idx].Identity.NationalityIds.Split("|");

        onNationalitiesLoaded.Invoke(nationalities);

        PageManager.Instance.ChangePage(pagName);
    }

    private void New()
    {
        ChoiceDialog.Instance.Menu("Menú", new UnityAction[] { () => { PageManager.Instance.ChangePage(pagIdentityDpi); }, 
                                                               () => { PageManager.Instance.ChangePage(pagName); },},
                                           new String[] { "Capturar DPI", "Ingreso manual", "Regresar" });

        ClearNew();

        isAdd = true;

        cmbPhoneCountry.Select(84);

        cmbPercentage.gameObject.SetActive(!isSigner);

        if (!isSigner)
        {
            vllPercentage.MaxValue = Convert.ToInt32(currPercentage * 100.0f);
            vllPercentage.RefreshRecords();
            cmbPercentage.SelectIndex(Convert.ToInt32(currPercentage * 100.0f - 1));
        }

        onPepIdentityRequestsAdded.Invoke(null);
    }

    public bool AddEdit()
    {
        if (isAdd)
            Add();
        else
            Edit();

        return false;
    }

    private void Add()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        // Identity

        Identity identity = dtmIdentity.BuildClass<Identity>();

        if (!ValidateIdentity(identity))
            return;

        String nationalityIds = vllNationality.RecordCount > 0 ? vllNationality[0].Id.ToString() : "";
        for (int i = 1; i < vllNationality.RecordCount; i++)
            nationalityIds += "|" + vllNationality[i].Id;

        identity.NationalityIds = nationalityIds;
        identity.AppUserId = -1;
        identity.BirthStateId = identity.BirthStateId == 0 ? -1 : identity.BirthStateId;
        identity.BirthCityId = identity.BirthCityId == 0 ? -1 : identity.BirthCityId;

        // Investment Identity

        InvestmentIdentity invIdentity = new InvestmentIdentity();
        invIdentity.InvestmentId = investmentId;
        invIdentity.InvestmentIdentityTypeId = isSigner ? 1 : 2;
        invIdentity.Relationship = ifdRelatioship.Text;
        //invIdentity.Pourcentage = isSignatory ? 0.0f : ((cmbPourcentage.SelectedIndex + 1) / 100f);

        if (!isSigner)
        {
            invIdentity.Pourcentage = (cmbPercentage.SelectedIndex + 1) / 100f;
            currPercentage -= (cmbPercentage.SelectedIndex + 1) / 100.0f;
        }
        else
            invIdentity.Pourcentage = 0.0f;

        // Investment Identity Request

        invIdentitiesRequest.Add(new InvestmentIdentityRequest(invIdentity, identity, dtmAddress.BuildClass<Address>(), 
                                                                                      tggPep.Value == "1" ? dtmPep.BuildClass<Pep>() : null,
                                                                                      tggPepIdentity.Value == "1" ? pepIdentityRequests.ToArray() : null,
                                                                                      tggCpe.Value == "1" ? dtmCpe.BuildClass<Cpe>() : null));

        dtmIdentity.ClearElements();
        dtmAddress.ClearElements();
        dtmPep.ClearElements();
        dtmCpe.ClearElements();

        Refresh();

        PageManager.Instance.ChangePage(pagInvIdentity);
    }

    private void Edit()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        Identity identity = dtmIdentity.BuildClass<Identity>();

        if (!ValidateIdentity(identity))
            return;

        String nationalityIds = vllNationality.RecordCount > 0 ? vllNationality[0].Id.ToString() : "";
        for (int i = 1; i < vllNationality.RecordCount; i++)
            nationalityIds += "|" + vllNationality[i].Id;

        identity.NationalityIds = nationalityIds;
        identity.AppUserId = -1;
        identity.BirthStateId = identity.BirthStateId == 0 ? -1 : identity.BirthStateId;
        identity.BirthCityId = identity.BirthCityId == 0 ? -1 : identity.BirthCityId;

        invIdentitiesRequest[idx].Identity = identity;

        Address address = dtmAddress.BuildClass<Address>();

        invIdentitiesRequest[idx].Address = address;

        invIdentitiesRequest[idx].InvestmentIdentity.Relationship = ifdRelatioship.Text;
        //invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage = isSignatory ? 0.0f : ((cmbPourcentage.SelectedIndex + 1) / 100f);

        if (!isSigner)
        {
            if (((cmbPercentage.SelectedIndex + 1) / 100f) >= invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage)
                currPercentage -= ((cmbPercentage.SelectedIndex + 1) / 100f) - invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage;
            else
                currPercentage += invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage - ((cmbPercentage.SelectedIndex + 1) / 100f);

            invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage = (cmbPercentage.SelectedIndex + 1) / 100f;
        }
        else
            invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage = 0.0f;


        invIdentitiesRequest[idx].Pep = tggPep.Value == "1" ? dtmPep.BuildClass<Pep>() : null;
        invIdentitiesRequest[idx].PepIdentityRequests = tggPepIdentity.Value == "1" ? pepIdentityRequests.ToArray() : null;
        invIdentitiesRequest[idx].Cpe = tggCpe.Value == "1" ? dtmCpe.BuildClass<Cpe>() : null;

        Refresh();

        PageManager.Instance.ChangePage(pagInvIdentity);
    }

    public void Remove(int idx)
    {
        if (!isSigner)
            currPercentage += invIdentitiesRequest[idx].InvestmentIdentity.Pourcentage;

        invIdentitiesRequest.RemoveAt(idx);

        Refresh();

        PageManager.Instance.ChangePage(pagInvIdentity);
    }

    public void Refresh()
    {
        lstInvIdentity.Clear();

        for (int i = 0; i < invIdentitiesRequest.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, invIdentitiesRequest[i].Identity.FirstName1 + " " + invIdentitiesRequest[i].Identity.LastName1);
            scrollerValue.SetText(1, !isSigner ? (invIdentitiesRequest[i].InvestmentIdentity.Pourcentage * 100.0d).ToString() + "%" : invIdentitiesRequest[i].InvestmentIdentity.Relationship);
            lstInvIdentity.ApplyAddValue(scrollerValue);
        }
        
        if (invIdentitiesRequest.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        btnNew.gameObject.SetActive(invIdentitiesRequest.Count < maxCount && Math.Abs(currPercentage) > 1e-6f);
    }

    public void RegisterEmpty()
    {
        ScreenDialog.Instance.Display();
        if (isSigner)
        {
            if (!isUpdate)
                investmentService.RegisterSignatoryEmpty(investmentId);
        }
        else
        {
            if (!isUpdate)
                investmentService.RegisterBeneficiaryEmpty(investmentId);
        }
    }

    
    public bool AskRegister()
    {
        if (!isSigner && Math.Abs(currPercentage) > 1e-6f)
        {
            ChoiceDialog.Instance.Error("Beneficiarios", "Los porcentajes deben sumar el 100%.");
            return false;
        }

        ChoiceDialog.Instance.Info(askRegisteredTitle, askRegisteredMessage, () => Register(), (UnityAction)null, "Sí", "No");

        return false;
    }
    
    private void Register()
    {
        ScreenDialog.Instance.Display();

        if (isSigner)
        {
            if (!isUpdate)
                investmentService.RegisterSignatories(invIdentitiesRequest.ToArray());
            else
                investmentService.UpdateSignatories(invIdentitiesRequest.ToArray());
        }
        else
        {
            if (!isUpdate)
                investmentService.RegisterBeneficiaries(invIdentitiesRequest.ToArray());
            else
                investmentService.UpdateBeneficiaries(invIdentitiesRequest.ToArray());
        }
    }

    public void ApplyInvIdentityEmtpy()
    {
        ApplyInvIdentity(null);
    }

    public void ApplyInvIdentity(int[] ids)
    {
        if (!isUpdate)
        {
            StateManager.Instance.UpdateInvestmentStatus(investmentId, isSigner ? 8 : 9);
            ChoiceDialog.Instance.Info(invIdentityRegisteredTitle, invIdentityRegisteredMessage, () => PageManager.Instance.ChangePage(pagNext));
        }
        else
        {
            StateManager.Instance.UpdateInvestmentMotive(new int[] { investmentId, 0 });
            ChoiceDialog.Instance.Info(invIdentityRegisteredTitle, invIdentityRegisteredMessage, () => PageManager.Instance.ChangePage(pagInvestment));
        }

        Clear();
    }

    private bool ValidateIdentity(Identity identity)
    {
        if (identity.BirthDate == new DateTime(0001, 1, 1))
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, birthDateErrorMessage, () => PageManager.Instance.ChangePage(pagBirth));
            return false;
        }

        if (CalculateAge(identity.BirthDate) < 18)
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, birthDateErrorMessage, () => PageManager.Instance.ChangePage(pagBirth));
            return false;
        }

        if (identity.DpiIssueDate == new DateTime(0001, 1, 1))
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, issueDateErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }

        if (identity.DpiDueDate == new DateTime(0001, 1, 1))
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, dueDateErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }

        int years = identity.DpiDueDate.Year - identity.DpiIssueDate.Year;
        double days = (identity.DpiIssueDate - identity.DpiDueDate.AddYears(-10)).TotalDays;

        if (years != 10 || days != 1)
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, dpiDatesErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }

        if (identity.DpiDueDate.Date < DateTime.Now.Date)
        {
            ChoiceDialog.Instance.Error(dateErrorTitle, dueDateErrorMessage, () => PageManager.Instance.ChangePage(pagDpi));
            return false;
        }
        return true;
    }

    public int CalculateAge(DateTime birthDate)
    {
        int age = DateTime.Today.Year - birthDate.Year;

        if (DateTime.Today.Month < birthDate.Month)
            --age;
        else if (DateTime.Today.Month == birthDate.Month && DateTime.Today.Day < birthDate.Day)
            --age;
        return age;
    }
}
