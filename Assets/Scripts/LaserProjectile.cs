using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    private TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();

        if (trail != null)
        {
            // Basic trail properties
            trail.time = 0.4f;
            trail.startWidth = 0.3f;
            trail.endWidth = 0.0f;

            // Green color settings
            trail.startColor = new Color(0f, 1f, 0f, 1f);    // Bright green
            trail.endColor = new Color(0f, 1f, 0f, 0f);      // Transparent green

            trail.minVertexDistance = 0.1f;

            // Material settings
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.material.SetColor("_Color", Color.green);
        }
    }
}