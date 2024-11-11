using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteExplosion : MonoBehaviour
{
    public GameObject explosionPrefab; // Particle System ������

    public void TriggerExplosion(Vector3 position)
    {
        // ���� �������� Ư�� ��ġ�� ����
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);

        // ��ƼŬ ȿ���� ���� �ð��� ������ ������Ʈ �ı�
        Destroy(explosion, 2f); // 2�� �� �ı�
    }
}

