using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Leap.UI.Elements;
using Leap.Data.Collections;

using TMPro;
using MPUIKIT;

public class FilterScrollController : MonoBehaviour
{
    [Serializable]
    public class UnityIntListEvent : UnityEvent<List<int>>{}

    [Header("Scroll")]
    [SerializeField]
    UnityEngine.UI.ScrollRect scrollRect = null;
    [SerializeField]
    RectTransform content = null;

    [Header("Data")]
    [SerializeField]
    ValueList valueList = null;

    [SerializeField]
    Toggle togglePrefab = null;


    [Header("Layout")]
    [SerializeField]
    float additionalWidth = 0f;

    [Header("Event")]
    [SerializeField]
    UnityIntListEvent onSelectionChanged = null;

    List<Toggle> toggles = new List<Toggle>();

    List<int> selectedIndexes = new List<int>();
    List<long> selectedIds = new List<long>();

    bool initialize = false;

    public void Clear()
    {
        for (int i = toggles.Count - 1; i >= 0; i--)
        {
            if (toggles[i] != null)
                Destroy(toggles[i].gameObject);
        }

        toggles.Clear();
        selectedIndexes.Clear();
        selectedIds.Clear();
    }

    public long[] SelectedIds
    {
        get
        {
            return selectedIds.ToArray();
        }
    }

    public int[] SelectedIndexes
    {
        get
        {
            return selectedIndexes.ToArray();
        }
    }

    public void Initialize()
    {
        if (initialize)
            return;

        initialize = true;

        Clear();

        // TglAll
        Toggle tglAll = Instantiate(togglePrefab, content);
        tglAll.Title = "  Todos";

        CreateAllCircle(tglAll);

        UnityIntBoolEvent allToggleEvent = new UnityIntBoolEvent();
        allToggleEvent.AddListener(OnToggleChanged);
        tglAll.SetIntBoolEvent(allToggleEvent, -1);
        toggles.Add(tglAll);

        UpdateToggleWidth(tglAll);

        // Separator
        CreateSeparator();

        // All Toggles
        int recordCount = valueList.RecordCount;

        for (int i = 0; i < recordCount; i++)
        {
            Toggle toggle = Instantiate(togglePrefab, content);
            
            String title = valueList.GetRecordCellString(i, "Name");
            toggle.Title = title;

            UnityIntBoolEvent toggleEvent = new UnityIntBoolEvent();
            toggleEvent.AddListener(OnToggleChanged);
            toggle.SetIntBoolEvent(toggleEvent, i + 1);
            toggles.Add(toggle);

            UpdateToggleWidth(toggle);
        }

        toggles[0].Checked = true;

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        scrollRect.horizontalNormalizedPosition = 0f;

        List<int> indexes = new List<int> { -1 };

        Debug.Log("SelectionChanged: " + string.Join(", ", indexes));

        onSelectionChanged?.Invoke(indexes);
    }

    private void UpdateToggleWidth(Toggle toggle)
    {
        toggle.SetStyle();

        RectTransform toggleRect = toggle.GetComponent<RectTransform>();
        TMP_Text text = toggle.GetComponentInChildren<TMP_Text>(true);

        text.text = toggle.Title;
        text.ForceMeshUpdate();

        float width = text.preferredWidth + additionalWidth;

        if (toggle == toggles[0])
        {
            width += AllCircleLeft + AllCircleSize + AllCircleSpacing;

            RectTransform textRect = text.GetComponent<RectTransform>();

            textRect.anchoredPosition = new Vector2(
                AllCircleLeft + AllCircleSize + AllCircleSpacing,
                textRect.anchoredPosition.y);
        }

        toggleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        //Debug.Log(toggle.Title + " = " + text.preferredWidth);
    }

    private void OnToggleChanged(int index, bool checkedValue)
    {
        if (!checkedValue && index == -1)
        {
            // "Todos" no puede quedar desactivado si no hay otro filtro seleccionado
            toggles[0].Checked = true;
            return;
        }
        else if (checkedValue)
        {
            if (index == -1)
                // "Todos" activado: desactivar los demás
                for (int i = 1; i < toggles.Count; i++)
                    toggles[i].Checked = false;
            else
                // Otro toggle activado: desactivar "Todos"
                toggles[0].Checked = false;
        }
        else
        {
            // Se desactivó un filtro.
            // Si no queda ninguno seleccionado, activar "Todos".
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
                toggles[0].Checked = true;
        }

        UpdateSelectedValues();
    }

    private void UpdateSelectedValues()
    {
        selectedIndexes.Clear();
        selectedIds.Clear();

        if (toggles.Count > 0 && toggles[0].Checked)
            selectedIndexes.Add(-1);

        for (int i = 1; i < toggles.Count; i++)
        {
            if (!toggles[i].Checked)
                continue;

            selectedIndexes.Add(i);
            selectedIds.Add(valueList.GetRecordId(i - 1));
        }

        onSelectionChanged?.Invoke(selectedIndexes);

        Debug.Log(string.Join(", ", selectedIndexes));
    }

    //

    public void ClearSelection()
    {
        for (int i = 0; i < toggles.Count; i++)
            toggles[i].Checked = false;

        UpdateSelectedValues();
    }

    public void SelectAll()
    {
        for (int i = 0; i < toggles.Count; i++)
            toggles[i].Checked = true;

        UpdateSelectedValues();
    }

    public void SetSelectedIds(long[] ids)
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            bool selected = false;

            if (ids != null)
            {
                long id = valueList.GetRecordId(i);

                for (int j = 0; j < ids.Length; j++)
                {
                    if (ids[j] == id)
                    {
                        selected = true;
                        break;
                    }
                }
            }

            toggles[i].Checked = selected;
        }

        UpdateSelectedValues();
    }

    private void CreateSeparator()
    {
        GameObject separatorObject = new GameObject("Separator");
        separatorObject.transform.SetParent(content, false);

        MPImage separator = separatorObject.AddComponent<MPImage>();
        separator.color = Color.gray;
        separator.DrawShape = DrawShape.Circle;

        Circle circle = separator.Circle;
        circle.Radius = 7f;
        separator.Circle = circle;

        RectTransform separatorRect = separatorObject.GetComponent<RectTransform>();
        separatorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 14f);
        separatorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 14f);

        UnityEngine.UI.LayoutElement separatorLayout = separatorObject.AddComponent<UnityEngine.UI.LayoutElement>();

        separatorLayout.preferredWidth = 30f;
        separatorLayout.minWidth = 30f;
        separatorLayout.flexibleWidth = 0f;
    }

    private const float AllCircleSize = 14f;
    private const float AllCircleLeft = 55f;
    private const float AllCircleSpacing = 8f;
    private void CreateAllCircle(Toggle toggle)
    {
        GameObject circleObject = new GameObject("AllCircle");
        circleObject.transform.SetParent(toggle.transform, false);

        MPImage circleImage = circleObject.AddComponent<MPImage>();
        circleImage.color = Color.white;
        circleImage.DrawShape = DrawShape.Circle;

        Circle circle = circleImage.Circle;
        circle.Radius = AllCircleSize / 2f;
        circleImage.Circle = circle;

        RectTransform circleRect = circleObject.GetComponent<RectTransform>();

        circleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, AllCircleSize);

        circleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, AllCircleSize);

        circleRect.anchorMin = new Vector2(0f, 0.5f);
        circleRect.anchorMax = new Vector2(0f, 0.5f);
        circleRect.pivot = new Vector2(0f, 0.5f);

        circleRect.anchoredPosition = new Vector2(AllCircleLeft, 0f);
    }
}