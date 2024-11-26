using UnityEngine;
using System.Collections;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    private Transform player; // �÷��̾��� Transform
    public Vector3 offset = new Vector3(0, 3, -5); // ���ʰ� ���������� �Ÿ�
    public float followSpeed = 5f;
    public float pitch = 30f; // ī�޶��� X�� ���� (�������� �ణ �� ����)
    public Transform bossTransform; // ���� Transform
    public float orbitDuration = 3f; // ���� ������ ��ȸ�ϴ� �� �ɸ��� �ð�
    private bool isOrbiting = false;
    private bool cameraFixed = false;
    public GameObject[] characters;
    private BossStagePlayer playerScript;

    private void Awake() 
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].activeSelf)
            {
                playerScript = characters[i].GetComponent<BossStagePlayer>();
                player = characters[i].transform;
                break;
            }
        }
    }
    private void LateUpdate()
    {
        if (!isOrbiting && !cameraFixed)
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

    // ���� ������ ��ȸ�ϴ� �ڷ�ƾ
    public IEnumerator OrbitAroundBoss()
    {
        isOrbiting = true;

        // ���ο� ��� ������ �̹� BossStageManager���� ó����

        float elapsed = 0f;

        while (elapsed < orbitDuration)
        {
            // ���� ���
            float angle = (elapsed / orbitDuration) * 360f; // 360�� ��ȸ
            Vector3 orbitPosition = bossTransform.position + new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) *(-8),
                15,
                Mathf.Sin(Mathf.Deg2Rad * angle) * (-8)
            );

            // ī�޶� ��ġ �� ���� �ٶ󺸱�
            transform.position = orbitPosition;
            transform.LookAt(bossTransform); // �׻� ������ �ٶ�

            elapsed += Time.unscaledDeltaTime; // ���ο� ��ǿ����� ���������� ����ǵ��� unscaledDeltaTime ���
            yield return null;
        }

        isOrbiting = false;
    }

    // ī�޶� �÷��̾�� �����ϴ� �޼���
    public IEnumerator FixCameraOnPlayerDuringSpin()
    {
        cameraFixed = true;

        // �÷��̾��� ���� ���ߵ��� ī�޶� ��ġ ����
        Vector3 fixedPosition = player.position + new Vector3(0, 0.5f, -2f); // �÷��̾� ���ʿ� ��ġ
        Quaternion fixedRotation = Quaternion.LookRotation(player.position - fixedPosition);

        // ī�޶� �̵� (�ε巴��)
        float transitionDuration = 1f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < transitionDuration)
        {
            transform.position = Vector3.Lerp(startPos, fixedPosition, elapsed / transitionDuration);
            transform.rotation = Quaternion.Slerp(startRot, fixedRotation, elapsed / transitionDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.position = fixedPosition;
        transform.rotation = fixedRotation;

        // �÷��̾ ȸ���ϴ� ���� ī�޶� ����
        while (playerScript.IsSpinning())
        {
            // ī�޶�� ������ ���·� ����
            yield return null;
        }

        // �÷��̾� ȸ���� ������ ī�޶� ���󺹱�
        ResetCamera();
    }

    // ī�޶� ���� ���·� �����ϴ� �޼���
    public void ResetCamera()
    {
        cameraFixed = false;
    }

}
