using System.Collections.Generic;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;

using MPUIKIT;

using Sirenix.OdinInspector;

public class TwoWorldsAction : MonoBehaviour
{
    [Space, SerializeField]
    ListScroller lstTwoWorlds = null;

    [Title("Indicators")]
    [SerializeField]
    RectTransform indicatorPrefab;
    [SerializeField]
    RectTransform indicatorsParent;
    [SerializeField]
    Color indicatorOnColor = Color.white;
    [SerializeField]
    Color indicatorOffColor = Color.gray;

    [Space, SerializeField]
    TwoWorldsOption[] options = null;

    public void CreateTwoWorlds()
    {
        lstTwoWorlds.ClearValues();
        for (int i = 0; i < options.Length; i++)
        {
            ListScrollerValue listValue = new ListScrollerValue(lstTwoWorlds.ListItem, false);

            listValue.SetSprite(0, options[i].Sprite);
            listValue.SetText(1, options[i].Title);
            listValue.SetText(2, options[i].DescLine1);
            listValue.SetText(3, options[i].DescLine2);

            lstTwoWorlds.AddValue(listValue);
        }
        lstTwoWorlds.ApplyValues();

        CreateIndicator();
    }

    public void GoToPage(int idx)
    {
        PageManager.Instance.ChangePage(options[idx].Page);
    }

    // Indicators

    List<MPImageBasic> indicators = new List<MPImageBasic>();
    int curOption = 0;

    void CreateIndicator()
    {
        indicators.Clear();

        foreach (Transform child in indicatorsParent)
            Destroy(child.gameObject);

        for (int i = 0; i < options.Length; i++)
        {
            MPImageBasic imgIndicator = Instantiate(indicatorPrefab, indicatorsParent).GetComponent<MPImageBasic>();
            imgIndicator.color = indicatorOffColor;

            indicators.Add(imgIndicator);
        }

        indicators[0].color = indicatorOnColor;
    }

    public void UpdateIndicators(int newOption)
    {
        indicators[curOption].color = indicatorOffColor;
        indicators[newOption].color = indicatorOnColor;
        curOption = newOption;
    }
}