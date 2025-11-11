using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class HouseholdBillUpdateAction : MonoBehaviour
{
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

    [Title("Page")]
    [SerializeField]
    Page pagValidateHouseholdBill = null;
    [SerializeField]
    Page pagHouseholdBills = null;
    [SerializeField]
    Page pagNext = null;


    [Title("Gallery")]
    [SerializeField]
    Vector2Int gallerySize = new Vector2Int(794, 560);

    int backPage = -1;
    List<Texture2D> householdBills = new List<Texture2D>();
    Texture2D currentHouseholdBill = null;

    OnboardingService onboardingService = null;

    private void Awake()
    {
        onboardingService = GetComponent<OnboardingService>();
    }

    public void Clear()
    {
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
        if (backPage == 2)
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

    public bool SendForm()
    {
        ScreenDialog.Instance.Display();

        String[] images = new String[householdBills.Count];
        for (int i = 0; i < householdBills.Count; i++)
            images[i] = householdBills[i].CreateSprite($"{spriteName}_{i}").ToStrBase64(ImageType.JPG);

        onboardingService?.UpdateHouseholdBill(StateManager.Instance.AppUser.Id, images);

        return false;
    }

    // Pages
    public void ChangeNextPage()
    {
        StateManager.Instance.OnboardingStage = 0;

        PageManager.Instance.ChangePage(pagNext);
    }
}
