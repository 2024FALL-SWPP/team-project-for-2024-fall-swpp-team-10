using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    private TrailRenderer trail;

    [Header("Trail Settings")]
    public float trailTime = 0.4f;
    public float startWidth = 0.3f;
    public float endWidth = 0.0f;
    public float minVertexDistance = 0.1f;

    [Header("Color Settings")]
    [Tooltip("Starting color of the trail")]
    public Color startColor = new Color(0f, 1f, 0f, 1f);    // Default bright green

    [Tooltip("Ending color of the trail")]
    public Color endColor = new Color(0f, 1f, 0f, 0f);      // Default transparent green

    [Tooltip("Main color of the trail material")]
    public Color materialColor = new Color(0f, 1f, 0f, 1f);  // Green

    void Start()
    {
        trail = GetComponent<TrailRenderer>();

        if (trail != null)
        {
            // Basic trail properties
            trail.time = trailTime;
            trail.startWidth = startWidth;
            trail.endWidth = endWidth;

            // Set the colors
            trail.startColor = startColor;
            trail.endColor = endColor;

            trail.minVertexDistance = minVertexDistance;

            // Material settings
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.material.SetColor("_Color", materialColor);
        }
    }
}