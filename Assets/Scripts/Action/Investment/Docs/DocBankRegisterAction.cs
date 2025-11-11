using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class DocBankRegisterAction : MonoBehaviour
{
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
    [Title("Params")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String docType;

    [Space]
    [Title("Lists")]
    [SerializeField]
    ListScroller lstDoc = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnNew = null;

    [Title("Page")]
    [SerializeField]
    Page pagDocs = null;
    [SerializeField]
    Page nextPage = null;
    [SerializeField]
    Page investmentPage = null;

    [Title("Message")]
    [SerializeField]
    String docAddedTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String docAddedMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";

    InvestmentService investmentService = null;

    List<Texture2D> docs = new List<Texture2D>();
    Texture2D currentDoc = null;
    int investmentId = -1;
    int backPage = -1;
    bool isUpdate = false;

    private void Awake()
    {
        investmentService = GetComponent<InvestmentService>();
    }

    public void Clear()
    {
        investmentId = -1;
        isUpdate = false;
        lstDoc.Clear();
        docs.Clear();
    }

    public void SetInvestmentRequest((int investmentId, bool isUpdate) investmentRequest)
    {
        Clear();
        this.investmentId = investmentRequest.investmentId;
        this.isUpdate = investmentRequest.isUpdate;
    }

    public void SetBackPage(int backPage)
    {
        this.backPage = backPage;
    }

    public void AddNewDoc()
    {
        if (docs.Count != 0)
            PageManager.Instance.ChangePage(pagDocs);
        else
            ChoiceDialog.Instance.Menu(title, options);
    }

    public void SetCurrentDoc(Texture2D currentDoc)
    {
        this.currentDoc = currentDoc;
        imgCurrentDoc.Sprite = currentDoc.CreateSprite(false);
    }

    public bool AddList()
    {
        docs.Add(currentDoc);
        Refresh();
        PageManager.Instance.ChangePage(pagDocs);
        return false;
    }

    public void Remove(int idx)
    {
        docs.RemoveAt(idx);
        Refresh();
    }

    public void Refresh()
    {
        lstDoc.Clear();

        for (int i = 0; i < docs.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(3, true);
            scrollerValue.SetText(0, "Recibo de servicios");
            scrollerValue.SetText(1, $"Imagen {(i + 1)}");
            scrollerValue.SetSprite(2, docs[i].CreateSprite($"{docType}_{i}"));
            lstDoc.ApplyAddValue(scrollerValue);
        }

        if (docs.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (docs.Count < maxCount)
            btnNew.gameObject.SetActive(true);
        else
            btnNew.gameObject.SetActive(false);
    }

    public bool Register()
    {
        ScreenDialog.Instance.Display();

        String[] docs = new String[this.docs.Count];
        for (int i = 0; i < this.docs.Count; i++)
            docs[i] = this.docs[i].CreateSprite($"{docType}_{i}").ToStrBase64(ImageType.JPG);

        InvestmentDocRequest investmentDocRequest = new InvestmentDocRequest(investmentId, docs);

        if (!isUpdate)
            investmentService.RegisterDocBank(investmentDocRequest);
        else
            investmentService.UpdateDocBank(investmentDocRequest);

        return false;
    }

    public void ApplyDoc()
    {
        if (!isUpdate)
        {
            StateManager.Instance.UpdateInvestmentStatus(investmentId, 6);
            ChoiceDialog.Instance.Info(docAddedTitle, docAddedMessage, () => PageManager.Instance.ChangePage(nextPage));
        }
        else
        {
            StateManager.Instance.UpdateInvestmentMotive(new int[] { investmentId, 0 });
            ChoiceDialog.Instance.Info(docAddedTitle, docAddedMessage, () => PageManager.Instance.ChangePage(investmentPage));
        }

        Clear();
    }
}
