using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.Events;


public class UIManager : MonoBehaviour
{
    private const float RESTORE_AUTORUN_ITEM_AFTER = 5f;
    int diamondGot = 0;
    float time;

    public GameObject GeneralUI;
    public Text txtScore;
    public Text txtDistance;
    public Text txtItemAUtoCount;
    public Text txtAutoRunMessage;
    public Image imgItemAuto;
    [Space]
    public GameObject ReplayUI;
    public Text txtDistance_ReplayUI;
    public Text txtTotalDiamond_ReplayUI;
    public GameObject center_ReplayUI;
    public GameObject bottom_ReplayUI;
    public Text txtBestStage;
    public Image background_ReplayUI;
    [Space]
    public GameObject friendListUI;
    [Space]
    public Text txtOverlay;
    Tween overlay;

    public bool showHelp = true;
    public bool showHelp2 = true;
    private float lastTimeScale;

    void Start()
    {
        DiamondGot = 0;
        txtAutoRunMessage.DOFade(0f, 0f);
        SetImgAutoItem();
        SoundManager.perfectSoundReady = true;
        if (GameStateManager.Instance.itemAutoCount < 1)
            GameStateManager.Instance.itemAutoCount = 1;
        SetImgAutoItem();
    }

    private int GetItemAutoCount()
    {
        return GameStateManager.Instance.itemAutoCount;
    }
    private void SetImgAutoItem()
    {
        GameStateManager.Instance.itemAutoCount.ToString();
        if (GameStateManager.Instance.itemAutoCount <= 0)
        {
            GameStateManager.Instance.itemAutoCount = 0;
            imgItemAuto.DOFade(.33f, 0f);
        }
        txtItemAUtoCount.text = GetItemAutoCount().ToString();
    }
    private void DecreaseAutoItemCount()
    {
        GameStateManager.Instance.itemAutoCount--;
        SetImgAutoItem();
    }
    public static float SPEED_DASH = 1f;
    public void OnBtnDashDown()
    {
        Time.timeScale = 2f;
    }
    public void OnBtnDashUp()
    {
        Time.timeScale = 1f;
    }

    public void OnBtnSettingClicked()
    {
        CommonUI.ShowOptionPanel();
    }

