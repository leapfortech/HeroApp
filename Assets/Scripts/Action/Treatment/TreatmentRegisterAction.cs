using System;
using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.UI.Dialog;
using Leap.Data.Mapper;
using Leap.Graphics.Tools;

using Sirenix.OdinInspector;

public class TreatmentRegisterAction : MonoBehaviour
{

    [Title("Elements")]
    [SerializeField]
    ElementValue[] elementValues = null;

    [Title("Data")]
    [SerializeField]
    DataMapper dtmPost = null;
    [SerializeField]
    DataMapper dtmTreatment = null;
    [SerializeField]
    DataMapper dtmDiseaseVLL = null;
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
    String disclaimerMessage = "Estás compartiendo una experiencia personal o recomendación. No publiques información falsa, peligrosa ni que sustituya la evaluación de un profesional de la salud. El contenido que compartas será visible para otros usuarios.";

    TreatmentService treatmentService = null;

    private void Awake()
    {
        treatmentService = GetComponent<TreatmentService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(Register);
    }

    public void Clear()
    {
        dtmPost.ClearElements();
        dtmTreatment.ClearElements();
        dtmImagesVLL.ClearElements();
    }

    private void Register()
    {
        ChoiceDialog.Instance.Warning(disclaimerTitle, disclaimerMessage, () => DoRegister(), null, "De acuerdo", "Regresar");
    }
    
    private void DoRegister()
    {
        if (!ElementHelper.Validate(elementValues))
            return;

        ScreenDialog.Instance.Display();

        Post post = dtmPost.BuildClass<Post>();
        post.AppUserId = StateManager.Instance.AppUser.Id;

        //RM REVIEW
        post.CountryId = StateManager.Instance.InterestLocality.CountryId;
        post.StateId = StateManager.Instance.InterestLocality.StateId;

        Treatment treatment = dtmTreatment.BuildClass<Treatment>();
        List<Disease> diseases = dtmDiseaseVLL.BuildClassList<Disease>();

        List<Sprite> images = dtmImagesVLL.BuildBuiltInList<Sprite>();
        String[] strImages = new String[images.Count];
        for (int i = 0; i < images.Count; i++)
            strImages[i] = images[i].ToStrBase64(ImageType.JPG);

        treatmentService.Register(new RegisterTreatmentRequest(post, strImages, treatment, diseases));
    }

    public void ApplyTreatment(long treatmentId)
    {
        Clear();
        PageManager.Instance.ChangePage(pagNext);
    }
}
