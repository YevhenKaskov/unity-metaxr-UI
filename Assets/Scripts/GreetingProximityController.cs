using UnityEngine;

public class GreetingProximityController : MonoBehaviour
{
    [Header("State Objects")]
    public GameObject hiddenState;
    public GameObject expandedState;

    [Header("Animator Script")]
    public MonoBehaviour panelAnimator; // must have Expand() / Collapse()

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;
        ShowExpanded();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        ShowHidden();
    }

    bool IsPlayer(Collider other)
    {
        return other.gameObject.layer == 6;
    }

    void ShowExpanded()
    {
        InvokeIfExists("Expand");
        hiddenState.SetActive(false);
        expandedState.SetActive(true);
    }

    void ShowHidden()
    {
        InvokeIfExists("Collapse");
        hiddenState.SetActive(true);
        expandedState.SetActive(false);
    }

    void InvokeIfExists(string method)
    {
        if (panelAnimator == null) return;
        panelAnimator.Invoke(method, 0f);
    }
}
