using UnityEngine;

public class UILevelFail : UIBaseFadingCanvas
{
    [Header("UILevelFail")]
    [SerializeField]
    [Tooltip("Fade out time is overriden in the awake.")]
    private string readTooltip;
    protected override void Awake()
    {
        base.Awake();
        GeneralEventsContainer.LevelFailed += ShowItself;
        GeneralEventsContainer.ShouldLoadNextScene += HideItself;
        fadeOutTime = 0.2f;
    }

    private void OnDestroy()
    {
        GeneralEventsContainer.LevelFailed -= ShowItself;
        GeneralEventsContainer.ShouldLoadNextScene -= HideItself;
    }

    public void OnNextLevelButtonDown()
    {
        GeneralEventsContainer.ShouldLoadNextScene?.Invoke();
    }
}