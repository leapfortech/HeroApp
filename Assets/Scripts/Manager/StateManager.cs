using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class StateManager : SingletonBehaviour<StateManager>
{
    private readonly String[] monthNames = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
    public String[] MonthNames => monthNames;

    public CultureInfo CultureInfo = new CultureInfo("es-ES");

    public ReferredCount ReferredCount { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public long appUserId { get; set; } = -1;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public AppUser AppUser { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Identity Identity { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Address Address { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Card Card { get; set; } = null;

    private Sprite portrait = null;
    public Sprite Portrait
    {
        get => portrait;
        set { portrait?.Destroy(); portrait = value; }
    }

    // FEEDS
    [Space]
    [Header("Feeds")]
    [SerializeField]
    private List<FeedState> feedStates = new();

    private Dictionary<String, FeedState> feedMap;

    private void FeedInitilize()
    {
        if (feedMap != null)
            return;

        feedMap = new Dictionary<String, FeedState>();

        for (int i = 0; i < feedStates.Count; i++)
        {
            FeedState feedState = Instantiate(feedStates[i]);
            feedState.ResetRuntime();

            feedMap[feedState.FeedKey] = feedState;
        }
    }

    public FeedState GetFeed(string feedKey)
    {
        FeedInitilize();

        if (feedMap.TryGetValue(feedKey, out FeedState state))
            return state;

        Debug.LogError("Feed not found: " + feedKey);
        return null;
    }

    public void ResetAllFeeds()
    {
        FeedInitilize();

        foreach (FeedState feed in feedMap.Values)
            feed.ResetRuntime();
    }


    // TALE
    public List<TaleFull> TaleFulls { get; set; }
    private Dictionary<long, TaleFull> DictTaleFulls { get; set; } = new Dictionary<long, TaleFull>();

    public void SetTaleFulls(List<TaleFull> taleFulls)
    {
        Dictionary<long, TaleFull> newDict = new Dictionary<long, TaleFull>();

        foreach (TaleFull taleFull in taleFulls)
            newDict[taleFull.Id] = taleFull;

        TaleFulls = taleFulls;
        DictTaleFulls = newDict;
    }

    public TaleFull GetTaleFullById(long taleId)
    {
        if (!DictTaleFulls.TryGetValue(taleId, out TaleFull taleFull))
            return null;
        return taleFull;
    }

    public long GetPostIdByTaleId(long taleId)
    {
        if (!DictTaleFulls.TryGetValue(taleId, out TaleFull taleFull))
            return -1;

        return taleFull.PostId;
    }

    // TALE IMAGES

    private Dictionary<long, List<Sprite>> taleImagesDic = new Dictionary<long, List<Sprite>>();
    public void AddTaleImages(long taleId, String[] stgImages)
    {
        List<Sprite> taleImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            taleImages.Add(stgImages[i].CreateSprite($"TaleImages_{i}"));
        taleImagesDic.Add(taleId, taleImages);
    }

    public List<Sprite> GetTaleImagesById(long taleId)
    {
        if (!taleImagesDic.TryGetValue(taleId, out List<Sprite> taleImages))
            return null;
        return taleImages;
    }

    // Clear

    public void ClearAll()
    {
        ReferredCount = null;
        AppUser = null;
        Address = null;
        Identity = null;
        Card = null;
        Portrait = null;

        // FEEDS
        ResetAllFeeds();

        // TALE
        TaleFulls = null;
        DictTaleFulls.Clear();
        taleImagesDic.Clear();
    }
}