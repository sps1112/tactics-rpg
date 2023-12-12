using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public IEnumerator SnapToTarget(GameObject target_)
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

    void LateUpdate()
    {
        if (canDrag) // Camera is not snapping to target and can be dragged
        {
            if (Input.GetMouseButtonDown(1)) // Drag Start
            {
                toSnap = false;
                toDrag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        if (toSnap && target != null) // Follow target
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, cameraSpeed * Time.deltaTime);
        }
        else if (toDrag) // Drag camera's look at origin
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position,
                                            transform.position + 60.0f * ((origin - pos) * (Time.deltaTime)),
                                            cameraSpeed * dragFactor);
            origin = pos;
        }
    }
}
