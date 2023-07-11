using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject target; // Current Target for the camera

    private bool isTargetMoving; // Whether the target is currently moving (cannot drag while moving)

    public Vector3 offset; // Offset to keep when snapped to target

    public float cameraSpeed = 0.025f; // Move speed while snapped

    public float dragFactor = 5.0f; // Speed factor for dragging camera

    private bool toSnap = true; // Whether to snap to target

    private bool toDrag = false; // Whether to drag the camera

    private Vector3 origin; // Origin wrt dragging

    // Sets the target for the camera to snap to
    public void SetTarget(GameObject target_)
    {
        toSnap = true;
        target = target_;
    }

    // Sets the motion state for the camera
    public void SetMotion(bool isTargetMoving_)
    {
        isTargetMoving = isTargetMoving_;
    }

    void LateUpdate()
    {
        if (!isTargetMoving)
        {
            if (Input.GetMouseButtonDown(1))
            {
                toSnap = false;
                toDrag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (!Input.GetMouseButton(1) && toDrag)
            {
                toDrag = false;
            }
        }
        else if (!toSnap)
        {
            toSnap = true;
        }
        if (toSnap)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, cameraSpeed);
        }
        else if (toDrag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position, transform.position + 60.0f * ((origin - pos) * (Time.deltaTime)), cameraSpeed * dragFactor);
            origin = pos;
        }
    }
}
