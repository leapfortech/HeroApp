using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class RecipeUpdateAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmRecipe = null;
    [SerializeField]
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnUpdate = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    RecipeService recipeService = null;

    long postId = -1, recipeId = -1;
    Post post = null;
    Recipe recipe = null;

    private void Awake()
    {
        recipeService = GetComponent<RecipeService>();
    }

    private void Start()
    {
        btnUpdate?.AddAction(DoUpdate);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmRecipe.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    public void SetIds(long[] ids)
    {
        postId = ids[0];
        recipeId = ids[1];
    }

    public void Populate()
    {
        RecipeFull recipeFull = null; // StateManager.Instance.GetRecipeFullById(recipeId);

        post = new Post(recipeFull);
        dtmPost.PopulateClass<Post>(post);

        recipe = new Recipe(recipeFull);
        dtmRecipe.PopulateClass<Recipe>(recipe);

        //List<Sprite> images = StateManager.Instance.GetRecipeImagesById(recipeId);
        //dtmImagesVLL.PopulateBuiltInList<Sprite>(images);
    }

    private void DoUpdate()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        post.Update(dtmPost.BuildClass<Post>());

        recipe.Update(dtmRecipe.BuildClass<Recipe>());

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        recipeService.UpdateRecipe(new RegisterRecipeRequest(post, strImages, recipe));
    }

    public void ApplyUpdate(bool updated)
    {
        if (!updated)
        {
            ChoiceDialog.Instance.Error("Error", "No se pudo realizar la actualización.");
            return;
        }

        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
