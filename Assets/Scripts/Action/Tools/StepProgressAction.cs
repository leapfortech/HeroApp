using UnityEngine;
using UnityEngine.UI;


public class StepProgressAction : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    RectTransform segmentsContainer;

    [SerializeField]
    GameObject segmentOnPrefab;
    [SerializeField]
    GameObject segmentOffPrefab;

    [Header("Layout")]
    [SerializeField]
    int totalSteps = 5;

    [SerializeField]
    float segmentSpacing = 10f;

    public void DisplayStep(int currentStep)
    {
        totalSteps = Mathf.Max(1, totalSteps);
        currentStep = Mathf.Clamp(currentStep, 1, totalSteps);

        BuildSegments(currentStep);
    }

    private void BuildSegments(int currentStep)
    {
        foreach (Transform child in segmentsContainer)
            Destroy(child.gameObject);

        HorizontalLayoutGroup layout = segmentsContainer.GetComponent<HorizontalLayoutGroup>();

        if (layout != null)
            layout.spacing = segmentSpacing;

        for (int i = 1; i <= totalSteps; i++)
        {
            bool isCurrent = i == currentStep;

            GameObject prefabToUse = isCurrent ? segmentOnPrefab : segmentOffPrefab;

            GameObject segment = Instantiate(prefabToUse, segmentsContainer);

            LayoutElement layoutElement = segment.GetComponent<LayoutElement>();

            if (layoutElement == null)
                layoutElement = segment.AddComponent<LayoutElement>();

            layoutElement.flexibleWidth = 1;

            if (isCurrent)
            {
                Leap.UI.Elements.Text txtStep = segment.GetComponentInChildren<Leap.UI.Elements.Text>();

                if (txtStep != null)
                    txtStep.TextValue = $"Paso {currentStep}";
            }
        }
    }
}