using UnityEngine;
using System.Collections;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // 플레이어의 Transform
    public Vector3 offset = new Vector3(0, 3, -5); // 뒤쪽과 위쪽으로의 거리
    public float followSpeed = 5f;
    public float pitch = 30f; // 카메라의 X축 기울기 (위쪽으로 약간 더 기울기)
    public Transform bossTransform; // 보스 Transform
    public float orbitDuration = 3f; // 보스 주위를 순회하는 데 걸리는 시간
    private bool isOrbiting = false;

    private void LateUpdate()
    {
        if (!isOrbiting)
        {
            FollowPlayer();
            LookAtPlayer();
        }
    }

    private void FollowPlayer()
    {
        // 플레이어의 Y축 회전만 사용하여 카메라 위치 계산
        Quaternion yawRotation = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 desiredPosition = player.position + yawRotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    private void LookAtPlayer()
    {
        // 카메라가 약간 위를 바라보도록 조정
        Vector3 lookAtTarget = player.position + Vector3.up * 2f; // 플레이어보다 약간 위쪽을 바라보도록 보정
        Quaternion targetRotation = Quaternion.LookRotation(lookAtTarget - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
    }

    public IEnumerator OrbitAroundBoss()
    {
        isOrbiting = true;
        float elapsed = 0f;

        while (elapsed < orbitDuration)
        {
            // 각도 계산
            float angle = (elapsed / orbitDuration) * 360f; // 360도 순회
            Vector3 orbitPosition = bossTransform.position + new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) * offset.z,
                10,
                Mathf.Sin(Mathf.Deg2Rad * angle) * offset.z
            );

            // 카메라 위치 및 보스 바라보기
            transform.position = orbitPosition;
            transform.LookAt(bossTransform); // 항상 보스를 바라봄

            elapsed += Time.deltaTime;
            yield return null;
        }

        isOrbiting = false;
    }
}
