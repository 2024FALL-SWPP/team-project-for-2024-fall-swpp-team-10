using UnityEngine;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // �÷��̾��� Transform
    public Vector3 offset = new Vector3(0, 3, -5); // ���ʰ� ���������� �Ÿ�
    public float followSpeed = 5f;
    public float pitch = 20f; // ī�޶��� X�� ����

    private void LateUpdate()
    {
        FollowPlayer();
        LookAtPlayer();
    }

    private void FollowPlayer()
    {
        // �÷��̾��� Y�� ȸ���� ����Ͽ� ī�޶� ��ġ ���
        Quaternion yawRotation = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 desiredPosition = player.position + yawRotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    private void LookAtPlayer()
    {
        // ī�޶��� ȸ���� �÷��̾��� Y�� ȸ���� ���߰�, X�� ���� ����
        Quaternion cameraRotation = Quaternion.Euler(pitch, player.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, followSpeed * Time.deltaTime);
    }
}
