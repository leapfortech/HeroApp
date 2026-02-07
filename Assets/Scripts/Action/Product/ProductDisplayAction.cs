using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Core.Tools;

using Sirenix.OdinInspector;


public class ProductDisplayAction : MonoBehaviour
{
    [Serializable]
    public class ImagesEvent : UnityEvent<List<Sprite>> { }
    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtTitle = null;

    [Title("Event")]
    [SerializeField]
    ImagesEvent onImagesDisplay = null;
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

        onImagesDisplay.Invoke(images);
        onDisplayed.Invoke(new long[2] {productFull.PostId, productFull.Id});

        PageManager.Instance.ChangePage(pagDetail);
    }
}