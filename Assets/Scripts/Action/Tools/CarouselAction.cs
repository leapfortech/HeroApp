using UnityEngine;
using MPUIKIT;

using Leap.UI.Elements;

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
    public Color colorOn = Color.white;
    [SerializeField]
    public Color colorOff = Color.gray;


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
            MPImage indicatorImage = indicators[i].GetComponent<MPImage>();
            
            if (indicatorImage != null)
                indicatorImage.color = (i == currentIndex) ? colorOn : colorOff;
        }
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
