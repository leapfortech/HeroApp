using UnityEngine;
using UnityEngine.UI;


public class StepProgressAction : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    RectTransform segmentsContainer;

    [SerializeField]
    GameObject segmentDonePrefab;
    [SerializeField]
    GameObject segmentCurrentPrefab;
    [SerializeField]
    GameObject segmentPendingPrefab;

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
            GameObject prefabToUse;

            if (i < currentStep)
                prefabToUse = segmentDonePrefab;
            else if (i == currentStep)
                prefabToUse = segmentCurrentPrefab;
            else
                prefabToUse = segmentPendingPrefab;

            GameObject segment = Instantiate(prefabToUse, segmentsContainer);

            LayoutElement layoutElement = segment.GetComponent<LayoutElement>();

#if UNITY_EDITOR
            if (layoutElement == null)
                Debug.LogError($"LayoutElement NOT found in Segment Prefab {prefabToUse.name}");
#endif

            layoutElement.flexibleWidth = 1;

            if (i == currentStep)
            {
                Leap.UI.Elements.Text txtStep = segment.GetComponentInChildren<Leap.UI.Elements.Text>();

                if (txtStep != null)
                    txtStep.TextValue = $"Paso {currentStep}";
            }
        }
    }
}