using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoverStage2 : MonoBehaviour
{
    public float speed = 1f; // 이동 속도 설정

    private void Start()
    {
        // 시작 위치 설정
        transform.position = new Vector3(0, 7, 43);
    }

    private void Update()
    {
        // x축 음의 방향으로 카메라를 서서히 이동
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
