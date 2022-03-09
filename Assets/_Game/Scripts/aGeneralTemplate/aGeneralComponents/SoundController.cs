using System.Collections;
using UnityEngine;

namespace GeneralTemplate
{
    /// <summary>
    /// There should be multiple AudioSources as children of this transform.
    /// If needed, for each AudioSource there can be mutliple AudioClips.
    ///
    /// Start and stop looping sound worked good in chainsaw.
    /// </summary>
    public class SoundController : MonoBehaviour
    {
        /*
         * Useful Methods for AudioSource:
         * Play(), Stop(), PlayOneShot(), PlayDelayed()
         * 
         */

        [SerializeField]
        private AudioSource turnOnSound;

        [SerializeField]
        private AudioSource winSound;

        [SerializeField]
        private AudioSource backGroundSoundMusic;

        [SerializeField]
        private float initialRewindSpeed = 2;

        [SerializeField]
        private float subsequentRewindSpeed = 0.4f;

        [SerializeField]
        private float returningToUsualRewindSpeed = 5;

        private bool isRevertingTimeFlow;

        private void Awake()
        {
            EventsContainer.RevertingTimeFlow += OnRevertingTimeFlow;
            EventsContainer.UsualTimeFlow += OnUsualTimeFlow;

            GeneralEventsContainer.LevelComplete += OnLevelComplete;
        }

        private void OnDestroy()
        { 
            EventsContainer.RevertingTimeFlow -= OnRevertingTimeFlow;
            EventsContainer.UsualTimeFlow -= OnUsualTimeFlow;

            GeneralEventsContainer.LevelComplete -= OnLevelComplete;
        }

        private void OnRevertingTimeFlow()
        {
            //backGroundSoundMusic.pitch = -1;
            StartCoroutine(RewindSoundManipulation());
        }

        private IEnumerator RewindSoundManipulation()
        {
            isRevertingTimeFlow = true;
            float pitchValue = 1;
            while (pitchValue > -1)
            {
                pitchValue -= initialRewindSpeed * Time.deltaTime;
                backGroundSoundMusic.pitch = pitchValue;
                yield return null;
            }

            while (isRevertingTimeFlow)
            {
                pitchValue -= subsequentRewindSpeed * Time.deltaTime;
                backGroundSoundMusic.pitch = pitchValue;
                yield return null;
            }

            do
            {
                pitchValue += returningToUsualRewindSpeed * Time.deltaTime;
                backGroundSoundMusic.pitch = pitchValue;
                yield return null;
            } while (pitchValue < 1);

            backGroundSoundMusic.pitch = 1;
        }

        private void OnUsualTimeFlow()
        {
            isRevertingTimeFlow = false;
        }

        private void OnLevelComplete(int notUsed)
        {
            winSound.Play();
        }


        private bool shouldProduceSound;
        public bool ShouldProduceSound
        {
            get
            {
                return shouldProduceSound;
            }
            set
            {
                shouldProduceSound = value;
                if (shouldProduceSound)
                {
                    AudioListener.volume = 1;
                }
                else
                {
                    AudioListener.volume = 0;
                }
            }
        }

        public void PlaySoundTurnOn()
        {
            turnOnSound.Play();
        }
    }
}
