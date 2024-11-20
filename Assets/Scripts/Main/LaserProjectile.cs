using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public float projectileDurationTime = 2f;
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

    [Header("Particle System")]
    public ParticleSystem hitParticle;

    private BossControl bossControl;

    void Awake()
    {
        bossControl = GameObject.Find("Boss")?.GetComponent<BossControl>();

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

        // Destroy the projectile after some time
        Destroy(gameObject, projectileDurationTime);
    }

    private void Update()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
            GameManager.inst.AddScore(1000);
            gameObject.SetActive(false);
            Instantiate(hitParticle, transform.position, new Quaternion(0, 0, 0, 0));
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("WeakSpot")) // If weak spot hit, change the color
        {
            bossControl.TransformWeakSpot(other.gameObject);
            gameObject.SetActive(false); // Turn off laser
        }
        else if (bossControl is not null)   // If boss stage active,
        {
            gameObject.SetActive(false);    // Remove laser on collision
        }
    }
}