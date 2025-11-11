using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Sirenix.OdinInspector;

public class UILineDrawer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
#if UNITY_EDITOR
    [Title("Line"), LabelText("UILine Prefab")]
#endif
    [SerializeField]
    GameObject uiLinePrefab;

    [SerializeField]
#if UNITY_EDITOR
    [Space, Range(3f, 90f), OnValueChanged(nameof(ChangeCapAngle))]
#endif
    float capAngle = 36f;

    [SerializeField]
#if UNITY_EDITOR
    [OnValueChanged(nameof(ChangeCapDisplay))]
#endif
    private UILineCaps capDisplay = UILineCaps.Both;

    [Title("Pen")]
    [SerializeField]
#if UNITY_EDITOR
    [OnValueChanged(nameof(ChangeColor))]
#endif
    Color32 color = Color.red;

    [SerializeField]
#if UNITY_EDITOR
    [Range(1f, 20f), OnValueChanged(nameof(ChangeThickness))]
#endif
    float thickness = 6f;

    [Title("Drawing")]
    [SerializeField]
    bool on = true;

    [SerializeField]
    float minDistance = 4f;

    RectTransform rectTransform;

    UILineRenderer lineRdr = null;
    Vector2 prvDrawPos;
    Vector2 BL, BR, TL, TR;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        BL = Vector2.zero;
        BR = new Vector2(rectTransform.rect.width, 0f);
        TL = new Vector2(0f, rectTransform.rect.height);
        TR = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        prvDrawPos = Vector2.zero;
    }

#if UNITY_EDITOR
    private void ChangeThickness()
    {
        UILineRenderer[] lineRdrs = GetComponentsInChildren<UILineRenderer>();
        for (int i = 0; i < lineRdrs.Length; i++)
            lineRdrs[i].Thickness = thickness;
    }

    private void ChangeCapAngle()
    {
        UILineRenderer[] lineRdrs = GetComponentsInChildren<UILineRenderer>();
        for (int i = 0; i < lineRdrs.Length; i++)
            lineRdrs[i].CapAngle = capAngle;
    }

    private void ChangeCapDisplay()
    {
        UILineRenderer[] lineRdrs = GetComponentsInChildren<UILineRenderer>();
        for (int i = 0; i < lineRdrs.Length; i++)
            lineRdrs[i].CapDisplay = capDisplay;
    }

    private void ChangeColor()
    {
        UILineRenderer[] lineRdrs = GetComponentsInChildren<UILineRenderer>();
        for (int i = 0; i < lineRdrs.Length; i++)
            lineRdrs[i].Color = color;
    }
#endif

    private void StartLine(Vector2 drawPos)
    {
        lineRdr = Instantiate(uiLinePrefab, rectTransform).GetComponent<UILineRenderer>();
        lineRdr.Init(drawPos, thickness, capAngle, capDisplay, color);

        prvDrawPos = drawPos;
    }

    private void AddLinePoint(Vector2 drawPos)
    {
        lineRdr.AddPoint(drawPos);
        prvDrawPos = drawPos;
    }

    public bool IsEmtpy()
    {
        return rectTransform.childCount == 0;
    }

    public void Undo()
    {
        if (IsEmtpy())
            return;

        Transform trf = transform.GetChild(rectTransform.childCount - 1);
        trf.SetParent(null);
        Destroy(trf.gameObject);
    }

    public void Clear()
    {
        if (rectTransform == null)
            return;

        Transform trf;

        while (rectTransform.childCount > 0)
        {
            trf = transform.GetChild(0);
            trf.SetParent(null);
            Destroy(trf.gameObject);
        }

        lineRdr = null;
    }

    // Drag

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!on)
            return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 drawPos))
            return;

        StartLine(drawPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!on)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 drawPos) ||
            drawPos.x < 0f || drawPos.x >= rectTransform.rect.width || drawPos.y < 0f || drawPos.y >= rectTransform.rect.height)
        {
            if (lineRdr != null)
            {
                AddLinePoint(IxSegmentBBox(drawPos, prvDrawPos));
                lineRdr = null;
            }

            prvDrawPos = drawPos;
            return;
        }

        if (lineRdr == null)
        {
            StartLine(IxSegmentBBox(prvDrawPos, drawPos));
            AddLinePoint(drawPos);
        }
        else if (Vector2.Distance(drawPos, prvDrawPos) >= minDistance)
        {
            AddLinePoint(drawPos);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lineRdr = null;
    }

    private Vector2 IxSegmentBBox(Vector2 S1, Vector2 S2)
    {
        if (!Vector2Ext.IxSegmentBBox(S1, S2, BL, TR, out Vector2 P))
            return Vector2.zero;
        return P;
    }

    // Strokes

    public void CreateLines(String strokes)
    {
        Clear();

        byte[] byteStrokes = Convert.FromBase64String(strokes);

        float[] floatStrokes = new float[byteStrokes.Length / 4];
        Buffer.BlockCopy(byteStrokes, 0, floatStrokes, 0, byteStrokes.Length);

        int f = 0;
        int lineRdrLength = (int)floatStrokes[f];
        Vector2 pos = new Vector2(0f, 0f);
        for (int i = 0; i < lineRdrLength; i++)
        {
            int positionCount = (int)floatStrokes[++f];

            pos.x = floatStrokes[++f];
            pos.y = floatStrokes[++f];
            StartLine(pos);

            for (int k = 1; k < positionCount; k++)
            {
                pos.x = floatStrokes[++f];
                pos.y = floatStrokes[++f];
                AddLinePoint(pos);
            }
        }
    }

    public String GetStrokes()
    {
        UILineRenderer[] lineRdrs = GetComponentsInChildren<UILineRenderer>();

        List<float> strokes = new List<float> { lineRdrs.Length };
        for (int i = 0; i < lineRdrs.Length; i++)
        {
            strokes.Add(lineRdrs[i].points.Count);
            for (int k = 0; k < lineRdrs[i].points.Count; k++)
            {
                strokes.Add(lineRdrs[i].points[k].x);
                strokes.Add(lineRdrs[i].points[k].y);
            }
        }

        float[] floatStrokes = strokes.ToArray();

        byte[] byteStrokes = new byte[floatStrokes.Length * 4];
        Buffer.BlockCopy(floatStrokes, 0, byteStrokes, 0, byteStrokes.Length);

        return Convert.ToBase64String(byteStrokes);
    }
}
