using System;
using UnityEngine;

using Leap.UI.Extensions;

using Sirenix.OdinInspector;


public class DateTimeDefaultIndexAction : MonoBehaviour
{
    [Title("Elements")]
    [SerializeField]
    String year = null;

    public int[] GetDateIndexes()
    {
        return new int[] { DateTime.Today.Day - 1, DateTime.Today.Month - 1, DateTime.Today.Year - Convert.ToInt32(year) };
    }

    public int[] GetTimeIndexes()
    {
        return new int[] { DateTime.Now.Hour, DateTime.Now.Minute };
    }
}
