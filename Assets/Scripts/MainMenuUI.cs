using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Text txtConvertResult;
    public CanvasGroup shopUI;
    
    public void Log(string s)
    {
        print(s);
    }
    public void OnBtnShopClicked()
    {
        shopUI.gameObject.SetActive(true);
        OnShopUIShowed();
        shopUI.DOFade(1f, 1f);
    }

    public void OnBtnRateUsClicked()
    {
        Application.OpenURL("market://details?id=com.vinh.tap");
    }

    private void OnShopUIShowed()
    {
        shopUI.interactable = true;
        int diamond = GameStateManager.Instance.DiamondCount;

        if (diamond < 1000)
        {
            txtConvertResult.text = "Sorry you don't have enough diamonds, please get at least 1000 diamonds, Login with facebook will give you 2000 diamonds for free :)";
        }
        else
        {
            int auto = diamond / 1000;
            int diamondRemaining = diamond - 1000 * auto;

            txtConvertResult.text = "Congratulation! You had " + diamond + " Diamonds, that have been converted to " + auto + " Auto Run!";

            if (auto == 1) auto = 2;

            GameStateManager.Instance.itemAutoCount += auto;
            GameStateManager.Instance.DiamondCount = diamondRemaining;
            GameStateManager.Instance.Save();
        }

    }

    public void OnBtnShopReturnToGameClicked()
    {
        shopUI.DOFade(0f, .5f).OnComplete(() =>
        {
            shopUI.interactable = false;
            shopUI.gameObject.SetActive(false);
        });
    }
}
