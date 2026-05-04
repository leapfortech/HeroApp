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

    public void ResetAllFeedStates()
    {
        FeedInitialize();

        foreach (FeedState feed in feedMap.Values)
            feed.PostFulls = new List<PostFull>();
    }


    // Tale
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

    // Tale Images
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

    // Treatment
    public List<TreatmentFull> TreatmentFulls { get; set; }
    private Dictionary<long, TreatmentFull> DictTreatmentFulls { get; set; } = new Dictionary<long, TreatmentFull>();
    private Dictionary<long, TreatmentFull> DictTreatmentFullsByPostId = new Dictionary<long, TreatmentFull>();

    public void ClearTreatment()
    {
        TreatmentFulls = null;
        DictTreatmentFulls.Clear();
        DictTreatmentFullsByPostId.Clear();
        treatmentImagesDic.Clear();
    }

    public TreatmentFull GetTreatmentFullById(long treatmentId)
    {
        if (!DictTreatmentFulls.TryGetValue(treatmentId, out TreatmentFull treatmentFull))
            return null;
        return treatmentFull;
    }

    public TreatmentFull GetTreatmentFullByPostId(long postId)
    {
        if (!DictTreatmentFullsByPostId.TryGetValue(postId, out TreatmentFull treatmentFull))
            return null;

        return treatmentFull;
    }

    public void AddTreatmentFull(TreatmentFull treatmentFull)
    {
        if (treatmentFull == null)
            return;

        if (TreatmentFulls == null)
            TreatmentFulls = new List<TreatmentFull>();

        DictTreatmentFulls[treatmentFull.Id] = treatmentFull;
        DictTreatmentFullsByPostId[treatmentFull.PostId] = treatmentFull;

        for (int i = 0; i < TreatmentFulls.Count; i++)
        {
            if (TreatmentFulls[i].Id == treatmentFull.Id)
            {
                TreatmentFulls[i] = treatmentFull;
                return;
            }
        }

        TreatmentFulls.Add(treatmentFull);
    }

    // Treatment Images
    public List<Sprite> GetTreatmentImagesById(long treatmentId)
    {
        if (!treatmentImagesDic.TryGetValue(treatmentId, out List<Sprite> treatmentImages))
            return null;
        return treatmentImages;
    }

    private Dictionary<long, List<Sprite>> treatmentImagesDic = new Dictionary<long, List<Sprite>>();
    public void AddTreatmentImages(long treatmentId, String[] stgImages)
    {
        List<Sprite> treatmentImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            treatmentImages.Add(stgImages[i].CreateSprite($"TreatmentImages_{i}"));
        treatmentImagesDic.Add(treatmentId, treatmentImages);
    }

    // Radio
    public List<RadioFull> RadioFulls { get; set; }
    private Dictionary<long, RadioFull> DictRadioFulls { get; set; } = new Dictionary<long, RadioFull>();
    private Dictionary<long, RadioFull> DictRadioFullsByPostId = new Dictionary<long, RadioFull>();

    public void ClearRadio()
    {
        RadioFulls = null;
        DictRadioFulls.Clear();
        DictRadioFullsByPostId.Clear();
        radioImagesDic.Clear();
    }

    public RadioFull GetRadioFullById(long radioId)
    {
        if (!DictRadioFulls.TryGetValue(radioId, out RadioFull radioFull))
            return null;
        return radioFull;
    }

    public RadioFull GetRadioFullByPostId(long postId)
    {
        if (!DictRadioFullsByPostId.TryGetValue(postId, out RadioFull radioFull))
            return null;

        return radioFull;
    }

    public void AddRadioFull(RadioFull radioFull)
    {
        if (radioFull == null)
            return;

        if (RadioFulls == null)
            RadioFulls = new List<RadioFull>();

        DictRadioFulls[radioFull.Id] = radioFull;
        DictRadioFullsByPostId[radioFull.PostId] = radioFull;

        for (int i = 0; i < RadioFulls.Count; i++)
        {
            if (RadioFulls[i].Id == radioFull.Id)
            {
                RadioFulls[i] = radioFull;
                return;
            }
        }

        RadioFulls.Add(radioFull);
    }

    // Radio Images
    public List<Sprite> GetRadioImagesById(long radioId)
    {
        if (!radioImagesDic.TryGetValue(radioId, out List<Sprite> radioImages))
            return null;
        return radioImages;
    }

    private Dictionary<long, List<Sprite>> radioImagesDic = new Dictionary<long, List<Sprite>>();
    public void AddRadioImages(long radioId, String[] stgImages)
    {
        List<Sprite> radioImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            radioImages.Add(stgImages[i].CreateSprite($"RadioImages_{i}"));
        radioImagesDic.Add(radioId, radioImages);
    }

    // Product
    public List<ProductFull> ProductFulls { get; set; }
    private Dictionary<long, ProductFull> DictProductFulls { get; set; } = new Dictionary<long, ProductFull>();
    private Dictionary<long, ProductFull> DictProductFullsByPostId = new Dictionary<long, ProductFull>();

    public void ClearProduct()
    {
        ProductFulls = null;
        DictProductFulls.Clear();
        DictProductFullsByPostId.Clear();
        productImagesDic.Clear();
    }

    public ProductFull GetProductFullById(long productId)
    {
        if (!DictProductFulls.TryGetValue(productId, out ProductFull productFull))
            return null;
        return productFull;
    }

    public ProductFull GetProductFullByPostId(long postId)
    {
        if (!DictProductFullsByPostId.TryGetValue(postId, out ProductFull productFull))
            return null;

        return productFull;
    }

    public void AddProductFull(ProductFull productFull)
    {
        if (productFull == null)
            return;

        if (ProductFulls == null)
            ProductFulls = new List<ProductFull>();

        DictProductFulls[productFull.Id] = productFull;
        DictProductFullsByPostId[productFull.PostId] = productFull;

        for (int i = 0; i < ProductFulls.Count; i++)
        {
            if (ProductFulls[i].Id == productFull.Id)
            {
                ProductFulls[i] = productFull;
                return;
            }
        }

        ProductFulls.Add(productFull);
    }

    // Product Images
    public List<Sprite> GetProductImagesById(long productId)
    {
        if (!productImagesDic.TryGetValue(productId, out List<Sprite> productImages))
            return null;
        return productImages;
    }

    private Dictionary<long, List<Sprite>> productImagesDic = new Dictionary<long, List<Sprite>>();
    public void AddProductImages(long productId, String[] stgImages)
    {
        List<Sprite> productImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            productImages.Add(stgImages[i].CreateSprite($"ProductImages_{i}"));
        productImagesDic.Add(productId, productImages);
    }

    // Happening
    public List<HappeningFull> HappeningFulls { get; set; }
    private Dictionary<long, HappeningFull> DictHappeningFulls { get; set; } = new Dictionary<long, HappeningFull>();
    private Dictionary<long, HappeningFull> DictHappeningFullsByPostId = new Dictionary<long, HappeningFull>();

    public void ClearHappening()
    {
        HappeningFulls = null;
        DictHappeningFulls.Clear();
        DictHappeningFullsByPostId.Clear();
        happeningImagesDic.Clear();
    }

    public HappeningFull GetHappeningFullById(long happeningId)
    {
        if (!DictHappeningFulls.TryGetValue(happeningId, out HappeningFull happeningFull))
            return null;
        return happeningFull;
    }

    public HappeningFull GetHappeningFullByPostId(long postId)
    {
        if (!DictHappeningFullsByPostId.TryGetValue(postId, out HappeningFull happeningFull))
            return null;

        return happeningFull;
    }

    public void AddHappeningFull(HappeningFull happeningFull)
    {
        if (happeningFull == null)
            return;

        if (HappeningFulls == null)
            HappeningFulls = new List<HappeningFull>();

        DictHappeningFulls[happeningFull.Id] = happeningFull;
        DictHappeningFullsByPostId[happeningFull.PostId] = happeningFull;

        for (int i = 0; i < HappeningFulls.Count; i++)
        {
            if (HappeningFulls[i].Id == happeningFull.Id)
            {
                HappeningFulls[i] = happeningFull;
                return;
            }
        }

        HappeningFulls.Add(happeningFull);
    }

    // Happening Images
    public List<Sprite> GetHappeningImagesById(long happeningId)
    {
        if (!happeningImagesDic.TryGetValue(happeningId, out List<Sprite> happeningImages))
            return null;
        return happeningImages;
    }

    private Dictionary<long, List<Sprite>> happeningImagesDic = new Dictionary<long, List<Sprite>>();
    public void AddHappeningImages(long happeningId, String[] stgImages)
    {
        List<Sprite> happeningImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            happeningImages.Add(stgImages[i].CreateSprite($"HappeningImages_{i}"));
        happeningImagesDic.Add(happeningId, happeningImages);
    }

    // News
    public List<NewsFull> NewsFulls { get; set; }
    private Dictionary<long, NewsFull> DictNewsFulls { get; set; } = new Dictionary<long, NewsFull>();
    private Dictionary<long, NewsFull> DictNewsFullsByPostId = new Dictionary<long, NewsFull>();

    public void ClearNews()
    {
        NewsFulls = null;
        DictNewsFulls.Clear();
        DictNewsFullsByPostId.Clear();
        newsImagesDic.Clear();
    }

    public NewsFull GetNewsFullById(long newsId)
    {
        if (!DictNewsFulls.TryGetValue(newsId, out NewsFull newsFull))
            return null;
        return newsFull;
    }

    public NewsFull GetNewsFullByPostId(long postId)
    {
        if (!DictNewsFullsByPostId.TryGetValue(postId, out NewsFull newsFull))
            return null;

        return newsFull;
    }

    public void AddNewsFull(NewsFull newsFull)
    {
        if (newsFull == null)
            return;

        if (NewsFulls == null)
            NewsFulls = new List<NewsFull>();

        DictNewsFulls[newsFull.Id] = newsFull;
        DictNewsFullsByPostId[newsFull.PostId] = newsFull;

        for (int i = 0; i < NewsFulls.Count; i++)
        {
            if (NewsFulls[i].Id == newsFull.Id)
            {
                NewsFulls[i] = newsFull;
                return;
            }
        }

        NewsFulls.Add(newsFull);
    }

    // News Images
    public List<Sprite> GetNewsImagesById(long newsId)
    {
        if (!newsImagesDic.TryGetValue(newsId, out List<Sprite> newsImages))
            return null;
        return newsImages;
    }

    private Dictionary<long, List<Sprite>> newsImagesDic = new Dictionary<long, List<Sprite>>();
    public void AddNewsImages(long newsId, String[] stgImages)
    {
        List<Sprite> newsImages = new List<Sprite>();
        for (int i = 0; i < stgImages.Length; i++)
            newsImages.Add(stgImages[i].CreateSprite($"NewsImages_{i}"));
        newsImagesDic.Add(newsId, newsImages);
    }

    // Puzzle
    public List<PuzzleFull> PuzzleFulls { get; set; }
    private Dictionary<long, PuzzleFull> DictPuzzleFulls { get; set; } = new Dictionary<long, PuzzleFull>();
    private Dictionary<long, PuzzleFull> DictPuzzleFullsByPostId = new Dictionary<long, PuzzleFull>();

    public void ClearPuzzle()
    {
        PuzzleFulls = null;
        DictPuzzleFulls.Clear();
        DictPuzzleFullsByPostId.Clear();
    }

    public PuzzleFull GetPuzzleFullById(long puzzleId)
    {
        if (!DictPuzzleFulls.TryGetValue(puzzleId, out PuzzleFull puzzleFull))
            return null;
        return puzzleFull;
    }

    public PuzzleFull GetPuzzleFullByPostId(long postId)
    {
        if (!DictPuzzleFullsByPostId.TryGetValue(postId, out PuzzleFull puzzleFull))
            return null;

        return puzzleFull;
    }

    public void AddPuzzleFull(PuzzleFull puzzleFull)
    {
        if (puzzleFull == null)
            return;

        if (PuzzleFulls == null)
            PuzzleFulls = new List<PuzzleFull>();

        DictPuzzleFulls[puzzleFull.Id] = puzzleFull;
        DictPuzzleFullsByPostId[puzzleFull.PostId] = puzzleFull;

        for (int i = 0; i < PuzzleFulls.Count; i++)
        {
            if (PuzzleFulls[i].Id == puzzleFull.Id)
            {
                PuzzleFulls[i] = puzzleFull;
                return;
            }
        }

        PuzzleFulls.Add(puzzleFull);
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
        ResetAllFeedStates();

        // Tale
        ClearTale();
        ClearTreatment();
        ClearRadio();
        ClearProduct();
        ClearHappening();
        ClearNews();
        ClearPuzzle();
    }
}