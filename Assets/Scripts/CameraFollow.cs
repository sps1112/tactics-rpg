using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private TurnManager turnManager = null; // Turn Manager reference

    private GameObject target = null; // Current Target for the camera

    [SerializeField]
    private Vector3 offset; // Offset to keep when snapped to target

    [SerializeField]
    private float cameraSpeed = 0.025f; // Move speed while snapped

    [SerializeField]
    private float dragFactor = 5.0f; // Speed factor for dragging camera

    private bool toSnap = true; // Whether to snap to target

    private bool toDrag = false; // Whether to drag the camera

    private Vector3 origin; // Origin wrt dragging

    [SerializeField]
    [Range(0, 1)]
    private float camMoveSmoothness = 0.2f; // The tendency of camera to work as a smooth camera

    public Vector2 camZoomLimits; // The min and max size of the camera

    void Start()
    {
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
    }

    // Whether the camera is currently snapped to target or very close to the target
    private bool SnappedToTarget()
    {
        Vector3 pos = target.transform.position + offset;
        return (pos - transform.position).magnitude < 0.5f;
    }

    // Snaps and moves the camera to the set target
    public IEnumerator SnapToTarget(GameObject target_)
    {
        turnManager.StartSnapPhase();
        toSnap = true;
        target = target_;
        while (!SnappedToTarget()) // Wait till the cam is snapped to target
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    // Starts the dragging process
    public void StartDrag()
    {
        toSnap = false;
        toDrag = true;
        origin = Input.mousePosition;
    }

    // Stops the camera dragging
    public void StopDrag()
    {
        toDrag = false;
    }

    // Sets the camera to snap mode
    public void Snap()
    {
        toSnap = true;
    }

    void LateUpdate()
    {
        if (toSnap && target != null) // Follow target
        {
            Vector3 simplePos = CustomMath.MoveTowardsTarget(transform.position, target.transform.position + offset, cameraSpeed * Time.deltaTime);
            Vector3 smoothPos = Vector3.Lerp(transform.position, target.transform.position + offset, cameraSpeed * Time.deltaTime / 3.0f);
            transform.position = Vector3.Lerp(simplePos, smoothPos, camMoveSmoothness);
        }
        else if (toDrag) // Drag camera's look at origin
        {
            Vector3 pos = Input.mousePosition;
            Vector3 diff = origin - pos;
            diff.x /= Screen.currentResolution.width;
            diff.y /= Screen.currentResolution.height;
            diff *= dragFactor;
            transform.position = Vector3.Lerp(transform.position,
                                            CustomMath.MoveAlongPlane(transform, diff),
                                            cameraSpeed * Time.deltaTime * 60.0f);
            origin = pos;
        }
    }
}
