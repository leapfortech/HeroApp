using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.Data.Mapper;
using Leap.Data.Collections;

using Sirenix.OdinInspector;

public class DisplayFlagAction : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField]
    Combo cmbCountry = null;

    [SerializeField]
    Image image = null;

    [Title("Data")]
    [SerializeField]
    ValueList vllFlags = null;

    public void DisplayFlag()
    {
        String code = cmbCountry.GetValue<String>();
        if (code == null)
            code = "Flags_NON";

        image.Sprite = vllFlags.FindRecordCellSprite("Code", code, "Flag");
    }
}
