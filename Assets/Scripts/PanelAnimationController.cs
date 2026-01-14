using UnityEngine;
using TMPro;

public class PanelAnimationController : MonoBehaviour
{
    [Header("States")]
    public RectTransform hiddenState;
    public RectTransform expandedState;

    [Header("Animation")]
    public float animationSpeed = 10f;
    public Vector3 visibleScale = Vector3.one;
    public Vector3 hiddenScale = Vector3.one * 0.001f;

    [Header("Input field reset (optional)")]
    public TMP_InputField inputField;

    private bool isExpanded = false;

    void Start()
    {
        // Explicit initial state
        isExpanded = false;

        hiddenState.gameObject.SetActive(true);
        expandedState.gameObject.SetActive(true);

        hiddenState.localScale = visibleScale;
        expandedState.localScale = hiddenScale;
    }

    void Update()
    {
        // Animate expanded panel
        expandedState.localScale = Vector3.Lerp(
            expandedState.localScale,
            isExpanded ? visibleScale : hiddenScale,
            Time.deltaTime * animationSpeed
        );

        // Animate collapsed teaser
        hiddenState.localScale = Vector3.Lerp(
            hiddenState.localScale,
            isExpanded ? hiddenScale : visibleScale,
            Time.deltaTime * animationSpeed
        );

        // Handle SetActive AFTER animation
        if (isExpanded && expandedState.localScale.x > 0.95f)
        {
            hiddenState.gameObject.SetActive(false);
        }
        else if (!isExpanded && hiddenState.localScale.x > 0.95f)
        {
            expandedState.gameObject.SetActive(false);
        }
        else
        {
            // During animation both must be active
            hiddenState.gameObject.SetActive(true);
            expandedState.gameObject.SetActive(true);
        }
    }

    // Buttons call these
    public void Expand()
    {
        expandedState.gameObject.SetActive(true);
        isExpanded = true;
    }

    public void Collapse()
    {
        hiddenState.gameObject.SetActive(true);
        isExpanded = false;
        if (inputField) inputField.text = "";
    }
}
