using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialScreenManager : MonoBehaviour
{
    public GameObject[] tutorialPageGameObjects;
    public Button previousButton;
    public Button nextButton;

    private int currentPage = 0;
    public Sprite[] stageBackgrounds;
    public GameObject background;

    private Image backgroundImage;

    void Start()
    {
        SetTutorialImages();

        UpdateTutorialPage();
        previousButton.onClick.AddListener(GoToPreviousPage);
        nextButton.onClick.AddListener(GoToNextPage);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        backgroundImage = background.GetComponent<Image>();
        backgroundImage.sprite = stageBackgrounds[GameManager.inst.GetStage() - 1];
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void SetTutorialImages()
    {
        for (int i = 0; i < tutorialPageGameObjects.Length; i++)
        {
            // example of tutorial file name: Stage1Tutorial2.png ("second image of Stage 1 Tutorial")
            string tutorialFilename = "Stage" + GameManager.inst.GetStage() + "Tutorial" + (i + 1);
            tutorialPageGameObjects[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(tutorialFilename);
        }
    }

    void UpdateTutorialPage()
    {
        for (int i = 0; i < tutorialPageGameObjects.Length; i++)
        {
            tutorialPageGameObjects[i].SetActive(i == currentPage);
        }

        previousButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < tutorialPageGameObjects.Length - 1;
    }

    public void GoToPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateTutorialPage();
        }
    }

    public void GoToNextPage()
    {
        if (currentPage < tutorialPageGameObjects.Length - 1)
        {
            currentPage++;
            UpdateTutorialPage();
        }
    }
}
