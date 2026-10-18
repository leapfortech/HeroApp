using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UScrollRect = UnityEngine.UI.ScrollRect;
using ULayoutElement = UnityEngine.UI.LayoutElement;

using Leap.UI.Elements;
using Leap.Data.Collections;

using TMPro;
using MPUIKIT;

using Sirenix.OdinInspector;

public class FilterScrollController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Serializable]
    public class UnityLongListEvent : UnityEvent<List<long>>{}

    [Title("Data")]
    [SerializeField]
    ValueList valueList = null;

    [SerializeField]
    Toggle togglePrefab = null;

    [Title("Layout")]
    [SerializeField]
    Toggle tglFilterAll = null;

    [SerializeField]
    float additionalWidth = 0f;

    [Title("Event")]
    [SerializeField]
    UnityLongListEvent onSelectionChanged = null;

    Transform content = null;

    readonly List<Toggle> toggles = new List<Toggle>();

    public void OnBeginDrag(PointerEventData eventData)
    {
        tglFilterAll.ClickAllowed = false;
        for (int i = 0; i < toggles.Count; i++)
            toggles[i].ClickAllowed = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        tglFilterAll.ClickAllowed = true;
        for (int i = 0; i < toggles.Count; i++)
            toggles[i].ClickAllowed = true;
    }

    public void Clear()
    {
        for (int i = toggles.Count - 1; i >= 0; i--)
        {
            if (toggles[i] == null)
                continue;
            toggles[i].ClearActions();
            Destroy(toggles[i].gameObject);
            toggles[i] = null;
        }

        toggles.Clear();
    }

    public void Initialize()
    {
        if (toggles.Count > 0)
            return;

        content = transform.GetChild(0).GetChild(0);

        tglFilterAll.SetAction(OnToggleChanged, -1);

        // Toggles
        int recordCount = valueList.RecordCount;

        for (int i = 0; i < recordCount; i++)
        {
            Toggle toggle = Instantiate(togglePrefab, content);
            
            toggle.Title = valueList.GetRecordCellString(i, "Name");

            toggle.SetAction(OnToggleChanged, i + 1);
            toggles.Add(toggle);

            //UpdateToggleWidth(toggles[i]);
        }

        tglFilterAll.Checked = true;

        for (int i = 0; i < recordCount; i++)
            UpdateToggleWidth(toggles[i]);

        ThemeManager.Instance.RefreshStylesInHierarchy(content.gameObject);

        GetComponent<UScrollRect>().horizontalNormalizedPosition = 0f;

        //Debug.Log("SelectionChanged: -1"));
    }

    private void UpdateToggleWidth(Toggle toggle)
    {
        toggle.SetStyle();

        RectTransform toggleRect = toggle.GetComponent<RectTransform>();
        TMP_Text text = toggle.GetComponentInChildren<TMP_Text>(true);

        text.text = toggle.Title;

        toggleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth + additionalWidth);
    }

    private void OnToggleChanged(int index, bool checkedValue)
    {
        if (!checkedValue && index == -1)
        {
            tglFilterAll.Checked = true;
            return;
        }

        if (checkedValue)
        {
            if (index == -1)
                for (int i = 1; i < toggles.Count; i++)
                    toggles[i].Checked = false;
            else
                tglFilterAll.Checked = false;
        }
        else
        {
            bool anySelected = false;

            for (int i = 1; i < toggles.Count; i++)
            {
                if (toggles[i].Checked)
                {
                    anySelected = true;
                    break;
                }
            }

            if (!anySelected)
                tglFilterAll.Checked = true;
        }

        UpdateSelectedValues();
    }

    private void UpdateSelectedValues()
    {
        List<long> selectedIds = new List<long>();

        if (tglFilterAll.Checked)
            selectedIds.Add(-1L);
        else
            for (int i = 0; i < toggles.Count; i++)
            {
                if (!toggles[i].Checked)
                    continue;

                selectedIds.Add(valueList.GetRecordId(i));
            }

        onSelectionChanged?.Invoke(selectedIds);

        //Debug.Log(string.Join(", ", selectedIds));
    }
}