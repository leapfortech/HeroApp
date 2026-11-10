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

public class HappeningFeedAction : MonoBehaviour
{
    [Space]
    [Title("Feed")]
    [SerializeField]
    ToggleGroup tggFavoriteSelected = null;
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
    GameObject gobEmptySelected;
    [SerializeField]
    GameObject gobEmptyFavorite;
    [SerializeField]
    float smoothReload = 1f;

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

    HappeningService happeningService;
    int selectedIdx = -1;
    readonly HappeningFeed emptyHappeningFeed = new HappeningFeed();

    public bool MustReset { get; set; } = true;
    private bool resetting = false;

    private void Awake()
    {
        happeningService = GetComponent<HappeningService>();
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
        for (int k = 0; k < valueCount; k++)
        {
            LoopScrollerValue loopValue = new LoopScrollerValue(loopFeed.LoopItems[0].LoopItem, null);
            UpdateValue(emptyHappeningFeed, loopValue);
            loopFeed.AddValue(loopValue);
        }
        loopFeed.ApplyValues();
    }

    public void ResetPosts(bool force)
    {
        if (txtLocality != null)
        {
            String localityName = StateManager.Instance.CurrentLocality.StateId == -1 ? vllCountry.FindRecordCellString(StateManager.Instance.CurrentLocality.CountryId, 0)
                                                                                      : vllState.FindRecordCellString(StateManager.Instance.CurrentLocality.StateId, 1);
            txtLocality.TextValue = $"Eventos en {localityName}";
        }

        if (!MustReset && !force)
            return;

        gobEmptyFeed.SetActive(false);
        gobEmptySelected.SetActive(false);
        gobEmptyFavorite.SetActive(false);

        resetting = true;
        GetPosts(0, new HappeningUserData(-1, DateTime.UtcNow), 2);

        MustReset = false;
    }

    public void GetPosts(int startLoopIdx, object userData, int direction)
    {
        if (userData == null)
            return;

        HappeningUserData feedUserData = (HappeningUserData)userData;

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
            AppUserId = appUserId,
            CountryId = appUserId != -1 ? -1 : StateManager.Instance.CurrentLocality.CountryId,
            StateId = appUserId != -1 ? -1 : StateManager.Instance.CurrentLocality.StateId,
            Status = 1,

            HappeningTypeIds = typeIds,

            FavoriteAppUserId = appUserId != -1 ? -1 : favoriteAppUserId,
            SelectedAppUserId = appUserId != -1 ? -1 : selectedAppUserId
        };

