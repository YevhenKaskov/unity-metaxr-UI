using UnityEngine;

public class CubeGazeDetector : MonoBehaviour
{
    public CubeLookInteractor lookInteractor;
    public float maxDistance = 2f;

    Camera playerCam;
    bool isGazing;

    void Start()
    {
        playerCam = Camera.main;
    }

    void Update()
    {
        if (!playerCam) return;

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        bool hitThisCube =
            Physics.Raycast(ray, out hit, maxDistance) &&
            hit.collider == GetComponent<Collider>();

        if (hitThisCube && !isGazing)
        {
            isGazing = true;
            lookInteractor.OnGazeEnter();
        }
        else if (!hitThisCube && isGazing)
        {
            isGazing = false;
            lookInteractor.OnGazeExit();
        }
    }
}
