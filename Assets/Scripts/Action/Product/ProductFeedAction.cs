using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Graphics.Tools;
using Leap.Data.Collections;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class ProductFeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    ToggleGroup tggLocalityFavorite = null;
    [SerializeField]
    Text lblLocality = null;
    [SerializeField]
    Text txtLocality = null;
    [SerializeField]
    int feedCount = 20;

    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;
    [SerializeField]
    GameObject gobEmptyFeed;
    [SerializeField]
    GameObject gobEmptyFavorite;
    //[SerializeField]
    //bool smoothReload = true;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;
    [SerializeField]
    ValueList vllState = null;

    [Title("Errors")]
    [SerializeField]
    Page pagMenu = null;
    [SerializeField]
    [PropertySpace(4f, 0f), TextArea(2, 5)]
    String timeoutError = "La petición excedió el tiempo de espera.\nRevisa tu conexión a Internet.\n¿Deseas intentar cargar de nuevo?";

    [Title("Event")]
    [SerializeField]
    UnityLongEvent onValueSelected = null;

    ProductService productService;
    int selectedIdx = -1;
    readonly ProductFeed emptyProductFeed = new ProductFeed();

    public bool MustReset { get; set; } = true;
    private bool resetting = false;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
    }

    public bool SetMustReset()
    {
        MustReset = true;
        return true;
    }

    public void CreateLoopFeed()
    {
        int valueCount = feedCount * 4;

        loopFeed.ClearValues();
        for (int k = 0; k < valueCount; k += 2)
        {
            LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
            UpdateValue(emptyProductFeed, emptyProductFeed, loopValue);
            loopFeed.AddValue(loopValue);
        }
        loopFeed.ApplyValues();
    }

    public void ResetPosts(bool force)
    {
        if (tggLocalityFavorite.Value == null)
            SetLocalityFavorite();

        if (txtLocality != null)
        {
            if (tggLocalityFavorite.Value == "0")
            {
                lblLocality.TextValue = "Productos cerca de ti en";
                txtLocality.TextValue = StateManager.Instance.CurrentLocality.StateId == -1 ? vllCountry.FindRecordCellString(StateManager.Instance.CurrentLocality.CountryId, "Name")
                                                                                            : vllState.FindRecordCellString(StateManager.Instance.CurrentLocality.StateId, "Name");
            }
            else if (tggLocalityFavorite.Value == "1")
            {
                lblLocality.TextValue = "Productos de tu tierra en";
                txtLocality.TextValue = StateManager.Instance.InterestLocality.StateId == -1 ? vllCountry.FindRecordCellString(StateManager.Instance.InterestLocality.CountryId, "Name")
                                                                                             : vllState.FindRecordCellString(StateManager.Instance.InterestLocality.StateId, "Name");
            }
            else
            {
                lblLocality.TextValue = "Productos favoritos";
                txtLocality.TextValue = "De todas las localidades";
            }
        }

        if (!MustReset && !force)
            return;

        gobEmptyFeed.SetActive(false);
        gobEmptyFavorite.SetActive(false);

        resetting = true;
        GetPosts(0, new ProductUserData(-1L, DateTime.UtcNow), 2);

        MustReset = false;
    }

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        if (userData == null)
            return;

        ProductUserData productUserData = (ProductUserData)userData;

        if (productUserData.PostId == -1L || direction == 3)
            ScreenDialog.Instance.Display();

        ProductFeedRequest request = new ProductFeedRequest
        {
            Chunk = startLoopIdx * 2,

            StartDateTime = direction == 2 ? productUserData.PublicationDateTime1 : productUserData.PublicationDateTime0,
            Direction = direction,
            Count = productUserData.PostId == -1L ? feedCount + feedCount : feedCount,

            ReactionAppUserId = StateManager.Instance.AppUser.Id,

            PostTypeId = PostType.Product,
            AppUserId = appUserId,
            CountryId = interestLocality == 1 ? StateManager.Instance.InterestLocality.CountryId : interestLocality == 0 ? StateManager.Instance.CurrentLocality.CountryId : -1,
            StateId = interestLocality == 1 ? StateManager.Instance.InterestLocality.StateId : interestLocality == 0 ? StateManager.Instance.CurrentLocality.StateId: -1,
            Status = 1,

            ProductTypeIds = typeIds,

            FavoriteAppUserId = favoriteAppUserId,
            SelectedAppUserId = -1L
        };

        productService.GetFeed(request);
    }

    public void ApplyPosts(ProductFeedResponse response)
    {
        gobEmptyFeed.SetActive(response.Total == 0 && favoriteAppUserId == -1L);
        gobEmptyFavorite.SetActive(response.Total == 0 && favoriteAppUserId != -1L);

        int startLoopIdx = (response.Chunk / 2) % loopFeed.ValuesCount;

        if (response.Direction < 3)
        {
            for (int i = 0; i < response.ProductFeeds.Count; i += 2)
            {
                int k = (startLoopIdx + i / 2) % loopFeed.ValuesCount;
                UpdateValue(response.ProductFeeds[i], i + 1 < response.ProductFeeds.Count ? response.ProductFeeds[i + 1] : emptyProductFeed, loopFeed[k]);
            }

            if (resetting)
            {
                for (int i = (response.ProductFeeds.Count + 1) / 2; i < loopFeed.ValuesCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyProductFeed, emptyProductFeed, loopFeed[k]);
                }
                Invoke(nameof(ResetSelectedIndex), 0.2f);
                resetting = false;
            }
            else
            {
                for (int i = response.ProductFeeds.Count; i < feedCount; i += 2)
                {
                    int k = (startLoopIdx + i / 2) % loopFeed.ValuesCount;
                    UpdateValue(emptyProductFeed, emptyProductFeed, loopFeed[k]);
                }
            }
        }
        else
        {
            //int n = feedCount - response.ProductFeeds.Count;
            //for (int i = 0; i < n; i += 2)
            //{
            //    int k = (startLoopIdx + i / 2) % loopFeed.ValuesCount;
            //    UpdateValue(emptyProductFeed, emptyProductFeed, loopFeed[k]);
            //}

            //if (response.ProductFeeds.Count % 2 == 0)
            //{
            //    for (int i = 0; i < response.ProductFeeds.Count; i += 2)
            //    {
            //        int k = (startLoopIdx + (n + i) / 2) % loopFeed.ValuesCount;
            //        UpdateValue(response.ProductFeeds[i], response.ProductFeeds[i + 1], loopFeed[k]);
            //    }
            //}
            //else
            //{
            //    int k = (startLoopIdx + (n + 0) / 2) % loopFeed.ValuesCount;
            //    UpdateValue(emptyProductFeed, response.ProductFeeds[0], loopFeed[k]);

            //    for (int i = 1; i < response.ProductFeeds.Count; i += 2)
            //    {
            //        k = (startLoopIdx + (n + i + 1) / 2) % loopFeed.ValuesCount;
            //        UpdateValue(response.ProductFeeds[i], response.ProductFeeds[i + 1], loopFeed[k]);
            //    }
            //}

            if (response.ProductFeeds.Count > 0)
            {
                resetting = true;
                GetPosts(0, new ProductUserData(-1L, DateTime.UtcNow), 2);
                return;
            }
        }

        loopFeed.RefreshVisibleValues();

        ScreenDialog.Instance.Hide();

        //if (response.Direction == 3 && response.ProductFeeds.Count > 0)
        //{
        //    int dataIndex = (startLoopIdx + (feedCount - response.ProductFeeds.Count) / 2) % loopFeed.ValuesCount;

        //    if (smoothReload)
        //    {
        //        loopFeed.SelectedIndex = (startLoopIdx + feedCount / 2) % loopFeed.ValuesCount;
        //        loopFeed.SelectSmooth(dataIndex);
        //    }
        //    else
        //        loopFeed.SelectedIndex = dataIndex;
        //}
    }

    private void ResetSelectedIndex()
    {
        loopFeed.SelectedIndex = 0;
    }

    private void UpdateValue(ProductFeed productFeed0, ProductFeed productFeed1, LoopScrollerValue loopValue)
    {
        bool empty = productFeed0.PublicationDateTime.Year == 1753 && productFeed1.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : 1;
        loopValue.ItemSize = empty ? 2000 : 976;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new ProductUserData(productFeed0.PostId, productFeed0.PublicationDateTime, productFeed0.Link, productFeed1.PostId, productFeed1.PublicationDateTime, productFeed1.Link));

        if (empty)
            return;

        UpdateValue(0, productFeed0, loopValue);
        UpdateValue(1, productFeed1, loopValue);
    }

    private void UpdateValue(int pos, ProductFeed productFeed, LoopScrollerValue loopValue)
    {
        if (productFeed.PublicationDateTime.Year != 1753)
        {
            loopValue.GetSprite(pos + 0)?.Destroy();
            loopValue.SetSprite(pos + 0, productFeed.TitleSprite);
            String saleLocation = !String.IsNullOrEmpty(productFeed.State) ? productFeed.State : productFeed.Country;
            loopValue.SetText(pos + 2, saleLocation);
            loopValue.SetText(pos + 4, $"<line-height=70%>{productFeed.ProductSubtype}");
            loopValue.SetText(pos + 6, $"<line-height=70%>{productFeed.Title}");
            loopValue.SetText(pos + 8, $"{productFeed.Currency} {productFeed.Price.ToString("N2", CultureInfo.InvariantCulture)}");
            loopValue.SetActive(pos + 10, false);

            loopValue.SetCheck(pos + 0, productFeed.Favorite != 0);
        }
        else
        {
            loopValue.GetSprite(pos + 0)?.Destroy();
            loopValue.SetSprite(pos + 0, null);
            loopValue.SetText(pos + 2, null);
            loopValue.SetText(pos + 4, null);
            loopValue.SetText(pos + 6, null);
            loopValue.SetText(pos + 8, null);
            loopValue.SetActive(pos + 10, true);

            loopValue.SetCheck(pos + 0, false);
        }
    }

    public void SelectValue0(int idx)
    {
        SelectValue(idx, 0);
    }

    public void SelectValue1(int idx)
    {
        SelectValue(idx, 1);
    }

    private void SelectValue(int idx, int side)
    {
        selectedIdx = (2 * idx + side) % loopFeed.ValuesCount;
        if (side == 0)
            onValueSelected.Invoke(((ProductUserData)loopFeed[idx].UserData).PostId0);
        else
            onValueSelected.Invoke(((ProductUserData)loopFeed[idx].UserData).PostId1);
    }

    public void ApplyDetailPost(ProductFeed productFeed)
    {
        UpdateValue(selectedIdx % 2, productFeed, loopFeed[selectedIdx / 2]);

        loopFeed.RefreshVisibleValues();
    }

    // Type
    List<long> typeIds = new List<long> { -1L };

    public void ApplyTypes(List<long> typeIds)
    {
        this.typeIds = typeIds;

        ResetPosts(true);
    }

    // AppUser
    long appUserId = -1;

    public void ApplyAppUser(bool appUser)
    {
        appUserId = appUser ? StateManager.Instance.AppUser.Id : -1;

        ResetPosts(true);
    }

    // Filters

    int interestLocality = -1;
    long favoriteAppUserId = -1;

    private void SetLocalityFavorite()
    {
        if (tggLocalityFavorite.Value == null)
            tggLocalityFavorite.Value = "0";

        favoriteAppUserId = tggLocalityFavorite.Value == "2" ? StateManager.Instance.AppUser.Id : -1;

        if (tggLocalityFavorite.Value == "2")
            interestLocality = -1;
        else if (tggLocalityFavorite.Value == "1")
            interestLocality = 1;
        else
            interestLocality = 0;
    }

    public void ApplyLocalityFavorite()
    {
        SetLocalityFavorite();

        ResetPosts(true);
    }

    // Open

    public void OpenURL0(int idx)
    {
        OpenURL(idx, 0);
    }

    public void OpenURL1(int idx)
    {
        OpenURL(idx, 1);
    }

    private void OpenURL(int idx, int side)
    {
        String link = side == 0 ? ((ProductUserData)loopFeed[idx % loopFeed.ValuesCount].UserData).Link0
                                : ((ProductUserData)loopFeed[idx % loopFeed.ValuesCount].UserData).Link1;

        String[] linkParts = link.Split('|');

        if (linkParts.Length == 0)
            return;

        LinkType linkType = (LinkType)Convert.ToInt32(linkParts[0]);

        switch (linkType)
        {
            case LinkType.Url:
                {
                    Application.OpenURL(linkParts[1]);
                    break;
                }

            case LinkType.Phone:
                {
                    long countryId = Convert.ToInt64(linkParts[1]);
                    String phonePrefix = vllCountry.FindRecordCellString(countryId, "PhonePrefix");
                    String phone = phonePrefix + linkParts[2];

                    Application.OpenURL("tel://" + phone);
                    break;
                }

            case LinkType.WhatsApp:
                {
                    long countryId = Convert.ToInt64(linkParts[1]);
                    String phonePrefix = vllCountry.FindRecordCellString(countryId, "PhonePrefix");
                    String phone = phonePrefix + linkParts[2];
                    phone = phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

                    Application.OpenURL("https://wa.me/" + phone.Replace("+", ""));
                    break;
                }

            case LinkType.Email:
                {
                    if (linkParts.Length > 1)
                        Application.OpenURL("mailto:" + linkParts[1]);

                    break;
                }
        }
    }

    // Favorite
    public void ApplyFavorite0(int idx, bool check)
    {
        ApplyFavorite(idx, 0, check);
    }

    public void ApplyFavorite1(int idx, bool check)
    {
        ApplyFavorite(idx, 1, check);
    }

    private void ApplyFavorite(int idx, int side, bool check)
    {
        loopFeed[idx].SetCheck(side, check);

        Favorite favorite = new Favorite(side == 0 ? ((ProductUserData)loopFeed[idx].UserData).PostId0 : ((ProductUserData)loopFeed[idx].UserData).PostId1, StateManager.Instance.AppUser.Id);
        if (check)
            productService.RegisterFavorite(favorite);
        else
        {
            productService.DeleteFavorite(favorite);

            if (favoriteAppUserId != -1)
                ResetPosts(true);
        }
    }

    public void ApplyDetailFavorite(bool check)
    {
        loopFeed[selectedIdx / 2].SetCheck(selectedIdx % 2, check);

        loopFeed.RefreshVisibleValues();

        MustReset = true;
    }

    // Errors

    public void OnSendError(String error)
    {
        ChoiceDialog.Instance.Message(1, new String[] { $"{error}\n¿Deseas intentar de nuevo ahora?" }, new UnityAction[] { () => ResetPosts(true), ChangePageOnError }, new String[] { "Sí", "No" });
    }

    public void OnResponseError(String error)
    {
        ChoiceDialog.Instance.Message(2, new String[] { $"{error}\n¿Deseas cargar de nuevo ahora?" }, new UnityAction[] { () => ResetPosts(true), ChangePageOnError }, new String[] { "Sí", "No" });
    }

    public void OnTimeoutError(String error)
    {
        ChoiceDialog.Instance.Message(0, new String[] { timeoutError }, new UnityAction[] { () => ResetPosts(true) , ChangePageOnError }, new String[] { "Sí", "No" });
    }

    private void ChangePageOnError()
    {
        MustReset = true;
        PageManager.Instance.ChangePage(pagMenu);
    }
}