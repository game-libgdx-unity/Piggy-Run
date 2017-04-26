using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FacebookUI : MonoBehaviour
{

    public static Texture UserPicture;
    public static string UserName;
    public RawImage profilePic;
    public Text playerName;

    public static int ID;

    void UpdateUI()
    {
        if (UserPicture)
            profilePic.texture = UserPicture;

        playerName.text = UserName;

        GetComponent<RectTransform>().DOAnchorPosY(0f, 1f);
        RectTransform rectTransform = GetComponent<RectTransform>();
        DOVirtual.DelayedCall(4f, () => rectTransform.DOAnchorPosY(120f, 1f));

        if(GameStateManager.Instance.getDiamondFromFB == 0)
        {
            GameStateManager.Instance.DiamondCount += 2000;
            GameStateManager.Instance.getDiamondFromFB = 1;
            GameStateManager.Instance.Save();
        }
    }

    void OnEnable()
    {
        FBGraph.OnFacebookLoggedInUpdated += UpdateUI; 
    }

    void OnDisable()
    {
        FBGraph.OnFacebookLoggedInUpdated -= UpdateUI;
    }
}
