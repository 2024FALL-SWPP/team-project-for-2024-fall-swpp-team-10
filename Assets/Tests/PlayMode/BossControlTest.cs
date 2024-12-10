using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using EnumManager;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class BossControlTest
{
    private BossControl bossControl;
    private GameObject bossObject;
    private GameObject playerObject;

    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        GameManager.inst = gameManager;

        gameManager.maxLife = 3;
        gameManager.bossStageMaxLife = 5;
        gameManager.SetCharacter(Character.Minji);
        gameManager.SetStage(1);
    }

    private object GetPrivateField(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
        {
            Debug.LogError($"Field '{fieldName}' does not exist in '{obj.GetType().Name}'");
            throw new System.NullReferenceException($"Field '{fieldName}' does not exist.");
        }
        return field.GetValue(obj);
    }

    [TearDown]
    public void Teardown()
    {
        if (bossObject != null && bossObject.gameObject != null)
        {
            Object.DestroyImmediate(bossObject);
        }

        if (playerObject != null && playerObject.gameObject != null)
        {
            Object.DestroyImmediate(playerObject);
        }

        var remainingObjects = Object.FindObjectsOfType<GameObject>();
        foreach (var obj in remainingObjects)
        {
            if (obj != null && obj.name != "TestRunner")
            {
                Object.DestroyImmediate(obj);
            }
        }

        if (GameManager.inst != null)
        {
            Object.DestroyImmediate(GameManager.inst.gameObject);
            GameManager.inst = null;
        }
    }

    public IEnumerator InitializeBossControl()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        bossObject = GameObject.Find("Boss");
        Assert.IsNotNull(bossObject);
        bossControl = bossObject.GetComponent<BossControl>();
        Assert.IsNotNull(bossControl);

        playerObject = Object.FindObjectOfType<StageManager>().ActiveCharacter();
        Assert.IsNotNull(playerObject);
    }

    [UnityTest]
    public IEnumerator TestBossInitialization()
    {
        yield return InitializeBossControl();

        Transform bossTransform = bossObject.transform;
        Assert.IsNotNull(bossTransform);
    }

    [UnityTest]
    public IEnumerator TestBossMovesTowardTarget()
    {
        yield return InitializeBossControl();

        Assert.IsNotNull(bossObject);
        Assert.IsNotNull(playerObject);
        Assert.IsNotNull(playerObject.transform);

        bossObject.transform.position = Vector3.zero;

        Assert.IsNotNull(bossObject.transform);
        BossStageManager bossStageManager = Object.FindObjectOfType<BossStageManager>();
        float timeToWait = (float)GetPrivateField(bossStageManager, "introAnimationDuration") + 0.001f;
        yield return new WaitForSeconds(timeToWait);
        float targetPos = (float)GetPrivateField(bossControl, "bossHorizontalPos");
        float initialDistance = Mathf.Abs(targetPos - bossObject.transform.position.x);

        yield return new WaitForSeconds(0.1f);

        Assert.Less(Mathf.Abs(bossObject.transform.position.x - targetPos), initialDistance);
    }

    [UnityTest]
    public IEnumerator TestBossLooksAtPlayer()
    {
        yield return InitializeBossControl();

        Assert.IsNotNull(bossObject);
        Assert.IsNotNull(playerObject);
        Assert.IsNotNull(bossObject.transform);
        Assert.IsNotNull(playerObject.transform);

        Vector3 directionToPlayer = (playerObject.transform.position - bossObject.transform.position).normalized;
        Vector3 bossForward = bossObject.transform.forward;

        Assert.That(Vector3.Dot(directionToPlayer, bossForward), Is.GreaterThan(0.9f));
    }

    [UnityTest]
    public IEnumerator TestBossDeathSequence()
    {
        yield return InitializeBossControl();

        float initialBossX = bossObject.transform.localScale.x;

        bossControl.BossDeath();

        yield return new WaitForSeconds(2.5f);

        Assert.IsTrue(bossControl.IsDead());
        Assert.IsTrue(((ParticleSystem)GetPrivateField(bossControl, "bossSmoke")).isPlaying);
        Assert.AreEqual((float)GetPrivateField(bossControl, "bossReducedSize"), bossObject.transform.localScale.x / initialBossX, 0.01f);
    }
}
