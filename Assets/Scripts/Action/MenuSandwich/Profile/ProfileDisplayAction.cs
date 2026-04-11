using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class ProfileDisplayAction : MonoBehaviour
{
    [Title("Profile")]
    [SerializeField]
    Image[] imgSqrPortraits = null;
    [Space]
    [SerializeField]
    Image[] imgRctPortraits = null;
    [Space]
    [SerializeField]
    Button[] btnSqrPortraits = null;
    [Space]
    [SerializeField]
    Text[] txtAliass = null;
    [Space]
    [SerializeField]
    Text txtName = null;
    [SerializeField]
    Text txtBirthDate = null;
    [SerializeField]
    Text txtBirthPlace = null;
    [SerializeField]
    Text txtAddress = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;

    [Title("Pages")]
    [SerializeField]
    Page pagRegister = null;
    [SerializeField]
    Page pagDisplay = null;

    public void Clear()
    {
        for (int i = 0; i < imgSqrPortraits.Length; i++)
            imgSqrPortraits[i].Clear();
        for (int i = 0; i < imgRctPortraits.Length; i++)
            imgRctPortraits[i].Clear();
        for (int i = 0; i < btnSqrPortraits.Length; i++)
            btnSqrPortraits[i].Clear();
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].Clear();
        txtName.Clear();
        txtBirthDate.Clear();
        txtBirthPlace.Clear();
        txtAddress.Clear();
    }

    public void ChangeToStartPage()
    {
        long obdStatus = (StateManager.Instance.AppUser.Options / (long)Math.Pow(10, 0)) % 10;

        if (obdStatus == 1)
            PageManager.Instance.ChangePage(pagRegister);
        else
            PageManager.Instance.ChangePage(pagDisplay);

        SandwichMenu.Instance.Close();
    }

    public void DisplayProfile()
    {
        // Alias
        for (int i = 0; i < txtAliass.Length; i++)
            txtAliass[i].TextValue = StateManager.Instance.AppUser.Alias;

        // Identity
        bool isNameEmpty = true, isBirthDateEmpty = true, isBirthPlaceEmpty = true;
        Identity identity = StateManager.Instance.Identity;

        if (identity != null)
        { 
            DateTime sqlMinDate = new DateTime(1753, 1, 1);
            isNameEmpty = String.IsNullOrEmpty(identity.FirstName1) && String.IsNullOrEmpty(identity.LastName1);
            isBirthDateEmpty = identity.BirthDate == sqlMinDate;
            isBirthPlaceEmpty = identity.BirthCountryId == -1;
        }

        txtName.TextValue = isNameEmpty ? "Sin nombre" : identity.FirstName1 + " " + identity.LastName1;
        txtBirthDate.TextValue = isBirthDateEmpty ? "Sin fecha de nacimineto" : identity.BirthDate.ToString("dd/MM/yyyy");
        txtBirthPlace.TextValue = isBirthPlaceEmpty ? "Sin lugar de nacimiento" : "De " + vllCountry.FindRecordCellString(identity.BirthCountryId, "Name");

        // Address
        bool isAddressEmpty = true;
        Address address = StateManager.Instance.Address;

        if (address != null)
            isAddressEmpty = address.CountryId == -1;

        txtAddress.TextValue = isAddressEmpty ? "Sin dirección" : "En " + vllCountry.FindRecordCellString(address.CountryId, "Name");

        //Portrait
        Sprite sprite = StateManager.Instance.Portrait;

        if (imgSqrPortraits.Length > 0)
        {
            //Sprite sprite = StateManager.Instance.Portrait.texture.CreateSprite(new Rect(0, 0, 600, 600));
            for (int i = 0; i < imgSqrPortraits.Length; i++)
                imgSqrPortraits[i].Sprite = sprite;
        }

        if (imgSqrPortraits.Length > 0)
        {
            //Sprite sprite = StateManager.Instance.Portrait.texture.CreateSprite(new Rect(0, 40, 180, 240));
            for (int i = 0; i < imgRctPortraits.Length; i++)
                imgRctPortraits[i].Sprite = sprite;
        }

        if (btnSqrPortraits.Length > 0)
        {
            //Sprite sprite = StateManager.Instance.Portrait.texture.CreateSprite(new Rect(0, 0, 600, 600));
            for (int i = 0; i < btnSqrPortraits.Length; i++)
                btnSqrPortraits[i].Sprite = sprite;
        }
    }
}
