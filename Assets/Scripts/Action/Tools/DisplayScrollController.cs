using System.Collections.Generic;

using UnityEngine;
using MPUIKIT;

using System.Collections;
using UnityEngine.UI;

public class DisplayScrollController : MonoBehaviour
{
    [Header("Scroll")]
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public RectTransform content;

    [Header("Indicators")]
    [SerializeField] RectTransform pillPrefab;
    [SerializeField] Transform pillParent;

    [Header("Config")]
    public float animTime = 0.25f;

    List<RectTransform> pills = new List<RectTransform>();
    int currentPage = 0;
    int pageCount;

    bool isAnimating = false;

    void Start()
    {
        scrollRect.content = content;
        scrollRect.viewport = viewport;

        float w = viewport.rect.width;
        float h = viewport.rect.height;

        foreach (RectTransform page in content)
            page.sizeDelta = new Vector2(w, h);

        pageCount = content.childCount;

        CreateIndicador();

        currentPage = 0;
        UpdateIndicators(-1, 0);

        SetPage(0, false);
    }

    void Update()
    {
        if (isAnimating)
            return;

        float spacing = ((HorizontalLayoutGroup)content.GetComponent<HorizontalLayoutGroup>())?.spacing ?? 0f;
        float pageWidth = GetPageWidth();
        int page = Mathf.RoundToInt(content.anchoredPosition.x * -1f / pageWidth);

        page = Mathf.Clamp(page, 0, pageCount - 1);

        if (page != currentPage)
        {
            int oldPage = currentPage;
            currentPage = page;
            UpdateIndicators(oldPage, currentPage);
        }
    }

    float GetPageWidth()
    {
        var layout = content.GetComponent<HorizontalLayoutGroup>();
        float spacing = layout ? layout.spacing : 0f;
        return viewport.rect.width + spacing;
    }

    public void Next()
    {
        SetPage(currentPage + 1, true);
    }

    public void Back()
    {
        SetPage(currentPage - 1, true);
    }

    public void SetPage(int index, bool animated)
    {
        if (index < 0 || index >= pageCount)
            return;

        StopAllCoroutines();

        int oldIndex = currentPage;
        currentPage = index;

        UpdateIndicators(oldIndex, currentPage);

        if (animated)
            StartCoroutine(AnimateScroll(index));
        else
            SetScrollInstant(index);
    }

    IEnumerator AnimateScroll(int page)
    {
        isAnimating = true;

        float pageWidth = GetPageWidth();
        Vector2 start = content.anchoredPosition;
        Vector2 target = new Vector2(-page * pageWidth, start.y);

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

    void SetScrollInstant(int page)
    {
        float pageWidth = GetPageWidth();
        content.anchoredPosition = new Vector2(-page * pageWidth, content.anchoredPosition.y);
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

            pill.sizeDelta = new Vector2(36, 16);
            pill.pivot = new Vector2(0.5f, 0.5f);

            var img = pill.GetComponent<MPImage>();
            if (img)
                img.color = Hex("#D9D9D9");

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
                rt.sizeDelta = new Vector2(60, 16);
                if (img) img.color = Hex("#6A6A6A");
            }
            else
            {
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(36, 16);
                if (img) img.color = Hex("#D9D9D9");
            }
        }
    }

    Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}