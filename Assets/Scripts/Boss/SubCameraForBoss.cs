using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraForBoss : MonoBehaviour
{
    public Transform boss;    // 추적할 플레이어 Transform
    public float distance = 7f; // 플레이어로부터 떨어진 거리 (원하는 값으로)
    public float height = 12f;   // 플레이어 높이 대비 카메라 높이 오프셋

    [Header("Angle Settings (in degrees)")]
    public float minAngle = 30f;    // 최소 각도(도)
    public float maxAngle = 60f;   // 최대 각도(도)
    public float initialAngle = 45f; // 시작 각도(도)

    public float speed = 5f;      // 각도 변경 속도(도/초)

    private float currentAngle;    // 현재 각도 (라디안)
    private float minAngleRad;     // 최소 각도(라디안)
    private float maxAngleRad;     // 최대 각도(라디안)

    void Start()
    {
        // 도 -> 라디안 변환
        minAngleRad = minAngle * Mathf.Deg2Rad;
        maxAngleRad = maxAngle * Mathf.Deg2Rad;
        currentAngle = initialAngle * Mathf.Deg2Rad;

    }

    void LateUpdate()
    {
        if (boss == null) return;

        // 각도 업데이트 (speed는 도/초이므로 라디안 변환 필요)
        float angleChange = speed * Mathf.Deg2Rad * Time.deltaTime;
        currentAngle += angleChange;

        // 각도가 범위를 초과하면 방향 전환
        if (currentAngle > maxAngleRad)
        {
            currentAngle = maxAngleRad;
            speed = -speed; // 반대 방향 회전
        }
        else if (currentAngle < minAngleRad)
        {
            currentAngle = minAngleRad;
            speed = -speed; // 반대 방향 회전
        }

        // position 계산
        float x = boss.position.x + Mathf.Cos(currentAngle) * distance;
        float z = boss.position.z + Mathf.Sin(currentAngle) * distance;
        float y = boss.position.y + height;

        transform.position = new Vector3(x, y, z);
        transform.LookAt(boss.position);
    }
}
