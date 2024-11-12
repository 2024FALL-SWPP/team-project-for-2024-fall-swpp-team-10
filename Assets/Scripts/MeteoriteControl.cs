using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteControl : MonoBehaviour
{
    private float groundY; // ���� Y ��ǥ
    private GameObject explosionPrefab;

    public void Initialize(float groundYPosition, GameObject explosion)
    {
        groundY = groundYPosition;
        explosionPrefab = explosion;

        // Rigidbody�� ���ٸ� �߰��Ͽ� ���������� ���������� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }

        // ��� ���� ���� ������ üũ
        StartCoroutine(CheckLanding());
    }

    IEnumerator CheckLanding()
    {
        while (transform.position.y > groundY + 0.5f)
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
            Vector3 offset = Vector3.zero;
            offset.z = 2.5f;
            GameObject explosion = Instantiate(explosionPrefab, transform.position + offset, Quaternion.identity);
            Destroy(explosion, 2f); // 2�� �� ����
        }
    }
}