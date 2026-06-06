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
    public AppUser AppUser { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Identity Identity { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Address Address { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Locality InterestLocality { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Locality CurrentLocality { get; set; } = null;

    [PropertySpace]
    [ShowInInspector, HideReferenceObjectPicker, ReadOnly]
    public Card Card { get; set; } = null;

    private Sprite portrait = null;
    public Sprite Portrait
    {
        get => portrait;
        set { portrait?.Destroy(); portrait = value; }
    }

    public Player Player { get; set; } = null;

    public List<PuzzleResultSummary> PuzzleResultSummarys { get; set; }

    // Option
    public long GetOption(int idx)
    {
        return AppUser.Options / (long)Math.Pow(10, idx) % 10;  // JAD >> array
    }
    
    public long UpdateOption(int idx, int newStatus)
    {
        long options = AppUser.Options;

        long power = (long)Math.Pow(10, idx);
        long currentStatus = (options / power) % 10;

        long updatedOptions = options + (newStatus - currentStatus) * power;

        AppUser.Options = updatedOptions;

        return updatedOptions;
    }

    // Identity
    public void UpdateIdentityPersonal (long id, IdentityPersonal identityPersonal)
    {
        if (Identity == null)
            return;

        Identity.FirstName1 = identityPersonal.FirstName1;
        Identity.FirstName2 = identityPersonal.FirstName2;
        Identity.LastName1 = identityPersonal.LastName1;
        Identity.LastName2 = identityPersonal.LastName2;
        Identity.BirthDate = identityPersonal.BirthDate;
        Identity.GenderId = identityPersonal.GenderId;
    }

    public void UpdateIdentityPlace(long id, IdentityPlace identityPlace)
    {
        if (Identity == null)
            return;

        Identity.BirthCountryId = identityPlace.BirthCountryId;
        Identity.BirthStateId = identityPlace.BirthStateId;
        Identity.BirthCityId = identityPlace.BirthCityId;
    }

    // Address
    public void UpdateAddressCity(long id, AddressCity addressCity)
    {
        if (Identity == null)
            return;

        Address.CountryId = addressCity.CountryId;
        Address.StateId = addressCity.StateId;
        Address.CityId = addressCity.CityId;
    }

    // FEEDS
    [Space]
    [Header("Feeds")]
    [SerializeField]
    private List<FeedState> feedStates = new();
    private Dictionary<String, FeedState> feedMap;

    private void FeedInitialize()
    {
        if (feedMap != null)
            return;

        feedMap = new Dictionary<String, FeedState>();

        for (int i = 0; i < feedStates.Count; i++)
            feedMap[feedStates[i].FeedKey] = Instantiate(feedStates[i]);
    }

    public FeedState GetFeedState(String feedKey)
    {
        FeedInitialize();

        if (!feedMap.TryGetValue(feedKey, out FeedState state))
        {
            Debug.LogError($"Feed not found: {feedKey.ToString()}");
            return null;
        }

        return state;
    }

    // Puzzle
    public PuzzleFull PuzzleFull { get; set; }

    public void ClearPuzzle()
    {
        PuzzleFull = null;
    }

    public int GetTotalPuzzlePoints()
    {
        if (PuzzleResultSummarys == null)
            return 0;

        int totalPoints = 0;

        for (int i = 0; i < PuzzleResultSummarys.Count; i++)
            totalPoints += PuzzleResultSummarys[i].TotalPoints;

        return totalPoints;
    }

    public int GetTotalPuzzleMedals()
    {
        if (PuzzleResultSummarys == null)
            return 0;

        int totalMedals = 0;

        for (int i = 0; i < PuzzleResultSummarys.Count; i++)
            totalMedals += PuzzleResultSummarys[i].TotalMedals;

        return totalMedals;
    }

    public int GetTotalPuzzleCups()
    {
        if (PuzzleResultSummarys == null)
            return 0;

        int totalCups = 0;

        for (int i = 0; i < PuzzleResultSummarys.Count; i++)
            totalCups += PuzzleResultSummarys[i].TotalCups;

        return totalCups;
    }

    public PuzzleResultSummary GetPuzzleResultSummary(long puzzleGameId)
    {
        if (PuzzleResultSummarys == null)
            return null;

        for (int i = 0; i < PuzzleResultSummarys.Count; i++)
        {
            if (PuzzleResultSummarys[i].PuzzleGameId == puzzleGameId)
                return PuzzleResultSummarys[i];
        }

        return null;
    }

    public void UpdatePuzzleResultSummary(long puzzleGameId, int points, int medals, int cups)
    {
        if (PuzzleResultSummarys == null)
            PuzzleResultSummarys = new List<PuzzleResultSummary>();

        for (int i = 0; i < PuzzleResultSummarys.Count; i++)
        {
            if (PuzzleResultSummarys[i].PuzzleGameId == puzzleGameId)
            {
                PuzzleResultSummarys[i].TotalPoints += points;
                PuzzleResultSummarys[i].TotalMedals += medals;
                PuzzleResultSummarys[i].TotalCups += cups;
                return;
            }
        }

        PuzzleResultSummarys.Add(new PuzzleResultSummary(puzzleGameId, points, medals, cups));
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

        // Locality
        InterestLocality = null;
        CurrentLocality = null;

        // Feeds
        FeedInitialize();

        // Clear
        ClearPuzzle();
    }
}