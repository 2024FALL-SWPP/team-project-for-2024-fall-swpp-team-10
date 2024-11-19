using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpTargetControl : MonoBehaviour
{
    public Camera mainCamera;   // Reference to the main camera
    public float yPosition = 0; // The fixed Y position on the XZ plane

    void Update()
    {
        FollowMouseOnXZPlane();
    }

    void FollowMouseOnXZPlane()
    {
        if (mainCamera == null) return;

        // Get the mouse position in screen space
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        // Calculate the intersection of the ray with the XZ plane
        Plane xzPlane = new Plane(Vector3.up, new Vector3(0, yPosition, 0)); // Plane at yPosition on the XZ plane

        if (xzPlane.Raycast(ray, out float distance))
        {
            // Get the point where the ray intersects the XZ plane
            Vector3 pointOnXZPlane = ray.GetPoint(distance);

            // Update the sprite position to follow the mouse on the XZ plane
            transform.position = pointOnXZPlane;
        }
    }
}