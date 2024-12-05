using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MainStagePlayerTest
{
    private MainStageManager mainStageManager;
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private GameObject playerGameObject;
    private MainStagePlayer mainStagePlayer;

    [SetUp]
    public void Setup()
    {
        // Create a GameManager instance
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        GameManager.inst = gameManager;

        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
        mainStagePlayer = GameObject.FindObjectOfType<MainStagePlayer>();
    }

    [UnityTest]
    public IEnumerator TestLaserFire()
    {
        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
        mainStagePlayer = GameObject.FindObjectOfType<MainStagePlayer>();

        // 초기 프로젝타일 카운트 확인
        int initialProjectileCount = GameObject.FindGameObjectsWithTag("Laser").Length;
        yield return null;

        mainStagePlayer = GameObject.FindObjectOfType<MainStagePlayer>();
        mainStagePlayer.projectileSpawnPoint = mainStagePlayer.transform;

        // 레이저 발사
        mainStagePlayer.FireLaser();
        yield return new WaitForSeconds(0.1f);

        // 프로젝타일 증가 확인
        int newProjectileCount = GameObject.FindGameObjectsWithTag("Laser").Length;
        Assert.AreEqual(initialProjectileCount + 1, newProjectileCount);

        yield return new WaitForSeconds(1f);

        // 프로젝타일 파괴 확인
        newProjectileCount = GameObject.FindGameObjectsWithTag("Laser").Length;
        Assert.AreEqual(initialProjectileCount, newProjectileCount);
    }
}
