using UnityEngine;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // �÷��̾��� Transform
    public Vector3 offset = new Vector3(0, 3, -5); // ���ʰ� ���������� �Ÿ�
    public float followSpeed = 5f;
    public float pitch = 30f; // ī�޶��� X�� ���� (�������� �ణ �� ����)

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
        // ī�޶� �ణ ���� �ٶ󺸵��� ����
        Vector3 lookAtTarget = player.position + Vector3.up * 2f; // �÷��̾�� �ణ ������ �ٶ󺸵��� ����
        Quaternion targetRotation = Quaternion.LookRotation(lookAtTarget - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
    }
}
