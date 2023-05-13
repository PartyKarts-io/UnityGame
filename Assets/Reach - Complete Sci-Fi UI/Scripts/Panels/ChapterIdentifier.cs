using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Michsky.UI.Reach.HotkeyEvent;

namespace Michsky.UI.Reach
{
    public class ChapterIdentifier : MonoBehaviour
    {
        [Header("Resources")]
        public Animator animator;
        [SerializeField] private RectTransform backgroundRect;
        public Image backgroundImage;
        public TextMeshProUGUI titleObject;
        public TextMeshProUGUI descriptionObject;
        public ButtonManager continueButton;
        public ButtonManager playButton;
        public ButtonManager replayButton;

        [HideInInspector] public ChapterManager chapterManager;
        [HideInInspector] public bool isLocked;
        [HideInInspector] public bool isCurrent;

        void Awake()
        {
            ControllerManager cm = null;

            if (FindObjectsOfType(typeof(ControllerManager)).Length > 0) { cm = (ControllerManager)FindObjectsOfType(typeof(ControllerManager))[0]; }
            if (cm != null)
            {
                if (continueButton != null) { cm.AddButton(continueButton); }
                if (playButton != null) { cm.AddButton(playButton); }
                if (replayButton != null) { cm.AddButton(replayButton); }
            }
        }

        public void UpdateBackgroundRect()
        {
            chapterManager.currentBackgroundRect = backgroundRect;
            chapterManager.DoStretch();
        }

        public void SetCurrent()
        {

            continueButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(true);

            isLocked = false;
            isCurrent = true;
            continueButton.isInteractable = true;
            replayButton.isInteractable = true;
        }

        public void SetLocked()
        {
            continueButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(false);

            isLocked = true;
            isCurrent = false;
            playButton.isInteractable = false;
        }

        public void SetUnlocked()
        {
            continueButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(false);

            isLocked = false;
            isCurrent = false;
            playButton.isInteractable = true;
        }

        public void SetCompleted()
        {
            continueButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(true);

            isLocked = false;
            isCurrent = false;
            replayButton.isInteractable = true;
        }
    }
}