using UnityEngine;
using MPUIKIT;

using Leap.UI.Elements;
using System.Collections;

public class CarouselAction : MonoBehaviour
{
    [Header("Display")]
    [SerializeField]
    public GameObject[] displays;

    [Space]
    [Header("Indicator")]
    [SerializeField]
    public GameObject indicatorPrefab; 
    [SerializeField]
    public Transform indicatorParent;

    [SerializeField]
    Vector2 sizeOn = new Vector2(60, 16);
    [SerializeField]
    Vector2 sizeOff = new Vector2(36, 16);

    [SerializeField]
    Color colorOn = new Color32(0x6A, 0x6A, 0x6A, 0xFF);
    [SerializeField]
    Color colorOff = new Color32(0xD9, 0xD9, 0xD9, 0xFF);


    [Header("Action")]
    [SerializeField]
    public float autoSlideInterval = 3f;
    [SerializeField]
    public Button btnNext;
    [SerializeField]
    public Button btnPrev;

    private int currentIndex = 0;
    private float timer;
    private GameObject[] indicators;

    void Start()
    {
        if (displays.Length == 0)
            return;

        CreateIndicators();
        UpdateDisplay();

        btnNext?.AddAction(NextImage);
        btnPrev.AddAction(PreviousImage);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= autoSlideInterval)
        {
            timer = 0;
            NextImage();
        }
    }

    void CreateIndicators()
    {
        foreach (Transform child in indicatorParent)
            Destroy(child.gameObject);

        indicators = new GameObject[displays.Length];
        for (int i = 0; i < displays.Length; i++)
        {
            GameObject indicator = Instantiate(indicatorPrefab, indicatorParent);
            indicators[i] = indicator;
        }
    }

    void UpdateIndicators()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            bool active = (i == currentIndex);

            MPImage img = indicators[i].GetComponent<MPImage>();
            RectTransform rt = indicators[i].GetComponent<RectTransform>();

            if (img != null)
                img.color = active ? colorOn : colorOff;

            if (rt != null)
            {
                Vector2 target = active ? sizeOn : sizeOff;
                StartCoroutine(AnimateIndicator(rt, target));
            }
        }
    }

    IEnumerator AnimateIndicator(RectTransform rt, Vector2 target)
    {
        Vector2 start = rt.sizeDelta;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            rt.sizeDelta = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rt.sizeDelta = target;
    }

    void UpdateDisplay()
    {
        for (int i = 0; i < displays.Length; i++)
        {
            if (i == currentIndex)
                displays[i].gameObject.SetActive(true);
            else
                displays[i].gameObject.SetActive(false);
        }
        UpdateIndicators();
    }

    public void NextImage()
    {
        currentIndex = (currentIndex + 1) % displays.Length;
        UpdateDisplay();
        timer = 0;
    }

    public void PreviousImage()
    {
        currentIndex = (currentIndex - 1 + displays.Length) % displays.Length;
        UpdateDisplay();
        timer = 0;
    }
}
