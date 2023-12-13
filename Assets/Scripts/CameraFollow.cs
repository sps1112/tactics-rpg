using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Device;

public class CameraFollow : MonoBehaviour
{
    private GameObject target = null; // Current Target for the camera

    private bool canDrag = false; // Whether the target is currently moving (cannot drag while moving)

    public Vector3 offset; // Offset to keep when snapped to target

    public float cameraSpeed = 0.025f; // Move speed while snapped

    public float dragFactor = 5.0f; // Speed factor for dragging camera

    private bool toSnap = true; // Whether to snap to target

    private bool toDrag = false; // Whether to drag the camera

    private Vector3 origin; // Origin wrt dragging

    [SerializeField]
    [Range(0, 1)]
    private float camMoveSmoothness = 0.2f;

    // Sets the drag status on whether the camera can be dragged
    public void SetDrag(bool status)
    {
        canDrag = status;
    }

    // Whether the camera is currently snapped to target or very close to the target
    public bool SnappedToTarget()
    {
        Vector3 pos = target.transform.position + offset;
        return (pos - transform.position).magnitude < 0.5f;
    }

    // Snaps and moves the camera to the set target
    private IEnumerator SnapToTarget(GameObject target_)
    {
        toSnap = true;
        target = target_;
        SetDrag(false);
        while (!SnappedToTarget())
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        SetDrag(true);
    }

    void Update()
    {
        if (canDrag) // Camera is not snapping to target and can be dragged
        {
            if (Input.GetMouseButtonDown(1)) // Drag Start
            {
                toSnap = false;
                toDrag = true;
                origin = Input.mousePosition;
            }
            if (toDrag && !Input.GetMouseButton(1)) // Dragging
            {
                toDrag = false;
            }
            if (Input.GetMouseButtonDown(2)) // Set back to snap
            {
                toSnap = true;
            }
        }
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
            diff.x /= UnityEngine.Screen.currentResolution.width;
            diff.y /= UnityEngine.Screen.currentResolution.height;
            diff *= dragFactor;
            transform.position = Vector3.Lerp(transform.position,
                                            CustomMath.MoveAlongPlane(transform, diff),
                                            cameraSpeed * Time.deltaTime * 60.0f);
            origin = pos;
        }
    }
}
