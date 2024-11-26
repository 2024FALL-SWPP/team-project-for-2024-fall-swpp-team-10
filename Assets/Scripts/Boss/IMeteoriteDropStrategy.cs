using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeteoriteDropStrategy
{
    /// <summary>
    /// Meteorite Drop Strategy
    /// </summary>
    /// <param name="bossAttackPattern">BossAttackPattern 참조</param>
    IEnumerator Execute(BossAttackPattern bossAttackPattern);

    /// <summary>
    /// 그리드 위치 반환
    /// </summary>
    Vector3[] GetPatternPositions();
}
