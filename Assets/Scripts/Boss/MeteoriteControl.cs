using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteControl : MonoBehaviour
{
    public GameObject explosionPrefab;
    private Vector3 offset = new Vector3(0, -1f, 2f);

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
            GameObject explosion = Instantiate(explosionPrefab, transform.position + offset, Quaternion.identity);
            Destroy(explosion, 2f); // 2�� �� ����
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Laser"))
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position + offset, Quaternion.identity);
            Destroy(explosion, 0.5f); // 0.5�� �� ����
        }
    }
}