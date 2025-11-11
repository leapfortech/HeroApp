using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Graphics.Tools;
using Leap.Data.Mapper;

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

public class AddressRegisterAction : MonoBehaviour
{
    [Space]
    [Title("Add HouseholdBill")]
    [SerializeField]
    String title = null;
    [PropertySpace(6f)]
    [SerializeField]
    ChoiceOption[] options = null;

    [Title("Validate HouseholdBill")]
    [Space]
    [SerializeField]
    Image imgCurrentHouseholdBill = null;

    [Space]
    [Title("Lists")]
    [SerializeField]
    int maxCount = 4;
    [SerializeField]
    String spriteName;
    [SerializeField]
    ListScroller lstHouseholdBill = null;
    [SerializeField]
    Text txtEmpty;

    [Title("Action")]
    [SerializeField]
    Button btnNew = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmAddress = null;

    [Title("Page")]
    [SerializeField]
    Page pagAddress = null;
    [SerializeField]
    Page pagValidateHouseholdBill = null;
    [SerializeField]
    Page pagHouseholdBills = null;
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField]
    String addressAddedTitle = "Paso Completo";
    [SerializeField, TextArea(2, 4)]
    String addressAddedMessage = "La información fue guardada exitosamente, puedes continuar con el siguiente paso.";

    Address address = null;
    AddressService addressService = null;

    int backPage = -1;
    List<Texture2D> householdBills = new List<Texture2D>();
    Texture2D currentHouseholdBill = null;

    private void Awake()
    {
        addressService = GetComponent<AddressService>();
    }

    public void Clear()
    {
        dtmAddress.ClearElements();
        lstHouseholdBill.Clear();
        householdBills.Clear();
        btnNew.gameObject.SetActive(true);
    }

    public void SetBackPage(int backPage)
    {
        this.backPage = backPage;
    }

    public void ChangeBackPage()
    {
        if (backPage == 1)
            PageManager.Instance.ChangePage(pagAddress);
        else if (backPage == 2)
            PageManager.Instance.ChangePage(pagValidateHouseholdBill);
        else
            PageManager.Instance.ChangePage(pagHouseholdBills);
    }

    public void Refresh()
    {
        lstHouseholdBill.Clear();

        for (int i = 0; i < householdBills.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(3, true);
            scrollerValue.SetText(0, $"Imagen {(i + 1)}");
            scrollerValue.SetText(1, "Recibo de servicios");
            scrollerValue.SetSprite(2, householdBills[i].CreateSprite($"{spriteName}_{i}"));
            lstHouseholdBill.ApplyAddValue(scrollerValue);
        }

        if (householdBills.Count > 0)
            txtEmpty.gameObject.SetActive(false);
        else
            txtEmpty.gameObject.SetActive(true);

        if (householdBills.Count < maxCount)
            btnNew.gameObject.SetActive(true);
        else
            btnNew.gameObject.SetActive(false);
    }

    public bool AddNewHouseholdBill()
    {
        if (householdBills.Count != 0)
            PageManager.Instance.ChangePage(pagHouseholdBills);
        else
            ChoiceDialog.Instance.Menu(title, options);
        
        return false;
    }

    public void SetCurrentHouseholdBill(Texture2D currentHouseholdBill)
    {
        this.currentHouseholdBill = currentHouseholdBill;
        imgCurrentHouseholdBill.Sprite = currentHouseholdBill.CreateSprite(false);
    }
    
    public bool AddList()
    {
        householdBills.Add(currentHouseholdBill);
        Refresh();
        PageManager.Instance.ChangePage(pagHouseholdBills);
        return false;
    }
    public void RemoveList(int idx)
    {
        householdBills.RemoveAt(idx);
        Refresh();
    }

    public bool RegisterAddress()
    {
        ScreenDialog.Instance.Display();

        address = dtmAddress.BuildClass<Address>();

        String[] images = new String[householdBills.Count];
        for (int i = 0; i < householdBills.Count; i++)
            images[i] = householdBills[i].CreateSprite($"{spriteName}_{i}").ToStrBase64(ImageType.JPG);

        addressService.RegisterAppUser(new AddressInfo(address, images));

        return false;
    }

    public void ApplyAddress(int addressId)
    {
        address.Id = addressId;
        StateManager.Instance.Address = address;
        StateManager.Instance.AppUser.AppUserStatusId = 3;
        ChoiceDialog.Instance.Info(addressAddedTitle, addressAddedMessage, () => PageManager.Instance.ChangePage(pagNext));

        Clear();
    }
}
