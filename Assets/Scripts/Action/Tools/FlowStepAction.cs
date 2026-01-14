using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class FlowStepAction : MonoBehaviour
{
    [Title("Images")]
    [SerializeField]
    Image[] imgCurrentSteps = null;
    [SerializeField]
    Image[] imgCheckSteps = null;

    public void DisplayCurrentStep(int currentStep)
    {
        currentStep -= 1;

        for (int i = 0; i < imgCurrentSteps.Length; i++)
        {
            if (i < currentStep)
                imgCheckSteps[i].gameObject.SetActive(true);
            else
                imgCheckSteps[i].gameObject.SetActive(false);

            if (i == currentStep)
                imgCurrentSteps[i].gameObject.SetActive(true);
            else
                imgCurrentSteps[i].gameObject.SetActive(false);
        }
    }
}