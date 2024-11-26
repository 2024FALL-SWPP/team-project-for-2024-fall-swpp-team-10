using UnityEngine;
using System.Collections;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // �÷��̾��� Transform
    public Vector3 offset = new Vector3(0, 3, -5); // ���ʰ� ���������� �Ÿ�
    public float followSpeed = 5f;
    public float pitch = 30f; // ī�޶��� X�� ���� (�������� �ణ �� ����)
    public Transform bossTransform; // ���� Transform
    public float orbitDuration = 3f; // ���� ������ ��ȸ�ϴ� �� �ɸ��� �ð�
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

    public IEnumerator OrbitAroundBoss()
    {
        isOrbiting = true;
        float elapsed = 0f;

        while (elapsed < orbitDuration)
        {
            // ���� ���
            float angle = (elapsed / orbitDuration) * 360f; // 360�� ��ȸ
            Vector3 orbitPosition = bossTransform.position + new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) * offset.z,
                10,
                Mathf.Sin(Mathf.Deg2Rad * angle) * offset.z
            );

            // ī�޶� ��ġ �� ���� �ٶ󺸱�
            transform.position = orbitPosition;
            transform.LookAt(bossTransform); // �׻� ������ �ٶ�

            elapsed += Time.deltaTime;
            yield return null;
        }

        isOrbiting = false;
    }
}
