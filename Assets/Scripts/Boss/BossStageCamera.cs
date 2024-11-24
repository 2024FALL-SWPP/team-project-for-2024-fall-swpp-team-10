using UnityEngine;
using System.Collections;

public class BossStageCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    private Transform player; // 플레이어의 Transform
    public Vector3 offset = new Vector3(0, 3, -5); // 뒤쪽과 위쪽으로의 거리
    public float followSpeed = 5f;
    public float pitch = 30f; // 카메라의 X축 기울기 (위쪽으로 약간 더 기울기)
    public Transform bossTransform; // 보스 Transform
    public float orbitAroundBossDuration = 3f; // 보스 주위를 순회하는 데 걸리는 시간
    private bool isOrbiting = false;
    public float transitionAroundPlayerSpinDuration = 1f; //플레이어가 회전하는데에 걸리는 시간
    private bool cameraFixed = false;
    public GameObject[] characters;  //추후 삭제 예정
    private BossStagePlayer playerScript;
    public BossStageManager managerScript;

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
        /*playerScript = managerScript.characters[(int)GameManager.inst.GetCharacter()].GetComponent<BossStagePlayer>();
        player = managerScript.characters[(int)GameManager.inst.GetCharacter()].transform;*/

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

    // 보스 주위를 순회하는 코루틴
    public IEnumerator OrbitAroundBoss()
    {
        isOrbiting = true;

        // 슬로우 모션 적용은 이미 BossStageManager에서 처리됨

        float elapsed = 0f;

        while (elapsed < orbitAroundBossDuration)
        {
            // 각도 계산
            float angle = (elapsed / orbitAroundBossDuration) * 360f; // 360도 순회
            Vector3 orbitPosition = bossTransform.position + new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) *(-8),
                15,
                Mathf.Sin(Mathf.Deg2Rad * angle) * (-8)
            );

            // 카메라 위치 및 보스 바라보기
            transform.position = orbitPosition;
            transform.LookAt(bossTransform); // 항상 보스를 바라봄

            elapsed += Time.unscaledDeltaTime; // 슬로우 모션에서도 정상적으로 진행되도록 unscaledDeltaTime 사용
            yield return null;
        }

        isOrbiting = false;
    }

    // 카메라를 플레이어에게 고정하는 메서드
    public IEnumerator FixCameraOnPlayerDuringSpin()
    {
        cameraFixed = true;

        // 플레이어의 얼굴을 비추도록 카메라 위치 조정
        Vector3 fixedPosition = player.position + new Vector3(0, 0.5f, -2f); // 플레이어 앞쪽에 위치
        Quaternion fixedRotation = Quaternion.LookRotation(player.position - fixedPosition);

        // 카메라 이동 (부드럽게)
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

        // 플레이어가 회전하는 동안 카메라 고정
        while (playerScript.IsSpinning())
        {
            // 카메라는 고정된 상태로 유지
            yield return null;
        }

        // 플레이어 회전이 끝나면 카메라 원상복구
        ResetCamera();
    }

    // 카메라를 원래 상태로 복구하는 메서드
    public void ResetCamera()
    {
        cameraFixed = false;
    }

}
