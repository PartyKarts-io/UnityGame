using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCarPreview : MonoBehaviour
{
    [SerializeField] Image CarImage;
    [SerializeField] Sprite Apex;
    [SerializeField] Sprite Blaze;
    [SerializeField] Sprite Velocity;
    [SerializeField] Sprite Phantom;

    public void SetSprite(string carName)
    {
        switch (carName)
        {
            case "Apex":
                CarImage.sprite = Apex;
                break;
            case "Blaze":
                CarImage.sprite = Blaze;
                break;
            case "Velocity":
                CarImage.sprite = Velocity;
                break;
            case "Phantom":
                CarImage.sprite = Phantom;
                break;
            default:
                return;
        }
    }
}
