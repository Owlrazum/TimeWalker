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
        GeneralEventsContainer.LevelFailed += ShowItselfOnLevelFail;
        fadeOutTime = 0.2f;
    }

    private void OnDestroy()
    {
        GeneralEventsContainer.LevelFailed -= ShowItselfOnLevelFail;
    }

    private void ShowItselfOnLevelFail(int notUsed)
    {
        ShowItself();
    }

    public void OnNextLevelButtonDown()
    {
        GeneralEventsContainer.ShouldLoadNextSceneLevel?.Invoke();
        HideItself();
    }
}