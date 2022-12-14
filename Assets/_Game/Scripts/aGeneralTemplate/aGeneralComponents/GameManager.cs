using UnityEngine;

namespace GeneralTemplate
{
    // Fail should to loop.
    // Две частоты у всех ловушек.
    
    //MaybeDone:
    // Customized UI with the clock instead of button.

    // one big wave; // WOOOOOOOOH Made It!;

    // Build.

    // New types of timeable traps, гтльотина и песочные часы, маятник

    // Increase size of arrow hitbox.

    /// <summary>
    /// General functionality in our projects. 
    /// It is likely that no modifications will be needed.
    /// It is also desirable for it to be the only Singleton Monobehaviour.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Singleton;
        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(multiLevelsParent);
            }

            if (!shouldDestroyOnSceneLoad)
            {
                DontDestroyOnLoad(multiLevelsParent);
            }
        }

        [Header("MultiLevels")]
        [Space]
        [SerializeField]
        private bool shouldDestroyOnSceneLoad;
        
        [SerializeField]
        private GameObject multiLevelsParent;

        [SerializeField]
        private UIGeneralController userInterface;

        [SerializeField]
        private SoundController soundController;

        [SerializeField]
        private VibrationController vibrationController;

        private void Start()
        {
            InitializeSettings();

            GeneralEventsContainer.GameStart?.Invoke();
        }

        private void InitializeSettings()
        {
            hasSound = PlayerPrefs.GetInt("Sound") == 0;
            isHaptic = PlayerPrefs.GetInt("Haptic") == 0;

            soundController.ShouldProduceSound = hasSound;
            userInterface.SetSoundPresentation(hasSound);

            vibrationController.IsAllowedToVibrate = isHaptic;
            userInterface.SetVibrationPresentation(isHaptic);
        }

        #region SettingsAlternations
        private bool isHaptic;
        public void AlternateHaptic()
        {
            isHaptic = !isHaptic;
            PlayerPrefs.SetInt("Haptic", isHaptic ? 0 : 1);
            vibrationController.IsAllowedToVibrate = isHaptic;
            vibrationController.VibrateOnChangeHapticSetting();
        }

        private bool hasSound;
        public void AlternateSound()
        {
            hasSound = !hasSound;
            PlayerPrefs.SetInt("Sound", hasSound ? 0 : 1);
            soundController.ShouldProduceSound = hasSound;
            if (hasSound)
            {
                soundController.PlaySoundTurnOn();
            }
        }
        #endregion
    }
}


