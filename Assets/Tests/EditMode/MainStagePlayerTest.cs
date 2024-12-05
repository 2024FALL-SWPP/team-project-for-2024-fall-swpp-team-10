using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MainStagePlayerTest
{
    private GameObject playerGameObject;
    private MainStagePlayer mainStagePlayer;

    [SetUp]
    public void Setup()
    {
        // 가상 플레이어 오브젝트 생성
        playerGameObject = new GameObject();
        mainStagePlayer = playerGameObject.AddComponent<MainStagePlayer>();

        // 가상 설정 초기화
        mainStagePlayer.moveSpeed = 10f;
        mainStagePlayer.invincibleLength = 5f;
    }

    [TearDown]
    public void Teardown()
    {
        // 오브젝트 정리
        Object.DestroyImmediate(playerGameObject);
    }

    [Test]
    public void TestInitialPositionAndGrid()
    {
        // 초기 그리드 위치 확인
        Assert.AreEqual(new Vector2Int(0, 0), mainStagePlayer.GetCurrentGridPosition());
    }

    [Test]
    public void TestMovementWithinGrid()
    {
        // 움직임 가능 범위 내에서 동작 확인
        mainStagePlayer.SetEnableKeys(true);
        // set value of a private variable 
        FieldInfo lifeField = typeof(MainStagePlayer).GetField("currentGridPosition", BindingFlags.NonPublic | BindingFlags.Instance);
        lifeField.SetValue(mainStagePlayer, new Vector2Int(1, 0));

        mainStagePlayer.StartCoroutine(mainStagePlayer.SmoothMove());
        Assert.AreEqual(new Vector2Int(1, 0), mainStagePlayer.GetCurrentGridPosition());
    }

    [Test]
    public void TestInvincibilityToggle()
    {
        // 무적 상태 변경 확인
        mainStagePlayer.SetIsInvincible(true);
        Assert.IsTrue(mainStagePlayer.GetIsInvincible());

        mainStagePlayer.SetIsInvincible(false);
        Assert.IsFalse(mainStagePlayer.GetIsInvincible());
    }

    [Test]
    public void TestTripleShotToggle()
    {
        // TripleShot 상태 변경 확인
        mainStagePlayer.SetTripleShot(true);
        Assert.IsTrue(mainStagePlayer.GetIsTripleShot());

        mainStagePlayer.SetTripleShot(false);
        Assert.IsFalse(mainStagePlayer.GetIsTripleShot());
    }
}