        happeningService.GetFeed(request);
    }

    public void ApplyPosts(HappeningFeedResponse response)
    {
        gobEmptyFeed.SetActive(response.Total == 0 && tggFavoriteSelected.Value == "0");
        gobEmptySelected.SetActive(response.Total == 0 && tggFavoriteSelected.Value == "1");
        gobEmptyFavorite.SetActive(response.Total == 0 && tggFavoriteSelected.Value == "2");

        int startLoopIdx = response.Chunk % loopFeed.ValuesCount;

        if (response.Direction < 3)
        {
            for (int i = 0; i < response.HappeningFeeds.Count; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(response.HappeningFeeds[i], loopFeed[k]);
            }

            if (resetting)
            {
                for (int i = response.HappeningFeeds.Count; i < loopFeed.ValuesCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyHappeningFeed, loopFeed[k]);
                }
                Invoke(nameof(ResetSelectedIndex), 0.2f);
                resetting = false;
            }
            else
            {
                for (int i = response.HappeningFeeds.Count; i < feedCount; i++)
                {
                    int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                    UpdateValue(emptyHappeningFeed, loopFeed[k]);
                }
            }
        }
        else
        {
            int n = feedCount - response.HappeningFeeds.Count;
            for (int i = 0; i < n; i++)
            {
                int k = (startLoopIdx + i) % loopFeed.ValuesCount;
                UpdateValue(emptyHappeningFeed, loopFeed[k]);
            }

            for (int i = 0; i < response.HappeningFeeds.Count; i++)
            {
                int k = (startLoopIdx + n + i) % loopFeed.ValuesCount;
                UpdateValue(response.HappeningFeeds[i], loopFeed[k]);
            }
        }
        loopFeed.RefreshVisibleValues();

        ScreenDialog.Instance.Hide();

        if (response.Direction == 3 && response.HappeningFeeds.Count > 0)
        {
            int dataIndex = (startLoopIdx + feedCount - response.HappeningFeeds.Count) % loopFeed.ValuesCount;

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

    public void UpdateValue(HappeningFeed happeningFeed, LoopScrollerValue loopValue)
    {
        bool empty = happeningFeed.PublicationDateTime.Year == 1753;
        loopValue.ItemIdx = empty ? 0 : 1;
        loopValue.ItemSize = empty ? 2000 : 788;
        loopValue.Reset(loopFeed.LoopItems[loopValue.ItemIdx].LoopItem, empty ? null : new HappeningUserData(happeningFeed.PostId, happeningFeed.PublicationDateTime, happeningFeed.FavoriteCount));

        if (empty)
            return;

        loopValue.GetSprite(0)?.Destroy();
        loopValue.SetSprite(0, happeningFeed.TitleSprite);
        loopValue.SetActive(10, happeningFeed.TitleSprite != null);
        loopValue.SetText(1, $"<line-height=70%>{happeningFeed.Title}");
        loopValue.SetText(2, $"{(happeningFeed.Location.Length > 28 ? happeningFeed.Location[0..27] + "..." : happeningFeed.Location).Replace("\r", "").Replace("\n\n", " ").Replace('\n', ' ')}");
        loopValue.SetText(3, happeningFeed.StartDateTime?.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("es-GT")));
        loopValue.SetText(4, happeningFeed.StartDateTime?.ToString("HH:mm", CultureInfo.GetCultureInfo("es-GT")));
        loopValue.SetText(5, happeningFeed.EndDateTime?.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("es-GT")));
        loopValue.SetText(6, happeningFeed.EndDateTime?.ToString("HH:mm", CultureInfo.GetCultureInfo("es-GT")));
        loopValue.SetText(7, $"{happeningFeed.HappeningType}");
        loopValue.SetText(8, $"{happeningFeed.FavoriteCount}");
        loopValue.SetText(9, happeningFeed.Favorite != 0 ? "Asistencia<color=#023958>.</color>confirmada" : "Confirmar<color=#02558226>.</color>asistencia");

        loopValue.SetCheck(0, happeningFeed.Favorite != 0);
        loopValue.SetCheck(1, happeningFeed.Selected != 0);
    }

    public void SelectValue(int idx)
    {
        selectedIdx = idx % loopFeed.ValuesCount;
        onValueSelected.Invoke(((HappeningUserData)loopFeed[selectedIdx].UserData).PostId);
    }

    public void ApplyDetailPost(HappeningFeed happeningFeed)
    {
        UpdateValue(happeningFeed, loopFeed[selectedIdx]);

        loopFeed.RefreshVisibleValues();
    }

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

    long favoriteAppUserId = -1;
    long selectedAppUserId = -1;

    public void ApplyAppUserFavoriteSelected()
    {
        selectedAppUserId = tggFavoriteSelected.Value == "1" ? StateManager.Instance.AppUser.Id : -1;
        favoriteAppUserId = tggFavoriteSelected.Value == "2" ? StateManager.Instance.AppUser.Id : -1;

        ResetPosts(true);
    }


    // Favorite

    public void ApplyFavorite(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        HappeningUserData userData = (HappeningUserData)loopFeed[k].UserData;

        loopFeed[k].SetCheck(0, check);

        Favorite favorite = new Favorite(userData.PostId, StateManager.Instance.AppUser.Id);

        String title = "";
        if (check)
        {
            happeningService.RegisterFavorite(favorite);

            userData.FavoriteCount++;
            title = "Asistencia confirmada";
        }
        else
        {
            happeningService.DeleteFavorite(favorite);

            userData.FavoriteCount--;
            title = "Confirmar asistencia";

            if (favoriteAppUserId != -1)
                ResetPosts(true);
        }

        loopFeed[k].SetText(8, $"{userData.FavoriteCount}");
        loopFeed[k].SetText(9, title);

        loopFeed.RefreshVisibleValues();
    }

    public void ApplyDetailFavorite(bool check)
    {
        HappeningUserData userData = (HappeningUserData)loopFeed[selectedIdx].UserData;

        loopFeed[selectedIdx].SetCheck(0, check);

        String title = "";
        if (check)
        {
            title = "Asistencia confirmada";
            userData.FavoriteCount++;
        }
        else
        {
            title = "Confirmar asistencia";
            userData.FavoriteCount--;
        }

        loopFeed[selectedIdx].SetText(8, $"{userData.FavoriteCount}");
        loopFeed[selectedIdx].SetText(9, title);

        loopFeed.RefreshVisibleValues();

        MustReset = true;
    }

    // Selected

    public void ApplySelected(int dataIndex, bool check)
    {
        int k = dataIndex % loopFeed.ValuesCount;
        loopFeed[k].SetCheck(1, check);

        Selected selected = new Selected(((HappeningUserData)loopFeed[k].UserData).PostId, StateManager.Instance.AppUser.Id);
        if (check)
            happeningService.RegisterSelected(selected);
        else
        {
            happeningService.DeleteSelected(selected);

            if (selectedAppUserId != -1)
                ResetPosts(true);
        }
    }

    public void ApplyDetailSelected(bool check)
    {
        loopFeed[selectedIdx].SetCheck(1, check);

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