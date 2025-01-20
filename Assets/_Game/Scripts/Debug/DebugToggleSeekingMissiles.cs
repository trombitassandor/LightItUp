using UnityEngine;
using UnityEngine.UI;

namespace LightItUp
{
    [RequireComponent(typeof(Button))]
    public class DebugToggleSeekingMissiles : MonoBehaviour
    {
        [SerializeField] [TextArea]
        private string defaultText;

        private Text text;

        public static bool IsOn
        {
            get { return PlayerPrefs.GetInt("debug_seeking_missiles_enabled", 0) == 1; }
            set { PlayerPrefs.SetInt("debug_seeking_missiles_enabled", value ? 1 : 0); }
        }

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
            text = GetComponentInChildren<Text>();
            RefreshText();
        }

        private void OnClick()
        {
            IsOn = !IsOn;

            if (IsOn)
            {
                UnlockBooster();
            }
            else
            {
                LockBooster();
            }

            RefreshText();
        }

        private void UnlockBooster()
        {
            BoosterService.Instance.UnlockBooster(BoosterType.SeekingMissiles);
            BoosterService.Instance.PurchaseBooster(BoosterType.SeekingMissiles);
            BoosterService.Instance.AddRegularBoosterCharges(BoosterType.SeekingMissiles, 999);
        }

        private void LockBooster()
        {
            BoosterService.Instance.LockAllBoosters(BoosterType.SeekingMissiles);
        }

        void RefreshText()
        {
            text.text = $"{defaultText}{IsOn}";
        }
    }
}