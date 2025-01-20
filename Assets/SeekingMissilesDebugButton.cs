using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;
using LightItUp.Game;

namespace LightItUp
{
	public class SeekingMissilesDebugButton : MonoBehaviour
	{
		[SerializeField] private Button button;
        [SerializeField] private bool isUnlimited = false;

		private bool isUsed = false;
        private bool hasPlayerStarted = false;

        private SeekingMissilesLauncher seekingMissilesLauncher;

        private void OnEnable()
		{
            seekingMissilesLauncher = GameManager.Instance.currentLevel
                .player.GetComponent<SeekingMissilesLauncher>();
            isUsed = false;
            hasPlayerStarted = false;
            button.gameObject.SetActive(false);
            button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);

            GameManager.Instance.playerStart += OnPlayerStart;
            PlayerController.StateChangedEvent += OnPlayerStateChanged;
		}

        private void OnDisable()
        {
            GameManager.Instance.playerStart -= OnPlayerStart;
            PlayerController.StateChangedEvent -= OnPlayerStateChanged;
        }

        private void OnPlayerStart()
        {
            hasPlayerStarted = true;
        }

        private void OnPlayerStateChanged(PlayerController.State state)
        {
            if (state != PlayerController.State.Normal)
            {
                button.gameObject.SetActive(false);
            }
            else if (hasPlayerStarted)
            {
                isUsed = false;
                button.gameObject.SetActive(DebugToggleSeekingMissiles.IsOn);
            }
        }

        private void OnClick()
        {
			if (!isUsed || isUnlimited)
            {
				isUsed = true;
				FireSeekingMissiles();
            }
        }

        private void FireSeekingMissiles()
        {
			seekingMissilesLauncher.FireSeekingMissiles();
        }
    }
}