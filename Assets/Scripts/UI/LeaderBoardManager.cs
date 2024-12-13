using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using EnumManager;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("Stage Info")]
    public GameObject stageText;
    private TextMeshProUGUI currentStage; //현재 스테이지
    private Stage stage;


    [Header("My Info")]
    public GameObject myScoreText;
    private TextMeshProUGUI myScore; //현재 점수
    public GameObject myIDText;
    private TextMeshProUGUI myID; //현재 ID

    [Header("Rank Info")]
    public GameObject[] rankIDText = new GameObject[5];
    private TextMeshProUGUI[] rankID = new TextMeshProUGUI[5]; //보이는 ID
    public GameObject[] rankScoreText = new GameObject[5];
    private TextMeshProUGUI[] rankScore = new TextMeshProUGUI[5]; //보이는 점수

    string[] rankExpressions = new string[] { "1st", "2nd", "3rd", "4th", "5th" };

    private int[] savedRankScore = new int[6]; //저장된 점수
    private string[] savedRankID = new string[6]; //저장된 ID

    [Header("Export")]
    public string m_FilePrefix = "PowerpuffBuns";

    [Header("Image to be shown when character gets unlocked")]
    public GameObject HanniUnlock;
    public GameObject HyeinUnlock;
    public GameObject MinjiUnlock;

    [Header("Sound Effects")]
    public AudioClip tadaF;
    public AudioClip tadaG;
    public AudioClip tadaA;
    public AudioClip screenshot;

    [Header("Buttons")]
    public Button selectStage;
    public Button restartGame;
    public Button exitGame;

    // Start is called before the first frame update
    void Awake()
    {
        stage = (Stage)GameManager.inst.GetStage();
        myScore = myScoreText.GetComponent<TextMeshProUGUI>();
        myID = myIDText.GetComponent<TextMeshProUGUI>();
        currentStage = stageText.GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < rankIDText.Length; i++)
        {
            rankID[i] = rankIDText[i].GetComponent<TextMeshProUGUI>();
            rankScore[i] = rankScoreText[i].GetComponent<TextMeshProUGUI>();
        }

        ImportRank();
        RankSort();
        ShowRank();
        RankSave();
        StartCoroutine(ShowCharacterUnlock());
    }

    // Update is called once per frame
    void Update()
    {
        // 디버그용
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.R))  //mac의 경우 shift + option + R
        {
            RemoveRank();
            ImportRank();
            ShowRank();
        }
    }

    private string PlayerPrefsScoreKey(int rank)
    {
        return stage + $"_{rankExpressions[rank - 1]}Score";
    }
    private string PlayerPrefsIDKey(int rank)
    {
        return stage + $"_{rankExpressions[rank - 1]}ID";
    }

    void RemoveRank()
    {
        for (int i = 1; i <= rankExpressions.Length; i++)
        {
            PlayerPrefs.DeleteKey(PlayerPrefsScoreKey(i));
            PlayerPrefs.DeleteKey(PlayerPrefsIDKey(i));
        }
    }

    void ImportRank()   //랭킹 점수 불러오기
    {
        for (int i = 0; i < rankExpressions.Length; i++)
        {
            savedRankScore[i] = PlayerPrefs.GetInt(PlayerPrefsScoreKey(i + 1));
            savedRankID[i] = PlayerPrefs.GetString(PlayerPrefsIDKey(i + 1));
        }
        myScore.text = GameManager.inst.GetScore().ToString();
        myID.text = GameManager.inst.GetPlayerName();
    }

    IEnumerator ShowCharacterUnlock()
    {
        ButtonInteractable(false);
        yield return new WaitForSeconds(0.5f);

        if (GameManager.inst.enemyKill > 35 && !GameManager.inst.IsUnlocked(Character.Hanni))
        {
            StartCoroutine(ShowUnlock(Character.Hanni, HanniUnlock, tadaF));
            yield return new WaitForSeconds(2.5f);
        }

        if (GameManager.inst.GetScore() > 200000 && !GameManager.inst.IsUnlocked(Character.Hyein))
        {
            StartCoroutine(ShowUnlock(Character.Hyein, HyeinUnlock, tadaG));
            yield return new WaitForSeconds(2.5f);
        }

        if (GameManager.inst.GetLife() > 3 && !GameManager.inst.IsUnlocked(Character.Minji)) //Main Stage는 Life가 최대 3개기 때문에 확인 불필요
        {
            StartCoroutine(ShowUnlock(Character.Minji, MinjiUnlock, tadaA));
            yield return new WaitForSeconds(2.5f);
        }
        yield return null;
        ButtonInteractable(true);
    }

    void ButtonInteractable(bool buttonInteractable)
    {
        selectStage.interactable = buttonInteractable;
        restartGame.interactable = buttonInteractable;
        exitGame.interactable = buttonInteractable;
    }

    IEnumerator ShowUnlock(Character _character, GameObject _gameObject, AudioClip tada)
    {
        GameManager.inst.SetPlayerUnlockPrefs(_character);
        _gameObject.SetActive(true);
        AudioSource.PlayClipAtPoint(tada, transform.position);
        yield return new WaitForSeconds(2f);
        _gameObject.SetActive(false);
    }

    void RankSort()   //현재점수와 랭킹점수 비교 및 정렬
    {
        for (int i = 0; i < rankScore.Length; i++)
        {
            if (GameManager.inst.GetScore() > savedRankScore[i]) //i+1등보다 점수가 높으면
            {
                for (int j = rankScore.Length - 1; j >= i; j--)
                {
                    savedRankID[j + 1] = savedRankID[j];
                    savedRankScore[j + 1] = savedRankScore[j];
                }
                savedRankID[i] = myID.text;
                savedRankScore[i] = GameManager.inst.GetScore();
                break;
            }
        }
    }

    void ShowRank()   //화면에 랭킹 개시
    {
        currentStage.text = stage.ToString() + " Ranking";
        for (int i = 0; i < rankScore.Length; i++)
        {
            rankID[i].text = savedRankID[i];
            rankScore[i].text = savedRankScore[i].ToString();
        }
    }

    void RankSave()   //랭킹 점수 저장
    {
        for (int i = 0; i < rankExpressions.Length; i++)
        {
            PlayerPrefs.SetInt(PlayerPrefsScoreKey(i + 1), savedRankScore[i]);
            PlayerPrefs.Save();
            PlayerPrefs.SetString(PlayerPrefsIDKey(i + 1), savedRankID[i]);
            PlayerPrefs.Save();
        }
    }

    public void Export()
    {
        string fileName = m_FilePrefix + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".jpg";
        AudioSource.PlayClipAtPoint(screenshot, transform.position);
        StartCoroutine(SaveScreenJpg(fileName));
    }

    IEnumerator SaveScreenJpg(string fileName)
    {
        yield return new WaitForEndOfFrame();

        // 스크린샷 캡처
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        byte[] bytes = texture.EncodeToJPG();

        // WebGL 환경 확인
#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadScreenshot(fileName, bytes);
#else
        string filePath = Application.persistentDataPath + "/" + fileName;
        System.IO.File.WriteAllBytes(filePath, bytes);
        Debug.Log("Screenshot saved to: " + filePath);
#endif

        DestroyImmediate(texture);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadScreenshot(string fileName, byte[] imageData);
#endif

}
