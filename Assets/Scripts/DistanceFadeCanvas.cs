using UnityEngine;

public class DistanceFadeCanvas : MonoBehaviour
{
    public Transform playerHead;
    public float fadeStartDistance = 1.5f;
    public float fadeEndDistance = 3.0f;
    public float minAlpha = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!playerHead) return;

        float distance = Vector3.Distance(playerHead.position, transform.position);

        float t = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, distance);
        float alpha = Mathf.Lerp(1f, minAlpha, t);

        canvasGroup.alpha = alpha;
    }
}
