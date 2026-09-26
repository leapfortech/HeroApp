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

using Sirenix.OdinInspector;

public class TaleFeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    ToggleGroup tggLocality = null;

    [Space]
    [Title("Feed")]
    [SerializeField]
    int feedCount = 20;

    [Title("Loop")]
    [SerializeField]
    LoopScroller loopFeed = null;
    [SerializeField]
    GameObject txtEmpty;
    [SerializeField]
    float smoothReload = 1f;

    //[Title("Plaint")]
    //[SerializeField]
    //ComboAdapter cmbPlaintType = null;

    //[Title("Data")]
    //[SerializeField]
    //ValueList vllCountry = null;
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

    TaleService taleService;
    int selectedIdx = -1;
    readonly TaleFeed emptyTaleFeed = new TaleFeed();

    public bool MustReset { get; set; } = true;
    private bool resetting = false;

    private void Awake()
    {
        taleService = GetComponent<TaleService>();
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
            UpdateValue(emptyTaleFeed, loopValue, utcNow);
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
        GetPosts(0, new TaleUserData(-1, DateTime.UtcNow), 2);

        MustReset = false;
    }

    //public void ReloadPosts(bool force)
    //{
    //    GetPosts(firstPostIdx, new TaleUserData(firstPostId, DateTime.UtcNow), 3);
    //}

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        if (userData == null)
            return;

        FeedUserData feedUserData = (FeedUserData)userData;

        if (feedUserData.PostId == -1L || direction == 3)
            ScreenDialog.Instance.Display();

        TaleFeedRequest request = new TaleFeedRequest
        {
            Chunk = startLoopIdx,

            StartDateTime = feedUserData.PublicationDateTime,
            Direction = direction,
            Count = feedUserData.PostId == -1L ? feedCount + feedCount : feedCount,

            ReactionAppUserId = StateManager.Instance.AppUser.Id,

            PostTypeId = PostType.Tale,
            AppUserId = -1L, // appUserId,
            CountryId = interestLocality ? StateManager.Instance.InterestLocality.CountryId : StateManager.Instance.CurrentLocality.CountryId,
            StateId = interestLocality ? StateManager.Instance.InterestLocality.StateId : StateManager.Instance.CurrentLocality.StateId,
            Status = 1,

            FavoriteAppUserId = appUserId,
            SelectedAppUserId = -1L
        };

        taleService.GetFeed(request);
    }

    public void ApplyPosts(TaleFeedResponse response)
    {
        if (txtDebug != null)
            for (int i = 0; i < valueDates.Length; i++)
                valueDates[i] = valueDates[i].Replace("<color=red>", "").Replace("</color>", "");

        txtEmpty.SetActive(response.Total == 0);

        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;

        DateTime utcNow = DateTime.UtcNow;
        if (response.Direction < 3)
        {
            for (int i = 0; i < response.TaleFeeds.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.TaleFeeds[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.TaleFeeds[i]);
            }

            if (resetting)
            {
                for (int i = response.TaleFeeds.Count; i < loopFeed.ValuesCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyTaleFeed, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyTaleFeed);
                }
                Invoke(nameof(ResetSelectedIndex), 0.2f);
                resetting = false;
            }
            else
            {
                for (int i = response.TaleFeeds.Count; i < feedCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyTaleFeed, loopFeed[k], utcNow);
                    UpdateDebug(k, emptyTaleFeed);
                }
            }
        }
        else
        {
            int n = feedCount - response.TaleFeeds.Count;
            for (int i = 0; i < n; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyTaleFeed, loopFeed[k], utcNow);
                UpdateDebug(k, emptyTaleFeed);
            }

            for (int i = 0; i < response.TaleFeeds.Count; i++)
            {
                int k = (startLoopIdx + n + i) % loopFeed.ValuesCount;
                UpdateValue(response.TaleFeeds[i], loopFeed[k], utcNow);
                UpdateDebug(k, response.TaleFeeds[i]);
            }
        }
        loopFeed.RefreshVisibleValues();

        if (txtDebug != null)
            txtDebug.TextValue = String.Join('\n', valueDates);

        ScreenDialog.Instance.Hide();

        if (response.Direction == 3 && response.TaleFeeds.Count > 0)
        {
            int dataIndex = (startLoopIdx + feedCount - response.TaleFeeds.Count) % loopFeed.ValuesCount;

            if (smoothReload > 0f)
            {
                loopFeed.SelectedIndex = (startLoopIdx + feedCount) % loopFeed.ValuesCount;
                loopFeed.SelectSmooth(dataIndex);
            }
            else
                loopFeed.SelectedIndex = dataIndex;
        }
    }

    private void ResetSelectedIndex()
    {
        loopFeed.SelectedIndex = 0;
    }

    readonly String[] icnReactions = { "👍", "🙏", "💪", "😄" };

    public void UpdateValue(TaleFeed taleFeed, LoopScrollerValue loopValue, DateTime utcNow)
    {
        bool empty = taleFeed.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : 1;
        loopValue.ItemSize = empty ? 2000 : 1200;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new TaleUserData(taleFeed.PostId, taleFeed.PublicationDateTime, taleFeed.ReactionCounts));

        if (empty)
            return;


        loopValue.GetSprite(0)?.Destroy();
        loopValue.SetSprite(0, taleFeed.TitleSprite);
        loopValue.SetText(1, $"<line-height=70%>{taleFeed.Title}");
        loopValue.SetText(2, $"<line-height=80%>{(taleFeed.Description.Length > 120 ? taleFeed.Description[0..119] + "..." : taleFeed.Description).Replace("\r", "").Replace("\n\n", " ").Replace('\n', ' ')}");
        loopValue.SetText(3, $"<Size=80%><Color=#025581>{taleFeed.Alias} originari@ de: {taleFeed.InterestLocality}</Color>");
        loopValue.SetText(4, $"<Size=80%><Color=#025581>Veve en: {taleFeed.CurrentLocality}</Color>");
        loopValue.SetText(5, $"<Size=80%>{taleFeed.ReactionCounts[0] + taleFeed.ReactionCounts[1] + taleFeed.ReactionCounts[2] + taleFeed.ReactionCounts[3]} reacciones");
        loopValue.SetText(6, $"<Size=80%>💬  {taleFeed.CommentCount} comentarios");

        for (int i = 0; i < 4; i++)
        {
            loopValue.SetText(7 + i, $"{icnReactions[i]}  {taleFeed.ReactionCounts[i]}");
            loopValue.SetCheck(i, taleFeed.ReactionPhraseId == i + 1);
        }
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
        bool[] toggles = { loopValue.GetCheck(0), loopValue.GetCheck(1), loopValue.GetCheck(2), loopValue.GetCheck(3) };

        int itemIdx = loopValue.ItemIdx;
        loopValue.ItemIdx = post.ImageCount == 0 ? 1 : 2;
        loopValue.ItemSize = post.ImageCount == 0 ? 460 : 1058;
        if (itemIdx != loopValue.ItemIdx)
            loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, new FeedUserData(post.Id, post.PublicationDateTime));

        //loopValue.SetSprite(0, thumbnailSprite);
        //loopValue.SetText(1, $"<line-height=70%>{post.Title}");
        //loopValue.SetText(2, alias);
        //loopValue.SetText(3, $"<line-height=70%>{((post.Description != null && post.Description.Length > 180) ? post.Description[0..179] + "..." : post.Description)}");

        if (loopValue.ItemIdx == 2)
        {
            //loopValue.GetSprite(4)?.Destroy();
            //loopValue.SetSprite(4, titleSprite.Clone("CPY_" + titleSprite.name, true));
            //loopValue.SetText(5, post.ImageCount < 2 ? null : $"+{(post.ImageCount - 1).ToString()}");
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

    // Reaction

    public void ApplyReaction1(int dataIndex, bool check)
    {
        ApplyReaction(1, dataIndex, check);
    }

    public void ApplyReaction2(int dataIndex, bool check)
    {
        ApplyReaction(2, dataIndex, check);
    }

    public void ApplyReaction3(int dataIndex, bool check)
    {
        ApplyReaction(3, dataIndex, check);
    }

    public void ApplyReaction4(int dataIndex, bool check)
    {
        ApplyReaction(4, dataIndex, check);
    }

    private void ApplyReaction(int reactionPhraseId, int dataIndex, bool check)
    {
        selectedIdx = dataIndex % loopFeed.ValuesCount;
        LoopScrollerValue loopValue = loopFeed[selectedIdx];
        TaleUserData userData;

        for (int i = 0; i < 4; i++)
            if (loopValue.GetCheck(i))
            {
                userData = (TaleUserData)loopValue.UserData;
                taleService.DeleteReaction(new Reaction(-1, userData.PostId, StateManager.Instance.AppUser.Id));

                userData.ReactionCounts[i]--;
                loopValue.SetText(7 + i, $"{icnReactions[i]}  {userData.ReactionCounts[i]}");
                loopValue.SetCheck(i, false);

                loopValue.SetText(5, $"<Size=80%>{userData.ReactionCounts[0] + userData.ReactionCounts[1] + userData.ReactionCounts[2] + userData.ReactionCounts[3]} reacciones");
                loopFeed.RefreshVisibleValues();
                //break;
            }

        if (!check)
            return;

        userData = (TaleUserData)loopValue.UserData;
        taleService.RegisterReaction(new Reaction(reactionPhraseId, userData.PostId, StateManager.Instance.AppUser.Id));

        userData.ReactionCounts[reactionPhraseId - 1]++;
        loopValue.SetText(7 + reactionPhraseId - 1, $"{icnReactions[reactionPhraseId - 1]}  {userData.ReactionCounts[reactionPhraseId - 1]}");
        loopValue.SetCheck(reactionPhraseId - 1, true);

        loopValue.SetText(5, $"<Size=80%>{userData.ReactionCounts[0] + userData.ReactionCounts[1] + userData.ReactionCounts[2] + userData.ReactionCounts[3]} reacciones");
        loopFeed.RefreshVisibleValues();
    }

    public void ApplyDetailReaction(int reactionPhraseId, bool check)
    {
        loopFeed[selectedIdx].SetCheck(reactionPhraseId - 1, check);
        loopFeed.RefreshVisibleValues();
    }

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
    //    //radioService.RegisterPostPlaint(postPlaint);
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

    public void UpdateDebug(int k, TaleFeed taleFeed)
    {
        if (taleFeed.PublicationDateTime.Year == 1753)
            valueDates[k] = "<color=red>--:--:--:---- : -1</color>";
        else
            valueDates[k] = $"<color=red>{taleFeed.PublicationDateTime.ToString("HH:mm:ss:ffff")} : {taleFeed.Title}</color>";

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