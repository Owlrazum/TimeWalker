using UnityEngine;

public class UILevelComplete : UIBaseFadingCanvas
{
    [Header("UILevelComplete")]
    [SerializeField]
    [Tooltip("Fade out time is overriden in the awake.")]
    private string readTooltip;
    protected override void Awake()
    {
        base.Awake();
        EventsContainer.PlayerReachedGates += ShowItself;
        fadeOutTime = 0.2f;
    }

    private void OnDestroy()
    {
        EventsContainer.PlayerReachedGates -= ShowItself;
    }

    public void OnNextLevelButtonDown()
    {
        GeneralEventsContainer.ShouldLoadNextSceneLevel?.Invoke();
        HideItself();
    }
}