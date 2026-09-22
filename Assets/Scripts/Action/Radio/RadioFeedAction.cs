using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.Graphics.Tools;
using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.UI.Extensions;

using Sirenix.OdinInspector;

public class RadioFeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    FeedState feedConfig = null;

    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;
    [SerializeField]
    GameObject txtEmpty;
    [SerializeField]
    float smoothReload = 1f;

    [Title("Reaction")]
    [SerializeField]
    ComboAdapter cmbReaction = null;

    [Title("Plaint")]
    [SerializeField]
    ComboAdapter cmbPlaintType = null;

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

    RadioService radioService;
    FeedState feedState;
    int selectedIdx = -1;
    readonly RadioFeed emptyRadioFeed = new RadioFeed();

    public bool MustReset { get; set; } = true;
    private bool resetting = false;

    private void Awake()
    {
        radioService = GetComponent<RadioService>();
    }

    public void CreateLoopFeed()
    {
        feedState = StateManager.Instance.GetFeedState(feedConfig.FeedKey);
        int feedCount = feedState.Count * 4;

        valueDates = new String[feedCount];

        loopFeed.ClearValues();
        DateTime utcNow = DateTime.UtcNow;
        for (int k = 0; k < feedCount; k++)
        {
            LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
            UpdateValue(emptyRadioFeed, loopValue, utcNow);
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
        GetPosts(0, new FeedUserData(-1, DateTime.UtcNow), 2);

        MustReset = false;
    }

    //public void ReloadPosts(bool force)
    //{
    //    GetPosts(firstPostIdx, new FeedUserData(firstPostId, DateTime.UtcNow), 3);
    //}

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        if (userData == null)
            return;

        FeedUserData feedUserData = (FeedUserData)userData;

        if (feedUserData.PostId == -1L || direction == 3)
            ScreenDialog.Instance.Display();

        RadioFeedRequest request = new RadioFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedUserData.PublicationDateTime,
            Direction = direction,
            Count = feedUserData.PostId == -1L ? feedState.Count + feedState.Count : feedState.Count,

            ReactionAppUserId = StateManager.Instance.AppUser.Id,

            PostTypeId = feedState.PostTypeId,
            AppUserId = -1L, // appUserId,
            CountryId = -1L, // interestLocality ? StateManager.Instance.InterestLocality.CountryId : StateManager.Instance.CurrentLocality.CountryId,
            StateId = -1L, // interestLocality ? StateManager.Instance.InterestLocality.StateId : StateManager.Instance.CurrentLocality.StateId,
            Status = feedState.Status,

            FavoriteAppUserId = appUserId,
            SelectedAppUserId = -1L
        };

        //Debug.Log($"Request : {request.StartDateTime:yyyy/MM/dd HH:mm:ss.fff} [{request.Direction}:{request.Count}]");

        radioService.GetFeed(request);
    }

    public void ApplyPosts(RadioFeedResponse response)
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
            for (int i = 0; i < response.RadioFeeds.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.RadioFeeds[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.RadioFeeds[i]);
            }

            if (resetting)
            {
                for (int i = response.RadioFeeds.Count; i < loopFeed.ValuesCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyRadioFeed, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyRadioFeed);
                }
                Invoke(nameof(ResetSelectedIndex), 0.2f);
                resetting = false;
            }
            else
            {
                for (int i = response.RadioFeeds.Count; i < feedState.Count; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyRadioFeed, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyRadioFeed);
                }
            }
        }
        else
        {
            int n = feedState.Count - response.RadioFeeds.Count;
            for (int i = 0; i < n; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyRadioFeed, loopFeed[k], utcNow);
                UpdateDebug(k, emptyRadioFeed);
            }

            for (int i = 0; i < response.RadioFeeds.Count; i++)
            {
                int k = (startLoopIdx + n + i) % loopFeed.ValuesCount;
                UpdateValue(response.RadioFeeds[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.RadioFeeds[i]);
            }
        }
        loopFeed.RefreshVisibleValues();

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);

        ScreenDialog.Instance.Hide();

        if (response.Direction == 3 && response.RadioFeeds.Count > 0)
        {
            int dataIndex = (startLoopIdx + feedState.Count - response.RadioFeeds.Count) % loopFeed.ValuesCount;

            if (smoothReload > 0f)
            {
                loopFeed.SelectedIndex = (startLoopIdx + feedState.Count) % loopFeed.ValuesCount;
                loopFeed.SelectSmooth(dataIndex);
                //Debug.Log($"{loopFeed.SelectedIndex} | {startLoopIdx} | {feedState.Count} | {response.PostFulls.Count} | {dataIndex}");
            }
            else
                loopFeed.SelectedIndex = dataIndex;
        }
    }

    private void ResetSelectedIndex()
    {
        loopFeed.SelectedIndex = 0;
    }

    public void UpdateValue(RadioFeed radioFeed, LoopScrollerValue loopValue, DateTime utcNow)
    {
        bool empty = radioFeed.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : 1;
        loopValue.ItemSize = empty ? 2000 : 270;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new FeedUserData(radioFeed.PostId, radioFeed.PublicationDateTime));

        if (empty)
            return;

        loopValue.GetSprite(0)?.Destroy();
        loopValue.SetSprite(0, radioFeed.TitleSprite);
        loopValue.SetText(1, $"<line-height=70%>{radioFeed.Title}");

        loopValue.SetText(2, radioFeed.RadioType);
        //loopValue.SetText(3, radioFeed.Url);

        loopValue.SetCheck(0, radioFeed.Favorite != 0);
    }

    public void SelectValue(int idx)
    {
        selectedIdx = idx % loopFeed.ValuesCount;
        onValueSelected.Invoke(((FeedUserData)loopFeed[selectedIdx].UserData).PostId);
    }

    public void ApplyDetailPost(Post post, Sprite titleSprite)
    {
        LoopScrollerValue loopValue = loopFeed[selectedIdx];

        Sprite thumbnailSprite = loopValue.GetSprite(0);
        String alias = loopValue.GetText(2);
        bool[] toggles = { loopValue.GetCheck(0), loopValue.GetCheck(0), loopValue.GetCheck(0), loopValue.GetCheck(0) };

        int itemIdx = loopValue.ItemIdx;
        loopValue.ItemIdx = post.ImageCount == 0 ? 1 : 2;
        loopValue.ItemSize = post.ImageCount == 0 ? 460 : 1058;
        if (itemIdx != loopValue.ItemIdx)
            loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, new FeedUserData(post.Id, post.PublicationDateTime));

        loopValue.SetSprite(0, thumbnailSprite);
        loopValue.SetText(1, $"<line-height=70%>{post.Title}");
        loopValue.SetText(2, alias);
        loopValue.SetText(3, $"<line-height=70%>{((post.Description != null && post.Description.Length > 180) ? post.Description[0..179] + "..." : post.Description)}");

        if (loopValue.ItemIdx == 2)
        {
            //loopValue.GetSprite(4)?.Destroy();
            loopValue.SetSprite(4, titleSprite.Clone("CPY_" + titleSprite.name, true));
            loopValue.SetText(5, post.ImageCount < 2 ? null : $"+{(post.ImageCount - 1).ToString()}");
        }

        loopValue.SetCheck(0, toggles[0]);
        loopValue.SetCheck(1, toggles[1]);
        loopValue.SetCheck(2, toggles[2]);
        loopValue.SetCheck(3, toggles[3]);

        loopFeed.RefreshVisibleValues();
    }

    // AppUser

    long appUserId = -1;

    public void ApplyAppUser(bool appUser)
    {
        appUserId = appUser ? StateManager.Instance.AppUser.Id : -1;

        ResetPosts(true);
    }

    // Locality

    public void ApplyLocality()
    {
        //String locality = StateManager.Instance.CurrentLocality.StateId;
    }

    // Favorite

    public void ApplyFavorite(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(0, check);

        Favorite favorite = new Favorite(((FeedUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id);
        if (check)
            radioService.RegisterFavorite(favorite);
        else
            radioService.DeleteFavorite(favorite);
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
    //        radioService.DeleteReaction(new Reaction(-1, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id));
    //        return;
    //    }

    //    cmbReaction.Combo.Click();
    //}

    //public void RegisterReaction()
    //{
    //    long reactionPhraseId = cmbReaction.GetSelectedId();

    //    Reaction reaction = new Reaction(reactionPhraseId, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
    //    radioService.RegisterReaction(reaction);

    //    loopFeed[selectedIdx].SetCheck(3, true);
    //    loopFeed.RefreshVisibleValues();
    //}

    //public void ApplyDetailReaction(bool check)
    //{
    //    loopFeed[selectedIdx].SetCheck(3, check);
    //    loopFeed.RefreshVisibleValues();
    //}

    // Plaint

    //public void DisplayPlaintTypes()
    //{
    //    cmbPlaintType.Combo.Click();
    //}

    //public void ApplyPlaint()
    //{
    //    ScreenDialog.Instance.Display();

    //    long plaintTypeId = cmbPlaintType.GetSelectedId();

    //    PostPlaint postPlaint = new PostPlaint(plaintTypeId, ((FeedUserData)loopFeed[selectedIdx].UserData).PostId, StateManager.Instance.AppUser.Id);
    //    radioService.RegisterPostPlaint(postPlaint);
    //}

    //public void PlaintRegistered()
    //{
    //    ChoiceDialog.Instance.Info("Reporte", "Reporte registrado exitosamente.");
    //}

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

    public void UpdateDebug(int k, RadioFeed radioFeed)
    {
        if (radioFeed.PublicationDateTime.Year == 1753)
            valueDates[k] = "<color=red>--:--:--:---- : -1</color>";
        else
            valueDates[k] = $"<color=red>{radioFeed.PublicationDateTime.ToString("HH:mm:ss:ffff")} : {radioFeed.Title}</color>";

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