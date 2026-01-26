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
    private Dictionary<long, TaleFull> DictTaleFullsByPostId = new Dictionary<long, TaleFull>();

    public void ClearTale()
    {
        TaleFulls = null;
        DictTaleFulls.Clear();
        DictTaleFullsByPostId.Clear();
        taleImagesDic.Clear();
    }
    
    public TaleFull GetTaleFullById(long taleId)
    {
        if (!DictTaleFulls.TryGetValue(taleId, out TaleFull taleFull))
            return null;
        return taleFull;
    }

    public TaleFull GetTaleFullByPostId(long postId)
    {
        if (!DictTaleFullsByPostId.TryGetValue(postId, out TaleFull taleFull))
            return null;

        return taleFull;
    }

    public void AddTaleFull(TaleFull taleFull)
    {
        if (taleFull == null)
            return;

        if (TaleFulls == null)
            TaleFulls = new List<TaleFull>();

        DictTaleFulls[taleFull.Id] = taleFull;
        DictTaleFullsByPostId[taleFull.PostId] = taleFull;

        for (int i = 0; i < TaleFulls.Count; i++)
        {
            if (TaleFulls[i].Id == taleFull.Id)
            {
                TaleFulls[i] = taleFull;
                return;
            }
        }

        TaleFulls.Add(taleFull);
    }

    // TALE IMAGES

    public List<Sprite> GetTaleImagesById(long taleId)
    {
        if (!taleImagesDic.TryGetValue(taleId, out List<Sprite> taleImages))
            return null;
        return taleImages;
    }

    private Dictionary<long, List<Sprite>> taleImagesDic = new Dictionary<long, List<Sprite>>();
    public void AddTaleImages(long taleId, String[] stgImages)
    {
        List<Sprite> taleImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            taleImages.Add(stgImages[i].CreateSprite($"TaleImages_{i}"));
        taleImagesDic.Add(taleId, taleImages);
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

        // Feeds
        ResetAllFeeds();

        // Tale
        ClearTale();
    }
}