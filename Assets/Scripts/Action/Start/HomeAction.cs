using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using Leap.Data.Collections;
using Leap.UI.Elements;

using Sirenix.OdinInspector;


public class HomeAction : MonoBehaviour
{
    [Title("Carousel")]
    [SerializeField]
    GameObject carouselActive = null;
    [SerializeField]
    GameObject carouselInactive = null;

    [Title("Elements")]
    [SerializeField]
    Text txtPuzzleTitle1 = null;
    [SerializeField]
    Text txtPuzzleTitle2 = null;

    [Title("Values")]
    [SerializeField]
    ValueList vllCountry = null;


    public void RefreshHome()
    {
        bool localityStatus = StateManager.Instance.InterestLocality == null;

        carouselActive.SetActive(!localityStatus);
        carouselInactive.SetActive(localityStatus);

        if (!localityStatus)
        {
            String demonym = vllCountry.FindRecordCellString(Convert.ToInt32(StateManager.Instance.InterestLocality.CountryId), "Demonym");

            txtPuzzleTitle1.TextValue = !String.IsNullOrWhiteSpace(demonym) ? "Fiesta " : "Diversión y";
            txtPuzzleTitle2.TextValue = !String.IsNullOrWhiteSpace(demonym) ? demonym : " recreación";
        }
    }
}