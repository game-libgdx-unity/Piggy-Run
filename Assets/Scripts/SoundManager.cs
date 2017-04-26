using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement; 

public class SoundManager : MonoBehaviour
{

    public AudioClip[] backgroundMusics;
    public AudioClip[] pickCoinSFX;
    public float[] clipPauseTimes;
    public AudioSource player;
    public AudioSource pickCoinPlayer;
    public AudioSource perfectSFX;
    public AudioSource cartoonPlayer;
    public AudioSource losePlayer;

    public int maxIndex;
    public int currentIndex = 0;
    private void PlayPerfectMusic()
    {
        if(GameStateManager.Instance.SoundEffect == 0)
        {
            return;
        }

        if (perfectSoundReady)
        {
            perfectSoundReady = false;
            DOVirtual.DelayedCall(10f, () => perfectSoundReady = true);

            DOVirtual.DelayedCall(.4f, () =>
            {
                cartoonPlayer.Pause();
                player.Pause();
            });

            perfectSFX.Play();
            DOVirtual.DelayedCall(.8f, () => player.UnPause());
        }
    }
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        cartoonPlayer.Pause();
        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Start()
    {
        player = GetComponent<AudioSource>();
        clipPauseTimes = new float[backgroundMusics.Length];
        maxIndex = backgroundMusics.Length - 1;
        PlayBackgroundMusic(false);
        perfectSoundReady = true;
    }

    public void PlayEffectMusic(float duration = .7f)
    {
        if (GameStateManager.Instance.SoundEffect == 0)
        {
            return;
        }
        cartoonPlayer.UnPause();
        DOVirtual.DelayedCall(duration, () => { cartoonPlayer.Pause(); });
    }

    public void PlayLoseMusic()
    {
        if (GameStateManager.Instance.SoundEffect == 0)
        {
            return;
        }
        player.Pause();
        pickCoinPlayer.Pause();
        cartoonPlayer.Pause();
        losePlayer.Play();
        DOVirtual.DelayedCall(2f, () =>
        {
            player.UnPause();
            pickCoinPlayer.UnPause();
        });
    }

    public void PlayPickCoinMusic()
    {
        if (GameStateManager.Instance.SoundEffect == 0)
        {
            return;
        }
        print("Play pickcoin");
        int random = Random.Range(0, pickCoinSFX.Length);
        pickCoinPlayer.clip = pickCoinSFX[random];
        pickCoinPlayer.Play();
    }

    public void PlayBackgroundMusic(bool changeMusic = false)
    {
        if (GameStateManager.Instance.backgroundMusic == 0)
        {
            return;
        }
        if (changeMusic)
        {
            StopBackgroundMusic(changeMusic);
        }
        player.clip = backgroundMusics[currentIndex];
        player.time = clipPauseTimes[currentIndex];
        player.Play();
    }

    public void StopBackgroundMusic(bool changeMusic = false)
    {
        if (changeMusic)
        {
            clipPauseTimes[currentIndex] = player.time;
            currentIndex++;
            if (currentIndex > maxIndex)
                currentIndex = 0;
        }
        player.Stop();
    }

    public static SoundManager Instance;
    public static bool perfectSoundReady = true;

    public static void PlayPerfectSFX() { if (Instance) Instance.PlayPerfectMusic(); }
    public static void PlayPickCoinSFX() { if (Instance) Instance.PlayPickCoinMusic(); }
    public static void PlayEffectSFX(float duration = .7f) { if (Instance) Instance.PlayEffectMusic(duration); }
    public static void PlayLoseSFX() { if (Instance) Instance.PlayLoseMusic(); }
    public static void PlayBGMusic(bool changeMusic)
    {
        if (Instance)
        {
            Instance.PlayBackgroundMusic(changeMusic);
        }
        else
        {
            Debug.Log("Singleton missing!!!");
        }
    }

    public static void StopBGMusic(bool changeMusic)
    {
        if (Instance)
        {
            Instance.StopBackgroundMusic(changeMusic);
        }
        else
        {
            Debug.Log("Singleton missing!!!");
        }
    }
}
