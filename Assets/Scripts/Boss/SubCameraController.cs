using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    private Transform player;
    public float distance = 5f;    // �÷��̾�κ����� ������ �Ÿ�
    public float height = 2f;      // ī�޶� ���� ������

    [Header("Angle Settings (in degrees)")]
    public float minAngle = 0f;    // �ּ� ����(��)
    public float maxAngle = 90f;   // �ִ� ����(��)
    public float initialAngle = 45f; // ���� ����(��)

    public float speed = 5f;      // ���� ���� �ӵ�(��/��)

    private float currentAngle;    // ���� ���� (����)
    private float minAngleRad;     // �ּ� ����(����)
    private float maxAngleRad;     // �ִ� ����(����)

    private BossStagePlayer playerScript;
    public BossStageManager managerScript;
    void Start()
    {
        // �� -> ���� ��ȯ
        minAngleRad = minAngle * Mathf.Deg2Rad;
        maxAngleRad = maxAngle * Mathf.Deg2Rad;
        currentAngle = initialAngle * Mathf.Deg2Rad;
        playerScript = managerScript.ActiveCharacter().GetComponent<BossStagePlayer>();
        player = managerScript.ActiveCharacter().transform;
    }

    void LateUpdate()
    {
        if (player == null) return;

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
        float x = player.position.x + Mathf.Cos(currentAngle) * distance;
        float z = player.position.z + Mathf.Sin(currentAngle) * distance;
        float y = player.position.y + height;

        transform.position = new Vector3(x, y, z);
        transform.LookAt(player.position);
    }
}
