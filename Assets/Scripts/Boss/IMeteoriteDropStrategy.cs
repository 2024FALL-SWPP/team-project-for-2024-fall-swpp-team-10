using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeteoriteDropStrategy
{
    /// <summary>
    /// Meteorite Drop Strategy
    /// </summary>
    /// <param name="bossAttackPattern">BossAttackPattern ����</param>
    IEnumerator Execute(BossAttackPattern bossAttackPattern);

    /// <summary>
    /// �׸��� ��ġ ��ȯ
    /// </summary>
    Vector3[] GetPatternPositions();
}
