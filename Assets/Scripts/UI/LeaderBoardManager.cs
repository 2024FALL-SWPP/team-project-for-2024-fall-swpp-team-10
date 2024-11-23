using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

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

    private int[] savedRankScore = new int[6]; //저장된 점수
    private string[] savedRankID = new string[6]; //저장된 ID

    [Header("Export")]
    public string m_Path = @"C:\tmp\";
    public string m_FilePrefix = "PowerpuffBuns";
    private string m_FilePath;


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
    }

    // Update is called once per frame
    void Update()
    {
        // 디버그용
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RemoveRank();
            ImportRank();
            ShowRank();
        }
    }

    private string PlayerPrefsScoreKey(int rank)
    {
        string[] rankExpressions = new string[] { "1st", "2nd", "3rd", "4th", "5th" };
        return stage + $"_{rankExpressions[rank - 1]}Score";
    }
    private string PlayerPrefsIDKey(int rank)
    {
        string[] rankExpressions = new string[] { "1st", "2nd", "3rd", "4th", "5th" };
        return stage + $"_{rankExpressions[rank - 1]}ID";
    }

    void RemoveRank()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsScoreKey(1));
        PlayerPrefs.DeleteKey(PlayerPrefsIDKey(1));
        PlayerPrefs.DeleteKey(PlayerPrefsScoreKey(2));
        PlayerPrefs.DeleteKey(PlayerPrefsIDKey(2));
        PlayerPrefs.DeleteKey(PlayerPrefsScoreKey(3));
        PlayerPrefs.DeleteKey(PlayerPrefsIDKey(3));
        PlayerPrefs.DeleteKey(PlayerPrefsScoreKey(4));
        PlayerPrefs.DeleteKey(PlayerPrefsIDKey(4));
        PlayerPrefs.DeleteKey(PlayerPrefsScoreKey(5));
        PlayerPrefs.DeleteKey(PlayerPrefsIDKey(5));
    }

    void ImportRank()   //랭킹 점수 불러오기
    {
        savedRankScore[0] = PlayerPrefs.GetInt(PlayerPrefsScoreKey(1));
        savedRankID[0] = PlayerPrefs.GetString(PlayerPrefsIDKey(1));
        savedRankScore[1] = PlayerPrefs.GetInt(PlayerPrefsScoreKey(2));
        savedRankID[1] = PlayerPrefs.GetString(PlayerPrefsIDKey(2));
        savedRankScore[2] = PlayerPrefs.GetInt(PlayerPrefsScoreKey(3));
        savedRankID[2] = PlayerPrefs.GetString(PlayerPrefsIDKey(3));
        savedRankScore[3] = PlayerPrefs.GetInt(PlayerPrefsScoreKey(4));
        savedRankID[3] = PlayerPrefs.GetString(PlayerPrefsIDKey(4));
        savedRankScore[4] = PlayerPrefs.GetInt(PlayerPrefsScoreKey(5));
        savedRankID[4] = PlayerPrefs.GetString(PlayerPrefsIDKey(5));
        myScore.text = GameManager.inst.GetScore().ToString();
        myID.text = GameManager.inst.GetPlayerName();
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
        PlayerPrefs.SetInt(PlayerPrefsScoreKey(1), savedRankScore[0]);
        PlayerPrefs.SetString(PlayerPrefsIDKey(1), savedRankID[0]);
        PlayerPrefs.SetInt(PlayerPrefsScoreKey(2), savedRankScore[1]);
        PlayerPrefs.SetString(PlayerPrefsIDKey(2), savedRankID[1]);
        PlayerPrefs.SetInt(PlayerPrefsScoreKey(3), savedRankScore[2]);
        PlayerPrefs.SetString(PlayerPrefsIDKey(3), savedRankID[2]);
        PlayerPrefs.SetInt(PlayerPrefsScoreKey(4), savedRankScore[3]);
        PlayerPrefs.SetString(PlayerPrefsIDKey(4), savedRankID[3]);
        PlayerPrefs.SetInt(PlayerPrefsScoreKey(5), savedRankScore[4]);
        PlayerPrefs.SetString(PlayerPrefsIDKey(5), savedRankID[4]);
    }

    public void Export()
    {
        m_FilePath = m_Path + m_FilePrefix + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".jpg";
        StartCoroutine(SaveScreenJpg(m_FilePath));
    }

    IEnumerator SaveScreenJpg(string filePath)
    {
        yield return new WaitForEndOfFrame();

        Texture2D texture = new Texture2D(Screen.width, Screen.height);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        byte[] bytes = texture.EncodeToJPG();
        File.WriteAllBytes(filePath, bytes);
        DestroyImmediate(texture);
    }

}
