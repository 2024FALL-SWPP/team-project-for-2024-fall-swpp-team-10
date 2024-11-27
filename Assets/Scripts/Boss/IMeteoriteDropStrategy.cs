using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeteoriteDropStrategy
{
    /// <summary>
    /// Meteorite Drop Strategy
    /// </summary>
    IEnumerator Execute(BossAttackPattern bossAttackPattern);
}
