using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Data.Collections;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class ReferenceRegisterAction : MonoBehaviour
{
    [Space]
    [Title("Params")]
    [SerializeField]
    int maxCount = 4;

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;
    [Space]
    [SerializeField]
    ComboAdapter cmbPhoneCountry = null;

    [Space]
    [Title("Lists")]
    [SerializeField]
    ListScroller lstReference = null;
    [SerializeField]
    Text txtEmpty;

    [Space]
    [Title("Data")]
    [SerializeField]
    DataMapper dtmReference = null;

    [Space]
    [Title("Value")]
    [SerializeField]
    ValueList vllReferenceType = null;

    [Title("Action")]
    [SerializeField]
    Button btnNew = null;
    [SerializeField]
    Button btnAdd = null;
    [SerializeField]
    Button btnEdit = null;
    [SerializeField]
    Button btnRemove = null;

    [Title("Page")]
    [SerializeField]
    Page pagReference = null;
    [SerializeField]
    Page pagAddUpdate = null;
    [SerializeField]
    Page pagNext = null;
    [SerializeField]
    Page pagInvestment = null;

    [Title("Message")]
    [SerializeField]
    String refRegisteredTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String refRegisteredMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";

    List<InvestmentReference> references = new List<InvestmentReference>();
    InvestmentService investmentService = null;

    int investmentId = -1, idx = -1;
    bool isUpdate = false;

    private void Awake()
    {
        investmentService = GetComponent<InvestmentService>();
    }

    private void Start()
    {
        btnNew?.AddAction(New);
        btnAdd?.AddAction(Add);
        btnEdit?.AddAction(Edit);
        btnRemove?.AddAction(Remove);
    }

    public void Clear()
    {
        investmentId = -1;
        isUpdate = false;
        lstReference.Clear();
        references.Clear();
        dtmReference.ClearElements();
    }

    public void SetInvestmentRequest((int investmentId, bool isUpdate) investmentRequest)
    {
        Clear();
        this.investmentId = investmentRequest.investmentId;
        this.isUpdate = investmentRequest.isUpdate;
    }

    public void Select(int idx)
    {
        this.idx = idx;

        dtmReference.ClearElements();
        dtmReference.PopulateClass<InvestmentReference>(references[idx]);

        btnAdd.gameObject.SetActive(false);
        btnRemove.gameObject.SetActive(true);
        btnEdit.gameObject.SetActive(true);

        PageManager.Instance.ChangePage(pagAddUpdate);
    }

    public void New()
    {
        dtmReference.ClearElements();

        btnAdd.gameObject.SetActive(true);
        btnRemove.gameObject.SetActive(false);
        btnEdit.gameObject.SetActive(false);

        cmbPhoneCountry.Select(84);

        PageManager.Instance.ChangePage(pagAddUpdate);
    }

    public void Add()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        InvestmentReference reference = dtmReference.BuildClass<InvestmentReference>();
        reference.InvestmentId = investmentId;
        reference.AppUserId = StateManager.Instance.AppUser.Id;
        references.Add(reference);

        Refresh();

        PageManager.Instance.ChangePage(pagReference);
    }

    public void Edit()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        InvestmentReference reference = dtmReference.BuildClass<InvestmentReference>();

        references[idx] = reference;

        Refresh();

        PageManager.Instance.ChangePage(pagReference);
    }

    public void Remove()
    {
        references.RemoveAt(idx);

        Refresh();

        PageManager.Instance.ChangePage(pagReference);
    }

    public void Refresh()
    {
        lstReference.Clear();

        for (int i = 0; i < references.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, references[i].Name);
            scrollerValue.SetText(1, vllReferenceType.FindRecordCellString(references[i].ReferenceTypeId, "Name"));
            lstReference.ApplyAddValue(scrollerValue);
        }
        
        if (references.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (references.Count < maxCount)
            btnNew.gameObject.SetActive(true);
        else
            btnNew.gameObject.SetActive(false);
    }

    public bool Register()
    {
        ScreenDialog.Instance.Display();

        if (!isUpdate)
            investmentService.RegisterReferences(references.ToArray());
        else
            investmentService.UpdateReferences(references.ToArray());

        return false;
    }

    public void ApplyReferences(int[] referenceIds)
    {
        if (!isUpdate)
        {
            StateManager.Instance.UpdateInvestmentStatus(investmentId, 7);
            ChoiceDialog.Instance.Info(refRegisteredTitle, refRegisteredMessage, () => PageManager.Instance.ChangePage(pagNext));
        }
        else
        {
            StateManager.Instance.UpdateInvestmentMotive(new int[] { investmentId, 0 });
            ChoiceDialog.Instance.Info(refRegisteredTitle, refRegisteredMessage, () => PageManager.Instance.ChangePage(pagInvestment));
        }

        Clear();
    }
}
