using UnityEngine;

using Leap.UI.Page;

using Sirenix.OdinInspector;


public class HomeAction : MonoBehaviour
{
    [Title("Carousel")]
    [SerializeField]
    GameObject carouselActive = null;
    [SerializeField]
    GameObject carouselInactive = null;

    [Title("Page")]
    [SerializeField]
    Page pagHome = null;

    public void RefreshHome()
    {
        bool localityStatus = StateManager.Instance.InterestLocality == null;

        carouselActive.SetActive(!localityStatus);
        carouselInactive.SetActive(localityStatus);
    }

    public bool FromSandwich { get; set; } = false;

    public bool ChangeBackPage()
    {
        PageManager.Instance.ChangePage(pagHome);

        if (FromSandwich)
            SandwichMenu.Instance.Open();

        return false;
    }
}