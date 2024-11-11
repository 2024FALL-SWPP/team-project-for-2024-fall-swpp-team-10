using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteExplosion : MonoBehaviour
{
    public GameObject explosionPrefab; // Particle System 프리팹

    public void TriggerExplosion(Vector3 position)
    {
        // 폭발 프리팹을 특정 위치에 생성
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);

        // 파티클 효과의 지속 시간이 지나면 오브젝트 파괴
        Destroy(explosion, 2f); // 2초 후 파괴
    }
}

