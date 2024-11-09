using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossControl;

public class BossControl : MonoBehaviour
{
    public Animator animator;

    // Color related variables
    int col; // Testing purposes only
    Color[] myColors = { Color.white, Color.red, Color.gray };
    public enum BossColor
    {
        White,
        Red,
        Black
    }

    bool bossDead = false;

    [SerializeField] Transform bossTransform;

    // Carrot Shooting variables
    [SerializeField] Transform carrotSpawnOffset;
    [SerializeField] GameObject carrotPf;
    [SerializeField] Transform carrotTargetPos;
    [SerializeField] float carrotDelayTime; // Time between each carrot throw
    [SerializeField] float carrotSpeed;
    [SerializeField] int carrotNum; // Number of carrots to throw in one shooting interval
    int carrotCount;

    // Boss movement variables
    [SerializeField] float bossHorizontalRange; // x range of boss
    [SerializeField] float bossHorizontalSpeed;
    float bossHorizontalPos;    // Position boss is to move to

    // Boss Death animation variables
    [SerializeField] ParticleSystem bossSmoke;
    [SerializeField] float bossReducedSize;
    Dictionary<Transform, Vector3> bossComponentsPositions = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Quaternion> bossComponentsRotations = new Dictionary<Transform, Quaternion>();

    [SerializeField] BossColor bossStartColor;

    // Start is called before the first frame update
    void Start()
    {
        col = 0; // Testing purposes only
        carrotCount = 0;

        // Set up boss color
        ColorUtility.TryParseHtmlString("#CB5353", out myColors[1]);
        ChangeColor(bossStartColor);

        // Set up for boss death animation
        foreach (Transform childTransform in bossTransform)
        {
            bossComponentsPositions[childTransform] = childTransform.position;
            bossComponentsRotations[childTransform] = childTransform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossDead)
        {
            // Rotate to look at player (+ height adjustment to rotate only y axis)
            bossTransform.LookAt(carrotTargetPos.position + Vector3.up * (bossTransform.position.y - carrotTargetPos.position.y));
            // Generate new boss target position once in close enough proximity
            if (Mathf.Abs(bossHorizontalPos - bossTransform.position.x) < 0.1)
            {
                bossHorizontalPos = Random.Range(-bossHorizontalRange, bossHorizontalRange);
            }

            // Boss side to side movement
            if (bossHorizontalPos < bossTransform.position.x)
            {
                bossTransform.position -= new Vector3(bossHorizontalSpeed * Time.deltaTime, 0, 0);
            }
            if (bossHorizontalPos > bossTransform.position.x)
                bossTransform.position += new Vector3(bossHorizontalSpeed * Time.deltaTime, 0, 0);

        }
        // Finish condition for one shooting interval
        if (carrotCount == carrotNum || bossDead)
            StopShooting();
    }

    // Shoot one carrot
    public void ShootProjectile()
    {
        Vector3 carrotSpawnPos = carrotSpawnOffset.position;

        Vector3 orientation = carrotTargetPos.position - carrotSpawnPos;
        GameObject projectile = Instantiate(carrotPf, carrotSpawnPos, Quaternion.Euler(orientation));
        projectile.gameObject.transform.LookAt(carrotTargetPos);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.velocity = orientation.normalized * carrotSpeed;

        carrotCount += 1;
    }

    // Stop shooting and close mouth
    public void StopShooting()
    {
        CancelInvoke("ShootProjectile");
        carrotCount = 0;
        animator.SetBool("mouthOpen_b", false);
    }

    // Start shoot interval
    public void StartShootInterval()
    {
        animator.SetBool("mouthOpen_b", true);
        InvokeRepeating("ShootProjectile", 0.2f, carrotDelayTime);
    }

    // Change color testing purposes only
    public void ChangeColorRandom()
    {
        col = (col + 1) % 3;
        foreach (Transform childTransform in gameObject.transform)
        {
            GameObject child = childTransform.gameObject;
            SkinnedMeshRenderer smr = child.GetComponent<SkinnedMeshRenderer>();
            if (smr != null) smr.material.color = myColors[col];
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null) mr.material.color = myColors[col];
        }
    }

    // Change color function to keep
    public void ChangeColor(BossColor bossColor)
    {
        ChangeColorHelper(gameObject.transform, bossColor);
    }

    // Recursively change color of all children
    public void ChangeColorHelper(Transform transform, BossColor bossColor)
    {
        foreach (Transform childTransform in transform)
        {
            GameObject child = childTransform.gameObject;
            SkinnedMeshRenderer smr = child.GetComponent<SkinnedMeshRenderer>();
            if (smr != null) smr.material.color = myColors[(int)bossColor];
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null) mr.material.color = myColors[(int)bossColor];

            if (childTransform.childCount > 0) ChangeColorHelper(childTransform, bossColor);
        }


    }

    public void BossDeath()
    {
        if (!bossDead)
        {
            bossDead = true;

            // Fall back effect
            foreach (Transform childTransform in bossTransform)
            {
                GameObject child = childTransform.gameObject;
                Rigidbody rb = child.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.isKinematic = false;
                    rb.AddForce(Vector3.forward * 50 + Vector3.up * 10f, ForceMode.Impulse);
                }
            }

            Invoke("BossDeathHelper", 2f);
        }
    }

    public void BossDeathHelper()
    {
        // Particle effect
        bossSmoke.Play();
        Invoke("BossDeathTransform", 0.5f);
    }

    void BossDeathTransform()
    {
        // Transform boss into small white rabbit
        ChangeColor(BossColor.White);
        foreach (Transform childTransform in bossTransform)
        {
            GameObject child = childTransform.gameObject;
            Rigidbody rb = child.GetComponent<Rigidbody>();

            // Restore positions
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
            childTransform.position = bossComponentsPositions[childTransform];
            childTransform.rotation = bossComponentsRotations[childTransform];
        }
        bossTransform.localScale = new Vector3(bossReducedSize, bossReducedSize, bossReducedSize);
    }
}