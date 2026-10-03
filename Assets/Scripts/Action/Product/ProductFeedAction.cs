using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Graphics.Tools;
using Leap.Data.Collections;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class ProductFeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    ToggleGroup tggLocality = null;
    [SerializeField]
    ToggleGroup tggFavorite = null;
    [SerializeField]
    int feedCount = 20;

    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;
    [SerializeField]
    GameObject txtEmpty;
    [SerializeField]
    float smoothReload = 1f;

    [Title("Plaint")]
    [SerializeField]
    ComboAdapter cmbPlaintType = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;
    //[SerializeField]
    //ValueList vllState = null;

    [Title("Errors")]
    [SerializeField]
    Page pagMenu = null;
    [SerializeField]
    [PropertySpace(4f, 0f), TextArea(2, 5)]
    String timeoutError = "La petición excedió el tiempo de espera.\nRevisa tu conexión a Internet.\n¿Deseas intentar cargar de nuevo?";

    [Title("Debug")]
    [SerializeField]
    Text txtDebug = null;
    [SerializeField]
    RectTransform trfOverlay = null;

    [Title("Event")]
    [SerializeField]
    UnityLongEvent onValueSelected = null;
    [SerializeField]
    UnityBoolEvent onLocalityRefreshed = null;

    ProductService productService;
    int selectedIdx = -1;
    readonly ProductFeed emptyProductFeed = new ProductFeed();

    public bool MustReset { get; set; } = true;
    private bool resetting = false;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
    }

    public void CreateLoopFeed()
    {
        int valueCount = feedCount * 4;

        valueDates = new String[valueCount];

        loopFeed.ClearValues();
        for (int k = 0; k < valueCount; k++)
        {
            LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
            UpdateValue(emptyProductFeed, loopValue);
            loopFeed.AddValue(loopValue);

            valueDates[k] = "--:--:--:---- : -1";
        }
        loopFeed.ApplyValues();
    }

    public void ResetPosts(bool force)
    {
        if (!MustReset && !force)
            return;

        UpdateOverlay(0);

        txtEmpty.SetActive(false);

        resetting = true;
        GetPosts(0, new ProductUserData(-1, DateTime.UtcNow), 2);

        MustReset = false;
    }

    //public void ReloadPosts(bool force)
    //{
    //    GetPosts(firstPostIdx, new NewsUserData(firstPostId, DateTime.UtcNow), 3);
    //}

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        if (userData == null)
            return;

        ProductUserData feedUserData = (ProductUserData)userData;

        if (feedUserData.PostId == -1L || direction == 3)
            ScreenDialog.Instance.Display();

        ProductFeedRequest request = new ProductFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedUserData.PublicationDateTime,
            Direction = direction,
            Count = feedUserData.PostId == -1L ? feedCount + feedCount : feedCount,

            ReactionAppUserId = StateManager.Instance.AppUser.Id,

            PostTypeId = PostType.Product,
            AppUserId = appUserId,
            CountryId = interestLocality ? StateManager.Instance.InterestLocality.CountryId : StateManager.Instance.CurrentLocality.CountryId,
            StateId = interestLocality ? StateManager.Instance.InterestLocality.StateId : StateManager.Instance.CurrentLocality.StateId,
            Status = 1,

            FavoriteAppUserId = favoriteAppUserId,
            SelectedAppUserId = -1L
        };

        //Debug.Log($"Request : {request.StartDateTime:yyyy/MM/dd HH:mm:ss.fff} [{request.Direction}:{request.Count}]");

        productService.GetFeed(request);
    }

    public void ApplyPosts(ProductFeedResponse response)
    {
        if (txtDebug != null)
            for (int i = 0; i < valueDates.Length; i++)
                valueDates[i] = valueDates[i].Replace("<color=red>", "").Replace("</color>", "");

        txtEmpty.SetActive(response.Total == 0);

        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;

        //int endLoopIdx = startLoopIdx + loopFeed.PreloadCount;
        //Debug.Log($"ApplyPosts : {startLoopIdx.ToString()} > {endLoopIdx.ToString()}, {response.PostFulls[0].PublicationDateTime.ToString("dd/MM/yyyy")} > {response.PostFulls[^1].PublicationDateTime.ToString("dd/MM/yyyy")}");

        if (response.Direction < 3)
        {
            for (int i = 0; i < response.ProductFeeds.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.ProductFeeds[i], loopFeed[k]);
                UpdateDebug(k, response.ProductFeeds[i]);
            }

            if (resetting)
            {
                for (int i = response.ProductFeeds.Count; i < loopFeed.ValuesCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyProductFeed, loopFeed[k]);
                    UpdateDebug(k, emptyProductFeed);
                }
                Invoke(nameof(ResetSelectedIndex), 0.2f);
                resetting = false;
            }
            else
            {
                for (int i = response.ProductFeeds.Count; i < feedCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyProductFeed, loopFeed[k]);
                    UpdateDebug(k, emptyProductFeed);
                }
            }
        }
        else
        {
            int n = feedCount - response.ProductFeeds.Count;
            for (int i = 0; i < n; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyProductFeed, loopFeed[k]);
                UpdateDebug(k, emptyProductFeed);
            }

            for (int i = 0; i < response.ProductFeeds.Count; i++)
            {
                int k = (startLoopIdx + n + i) % loopFeed.ValuesCount;
                UpdateValue(response.ProductFeeds[i], loopFeed[k]);
                UpdateDebug(k, response.ProductFeeds[i]);
            }
        }
        loopFeed.RefreshVisibleValues();

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);

        ScreenDialog.Instance.Hide();

        if (response.Direction == 3 && response.ProductFeeds.Count > 0)
        {
            int dataIndex = (startLoopIdx + feedCount - response.ProductFeeds.Count) % loopFeed.ValuesCount;

            if (smoothReload > 0f)
            {
                loopFeed.SelectedIndex = (startLoopIdx + feedCount) % loopFeed.ValuesCount;
                loopFeed.SelectSmooth(dataIndex);
                //Debug.Log($"{loopFeed.SelectedIndex} | {startLoopIdx} | {feedCount} | {response.PostFulls.Count} | {dataIndex}");
            }
            else
                loopFeed.SelectedIndex = dataIndex;
        }
    }

    private void ResetSelectedIndex()
    {
        loopFeed.SelectedIndex = 0;
    }

    public void UpdateValue(ProductFeed productFeed, LoopScrollerValue loopValue)
    {
        bool empty = productFeed.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : 1;
        loopValue.ItemSize = empty ? 2000 : 1058;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new ProductUserData(productFeed.PostId, productFeed.PublicationDateTime, productFeed.Link));

        if (empty)
            return;

        loopValue.GetSprite(0)?.Destroy();
        loopValue.SetSprite(0, productFeed.TitleSprite);
        loopValue.SetText(1, $"<line-height=70%>{productFeed.Title}");

        loopValue.SetText(2, $"{productFeed.Currency} {productFeed.Price:N2}");
        loopValue.SetText(3, productFeed.ProductSubtype);

        String saleLocation = productFeed.SaleCountry;
        if (!String.IsNullOrEmpty(productFeed.SaleState))
            saleLocation += ", " + productFeed.SaleState;
        loopValue.SetText(4, saleLocation);

        loopValue.SetCheck(0, productFeed.Favorite != 0);
    }

    public void SelectValue(int idx)
    {
        selectedIdx = idx % loopFeed.ValuesCount;
        onValueSelected.Invoke(((ProductUserData)loopFeed[selectedIdx].UserData).PostId);
    }

    public void ApplyDetailPost(ProductFeed productFeed)
    {
        UpdateValue(productFeed, loopFeed[selectedIdx]);

        loopFeed.RefreshVisibleValues();
    }

    // AppUser
    long appUserId = -1;

    public void ApplyAppUser(bool appUser)
    {
        appUserId = appUser ? StateManager.Instance.AppUser.Id : -1;

        ResetPosts(true);
    }


    long favoriteAppUserId = -1;

    public void ApplyAppUserFavorite()
    {
        favoriteAppUserId = tggFavorite.Value == "1" ? StateManager.Instance.AppUser.Id : -1;

        ResetPosts(true);
    }

    // Locality

    bool interestLocality = true;

    public void ApplyLocality()
    {
        interestLocality = tggLocality.Value == "1";

        ResetPosts(true);

        onLocalityRefreshed.Invoke(interestLocality);
    }

    public void ChangeLocality(bool isInterest)
    {
        if (isInterest == interestLocality)
            MustReset = true;
    }

    // Open

    public void OpenURL(int idx)
    {
        String link = ((ProductUserData)loopFeed[idx % loopFeed.ValuesCount].UserData).Link;

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

    public void ApplyFavorite(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(0, check);

        Favorite favorite = new Favorite(((ProductUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id);
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
        loopFeed[selectedIdx].SetCheck(0, check);

        loopFeed.RefreshVisibleValues();
    }


    // Reaction

    //public void ApplyReaction(int dataIndex, bool check)
    //{
    //    selectedIdx = dataIndex % loopFeed.ValuesCount;

    //    loopFeed[selectedIdx].SetCheck(3, false);
    //    loopFeed.RefreshVisibleValues();

    //    if (!check)
    //    {
    //        productService.DeleteReaction(new Reaction(-1, ((ProductUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id));
    //        return;
    //    }

    //    cmbReaction.Combo.Click();
    //}

    //public void RegisterReaction()
    //{
    //    long reactionPhraseId = cmbReaction.GetSelectedId();

    //    Reaction reaction = new Reaction(reactionPhraseId, ((ProductUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
    //    productService.RegisterReaction(reaction);

    //    loopFeed[selectedIdx].SetCheck(3, true);
    //    loopFeed.RefreshVisibleValues();
    //}

    //public void ApplyDetailReaction(bool check)
    //{
    //    loopFeed[selectedIdx].SetCheck(3, check);
    //    loopFeed.RefreshVisibleValues();
    //}

    // Plaint

    public void DisplayPlaintTypes()
    {
        cmbPlaintType.Combo.Click();
    }

    public void ApplyPlaint()
    {
        ScreenDialog.Instance.Display();

        long plaintTypeId = cmbPlaintType.GetSelectedId();

        PostPlaint postPlaint = new PostPlaint(plaintTypeId, ((ProductUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
        //productService.RegisterPostPlaint(postPlaint);
    }

    public void PlaintRegistered()
    {
        ChoiceDialog.Instance.Info("Reporte", "Reporte registrado exitosamente.");
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

    // Debug

    String[] valueDates;

    public void UpdateDebug(int k, ProductFeed productFeed)
    {
        if (productFeed.PublicationDateTime.Year == 1753)
            valueDates[k] = "<color=red>--:--:--:---- : -1</color>";
        else
            valueDates[k] = $"<color=red>{productFeed.PublicationDateTime.ToString("HH:mm:ss:ffff")} : {productFeed.Title}</color>";

    }

    public void UpdateOverlay(int idx)
    {
        if (trfOverlay == null)
            return;

        if (loopFeed.ValuesCount == 0)
            return;

        trfOverlay.anchoredPosition = new Vector2(trfOverlay.anchoredPosition.x, -.5f - 22.2f * (idx % loopFeed.ValuesCount));
    }
}