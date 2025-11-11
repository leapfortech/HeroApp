using UnityEngine;

using Leap.UI.Elements;

using Sirenix.OdinInspector;

public class DisplaySdwAction : MonoBehaviour
{
    [Title("Display")]
    [SerializeField]
    ToggleGroup tggDisplay = null;
    [SerializeField]
    GameObject pnlPersonal = null;
    [SerializeField]
    GameObject pnlExpande = null;

    public void ChangeDisplay()
    {
        pnlPersonal.gameObject.SetActive(tggDisplay.Value == "1");
        pnlExpande.gameObject.SetActive(tggDisplay.Value == "2");
    }
}