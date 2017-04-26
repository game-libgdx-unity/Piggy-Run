using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CommonUI : MonoBehaviour
{
    public CanvasGroup OptionPanel;
    public Toggle soundEffect;

    public void SetBackgroundMusic(bool enabled)
    {
        GameStateManager.Instance.backgroundMusic = enabled ? 1 : 0;
        GameStateManager.Instance.Save();
        if (GameStateManager.Instance.backgroundMusic == 0)
            SoundManager.Instance.player.Pause();
        else
        {
            SoundManager.Instance.player.UnPause();
            if (!SoundManager.Instance.player.isPlaying)
                SoundManager.Instance.player.Play();
        }
    }
    public void SetEffectMusic(bool enabled) { GameStateManager.Instance.SoundEffect = soundEffect.isOn ? 1 : 0; GameStateManager.Instance.Save(); }
    public void SetPickCoinMusic(bool enabled) { GameStateManager.Instance.pickCoinSound = enabled ? 1 : 0; GameStateManager.Instance.Save(); }
    public void OnBtnOptionPanelCloseClicked() { _HideOptionPanel(); }
    public void OnBtnShowOptionPanelClicked() { _ShowOptionPanel(); }

    // Use this for initialization
    protected virtual void Initialize()
    {
        SharedInstance.OptionPanel.alpha = 0f;
        SharedInstance.OptionPanel.interactable = false;
        SharedInstance.OptionPanel.blocksRaycasts = false;
    }
    private void Awake()
    {
        if (SharedInstance == null)
        {
            DontDestroyOnLoad(this);
            SharedInstance = this;
            Initialize();
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    private void _ShowOptionPanel()
    {
        Toggle[] options = Resources.FindObjectsOfTypeAll<Toggle>();
        Debug.Log("Found: " + options.Length);
        options[0].isOn = GameStateManager.Instance.backgroundMusic == 1;
        options[1].isOn = GameStateManager.Instance.SoundEffect == 1;
        options[2].isOn = GameStateManager.Instance.pickCoinSound == 1;

        SharedInstance.OptionPanel.DOFade(1f, .4f).OnComplete(() =>
        {
            SharedInstance.OptionPanel.interactable = true;
            SharedInstance.OptionPanel.blocksRaycasts = true;
        });
    }
    private void _HideOptionPanel()
    {
        SharedInstance.OptionPanel.DOFade(0f, .4f).OnComplete(() =>
        {
            SharedInstance.OptionPanel.interactable = false;
            SharedInstance.OptionPanel.blocksRaycasts = false;
        });
    }

    public static CommonUI SharedInstance;
    public static void ShowOptionPanel()
    {
        if (SharedInstance != null)
        {
            SharedInstance._ShowOptionPanel();
        }
    }
    public static void HideOptionPanel()
    {
        if (SharedInstance != null)
        {
            SharedInstance._HideOptionPanel();
        }
    }
}
