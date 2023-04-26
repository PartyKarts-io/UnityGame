using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// To attention users about not the full version.
/// </summary>
public class AttentionUI : MonoBehaviour
{
    [SerializeField] AudioClip ClickClip;
    [SerializeField] string NextSceneName = "MainMenuScene";
    [SerializeField] GameObject PCTextObject;
    [SerializeField] GameObject MobileTextObject;

    private ThirdwebSDK sdk;

    void Awake()
    {
        PCTextObject.SetActive(!Application.isMobilePlatform);
        MobileTextObject.SetActive(Application.isMobilePlatform);
    }

    void Update()
    {
        if (Input.touchCount > 0 || Input.anyKey)
        {
            Destroy(this);
            SoundControllerInUI.PlayAudioClip(ClickClip);
            LoadingScreenUI.LoadScene(NextSceneName);
        }
    }
}
