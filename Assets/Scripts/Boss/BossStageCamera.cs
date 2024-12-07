using UnityEngine;
using System.Collections;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    private Transform player; // �÷��̾��� Transform
    public Vector3 offset = new Vector3(0, 2.5f, -5.5f); // ���ʰ� ���������� �Ÿ�
    public float followSpeed = 5f;
    public float pitch = 30f; // ī�޶��� X�� ���� (�������� �ణ �� ����)
    public Transform bossTransform; // ���� Transform
    private float orbitAroundBossDuration = 8f; // ���� ������ ��ȸ�ϴ� �� �ɸ��� �ð�
    private bool isOrbiting = false;
    public float transitionAroundPlayerSpinDuration = 1f; // ī�޶� �÷��̾� ���� ���ߴ� ��ġ�� �̵��ϴ� �� �ɸ��� �ð�
    private bool cameraFixed = false;
    public GameObject[] characters;  //���� ���� ����
    private BossStagePlayer playerScript;
    public BossStageManager managerScript;

    void Start() 
    {
        /*for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].activeSelf)
            {
                playerScript = characters[i].GetComponent<BossStagePlayer>();
                player = characters[i].transform;
                break;
            }
        }*/
        playerScript = managerScript.ActiveCharacter().GetComponent<BossStagePlayer>();
        player = managerScript.ActiveCharacter().transform;

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

        while (elapsed < orbitAroundBossDuration)
        {
            // ���� ���
            float angle = (elapsed / orbitAroundBossDuration) * 360f; // 360�� ��ȸ
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

        // �÷��̾��� �̵� ������ ������ �߽� ���
        Vector3 areaCenter = (playerScript.areaMin + playerScript.areaMax) / 2f;

        // �÷��̾�� ���� �߽� ���� ���� ���
        Vector3 directionToCenter = (areaCenter - player.position).normalized;
        // �÷��̾��� ���� ���ߵ��� ī�޶� ��ġ ����
        Vector3 fixedPosition = player.position + directionToCenter*2 + new Vector3(0, 1f, 0); // �÷��̾� ���ʿ� ��ġ
        Quaternion fixedRotation = Quaternion.LookRotation(player.position + new Vector3(0, 0.5f, 0) - fixedPosition);

        // ī�޶� �̵� (�ε巴��)
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < transitionAroundPlayerSpinDuration)
        {
            transform.position = Vector3.Lerp(startPos, fixedPosition, elapsed / transitionAroundPlayerSpinDuration);
            transform.rotation = Quaternion.Slerp(startRot, fixedRotation, elapsed / transitionAroundPlayerSpinDuration);
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

    // 카메라 고정을 외부에서 설정할 수 있는 메서드
    public void SetCameraFixed(bool fixedStatus)
    {
        cameraFixed = fixedStatus;
    }

}
