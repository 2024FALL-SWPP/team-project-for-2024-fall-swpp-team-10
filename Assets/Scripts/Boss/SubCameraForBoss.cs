using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraForBoss : MonoBehaviour
{
    public Transform boss;    // ������ �÷��̾� Transform
    public float distance = 7f; // �÷��̾�κ��� ������ �Ÿ� (���ϴ� ������)
    public float height = 12f;   // �÷��̾� ���� ��� ī�޶� ���� ������

    [Header("Angle Settings (in degrees)")]
    public float minAngle = 30f;    // �ּ� ����(��)
    public float maxAngle = 60f;   // �ִ� ����(��)
    public float initialAngle = 45f; // ���� ����(��)

    public float speed = 5f;      // ���� ���� �ӵ�(��/��)

    private float currentAngle;    // ���� ���� (����)
    private float minAngleRad;     // �ּ� ����(����)
    private float maxAngleRad;     // �ִ� ����(����)

    void Start()
    {
        // �� -> ���� ��ȯ
        minAngleRad = minAngle * Mathf.Deg2Rad;
        maxAngleRad = maxAngle * Mathf.Deg2Rad;
        currentAngle = initialAngle * Mathf.Deg2Rad;

    }

    void LateUpdate()
    {
        if (boss == null) return;

        // ���� ������Ʈ (speed�� ��/���̹Ƿ� ���� ��ȯ �ʿ�)
        float angleChange = speed * Mathf.Deg2Rad * Time.deltaTime;
        currentAngle += angleChange;

        // ������ ������ �ʰ��ϸ� ���� ��ȯ
        if (currentAngle > maxAngleRad)
        {
            currentAngle = maxAngleRad;
            speed = -speed; // �ݴ� ���� ȸ��
        }
        else if (currentAngle < minAngleRad)
        {
            currentAngle = minAngleRad;
            speed = -speed; // �ݴ� ���� ȸ��
        }

        // position ���
        float x = boss.position.x + Mathf.Cos(currentAngle) * distance;
        float z = boss.position.z + Mathf.Sin(currentAngle) * distance;
        float y = boss.position.y + height;

        transform.position = new Vector3(x, y, z);
        transform.LookAt(boss.position);
    }
}
