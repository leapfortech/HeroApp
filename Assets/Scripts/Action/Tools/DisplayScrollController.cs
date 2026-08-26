using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UHorizontalLayoutGroup = UnityEngine.UI.HorizontalLayoutGroup;
using MPUIKIT;

using Leap.UI.Elements;

public class DisplayScrollController : MonoBehaviour
{
    [Header("Scroll")]
    public UnityEngine.UI.ScrollRect scrollRect;

    [Header("Indicators")]
    [SerializeField]
    RectTransform pillPrefab;
    [SerializeField]
    Transform pillParent;
    [SerializeField] 
    Color indicatorOnColor = Color.white;
    [SerializeField] 
    Color indicatorOffColor = Color.gray;

    [SerializeField] 
    Vector2 indicatorOnSize = new Vector2(24, 24);
    [SerializeField] 
    Vector2 indicatorOffSize = new Vector2(24, 24);

    [Header("Nav")]
    [SerializeField]
    Text txtCurrentPage = null;
    [SerializeField]
    Button btnNext = null;
    [SerializeField]
    Button btnPrev = null;
    [Space, SerializeField]
    ToggleGroup tggNav = null;
    [Space, SerializeField]
    Toggle[] tglNav = null;

    [Header("Config")]
    public float animTime = 0.25f;

    RectTransform viewport;
    RectTransform content;

    int currentPage = 0;
    int pageCount;

    float spacing = 0f;
    float pageWidth = 0f;

    List<RectTransform> pills = new List<RectTransform>();

    bool initialized = false;
    bool isAnimating = false;
    bool tglPressed = false;

    void Start()
    {
        scrollRect.onValueChanged.AddListener(OnScrollChanged);

        content = scrollRect.content;
        viewport = scrollRect.viewport;
        pageCount = content.childCount;

        Canvas.ForceUpdateCanvases();
        spacing = content.GetComponent<UHorizontalLayoutGroup>().spacing;
        pageWidth = ((RectTransform)content.GetChild(0)).rect.width;

        if (pillPrefab != null && pillParent != null)
        {
            CreateIndicador();
            UpdateIndicators(-1, 0);
        }

        InitPosition();

        initialized = true;
    }

    public void InitPosition()
    {
        currentPage = 0;
        SetPage(0, false);
        UpdateButtons();
    }
    public void OnToggleChanged()
    {
        if (tggNav == null || !initialized)
            return;

        if (!tglPressed)
            SetPage(Convert.ToInt32(tggNav.Value), true);
        
        tglPressed = false;
    }

    void OnPageChanged()
    {
        if (txtCurrentPage != null)
            txtCurrentPage.TextValue = (currentPage + 1).ToString() + "/" + pageCount.ToString();

        if (tglNav == null || tglNav.Length == 0 || tglNav.Length <= currentPage)
            return;

        if (initialized)
            tglPressed = true;

        tglNav[currentPage].Press();
    }

    void OnScrollChanged(Vector2 vPos)
    {
        if (isAnimating)
            return;

        float pos = -content.anchoredPosition.x;
        float step = pageWidth + spacing;

        int page = Mathf.RoundToInt(pos / step);
        page = Mathf.Clamp(page, 0, pageCount - 1);

        if (page == currentPage)
            return;

        int oldPage = currentPage;
        currentPage = page;

        if (pillPrefab != null && pillParent != null)
            UpdateIndicators(oldPage, currentPage);

        UpdateButtons();

        OnPageChanged();
    }

    void SetScrollInstant(int page)
    {
        content.anchoredPosition = new Vector2(-(page * pageWidth) - (spacing * page), content.anchoredPosition.y);
    }

    public void Next()
    {
        SetPage(currentPage + 1, true);
    }

    public void Back()
    {
        SetPage(currentPage - 1, true);
    }

    public void SetPage(int index)
    {
        SetPage(index, true);
    }

    public void SetPage(int index, bool animated)
    {
        if (index < 0 || index >= pageCount)
            return;

        int oldIndex = currentPage;
        currentPage = index;

        UpdateButtons();

        OnPageChanged();

        if (pillPrefab != null && pillParent != null)
            UpdateIndicators(oldIndex, currentPage);

        // DisplayScroll está apagado
        if (!isActiveAndEnabled)
        {
            SetScrollInstant(index);
            return;
        }

        StopAllCoroutines();

        if (animated)
            StartCoroutine(AnimateScroll(index));
        else
            SetScrollInstant(index);
    }

    void UpdateButtons()
    {
        btnPrev.Interactable = currentPage > 0;
        btnNext.Interactable = currentPage < pageCount - 1;
    }

    IEnumerator AnimateScroll(int page)
    {
        isAnimating = true;

        Vector2 start = content.anchoredPosition;

        Vector2 target = new Vector2(-(page * pageWidth) - (spacing * page), start.y);

        float t = 0;
        while (t < animTime)
        {
            t += Time.deltaTime;
            content.anchoredPosition = Vector2.Lerp(start, target, t / animTime);
            yield return null;
        }

        content.anchoredPosition = target;
        isAnimating = false;
    }

    void CreateIndicador()
    {
        pills.Clear();

        foreach (Transform child in pillParent)
            Destroy(child.gameObject);

        for (int i = 0; i < pageCount; i++)
        {
            RectTransform pill =
                Instantiate(pillPrefab, pillParent).GetComponent<RectTransform>();

            pill.sizeDelta = indicatorOffSize;//new Vector2(16, 16);
            pill.pivot = new Vector2(0.5f, 0.5f);

            var img = pill.GetComponent<MPImage>();
            if (img)
                img.color = indicatorOffColor;//Hex("#A0A0A0");

            pills.Add(pill);
        }
    }

    void UpdateIndicators(int oldIndex, int newIndex)
    {
        for (int i = 0; i < pills.Count; i++)
        {
            RectTransform rt = pills[i];
            var img = rt.GetComponent<MPImage>();

            if (i == newIndex)
            {
                float pivotX = newIndex > oldIndex ? 0f : 1f;
                rt.pivot = new Vector2(pivotX, 0.5f);
                rt.sizeDelta = indicatorOnSize;//new Vector2(16, 16);
                if (img) img.color = indicatorOnColor;//Hex("#FFFFFF");
            }
            else
            {
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = indicatorOffSize;// new Vector2(16, 16);
                if (img) img.color = indicatorOffColor;//Hex("#A0A0A0");
            }
        }
    }

    Color Hex(String hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}