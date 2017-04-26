using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;


public class FacebookHandle : MonoBehaviour
{
    public FacebookUI facebookUI;
    private static FacebookHandle instance;
    public static FacebookHandle Instance
    {
        get
        {
            if (!instance)
            {
                GameObject container = new GameObject("FacebookHandle");
                container.AddComponent<FacebookHandle>();
                DontDestroyOnLoad(container);
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
            InitializeFacebook();
        }

    }

    private void InitializeFacebook()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void FBLogin()
    { 
        var perms = new List<string>() { "public_profile", "email", "user_friends"};
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void CallFBLoginForLeaderboard(FacebookDelegate<ILoginResult> result)
    { 
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, result);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            if (SceneManager.GetActiveScene().name == "start")
                FBGraph.GetPlayerInfo();
            else if (TO_DO == WhatToDo.Invite)
                FBGraph.GetFriends();
            else if (TO_DO == WhatToDo.Share)
                FacebookHandle.ShareLink();
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public static void ShareLink()
    {
        if (FB.IsLoggedIn)
        {
            FB.ShareLink(
            new Uri("https://play.google.com/store/apps/details?id=com.vinh.tap"),
            "Pet Up Up!!!",
            "I'm playing this great game!",
            callback: ShareCallback
             );
        }
        else
        {
            FacebookHandle.Instance.FBLogin();
            TO_DO = WhatToDo.Share;
        }
    }
    public enum WhatToDo { Nothing, Share, Invite }
    public static WhatToDo TO_DO = WhatToDo.Nothing;



    private static void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = MapGenerator.CurrentTimeScale;
        }
    }
}
