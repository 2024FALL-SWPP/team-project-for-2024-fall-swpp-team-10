using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class BossControlTest
{
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private GameObject bossControlObject;
    private BossControl bossControl;
    private GameObject bossStageManagerObject;
    private BossStageManager bossStageManager;

    [SetUp]
    public void Setup()
    {
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        GameManager.inst = gameManager;

        gameManager.maxLife = 3;
        gameManager.bossStageMaxLife = 5;
        gameManager.ResetStats();
        gameManager.AddLife(gameManager.bossStageMaxLife);

        bossControlObject = new GameObject("BossControl");
        bossControl = bossControlObject.AddComponent<BossControl>();

        bossStageManagerObject = new GameObject("BossStageManager");
        bossStageManager = bossStageManagerObject.AddComponent<BossStageManager>();

        InitializeStageManagerFields();
        InitializeBossStageManagerFields();
        InitializeBossControlFields();

        Debug.Log(GetProtectedField(bossStageManager, "activeCharacter"));
        bossStageManager.bossControlScript = bossControl;

        InvokeNonPublicMethod(bossControl, "Awake");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(bossControlObject);
        Object.DestroyImmediate(gameManagerObject);
        GameManager.inst = null;
    }

    private void InitializeStageManagerFields()
    {
        bossStageManager.characters = new GameObject[1];
        bossStageManager.characters[0] = new GameObject("Character0");
        bossStageManager.characters[0].AddComponent<BossStagePlayer>();

        bossStageManager.characterUI = new GameObject[1];
        bossStageManager.characterUI[0] = new GameObject("CharacterUI0");

        SetProtectedField(bossStageManager, "activeCharacter", bossStageManager.characters[0]);

        bossStageManager.pauseMenu = new GameObject("Pause");
        bossStageManager.pauseMenu.SetActive(false);

        bossStageManager.gameOverScreen = new GameObject("GameOver");
        bossStageManager.gameOverScreen.SetActive(false);

        bossStageManager.score = new GameObject("Score");
        bossStageManager.score.AddComponent<TextMeshProUGUI>();

        bossStageManager.hearts = new GameObject[gameManager.bossStageMaxLife];
        for (int i = 0; i < bossStageManager.hearts.Length; i++)
        {
            bossStageManager.hearts[i] = new GameObject($"Heart{i}");
            bossStageManager.hearts[i].SetActive(true);
        }

        MusicManager musicManager = new GameObject("MusicManager").AddComponent<MusicManager>();
        BossStageTransitionManager transitionManager = new GameObject("BossStageTransitionManager").AddComponent<BossStageTransitionManager>();
    }

    private void InitializeBossStageManagerFields()
    {
        bossStageManager.darkHearts = new GameObject[bossStageManager.GetBossMaxLife()];
        for (int i = 0; i < bossStageManager.darkHearts.Length; i++)
        {
            bossStageManager.darkHearts[i] = new GameObject($"DarkHeart{i}");
            bossStageManager.darkHearts[i].SetActive(true);
        }

        bossStageManager.gameClear = new GameObject("GameClear");
        bossStageManager.gameClear.SetActive(false);

        bossStageManager.bossControlScript = new BossControl();
        bossStageManager.cameraScript = new BossStageCamera();
        bossStageManager.weakspotsManagerScript = new WeakspotsManager();

        GameObject fire1 = new GameObject("Fire1") { tag = "Fire" };
        GameObject fire2 = new GameObject("Fire2") { tag = "Fire" };

        GameClearLight gameClearLight = bossStageManagerObject.AddComponent<GameClearLight>();
    }

    private void InitializeBossControlFields()
    {
        SetPrivateField(bossControl, "bossStartColor", Color.red);
        SetPrivateField(bossControl, "myColorRed", new Color(203f / 255f, 83f / 255f, 83f / 255f, 1));

        GameObject bossObject = new GameObject("BossObject");
        SetPrivateField(bossControl, "bossTransform", bossObject.transform);
        SetPrivateField(bossControl, "bossHorizontalRange", 10);
        SetPrivateField(bossControl, "bossHorizontalSpeed", 3);

        GameObject particleSystemObject = new GameObject("BossSmoke");
        ParticleSystem particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
        SetPrivateField(bossControl, "bossSmoke", particleSystem);
        SetPrivateField(bossControl, "bossReducedSize", 0.4f);


        var bossTransform = (Transform)GetPrivateField(bossControl, "bossTransform");
        GameObject child = new GameObject("Child");
        child.AddComponent<Rigidbody>();
        child.GetComponent<Rigidbody>().useGravity = false;
        child.GetComponent<Rigidbody>().isKinematic = true;
        child.transform.SetParent(bossTransform);
        Vector3 initialPosition = new Vector3(1, 1, 1);
        Quaternion initialRotation = Quaternion.Euler(30, 30, 30);
        child.transform.localPosition = initialPosition;
        child.transform.localRotation = initialRotation;

        Dictionary<Transform, Vector3> positions = new Dictionary<Transform, Vector3>
        {
            { child.transform, initialPosition }
        };
        Dictionary<Transform, Quaternion> rotations = new Dictionary<Transform, Quaternion>
        {
            { child.transform, initialRotation }
        };


        SetPrivateField(bossControl, "bossComponentsInitialPositions", positions);
        SetPrivateField(bossControl, "bossComponentsInitialRotations", rotations);

        SetPrivateField(bossControl, "bossStageManager", bossStageManager);
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(obj, value);
    }

    private void SetProtectedField(object obj, string fieldName, object value)
    {
        FieldInfo field = obj.GetType().BaseType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(obj, value);
    }

    private dynamic GetPrivateField(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
        {
            Debug.LogError($"Field '{fieldName}' does not exist in '{obj.GetType().Name}'");
            throw new System.NullReferenceException($"Field '{fieldName}' does not exist.");
        }
        return field.GetValue(obj);
    }

    private dynamic GetProtectedField(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().BaseType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return field.GetValue(obj);
    }

    private void InvokeNonPublicMethod(object obj, string methodName)
    {
        MethodInfo method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(obj, null);
    }

    [Test]
    public void Test_ColorChangeOnInitialization()
    {
        var bossStartColor = (Color)GetPrivateField(bossControl, "bossStartColor");

        var bossColorDict = (Dictionary<Color, Color>)GetPrivateField(bossControl, "myColorDict");
        Color actualColor = bossColorDict.GetValueOrDefault(bossStartColor, bossStartColor);

        Assert.AreEqual(new Color(203f / 255f, 83f / 255f, 83f / 255f, 1), actualColor);
    }

    [UnityTest]
    public IEnumerator Test_BossMovement()
    {
        SetPrivateField(bossControl, "bossHorizontalPos", 5f);

        Time.timeScale = 1f;

        InvokeNonPublicMethod(bossControl, "Start");
        Debug.Log(GetPrivateField(bossControl, "playerTransform"));

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            bossControl.HandleBossMovement();
            elapsedTime += 0.02f;
            yield return null;
        }

        var bossTransform = (Transform)GetPrivateField(bossControl, "bossTransform");

        Assert.AreNotEqual(1f, bossTransform.position.x);
    }

    [Test]
    public void Test_BossDeathSequence()
    {
        bossControl.BossDeath();
        var bossDead = (bool)GetPrivateField(bossControl, "bossDead");

        Assert.IsTrue(bossDead);
    }

    [Test]
    public void Test_ResetToInitialPositionAndRotation()
    {
        var bossTransform = (Transform)GetPrivateField(bossControl, "bossTransform");
        Transform child = bossTransform.GetChild(0);

        InvokeNonPublicMethod(bossControl, "BossDeathTransform");

        Dictionary<Transform, Vector3> initPos = GetPrivateField(bossControl, "bossComponentsInitialPositions");
        Dictionary<Transform, Quaternion> initRot = GetPrivateField(bossControl, "bossComponentsInitialRotations");

        Assert.AreEqual(initPos[child], child.localPosition);
        Assert.AreEqual(initRot[child], child.localRotation);
    }
}