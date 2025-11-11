using System;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DpiFrontParams", menuName = "Leap/Data/DpiFront Params")]
public class DpiFrontParams : ScriptableObject
{
    [Title("Mesures")]
    public Vector2 Rep = new Vector2(5.8f, 1.4f);
    [SerializeField]
    public Vector2 Date = new Vector2(3.9f, 1.2f);

    [Title("Thresholds")]
    public Vector2Int Distance = new Vector2Int(480, 580);

    [Title("Positions")]
    public Vector3 B = new Vector3(7.9f, 5.8f, 1f);

    [Space]
    public Vector2 Dpi = new Vector2(1.5f, 0.15f);

    [Space]
    public Vector3 M1 = new Vector3(0.5f, 0.6f, 1.65f);
    public Vector3 M = new Vector3(5.0f, 3.9f, 3.95f);

    [Space]
    public Vector3 C12 = new Vector3(6.4f, 4.75f, 4.75f);
    public Vector2 C = new Vector4(5.5f, 3.9f);

    [Space]
    public Vector2 D = new Vector2(7.9f, 5.5f);

    [Space]
    public Vector2 Bg = new Vector2(3f, 1.7f);

    [Title("Sizes")]
    public Vector2Int CropSize = new Vector2Int(790, 500);
    public Vector2Int PhotoSize = new Vector2Int(225, 300);
    public Vector2Int PhotoSizeBig = new Vector2Int(246, 328);

    [Title("Portrait")]
    public Vector3 XLR = new Vector3(30f, 7.75f, 7.96f);
    public Vector3 YU = new Vector3(2.9f, 1.25f, 1f);
    public Vector3 YD = new Vector3(1.75f, 2.25f, 1.1f);
}
