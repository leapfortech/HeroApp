using System;
using UnityEngine;
using UnityEngine.Events;
using ULayoutElement = UnityEngine.UI.LayoutElement;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using MPUIKIT;

public class StepProgressAction : MonoBehaviour
{
    [Space]
    [SerializeField]
    public int totalStage = 5;
    [SerializeField]
    public int countStep = 3;

    [Header("Points")]
    [SerializeField]
    public RectTransform points;
    [Space]
    [SerializeField]
    public GameObject leftPointPrefab;
    [SerializeField]
    public GameObject centerPointPrefab;
    [SerializeField]
    public GameObject rightPointPrefab;
    [Space]
    [SerializeField]
    public GameObject fullPointPrefab;
    [SerializeField]
    public GameObject currentPointPrefab;
    [SerializeField]
    public GameObject pendingPointPrefab;

    [Header("Steps")]
    [SerializeField]
    MPImage imgStep = null;
    [SerializeField]
    Text txtCurrentStep = null;
    [SerializeField]
    Text txtCountStep = null;

    public void DisplayStage(int currentStage)
    {
        imgStep.fillAmount = (float)currentStage/totalStage;
        txtCurrentStep.TextValue = currentStage.ToString();
        txtCountStep.TextValue = countStep.ToString();
    }


    public void DisplaySteps(int currentStep)
    {
        foreach (Transform child in points)
            Destroy(child.gameObject);

        if (countStep < 2)
            return;

        ULayoutElement leftPoint = Instantiate(leftPointPrefab, points).GetComponent<ULayoutElement>();
        leftPoint.preferredWidth = points.sizeDelta.x / ((countStep - 1) * 2);
        InstantiatePoint(1, currentStep, leftPoint.transform.GetChild(0));

        for (int i = 2; i <= countStep - 1; i++)
        {
            ULayoutElement centerPoint = Instantiate(centerPointPrefab, points).GetComponent<ULayoutElement>();
            InstantiatePoint(i, currentStep, centerPoint.transform);
        }

        ULayoutElement rightPoint = Instantiate(rightPointPrefab, points).GetComponent<ULayoutElement>();
        rightPoint.preferredWidth = leftPoint.preferredWidth;
        InstantiatePoint(countStep, currentStep, rightPoint.transform.GetChild(0));
    }

    private GameObject InstantiatePoint(int idx, int step, Transform trf)
    {
        if (idx < step)
            return Instantiate(fullPointPrefab, trf);

        if (idx == step)
            return Instantiate(currentPointPrefab, trf);

        return Instantiate(pendingPointPrefab, trf);

        
    }
}

