using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteControl : MonoBehaviour
{
    public GameObject explosionPrefab;
    void Awake() 
    {
        // Rigidbody�� ���ٸ� �߰��Ͽ� ���������� ���������� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }

    }
    void Start()
    {
        // ��� ���� ���� ������ üũ
        StartCoroutine(CheckLanding());
    }

    IEnumerator CheckLanding()
    {
        while (transform.position.y >  2.5f)
        {
            yield return null;
        }

        // ���� ȿ�� ����
        TriggerExplosion();

        // � ����
        Destroy(gameObject);
    }

    void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            Vector3 offset = new Vector3(0, 1f, 2.5f);
            GameObject explosion = Instantiate(explosionPrefab, transform.position + offset, Quaternion.identity);
            Destroy(explosion, 2f); // 2�� �� ����
        }
    }
}