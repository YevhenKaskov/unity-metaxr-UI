using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;


public class CubeLookInteractor : MonoBehaviour
{
    [Header("Text References")]
    public TMP_Text hintText;
    public TMP_Text respondText;

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    [Header("Input")]
    public InputActionReference interactAction;

    bool isGazing;
    bool respondLocked;

    Coroutine currentFade;

    void Start()
    {
        SetAlpha(hintText, 0);
        SetAlpha(respondText, 0);
    }

    // Gaze logic
    public void OnGazeEnter()
    {
        isGazing = true;

        if (respondLocked)
        {
            CancelFade();
            SetAlpha(respondText, 1);
            return;
        }

        FadeIn(hintText);
    }

    public void OnGazeExit()
    {
        isGazing = false;

        if (respondLocked)
        {
            FadeOutRespondText();
        }
        else
        {
            FadeOut(hintText);
        }
    }

    // Pressing interaction button logic
    public void OnInteract(InputAction.CallbackContext context)
    {
        respondLocked = true;

        CancelFade();
        SetAlpha(hintText, 0);
        FadeIn(respondText);
    }

    void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }

    // Make sure RespondText(TMP) fades out completely before showing HintText(TMP) again
    void FadeOutRespondText()
    {
        CancelFade();
        currentFade = StartCoroutine(Fade(respondText, 1, 0, () =>
        {
            respondLocked = false;

            if (isGazing)
            {
                FadeIn(hintText);
            }
        }));
    }

    // Fade animations
    void FadeIn(TMP_Text text)
    {
        CancelFade();
        currentFade = StartCoroutine(Fade(text, text.color.a, 1, null));
    }

    void FadeOut(TMP_Text text)
    {
        CancelFade();
        currentFade = StartCoroutine(Fade(text, text.color.a, 0, null));
    }

    IEnumerator Fade(TMP_Text text, float from, float to, System.Action onComplete)
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / fadeDuration);
            SetAlpha(text, a);
            yield return null;
        }

        SetAlpha(text, to);
        onComplete?.Invoke();
    }

    void CancelFade()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);
    }

    void SetAlpha(TMP_Text text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
