using System.Collections;
using UnityEngine;
using Leap.Core.Tools;
#if !UNITY_EDITOR && UNITY_ANDROID
using UnityEngine.Android;
#endif

public class LocationManager : SingletonBehaviour<LocationManager>
{
	private void Awake()
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
			Permission.RequestUserPermission(Permission.FineLocation);
#endif
	}

/*
	private void Start()
	{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
		StartCoroutine(StartLocationService());
#endif
	}
*/
	private IEnumerator StartLocationService()
	{
		if (!Input.location.isEnabledByUser)
		{
			Debug.Log("GPS User has not enabled location");
			yield break;
		}

		Input.location.Start();

		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		if (maxWait < 1)
		{
			Debug.Log("GPS Timed out");
			yield break;
		}

		if (Input.location.status == LocationServiceStatus.Failed)
		{
			Debug.Log("GPS Unable to determin device location");
			yield break;
		}

        Debug.Log("GPS Status: " + Input.location.status.ToString());
    }

	private void StopLocationService()
	{
        Input.location.Stop();
    }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
	public Vector3 GetInfo()
	{
		StartCoroutine(StartLocationService());

		if (Input.location.status != LocationServiceStatus.Running)
		{
			Debug.Log("GPS Stopped");
			return new Vector3(0.000000f, 0.000000f, 0.000000f);
		}

		Vector3 position = new Vector3(Input.location.lastData.longitude, Input.location.lastData.latitude, Input.location.lastData.altitude);
		StopLocationService();

		return position;
	}
#else
    public Vector3 GetInfo()
	{
		return Vector3.zero;
	}
#endif
}