using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class ProductDisplayAction : MonoBehaviour
{
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    [Space]
    [Title("Images")]
    [SerializeField]
    ListScroller lstImage = null;

    //[Title("Values")]
    //[SerializeField]
    //ValueList vllProjectDescriptionType = null;
    [Title("Event")]
    [SerializeField]
    UnityLongsEvent onDisplayed = null;

    [Title("Page")]
    [SerializeField]
    Page pagDetail;

    ProductService productService;

    long postId = -1, productId = -1;

    private void Awake()
    {
        productService = GetComponent<ProductService>();
    }

    public void Clear()
    {
        
    }

    // Display

    public void Display(long postId)
    {
        this.postId = postId;

        ProductFull productFull = StateManager.Instance.GetProductFullByPostId(postId);
        if (productFull != null)
        {
            productId = productFull.Id;
            Display(productFull);
            return;
        }

        ScreenDialog.Instance.Display();
        productService.GetFullByPostId(postId);
    }

    public void ApplyFull(ProductFull productFull)
    {
        productId = productFull.Id;
        StateManager.Instance.AddProductFull(productFull);
        StateManager.Instance.AddProductImages(productFull.Id, productFull.Images);
        Display(productFull);
    }

    private void Display(ProductFull productFull)
    {       
        if (productFull == null)
            return;

        txtTitle.TextValue = productFull.Title;

        List<Sprite> images = StateManager.Instance.GetProductImagesById(productId);

        lstImage.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(1, true);
            scrollerValue.SetSprite(0, images[i]);
            lstImage.ApplyAddValue(scrollerValue);
        }

        PageManager.Instance.ChangePage(pagDetail);

        onDisplayed.Invoke(new long[2] {productFull.PostId, productFull.Id});
    }
}