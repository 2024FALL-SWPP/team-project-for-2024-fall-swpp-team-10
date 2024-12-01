using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameManagerTest
{
    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        // Initialize a new instance of GameManager before each test
        gameManager = new GameManager();
    }

    // A Test behaves as an ordinary method
    [UnityTest]
    public IEnumerator TestLoadMainMenu()
    {
        gameManager.LoadMainMenu();
        yield return null;

        Assert.AreEqual("MainMenuScene", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator TestLoadStageSelection()
    {
        gameManager.LoadStageSelection();
        yield return null;

        Assert.AreEqual("StageSelectionScene", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator TestLoadCharacterSelection()
    {
        gameManager.LoadCharacterSelection();
        yield return null;

        Assert.AreEqual("CharacterSelectionScene", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator TestLoadMainStage()
    {
        gameManager.SetStage(1);
        gameManager.LoadMainStage();
        yield return null;

        Assert.AreEqual("MainStage1Scene", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator TestLoadBossStage()
    {
        gameManager.SetStage(2);
        gameManager.LoadBossStage();
        yield return null;

        Assert.AreEqual("BossStage2Scene", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator TestLoadLeaderBoard()
    {
        gameManager.LoadLeaderboard();
        yield return null;

        Assert.AreEqual("LeaderBoardScene", SceneManager.GetActiveScene().name);
    }
}
