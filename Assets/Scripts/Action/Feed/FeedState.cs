using System;
//using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leap/App/Feed State")]
public class FeedState : ScriptableObject
{
    [Header("Key")]
    public String FeedKey;

    [Header("Config")]
    public int Count = 10;
    public long PostTypeId = -1;
    public int Status = 1;

    //[NonSerialized]
    //public List<PostFull> PostFulls = new List<PostFull>();
}