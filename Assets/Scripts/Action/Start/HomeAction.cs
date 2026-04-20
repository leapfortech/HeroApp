using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.Data.Web;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class HomeAction : MonoBehaviour
{
    [Title("Carousel")]
    [SerializeField]
    GameObject carouselActive = null;
    [SerializeField]
    GameObject carouselInactive = null;

    [Title("Action")]
    [SerializeField]
    Button btnLocality = null;

    [Title("Pages")]
    [SerializeField]
    Page pagLocality = null;

    private void Start()
    {
        btnLocality?.AddAction(ChangePageLocality);
    }

    public void RefreshHome()
    {
        bool localityStatus = StateManager.Instance.InterestLocality == null;

        carouselActive.SetActive(!localityStatus);
        carouselInactive.SetActive(localityStatus);
    }

    private void ChangePageLocality()
    {
        PageManager.Instance.ChangePage(pagLocality);
    }
}