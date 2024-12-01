using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EnumManager;
using NUnit.Framework;
using TMPro;
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

    [Test]
    public void TestResetStats()
    {
        // set value of a private variable
        FieldInfo lifeField = typeof(GameManager).GetField("life", BindingFlags.NonPublic | BindingFlags.Instance);
        lifeField.SetValue(gameManager, 1);

        FieldInfo scoreField = typeof(GameManager).GetField("score", BindingFlags.NonPublic | BindingFlags.Instance);
        lifeField.SetValue(gameManager, 50000);

        // call
        gameManager.ResetStats();

        // check
        Assert.AreEqual(gameManager.maxLife, gameManager.GetLife());
        Assert.AreEqual(0, gameManager.GetScore());
    }

    [Test]
    public void TestGetSetPlayerName()
    {
        // setup
        string myPlayerName = "MINJOON";

        GameObject playerNameObject = new GameObject();
        TextMeshProUGUI textMeshPro = playerNameObject.AddComponent<TextMeshProUGUI>();
        playerNameObject.tag = "PlayerName";
        textMeshPro.text = myPlayerName;

        // call
        gameManager.SetPlayerName();

        // check
        Assert.AreEqual(myPlayerName, gameManager.GetPlayerName());
    }

    [Test]
    public void TestGetSetStage()
    {
        int myStage = 2;

        // call
        gameManager.SetStage(myStage);

        // check
        Assert.AreEqual(myStage, gameManager.GetStage());
    }

    [Test]
    public void TestGetSetCharacter()
    {
        // setup
        gameManager.originColorSave = new Color[1, 1] { { Color.red } };
        Character myCharacter = Character.Minji;

        // call
        gameManager.SetCharacter(myCharacter);

        // check
        Assert.AreEqual(myCharacter, gameManager.GetCharacter());
    }

    [Test]
    public void TestAddRemoveLife()
    {
        // setup
        FieldInfo lifeField = typeof(GameManager).GetField("life", BindingFlags.NonPublic | BindingFlags.Instance);
        lifeField.SetValue(gameManager, 2);

        // call & check
        gameManager.AddLife(3);
        Assert.AreEqual(3, gameManager.GetLife());

        gameManager.AddLife(3);
        Assert.AreEqual(3, gameManager.GetLife());

        gameManager.RemoveLife();
        Assert.AreEqual(2, gameManager.GetLife());

        for (int i = 0; i < 5; i++)
        {
            gameManager.RemoveLife();
        }
        Assert.AreEqual(0, gameManager.GetLife());
    }

    [Test]
    public void TestGetAddScore()
    {
        Assert.AreEqual(0, gameManager.GetScore());

        gameManager.AddScore(10000);
        Assert.AreEqual(10000, gameManager.GetScore());

        gameManager.AddScore(-5000);
        Assert.AreEqual(5000, gameManager.GetScore());

        gameManager.AddScore(-10000);
        Assert.AreEqual(0, gameManager.GetScore());
    }
}
