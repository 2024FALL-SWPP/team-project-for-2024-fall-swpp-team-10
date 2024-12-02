using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MainStageManagerTest
{
    private MainStageManager mainStageManager;

    [SetUp]
    public void Setup()
    {
        Debug.Log(GameObject.FindAnyObjectByType<Rigidbody>());
    }

    [UnityTest]
    public IEnumerator TestTest()
    {
        yield return null;
    }
}