    public void OnBtnSpaceClicked()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = lastTimeScale;
            return;
        }

        if (showHelp2)
        {
            DOVirtual.DelayedCall(.85f, () => ShowOverlayMessage("Tap to control!", 2f));
            showHelp2 = false;
        }

        if (OnScreenTouched != null)
            OnScreenTouched();
    }
    public void OnBtnAutoClicked()
    {
        if (GameStateManager.Instance.itemAutoCount <= 0)
        {
            GameStateManager.Instance.itemAutoCount = 0;
            SetImgAutoItem();
            return;
        }
        DecreaseAutoItemCount();
        ShowOverlayMessage("Auto Run Started", 2f);
        ShowOverlayAutoRunMessage();

        if (OnBtnAutoTouched != null)
            OnBtnAutoTouched();
    }

    public static Tweener AutoRunTween;
    private void ShowOverlayAutoRunMessage()
    {
        imgItemAuto.GetComponent<RectTransform>().DOAnchorPosY(-120, 1f);
        txtAutoRunMessage.GetComponent<RectTransform>().DOAnchorPosY(-373, 1f);
        PlayerController.autoRun = true;
        txtAutoRunMessage.DOFade(1f, .6f);
        AutoRunTween = txtAutoRunMessage.transform.DOLocalRotate(new Vector3(0, 0, 10), 1f).SetLoops(1000, LoopType.Yoyo)
            .OnComplete(() =>
            {
                ShowOverlayMessage("Auto Run Mode Stopped", 3f);
                PlayerController.autoRun = false;

                txtAutoRunMessage.GetComponent<RectTransform>().DOAnchorPosY(-275, 1f);

                DOVirtual.DelayedCall(RESTORE_AUTORUN_ITEM_AFTER, () => imgItemAuto.GetComponent<RectTransform>().DOAnchorPosY(0, 1f));


                txtAutoRunMessage.transform.DOLocalRotate(new Vector3(0, 0, 0), .4f);
                txtAutoRunMessage.DOFade(0f, .6f);
            });
    }

    public void OnBtnSpeedClicked()
    {
        if (OnBtnSpeedTouched != null)
            OnBtnSpeedTouched();
    }
    public void OnBtnSlowClicked()
    {
        if (OnBtnSlowTouched != null)
            OnBtnSlowTouched();
    }
    public void OnBtnPauseClicked()
    {
        if (Time.timeScale != 0)
        {
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
            Time.timeScale = lastTimeScale;
    }

    public static Action OnScreenTouched;
    public static Action OnBtnAutoTouched;
    public static Action OnBtnSlowTouched;
    public static Action OnBtnSpeedTouched;


    public int DiamondGot { get { return diamondGot; } set { diamondGot = value; txtScore.text = diamondGot.ToString(); } }

    public void OnBtnBackClicked()
    {
        ClearBeforeStartMainScene();
        SceneManager.LoadScene("start");
        SoundManager.PlayBGMusic(true);
    }

    public void OnBtnFacebookClicked()
    {
        FacebookHandle.TO_DO = FacebookHandle.WhatToDo.Share;
        if (FB.IsLoggedIn)
            FacebookHandle.ShareLink();
        else
            FacebookHandle.Instance.FBLogin();

    }

    public void ShowAchi()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
    }

    public void OnBtnInviteClicked()
    {
        FBGraph.Invite();
    }

    public void OnBtnRankingClicked()
    {
        FacebookHandle.TO_DO = FacebookHandle.WhatToDo.Invite;
        if (FB.IsLoggedIn)
        {
            friendListUI.SetActive(true); FBGraph.GetFriends();

            if (FBGraph.HavePublishActions)
            {
                Debug.Log("Success - Check log for details");
                FBGraph.PostScore(MapGenerator.CurrentLevel);
            }
            else
                FacebookHandle.Instance.CallFBLoginForLeaderboard(result =>
                {
                    if (!string.IsNullOrEmpty(result.RawResult))
                    {
                        Debug.Log("Success - Check log for details");
                        FBGraph.PostScore(MapGenerator.CurrentLevel);
                    }
                });
        }
        else
            FacebookHandle.Instance.FBLogin();


    }

    public void OnBtnReplayClicked()
    {
        ClearBeforeStartMainScene();
        center_ReplayUI.transform.DOLocalMoveX(-600, 1f);
        bottom_ReplayUI.transform.DOLocalMoveY(-200, 1f);
        background_ReplayUI.DOFade(0f, 1.1f);
        DOVirtual.DelayedCall(1.1f, () =>
        {
            SceneManager.LoadScene("main");
        });
    }

    void Update()
    {
        if (showHelp && PlayerController.running)
        {
            overlay.Kill();
            overlay = null;
            txtOverlay.DOFade(0f, 0f);
            showHelp = false;
        }
    }
    // Use this for initialization
    void OnEnable()
    {
        PlayerController.OnDiamondPicked += OnDiamondPicked;
        PlayerController.OnPlayerLost += OnPlayerLost;
        PlayerController.OnPerfectMove += OnPerfectMove;
        FBGraph.OnFriendListFetched += OnFriendListFetched;
        MapGenerator.OnNewLevelLoaded += IncreaseLevel;
        GeneralUI.SetActive(true);
        ReplayUI.SetActive(false);
        if (showHelp)
        {
            overlay = txtOverlay.DOFade(1f, 1f).SetLoops(4, LoopType.Yoyo);
        }
    }
    void OnDisable()
    {
        PlayerController.OnPerfectMove -= OnPerfectMove;
        PlayerController.OnDiamondPicked -= OnDiamondPicked;
        FBGraph.OnFriendListFetched -= OnFriendListFetched;
        MapGenerator.OnNewLevelLoaded -= IncreaseLevel;
        PlayerController.OnPlayerLost -= OnPlayerLost;
    }

    private void OnPerfectMove()
    {
        if (PlayerController.running)
        {
            SoundManager.PlayPerfectSFX();
            ShowOverlayMessage("Perfect Move!\n+10 Diamonds");
            DiamondGot += 10;
        }
    }

    private void ShowOverlayMessage(string message, float timeScale = 1f, Action OnFadedOut = null)
    {
        if (overlay != null)
        {
            overlay.Kill();
            overlay = null;
        }
        overlay = txtOverlay.DOFade(0f, 0f);
        txtOverlay.text = message;
        Color textColor = new Color();
        textColor.r = CameraController.currentColor.g;
        textColor.g = CameraController.currentColor.b;
        textColor.b = CameraController.currentColor.r;
        txtOverlay.color = textColor;
        txtOverlay.DOComplete();
        overlay = txtOverlay.DOFade(1f, .35f);
        overlay = DOVirtual.DelayedCall(.6f * timeScale, () =>
        {
            overlay = txtOverlay.DOFade(0f, .35f).OnComplete(() => { if (OnFadedOut != null) OnFadedOut(); overlay = null; });
        });
    }

    private void OnFriendListFetched(List<object> obj)
    {
        print("Friends: " + obj.Count);
        friendListUI.SetActive(true);
    }

    void IncreaseLevel(int level)
    {
        txtDistance.text = "S " + level;
    }
    // Update is called once per frame
    void OnDiamondPicked(Path path)
    {
        DiamondGot++;
        SoundManager.PlayPickCoinSFX();
    }

    public static void ClearBeforeStartMainScene()
    {
        PlayerController.running = false;
        MapGenerator.Maps.Clear();
        DOTween.Clear();
        DOTween.ClearCachedTweens();
    }

    void OnPlayerLost()
    {
        AutoRunTween.Complete();
        GeneralUI.SetActive(false);
        ReplayUI.SetActive(true);
        int currentStage = MapGenerator.CurrentLevel - 1;
        txtDistance_ReplayUI.text = "Stage: " + currentStage;
        if (currentStage > GameStateManager.Instance.bestStage)
        {
            GameStateManager.Instance.bestStage = currentStage;
        }
        txtBestStage.text = "Best: " + GameStateManager.Instance.bestStage;
        background_ReplayUI.DOFade(0.5f, 1.0f);
        center_ReplayUI.transform.DOLocalMoveX(0, 1f);
        bottom_ReplayUI.transform.DOLocalMoveY(0, 1f);
        GameStateManager.Instance.DiamondCount += DiamondGot;
        txtTotalDiamond_ReplayUI.text = GameStateManager.Instance.DiamondCount.ToString();
        //save stages permaturely
        PlayerPrefs.SetInt("CurrentLevel", currentStage);
        PlayerPrefs.Save();
        MapGenerator.CurrentLevel = currentStage;

        //UIManager.OnScreenTouched = null;
        //UIManager.OnBtnAutoTouched = null;
        //MapGenerator.OnNewLevelLoaded = null;
        //PlayerController.OnCameraRotated = null;
        //PlayerController.OnPerfectMove = null;
        //PlayerController.OnPlayerLost = null;
        //PlayerController.OnDiamondPicked = null;
    }

    public void Log(string content)
    {
        print(content);
    }
}
