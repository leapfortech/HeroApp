
using System;
using System.Collections.Generic;
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

    [NonSerialized]
    public bool IsLoading;
    [NonSerialized]
    public bool HasMore = true;

    [NonSerialized]
    public String PrevCursor;

    [NonSerialized]
    public String NextCursor;

    [NonSerialized]
    public List<PostFull> PostFulls;
    [NonSerialized]
    public HashSet<long> PostIds;

    public void ResetRuntime()
    {
        IsLoading = false;
        HasMore = true;

        PrevCursor = null;
        NextCursor = null;

        PostFulls = new List<PostFull>();
        PostIds = new HashSet<long>();
    }
}