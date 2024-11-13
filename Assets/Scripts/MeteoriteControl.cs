using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteControl : MonoBehaviour
{
    public GameObject explosionPrefab;
    void Awake() 
    {
        // Rigidbody가 없다면 추가하여 물리적으로 떨어지도록 설정
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }

    }
    void Start()
    {
        // 운석이 땅에 닿을 때까지 체크
        StartCoroutine(CheckLanding());
    }

    IEnumerator CheckLanding()
    {
        while (transform.position.y >  0.5f)
        {
            yield return null;
        }

        // 폭발 효과 생성
        TriggerExplosion();

        // 운석 제거
        Destroy(gameObject);
    }

    void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            Vector3 offset = new Vector3(0, 0, 2.5f);
            GameObject explosion = Instantiate(explosionPrefab, transform.position + offset, Quaternion.identity);
            Destroy(explosion, 2f); // 2초 후 제거
        }
    }
}