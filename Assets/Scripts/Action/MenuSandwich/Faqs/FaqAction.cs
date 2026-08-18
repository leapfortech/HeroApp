using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;
using Leap.Data.Collections;
using Leap.UI.Page;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;


public class FaqAction : MonoBehaviour
{
    [Title("Lists")]
    [SerializeField]
    ListScroller lstFaqType = null;
    [SerializeField]
    Text txtFaqTypeEmpty = null;
    [SerializeField]
    ListScroller lstFaq = null;
    [SerializeField]
    Text txtFaqEmpty = null;

    [Title("Elements")]
    [SerializeField]
    Text txtQuestion = null;
    [SerializeField]
    Text txtAnswer = null;

    [Space, Title("Contents")]
    [SerializeField]
    int charsPerLine = 40;
    [SerializeField]
    int lineHeight = 30;
    [SerializeField]
    float contentPadding = 40f;
    [Space, SerializeField]
    RectTransform content = null;
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect = null;

    [Title("Value")]
    [SerializeField]
    ValueList vllFaqType = null;

    [Title("Pages")]
    [SerializeField]
    Page pagQuestions = null;
    [SerializeField]
    Page pagFaq = null;

    FaqService faqService = null;

    Dictionary<long, List<Faq>> faqs = new Dictionary<long, List<Faq>>();

    int faqIdx = -1;
    long faqTypeId = -1;

    private void Awake()
    {
        faqService = GetComponent<FaqService>();
    }

    public void ClearElements()
    {
        txtQuestion.TextValue = "";
        txtAnswer.TextValue = "";
    }

    public void Clear()
    {
        lstFaqType.Clear();
        lstFaq.Clear();
        faqs.Clear();
        faqIdx = -1;
        faqTypeId = -1;
    }

    public void FillTypes()
    {
        txtFaqTypeEmpty.gameObject.SetActive(vllFaqType.RecordCount == 0);

        lstFaqType.ClearValues();

        for (int i = 0; i < vllFaqType.RecordCount; i++)
        {
            ListScrollerValue value = new ListScrollerValue(lstFaqType.ListItem, true);

            value.SetText(0, vllFaqType.FindRecordCellString(i + 1, "Name"));

            lstFaqType.AddValue(value);
        }

        lstFaqType.ApplyValues();
    }


    public void DisplayQuestions(int idx)
    {
        faqTypeId = idx + 1;
        
        if (faqs.ContainsKey(faqTypeId))
        {
            FillQuestions(faqs[faqTypeId]);
            return;
        }

        ScreenDialog.Instance.Display();

        faqService.GetAllByType(faqTypeId);
    }

    public void FillQuestions(List<Faq> faqsType)
    {
        ClearElements();

        faqs[faqTypeId] = faqsType;

        if (faqsType == null || faqsType.Count == 0)
        {
            ShowEmpty();
            return;
        }

        lstFaq.ClearValues();

        txtFaqEmpty.gameObject.SetActive(false);

        for (int i = 0; i < faqsType.Count; i++)
        {
            ListScrollerValue value = new ListScrollerValue(lstFaq.ListItem, true);

            value.SetText(0, faqsType[i].Question);

            lstFaq.AddValue(value);
        }

        lstFaq.ApplyValues();

        PageManager.Instance.ChangePage(pagQuestions);
    }

    void ShowEmpty()
    {
        txtFaqEmpty.gameObject.SetActive(true);
        lstFaq.ApplyClearValues();

        ClearElements();

        PageManager.Instance.ChangePage(pagQuestions);
    }

    public void Display(int idx)
    {
        faqIdx = idx;

        txtQuestion.TextValue = faqs[faqTypeId][idx].Question;
        txtAnswer.TextValue = faqs[faqTypeId][idx].Answer;
        
        RefreshContents();

        PageManager.Instance.ChangePage(pagFaq);
    }

    private void RefreshContents()
    {
        Text txtScroll = content.GetComponentInChildren<Text>();
        int lineCount = Mathf.CeilToInt((float)txtScroll.TextValue.Length / charsPerLine);
        float height = lineCount * lineHeight;

        content.sizeDelta = new Vector2(content.sizeDelta.x, height + contentPadding);

        scrollRect.verticalNormalizedPosition = 1f;
    }
}