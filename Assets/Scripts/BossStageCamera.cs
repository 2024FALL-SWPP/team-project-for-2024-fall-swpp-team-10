using UnityEngine;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // 플레이어의 Transform
    public Vector3 offset = new Vector3(0, 3, -5); // 뒤쪽과 위쪽으로의 거리
    public float followSpeed = 5f;
    public float pitch = 20f; // 카메라의 X축 기울기

    private void LateUpdate()
    {
        FollowPlayer();
        LookAtPlayer();
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
        // 카메라의 회전을 플레이어의 Y축 회전에 맞추고, X축 기울기 적용
        Quaternion cameraRotation = Quaternion.Euler(pitch, player.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, followSpeed * Time.deltaTime);
    }
}
