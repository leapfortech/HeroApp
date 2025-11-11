using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    [SerializeField, Space, Range(0.1f, 20f)]
    private float thickness = 6f;

    [SerializeField, Space, Range(3f, 90f)]
    private float capAngle = 36f;

    [SerializeField]
    private UILineCaps capDisplay = UILineCaps.Both;

    [SerializeField, Space]
    public List<Vector2> points = new List<Vector2>();

    float th = 3f;

    public float Thickness
    {
        get => thickness;
        set
        {
            thickness = value;
            SetAllDirty();
        }
    }

    public float CapAngle
    {
        get => capAngle;
        set
        {
            capAngle = value;
            SetAllDirty();
        }
    }

    public UILineCaps CapDisplay
    {
        get => capDisplay;
        set
        {
            capDisplay = value;
            SetAllDirty();
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            color = value;
            SetAllDirty();
        }
    }

    public void SetPoints(List<Vector2> points)
    {
        this.points = points;
        SetAllDirty();
    }

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
        SetAllDirty();
    }

    public void Init(Vector2 point, float thickness, float capAngle, UILineCaps capDisplay, Color color)
    {
        points = new List<Vector2>() { point };
        this.thickness = thickness;
        this.capAngle = capAngle;
        this.capDisplay = capDisplay;
        this.color = color;
        SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Count < 2)
            return;

        Vector2 offset = new Vector2(-rectTransform.pivot.x * rectTransform.rect.width, -rectTransform.pivot.y * rectTransform.rect.height);
        th = thickness * 0.5f;


        if (points.Count == 2)
        {
            AddSegment(vh, points[0] + offset, points[1] + offset, (Vector2.zero, Vector2.zero), 3);
            return;
        }

        (Vector2 p21, Vector2 p22) prv = AddSegment(vh, points[0] + offset, points[1] + offset, (Vector2.zero, Vector2.zero), 0);

        for (int i = 1; i < points.Count - 2; i++)
            prv = AddSegment(vh, points[i] + offset, points[i + 1] + offset, prv, 1);

        AddSegment(vh, points[points.Count - 2] + offset, points[points.Count - 1] + offset, prv, 2);
    }

    private (Vector2 p21, Vector2 p22) AddSegment(VertexHelper vh, Vector2 p1, Vector2 p2, (Vector2 p21, Vector2 p22) prv, int capType)
    {
        Vector2 n = (p2 - p1).normalized;
        n = new Vector2(-n.y, n.x);

        Vector2 p12 = p1 + n * th;
        Vector2 p21 = p2 + n * th;
        Vector2 p22 = p2 - n * th;

        vh.AddUIVertexQuad(SetVertex(p1 - n * th, p12, p21, p22));

        if (capType == 0 || capType == 3)
        {
            if (capDisplay == UILineCaps.Start || capDisplay == UILineCaps.Both)
                AddCap(vh, p1, p12 - p1, 180f);

            if (capType == 3 && (capDisplay == UILineCaps.End || capDisplay == UILineCaps.Both))
                AddCap(vh, p2, p22 - p2, 180f);

            return (p21, p22);
        }

        Vector2 v12 = p12 - p1;

        float a = Vector2.SignedAngle(v12, prv.p21 - p1);
        if (a >= 0f)
            AddCap(vh, p1, v12, a);
        else
            AddCap(vh, p1, prv.p22 - p1, -a);

        if (capType == 2 && (capDisplay == UILineCaps.End || capDisplay == UILineCaps.Both))
            AddCap(vh, p2, p22 - p2, 180f);

        return (p21, p22);
    }

    private void AddCap(VertexHelper vh, Vector2 c, Vector2 v, float angle)
    {
        int cIdx = vh.currentVertCount;
        vh.AddVert(c, color, Vector4.zero);

        int n = (int)(angle / capAngle);
        if (Mathf.Abs(n * capAngle - angle) < 0.001f)
            n--;

        int p0Idx = vh.currentVertCount;
        vh.AddVert(c + v, color, Vector4.zero);

        Vector2 p;
        for (int i = 1; i <= n; i++)
        {
            p = Quaternion.Euler(Vector3.forward * capAngle * i) * v;
            vh.AddVert(c + p, color, Vector4.zero);
            vh.AddTriangle(cIdx, p0Idx + i, p0Idx + i - 1);
        }

        p = Quaternion.Euler(Vector3.forward * angle) * v;
        vh.AddVert(c + p, color, Vector4.zero);
        vh.AddTriangle(cIdx, p0Idx + n + 1, p0Idx + n);
    }

    protected UIVertex[] SetVertex(params Vector2[] vertices)
    {
        UIVertex[] vtx = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            UIVertex vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = vertices[i];
            //vert.uv0 = uvs[i];
            vtx[i] = vert;
        }
        return vtx;
    }
}

public enum UILineCaps { None, Start, End, Both }