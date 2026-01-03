using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.Graphics.Tools;
using Leap.UI.Dialog;
using Leap.Data.Collections;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class TaleDisplayAction : MonoBehaviour
{
    [Space]
    [Title("List")]
    [SerializeField]
    ListScroller lstTale = null;
    [SerializeField]
    Text txtTaleEmpty;
   
    [Space]
    [Title("Details")]
    [SerializeField]
    Image imgTitle = null;
    [SerializeField]
    Text txtTitle = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    ListScroller lstImage = null;

    [Space]
    [Title("Events")]
    [SerializeField]
    private UnityLongEvent onSelected;
    
    //[Title("Values")]
    //[SerializeField]
    //ValueList vllProjectDescriptionType = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    PostService postService;
    public long SelIdx { get; set; } = 0;
    
    Dictionary<long, long> indexes = new Dictionary<long, long>();
    long idx = 0;


    private void Awake()
    {
        postService = GetComponent<PostService>();
    }

    public void Clear()
    {
        idx = 0;
        indexes.Clear();
        lstTale.ClearValues();
    }

    private long GetId()
    {
        if (indexes.TryGetValue(SelIdx, out long taleId))
            return taleId;
        return -1;
    }

    // Display

    public void Display()
    {
        Clear();

        for (int i = 0; i < StateManager.Instance.TaleFulls.Count; i++)
        {
            indexes.Add(idx, StateManager.Instance.TaleFulls[i].Id);
            idx++;

            ListScrollerValue scrollerValue = new ListScrollerValue(2, true);
            scrollerValue.SetText(0, StateManager.Instance.TaleFulls[i].Description);
            scrollerValue.SetSprite(1, StateManager.Instance.TaleFulls[i].TitleSprite);

            lstTale.AddValue(scrollerValue);
        }

        lstTale.ApplyClearValues();
        //txtTaleEmpty.gameObject.SetActive(false);
    }

    public void DisplayDetail()
    {
        TaleFull taleFull = StateManager.Instance.GetTaleFullById(GetId());

        if (taleFull == null)
            return;

        onSelected.Invoke(GetId());

        imgTitle.Sprite = taleFull.TitleSprite;
       
        txtTitle.TextValue = taleFull.Title;

        List<Sprite> taleImages = StateManager.Instance.GetTaleImagesById(GetId());

        lstImage.Clear();

        for (int i = 0; i < taleImages.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, taleImages[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }
        
        PageManager.Instance.ChangePage(pagDetail);
    }

    // Images
    public void GetImages()
    {
        if (StateManager.Instance.GetTaleImagesById(GetId()) != null)
        {
            DisplayDetail();
            return;
        }

        ScreenDialog.Instance.Display();
        postService.GetImagesById(GetId());
    }

    public void ApplyImages(String[] stgImages)
    {
        StateManager.Instance.AddTaleImages(GetId(), stgImages);
    }
}