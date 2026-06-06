using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.Data.Collections;
using Leap.UI.Elements;

using Sirenix.OdinInspector;


public class DisplayGameProfile : MonoBehaviour
{
    [Title("Element")]
    [SerializeField]
    Text txtName = null;
    [SerializeField]
    Text txtLevel= null;
    [SerializeField]
    Text txtTotalPoints = null;
    [SerializeField]
    Text txtTotalMedals = null;
    [SerializeField]
    Text txtTotalCups = null;
    [SerializeField]
    Image imgCountry = null;

    [Title("Values")]
    [SerializeField]
    ValueList vllCountryFlag = null;


    public void Clear()
    {
        
    }

    public void Display()
    {
        String fullName = $"{StateManager.Instance.Identity?.FirstName1} {StateManager.Instance.Identity?.LastName1}".Trim();

        txtName.TextValue = !String.IsNullOrWhiteSpace(fullName) ? fullName : StateManager.Instance.AppUser.Alias;

        txtLevel.TextValue = "Nivel 1";
        txtTotalPoints.TextValue = StateManager.Instance.GetTotalPuzzlePoints().ToString();
        txtTotalMedals.TextValue = StateManager.Instance.GetTotalPuzzleMedals().ToString();
        txtTotalCups.TextValue = StateManager.Instance.GetTotalPuzzleCups().ToString();

        imgCountry.Sprite = vllCountryFlag.FindRecordCellSprite(StateManager.Instance.InterestLocality.CountryId, "Flag");
    }
}