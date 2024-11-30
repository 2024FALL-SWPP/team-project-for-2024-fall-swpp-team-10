using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTest
{
    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        // Initialize a new instance of GameManager before each test
        gameManager = GameManager.inst;
    }

    // A Test behaves as an ordinary method
    [Test]
    public void GameManagerTestSimplePasses()
    {
        Assert.AreEqual(true, true);
    }

    [Test]
    public void GameManagerTestSimpleFails()
    {
        Assert.AreEqual(true, false);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator GameManagerTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
