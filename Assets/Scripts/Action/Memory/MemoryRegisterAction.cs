using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class MemoryRegisterAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmMemory = null;
    [SerializeField]
    DataMapper dtmTime = null;

    [SerializeField]
    DataMapper dtmImagesVLL = null;

    [Title("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Page")]
    [SerializeField]
    Page pagNext = null;

    [Title("Message")]
    [SerializeField]
    String disclaimerTitle = "Aviso importante";
    [Space, SerializeField, TextArea(2, 4)]
    String disclaimerMessage = "Te recomendamos";

    MemoryService memoryService = null;

    private void Awake()
    {
        memoryService = GetComponent<MemoryService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();

        dtmMemory.ClearElements();
        dtmTime.ClearElements();

        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        ChoiceDialog.Instance.Warning(disclaimerTitle, disclaimerMessage, DoRegister, null, "De acuerdo", "Regresar");
    }

    private void DoRegister()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;
        post.CountryId = interestLocality ? StateManager.Instance.InterestLocality.CountryId : StateManager.Instance.CurrentLocality.CountryId;
        post.StateId = interestLocality ? StateManager.Instance.InterestLocality.StateId : StateManager.Instance.CurrentLocality.StateId;

        Memory memory = dtmMemory.BuildClass<Memory>();

        if (memory.DateTime.HasValue)
        {
            String timeStr = dtmTime.BuildBuiltIn<String>();
            String[] time = timeStr.Split('|');
            memory.DateTime = new DateTime(memory.DateTime.Value.Year, memory.DateTime.Value.Month, memory.DateTime.Value.Day,
                                                Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);

        }

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        memoryService.Register(new RegisterMemoryRequest(post, strImages, memory));
    }

    public void ApplyMemory(long memoryId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }

    // Locality

    bool interestLocality = true;

    public void ApplyLocality(bool interestLocality)
    {
        this.interestLocality = interestLocality;
    }
}
