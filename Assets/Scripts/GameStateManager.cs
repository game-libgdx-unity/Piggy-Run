using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;
using System;

public class GameStateManager : MonoBehaviour
{
    //save-able data
    public int DiamondCount = 0;
    public int itemAutoCount = 0;
    public int bestStage = 1;
    public int getDiamondFromFB = 0;
    public int backgroundMusic = 1;
    public int SoundEffect = 1;
    public int pickCoinSound = 1;

    //   Singleton 
    private static GameStateManager instance;
    public static GameStateManager Instance
    {
        get
        {
            if (!instance)
            {
                GameObject container = new GameObject("GameStateManager");
                instance = container.AddComponent<GameStateManager>();
            } 
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
            return;
        }
    }

    void Start()
    {
        DiamondCount = PlayerPrefs.GetInt("DiamondCount", 1000);
        itemAutoCount = PlayerPrefs.GetInt("itemAutoCount", 5);
        bestStage = PlayerPrefs.GetInt("bestStage", 1);
        getDiamondFromFB = PlayerPrefs.GetInt("getDiamondFromFB", 0);
        backgroundMusic = PlayerPrefs.GetInt("backgroundMusic", 1);
        SoundEffect = PlayerPrefs.GetInt("SoundEffect", 1);
        pickCoinSound = PlayerPrefs.GetInt("pickCoinSound", 1);
    }

    void OnDestroy()
    {
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("DiamondCount", DiamondCount);
        PlayerPrefs.SetInt("itemAutoCount", itemAutoCount);
        PlayerPrefs.SetInt("bestStage", bestStage);
        PlayerPrefs.SetInt("getDiamondFromFB", getDiamondFromFB);
        PlayerPrefs.SetInt("backgroundMusic", backgroundMusic);
        PlayerPrefs.SetInt("SoundEffect", SoundEffect);
        PlayerPrefs.SetInt("pickCoinSound", pickCoinSound);
        PlayerPrefs.Save();
    }

    //   Game State   // 
    private int? highScore;
    public static int HighScore
    {
        get { return Instance.highScore.HasValue ? Instance.highScore.Value : 0; }
        set { Instance.highScore = value; }
    }
    public List<object> Friends;
    public Dictionary<string, Texture> FriendImages = new Dictionary<string, Texture>();
    public List<object> InvitableFriends = new List<object>();
    // Scores
    public static bool ScoresReady;
    private static List<object> scores;
    public static List<object> Scores
    {
        get { return scores; }
        set { scores = value; ScoresReady = true; }
    }


    public static void CallUIRedraw()
    {
    }
}
