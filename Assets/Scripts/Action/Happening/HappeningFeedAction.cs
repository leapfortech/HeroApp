using System;
using System.Globalization;
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

public class HappeningFeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    int feedCount = 20;
    [SerializeField]
    Text txtLocality = null;

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
    [SerializeField]
    ValueList vllState = null;

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

    HappeningService happeningService;
    int selectedIdx = -1;
    readonly HappeningFeed emptyHappeningFeed = new HappeningFeed();

    public bool MustReset { get; set; } = true;
    private bool resetting = false;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
    }

    public void CreateLoopFeed()
    {
        int valueCount = feedCount * 4;

        valueDates = new String[valueCount];

        loopFeed.ClearValues();
        DateTime utcNow = DateTime.UtcNow;
        for (int k = 0; k < valueCount; k++)
        {
            LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
            UpdateValue(emptyHappeningFeed, loopValue, utcNow);
            loopFeed.AddValue(loopValue);

            valueDates[k] = "--:--:--:---- : -1";
        }
        loopFeed.ApplyValues();
    }

    public void ResetPosts(bool force)
    {
        String localityName = StateManager.Instance.CurrentLocality.StateId == -1 ? vllCountry.FindRecordCellString(StateManager.Instance.CurrentLocality.CountryId, 0)
                                                                                  : vllState.FindRecordCellString(StateManager.Instance.CurrentLocality.StateId, 1);
        txtLocality.TextValue = $"Eventos en: {localityName}";

        if (!MustReset && !force)
            return;

        UpdateOverlay(0);

        txtEmpty.SetActive(false);

        resetting = true;
        GetPosts(0, new FeedUserData(-1, DateTime.UtcNow), 2);

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

        FeedUserData feedUserData = (FeedUserData)userData;

        if (feedUserData.PostId == -1L || direction == 3)
            ScreenDialog.Instance.Display();

        HappeningFeedRequest request = new HappeningFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedUserData.PublicationDateTime,
            Direction = direction,
            Count = feedUserData.PostId == -1L ? feedCount + feedCount : feedCount,

            ReactionAppUserId = StateManager.Instance.AppUser.Id,

            PostTypeId = PostType.Happening,
            AppUserId = -1L, // appUserId,
            CountryId = StateManager.Instance.CurrentLocality.CountryId,
            StateId = StateManager.Instance.CurrentLocality.StateId,
            Status = 1,

            FavoriteAppUserId = appUserId,
            SelectedAppUserId = -1L
        };

        //Debug.Log($"Request : {request.StartDateTime:yyyy/MM/dd HH:mm:ss.fff} [{request.Direction}:{request.Count}]");

        happeningService.GetFeed(request);
    }

    public void ApplyPosts(HappeningFeedResponse response)
    {
        if (txtDebug != null)
            for (int i = 0; i < valueDates.Length; i++)
                valueDates[i] = valueDates[i].Replace("<color=red>", "").Replace("</color>", "");

        txtEmpty.SetActive(response.Total == 0);

        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;

        //int endLoopIdx = startLoopIdx + loopFeed.PreloadCount;
        //Debug.Log($"ApplyPosts : {startLoopIdx.ToString()} > {endLoopIdx.ToString()}, {response.PostFulls[0].PublicationDateTime.ToString("dd/MM/yyyy")} > {response.PostFulls[^1].PublicationDateTime.ToString("dd/MM/yyyy")}");

        DateTime utcNow = DateTime.UtcNow;
        if (response.Direction < 3)
        {
            for (int i = 0; i < response.HappeningFeeds.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.HappeningFeeds[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.HappeningFeeds[i]);
            }

            if (resetting)
            {
                for (int i = response.HappeningFeeds.Count; i < loopFeed.ValuesCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyHappeningFeed, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyHappeningFeed);
                }
                Invoke(nameof(ResetSelectedIndex), 0.2f);
                resetting = false;
            }
            else
            {
                for (int i = response.HappeningFeeds.Count; i < feedCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyHappeningFeed, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyHappeningFeed);
                }
            }
        }
        else
        {
            int n = feedCount - response.HappeningFeeds.Count;
            for (int i = 0; i < n; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyHappeningFeed, loopFeed[k], utcNow);
                UpdateDebug(k, emptyHappeningFeed);
            }

            for (int i = 0; i < response.HappeningFeeds.Count; i++)
            {
                int k = (startLoopIdx + n + i) % loopFeed.ValuesCount;
                UpdateValue(response.HappeningFeeds[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.HappeningFeeds[i]);
            }
        }
        loopFeed.RefreshVisibleValues();

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);

        ScreenDialog.Instance.Hide();

        if (response.Direction == 3 && response.HappeningFeeds.Count > 0)
        {
            int dataIndex = (startLoopIdx + feedCount - response.HappeningFeeds.Count) % loopFeed.ValuesCount;

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

    public void UpdateValue(HappeningFeed happeningFeed, LoopScrollerValue loopValue, DateTime utcNow)
    {
        bool empty = happeningFeed.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : 1;
        loopValue.ItemSize = empty ? 2000 : 270;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new FeedUserData(happeningFeed.PostId, happeningFeed.PublicationDateTime));

        if (empty)
            return;

        loopValue.GetSprite(0)?.Destroy();
        loopValue.SetSprite(0, happeningFeed.TitleSprite);
        loopValue.SetText(1, $"<line-height=70%>{happeningFeed.Title}");
        loopValue.SetText(2, $"<line-height=70%>{happeningFeed.StartDateTime.Value.ToString("ddd dd MMM · h:mm tt", CultureInfo.GetCultureInfo("es-GT"))}");
        loopValue.SetText(3, $"<line-height=70%>{happeningFeed.Location}");
        loopValue.SetText(4, $"<line-height=70%>{happeningFeed.HappeningType}");
        loopValue.SetText(5, $"<line-height=70%>Ya van {happeningFeed.FavoriteCount}");

        loopValue.SetCheck(0, happeningFeed.Favorite != 0);
        loopValue.SetCheck(1, happeningFeed.Selected != 0);
    }

    public void SelectValue(int idx)
    {
        selectedIdx = idx % loopFeed.ValuesCount;
        onValueSelected.Invoke(((FeedUserData)loopFeed[selectedIdx].UserData).PostId);
    }

    public void ApplyDetailPost(Post post, Sprite titleSprite)
    {
        LoopScrollerValue loopValue = loopFeed[selectedIdx];

        //Sprite thumbnailSprite = loopValue.GetSprite(0);
        //String alias = loopValue.GetText(2);
        //bool[] toggles = { loopValue.GetCheck(0), loopValue.GetCheck(0), loopValue.GetCheck(0), loopValue.GetCheck(0) };

        int itemIdx = loopValue.ItemIdx;
        loopValue.ItemIdx = post.ImageCount == 0 ? 1 : 2;
        loopValue.ItemSize = post.ImageCount == 0 ? 460 : 1058;
        if (itemIdx != loopValue.ItemIdx)
            loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, new FeedUserData(post.Id, post.PublicationDateTime));

        //loopValue.SetSprite(0, thumbnailSprite);
        //loopValue.SetText(1, $"<line-height=70%>{post.Title}");
        //loopValue.SetText(2, alias);
        //loopValue.SetText(3, $"<line-height=70%>{((post.Description != null && post.Description.Length > 180) ? post.Description[0..179] + "..." : post.Description)}");

        //if (loopValue.ItemIdx == 2)
        //{
        //    //loopValue.GetSprite(4)?.Destroy();
        //    loopValue.SetSprite(4, titleSprite.Clone("CPY_" + titleSprite.name, true));
        //    loopValue.SetText(5, post.ImageCount < 2 ? null : $"+{(post.ImageCount - 1).ToString()}");
        //}

        //loopValue.SetCheck(0, toggles[0]);
        //loopValue.SetCheck(1, toggles[1]);
        //loopValue.SetCheck(2, toggles[2]);
        //loopValue.SetCheck(3, toggles[3]);

        loopFeed.RefreshVisibleValues();
    }

    // AppUser

    long appUserId = -1;

    public void ApplyAppUser(bool appUser)
    {
        appUserId = appUser ? StateManager.Instance.AppUser.Id : -1;

        ResetPosts(true);
    }


    // Favorite

    public void ApplyFavorite(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(0, check);

        Favorite favorite = new Favorite(((FeedUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id);
        if (check)
            happeningService.RegisterFavorite(favorite);
        else
            happeningService.DeleteFavorite(favorite);
    }

    public void ApplyDetailFavorite(bool check)
    {
        loopFeed[selectedIdx].SetCheck(0, check);
        loopFeed.RefreshVisibleValues();
    }

    // Selected

    public void ApplySelected(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(1, check);

        Selected selected = new Selected(((FeedUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id);
        if (check)
            happeningService.RegisterSelected(selected);
        else
            happeningService.DeleteSelected(selected);
    }

    public void ApplyDetailSelected(bool check)
    {
        loopFeed[selectedIdx].SetCheck(1, check);
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
    //        happeningService.DeleteReaction(new Reaction(-1, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id));
    //        return;
    //    }

    //    cmbReaction.Combo.Click();
    //}

    //public void RegisterReaction()
    //{
    //    long reactionPhraseId = cmbReaction.GetSelectedId();

    //    Reaction reaction = new Reaction(reactionPhraseId, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
    //    happeningService.RegisterReaction(reaction);

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

        PostPlaint postPlaint = new PostPlaint(plaintTypeId, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
        //happeningService.RegisterPostPlaint(postPlaint);
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

    public void UpdateDebug(int k, HappeningFeed happeningFeed)
    {
        if (happeningFeed.PublicationDateTime.Year == 1753)
            valueDates[k] = "<color=red>--:--:--:---- : -1</color>";
        else
            valueDates[k] = $"<color=red>{happeningFeed.PublicationDateTime.ToString("HH:mm:ss:ffff")} : {happeningFeed.Title}</color>";

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