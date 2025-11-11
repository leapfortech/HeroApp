using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class DisplayInformationAction : MonoBehaviour
{
    [Title("Display")]
    [SerializeField]
    ToggleGroup tggDisplay = null;
    [SerializeField]
    Image imgDetails1 = null;
    [SerializeField]
    Image imgDetails2 = null;
    [SerializeField]
    Image imgFinancial1 = null;
    [SerializeField]
    Image imgFinancia2 = null;
    [SerializeField]
    Image imgProject1 = null;
    [SerializeField]
    Image imgProject2 = null;

    [SerializeField]
    ScrolledText sclDetails = null;
    [SerializeField]
    ScrolledText sclAdvantages = null;


    public void ChangeDisplayInfo()
    {
        imgDetails1.gameObject.SetActive(tggDisplay.Value == "1");
        imgDetails2.gameObject.SetActive(tggDisplay.Value == "1");

        imgFinancial1.gameObject.SetActive(tggDisplay.Value == "2");
        imgFinancia2.gameObject.SetActive(tggDisplay.Value == "2");

        imgProject1.gameObject.SetActive(tggDisplay.Value == "3");
        imgProject2.gameObject.SetActive(tggDisplay.Value == "3");

        if (tggDisplay.Value == "1")
            sclDetails.DelayScale();
        else if (tggDisplay.Value == "3")
            sclAdvantages.DelayScale();
    }
}