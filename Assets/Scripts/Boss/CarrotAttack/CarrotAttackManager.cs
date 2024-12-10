using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotAttackManager : MonoBehaviour
{
    public Animator animator;

    [Header("Carrot Shooting variables")]
    [SerializeField] Transform carrotSpawnOffset;
    [SerializeField] GameObject carrotPf;
    private Transform carrotTargetPos;
    [SerializeField] float carrotDelayTime; // Time between each carrot throw

    BossStageManager bossStageManager;

    // Start is called before the first frame update
    void Start()
    {
        bossStageManager = gameObject.GetComponent<BossStageManager>();
        carrotTargetPos = bossStageManager.ActiveCharacter().transform;

        StartShootInterval();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Shoot one carrot
    public void ShootProjectile()
    {
        StartCoroutine("ShootProjectileCoroutine");
    }

    IEnumerator ShootProjectileCoroutine()
    {
        animator.SetBool("mouthOpen_b", true);

        yield return new WaitForSeconds(0.2f);

        Vector3 carrotSpawnPos = carrotSpawnOffset.position;

        Vector3 orientation = carrotTargetPos.position - carrotSpawnPos;
        GameObject projectile = Instantiate(carrotPf, carrotSpawnPos, Quaternion.Euler(orientation));
        projectile.gameObject.transform.LookAt(carrotTargetPos);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.velocity = orientation.normalized * bossStageManager.carrotSpeed;

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("mouthOpen_b", false);
    }

    // Stop shooting and close mouth
    public void StopShooting()
    {
        CancelInvoke("ShootProjectile");
        animator.SetBool("mouthOpen_b", false);
    }

    // Start shoot interval
    public void StartShootInterval()
    {
        InvokeRepeating("ShootProjectile", 0f, Mathf.Max(carrotDelayTime, 1f)); // Appropriate delay time based on testing: 1f ~ 1.6f
    }
}
