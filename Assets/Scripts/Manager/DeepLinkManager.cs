using System;
using UnityEngine;

using Leap.Core.Tools;

public class DeepLinkManager : SingletonBehaviour<AppManager>
{
    private void Awake()
    {
        // 1. Handle Cold Start (App was closed)
        ProcessDeepLink(Application.absoluteURL);

        // 2. Handle Warm Start (App was running in background)
        Application.deepLinkActivated += OnDeepLinkActivated;
    }

    private void OnDestroy()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }

    private void OnDeepLinkActivated(String url)
    {
        // Must process on the main thread
        ProcessDeepLink(url);
    }

    private void ProcessDeepLink(String url)
    {
        if (String.IsNullOrEmpty(url) || url == "about:blank") return;

        Debug.Log("Received Deep Link URL: " + url);

        // Example URL format: mygame://open?room=12345
        try
        {
            // Split parameters if your game relies on query variables
            String[] splitUrl = url.Split('?');
            if (splitUrl.Length > 1)
            {
                String queryString = splitUrl[1]; // room=12345
                ParseAndExecuteParameters(queryString);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse deep link: " + e.Message);
        }
    }

    private void ParseAndExecuteParameters(String queryString)
    {
        String[] parameters = queryString.Split('&');
        foreach (String param in parameters)
        {
            String[] kvp = param.Split('=');
            if (kvp.Length == 2 && kvp[0] == "room")
            {
                String roomId = kvp[1];
                Debug.Log($"Joining room: {roomId}");
                // Execute logic, e.g., PlayerPrefs.SetString("JoinRoom", roomId); 
                // Then trigger your scene transition or network join logic.
            }
        }
    }
}
