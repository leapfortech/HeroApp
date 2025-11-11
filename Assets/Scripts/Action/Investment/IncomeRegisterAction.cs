using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;
using Leap.Data.Mapper;
using Leap.Data.Collections;

using Sirenix.OdinInspector;
using Leap.Graphics.Tools;

public class IncomeRegisterAction : MonoBehaviour
{
    [Space]
    [Title("Params")]
    [SerializeField]
    int maxIncomeCount = 4;
    [SerializeField]
    int maxDocCount = 4;
    [SerializeField]
    String docType;

    [Title("Elements")]
    [Space]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Space]
    [Title("Add Doc")]
    [SerializeField]
    String title = null;
    [PropertySpace(6f)]
    [SerializeField]
    ChoiceOption[] options = null;


    [Title("Validate Doc")]
    [Space]
    [SerializeField]
    Image imgCurrentDoc = null;

    [Space]
    [Title("Fields")]
    [SerializeField]
    ComboAdapter cmbIncomeCurrency = null;
    [SerializeField]
    ComboAdapter cmbExpensesCurrency = null;
    
    [Space]
    [Space]
    [Title("Lists")]
    [SerializeField]
    ListScroller lstIncome;
    [SerializeField]
    Text txtIncomeEmpty;
    [SerializeField]
    ListScroller lstDoc = null;
    [SerializeField]
    Text txtDocEmpty;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmEconomics = null;
    [SerializeField]
    DataMapper dtmIncome = null;

    [Space]
    [Title("Value")]
    [SerializeField]
    ValueList vllIncomeType = null;

    [Title("Action")]
    [SerializeField]
    Button btnNew = null;
    [SerializeField]
    Button btnAdd = null;
    [SerializeField]
    Button btnEdit = null;
    [SerializeField]
    Button btnRemove = null;

    [SerializeField]
    Button btnDocNew = null;

    [Title("Page")]
    [SerializeField]
    Page pagIncome = null;
    [SerializeField]
    Page pagAddUpdate = null;
    [SerializeField]
    Page pagDocs = null;
    [SerializeField]
    Page pagNext = null;
    [SerializeField]
    Page pagInvestment = null;

    [Title("Message")]
    [SerializeField]
    String incomeAddedTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String incomeAddedMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";

    EconomicsService incomeService = null;

    List<Income> incomes = new List<Income>();
    List<Texture2D> docs = new List<Texture2D>();
    Texture2D currentDoc = null;

    int investmentId = -1, idx = -1, backPage = -1;
    bool isUpdate = false;

    private void Awake()
    {
        incomeService = GetComponent<EconomicsService>();
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
        lstIncome.Clear();
        
        incomes.Clear();
        dtmEconomics.ClearElements();
        dtmIncome.ClearElements();

        lstDoc.Clear();
        docs.Clear();
    }

    public void FillCurrencies(int currencyId)
    {
        cmbIncomeCurrency.Select(currencyId);
        cmbExpensesCurrency.Select(currencyId);
    }

    public void SetBackPage(int backPage)
    {
        this.backPage = backPage;
    }

    public void SetInvestmentRequest((int investmentId, bool isUpdate) investmentRequest)
    {
        Clear();
        this.investmentId = investmentRequest.investmentId;
        this.isUpdate = investmentRequest.isUpdate;
    }

    public void SetCurrentDoc(Texture2D currentDoc)
    {
        this.currentDoc = currentDoc;
        imgCurrentDoc.Sprite = currentDoc.CreateSprite(false);
    }

    public bool AddNewDoc()
    {
        if (docs.Count != 0)
            PageManager.Instance.ChangePage(pagDocs);
        else
            ChoiceDialog.Instance.Menu(title, options);

        return false;
    }

    public void Select(int idx)
    {
        this.idx = idx;

        dtmIncome.ClearElements();
        dtmIncome.PopulateClass<Income>(incomes[idx]);

        btnAdd.gameObject.SetActive(false);
        btnRemove.gameObject.SetActive(true);
        btnEdit.gameObject.SetActive(true);

        PageManager.Instance.ChangePage(pagAddUpdate);
    }

    public void New()
    {
        dtmIncome.ClearElements();

        btnAdd.gameObject.SetActive(true);
        btnRemove.gameObject.SetActive(false);
        btnEdit.gameObject.SetActive(false);

        PageManager.Instance.ChangePage(pagAddUpdate);
    }

    public void Add()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        Income income = dtmIncome.BuildClass<Income>();
        income.InvestmentId = investmentId;

        incomes.Add(income);

        RefreshIncome();

        PageManager.Instance.ChangePage(pagIncome);
    }

    public void Edit()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        Income income = dtmIncome.BuildClass<Income>();

        incomes[idx] = income;

        RefreshIncome();

        PageManager.Instance.ChangePage(pagIncome);
    }

    public void Remove()
    {
        incomes.RemoveAt(idx);

        RefreshIncome();

        PageManager.Instance.ChangePage(pagIncome);
    }

    public void RefreshIncome()
    {
        lstIncome.Clear();

        for (int i = 0; i < incomes.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, vllIncomeType.FindRecordCellString(incomes[i].IncomeTypeId, "Name"));
            scrollerValue.SetText(1, incomes[i].Detail);
            lstIncome.ApplyAddValue(scrollerValue);
        }


        if (incomes.Count > 0)
            txtIncomeEmpty.gameObject.SetActive(false);
        else
            txtIncomeEmpty.gameObject.SetActive(true);

        if (incomes.Count < maxIncomeCount)
            btnNew.gameObject.SetActive(true);
        else
            btnNew.gameObject.SetActive(false);
    }

    public bool AddDoc()
    {
        docs.Add(currentDoc);
        RefreshDoc();
        PageManager.Instance.ChangePage(pagDocs);
        return false;
    }

    public void RemoveDoc(int idx)
    {
        docs.RemoveAt(idx);
        RefreshDoc();
    }

    public void RefreshDoc()
    {
        lstDoc.Clear();

        for (int i = 0; i < docs.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(3, true);
            scrollerValue.SetText(0, "Constacia");
            scrollerValue.SetText(1, $"Imagen {(i + 1)}");
            scrollerValue.SetSprite(2, docs[i].CreateSprite($"{docType}_{i}"));
            lstDoc.ApplyAddValue(scrollerValue);
        }

        if (docs.Count > 0)
            txtDocEmpty.gameObject.SetActive(false);
        else
            txtDocEmpty.gameObject.SetActive(true);

        if (docs.Count < maxDocCount)
            btnDocNew.gameObject.SetActive(true);
        else
            btnDocNew.gameObject.SetActive(false);
    }


    public bool Register()
    {
        ScreenDialog.Instance.Display();

        Economics economics = dtmEconomics.BuildClass<Economics>();
        economics.InvestmentId = investmentId;

        String[] docs = new String[this.docs.Count];
        for (int i = 0; i < this.docs.Count; i++)
            docs[i] = this.docs[i].CreateSprite($"{docType}_{i}").ToStrBase64(ImageType.JPG);

        EconomicsInfo economicsInfo = new EconomicsInfo(economics, incomes, docs);

        if (!isUpdate)
            incomeService.Register(economicsInfo);
        else
            incomeService.UpdateEconomics(economicsInfo);

        return false;
    }


    public void ApplyEconomics(int[] incomeIds)
    {
        if (!isUpdate)
        {
            StateManager.Instance.UpdateInvestmentStatus(investmentId, 5);
            ChoiceDialog.Instance.Info(incomeAddedTitle, incomeAddedMessage, () => PageManager.Instance.ChangePage(pagNext));
        }
        else
        {
            StateManager.Instance.UpdateInvestmentMotive(new int[] { investmentId, 0 });
            ChoiceDialog.Instance.Info(incomeAddedTitle, incomeAddedMessage, () => PageManager.Instance.ChangePage(pagInvestment));
        }

        Clear();
    }
}
