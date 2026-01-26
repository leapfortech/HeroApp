
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "App/Feed State")]
public class FeedState : ScriptableObject
{
    [Header("Key")]
    public String FeedKey;

    [Header("Config")]
    public int PageSize = 10;
    public long PostSubtypeId = -1;
    public int Status = 1;

    [NonSerialized]
    public bool IsLoading;
    [NonSerialized]
    public bool HasMore = true;

    [NonSerialized]
    public DateTime? FirstPublicationDateTime;
    [NonSerialized]
    public long FirstPostId = -1;

    [NonSerialized]
    public DateTime? LastPublicationDateTime;
    [NonSerialized]
    public long LastPostId = -1;

    [NonSerialized]
    public List<PostFull> PostFulls;
    [NonSerialized]
    public HashSet<long> PostIds;

    public void ResetRuntime()
    {
        IsLoading = false;
        HasMore = true;

        FirstPublicationDateTime = null;
        FirstPostId = -1;
        LastPublicationDateTime = null;
        LastPostId = -1;

        PostFulls = new List<PostFull>();
        PostIds = new HashSet<long>();
    }
}