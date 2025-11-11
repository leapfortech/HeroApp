using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Collections;
using Leap.Graphics.Tools;
using Leap.Data.Web;
using Leap.UI.Page;

using Sirenix.OdinInspector;

public class DisplayProfileAction : MonoBehaviour
{
    [Title("Personal")]
    [SerializeField]
    FieldValue fldPhone = null;
    [SerializeField]
    FieldValue fldEmail = null;
    [Space]
    [SerializeField]
    Image[] imgSqrPortraits = null;
    [Space]
    [SerializeField]
    Image[] imgRctPortraits = null;
    [Space]
    [SerializeField]
    Button[] btnSqrPortraits = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllCountry = null;

    [Title("Page")]
    [SerializeField]
    Page pagBack;

    public void Clear()
    {
        fldPhone.Clear();
        fldEmail.Clear();
       
        for (int i = 0; i < imgSqrPortraits.Length; i++)
            imgSqrPortraits[i].Clear();
        for (int i = 0; i < imgRctPortraits.Length; i++)
            imgRctPortraits[i].Clear();
        for (int i = 0; i < btnSqrPortraits.Length; i++)
            btnSqrPortraits[i].Clear();
    }

    bool isSdw = false;
    public void SetBackPage(bool isSdw)
    {
        this.isSdw = isSdw;
    }

    public bool ChangeBackPage()
    {
        PageManager.Instance.ChangePage(pagBack);

        if (isSdw)
            SandwichMenu.Instance.Open();

        return false;
    }

    public void ApplyPortrait(LoginAppInfo loginData)
    {
        if (loginData.Portrait != null)
            StateManager.Instance.Portrait = loginData.Portrait.CreateSprite("Portrait");

        DisplayProfile();
    }

    public void DisplayProfile()
    {
        fldPhone.TextValue = vllCountry.FindRecordCellString(WebManager.Instance.WebSysUser.PhoneCountryId, "PhonePrefix") + " " + WebManager.Instance.WebSysUser.Phone;
        fldEmail.TextValue = WebManager.Instance.WebSysUser.Email;

        if (StateManager.Instance.Portrait == null)
            return;

        if (imgSqrPortraits.Length > 0)
        {
            Sprite sprite = StateManager.Instance.Portrait.texture.CreateSprite(new Rect(0, 0, 600, 600));
            for (int i = 0; i < imgSqrPortraits.Length; i++)
                imgSqrPortraits[i].Sprite = sprite;
        }

        if (imgSqrPortraits.Length > 0)
        {
            Sprite sprite = StateManager.Instance.Portrait.texture.CreateSprite(new Rect(0, 40, 180, 240));
            for (int i = 0; i < imgRctPortraits.Length; i++)
                imgRctPortraits[i].Sprite = sprite;
        }

        if (btnSqrPortraits.Length > 0)
        {
            Sprite sprite = StateManager.Instance.Portrait.texture.CreateSprite(new Rect(0, 0, 600, 600));
            for (int i = 0; i < btnSqrPortraits.Length; i++)
                btnSqrPortraits[i].Sprite = sprite;
        }
    }
}
