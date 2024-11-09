using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    public Animator animator;
    int col;
    Color[] myColors = { Color.white, Color.red, Color.gray };

    [SerializeField] Transform bossPos;
    [SerializeField] Transform carrotSpawnOffset;

    [SerializeField] GameObject carrotPf;
    [SerializeField] Transform carrotTargetPos;
    [SerializeField] float carrotDelayTime; // Time between each carrot throw
    [SerializeField] float carrotSpeed;
    [SerializeField] int carrotNum; // Number of carrots to throw in one shooting interval
    int carrotCount;

    [SerializeField] float bossHorizontalRange; // x range of boss
    [SerializeField] float bossHorizontalSpeed;
    float bossHorizontalPos;    // Position boss is to move to


    // Start is called before the first frame update
    void Start()
    {
        col = 0;
        carrotCount = 0;
        ColorUtility.TryParseHtmlString("#CB5353", out myColors[1]);
    }

    // Update is called once per frame
    void Update()
    {
        bossPos.LookAt(carrotTargetPos.position + Vector3.up * (bossPos.position.y - carrotTargetPos.position.y));
        // Generate new boss target position once in close enough proximity
        if (Mathf.Abs(bossHorizontalPos - bossPos.position.x) < 0.1)
        {
            bossHorizontalPos = Random.Range(-bossHorizontalRange, bossHorizontalRange);
        }

        // Boss side to side movement
        if (bossHorizontalPos < bossPos.position.x)
        {
            bossPos.position -= new Vector3(bossHorizontalSpeed * Time.deltaTime, 0, 0);
        }
        if (bossHorizontalPos > bossPos.position.x)
            bossPos.position += new Vector3(bossHorizontalSpeed * Time.deltaTime, 0, 0);

        // Finish condition for one shooting interval
        if (carrotCount == carrotNum)
            StopShooting();
    }

    // Shoot one carrot
    public void ShootProjectile()
    {
        Vector3 carrotSpawnPos = carrotSpawnOffset.position; //bossPos.position + new Vector3(0, 1.6f, -0.4f);

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

    // Change color
    public void ChangeColor()
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
}
