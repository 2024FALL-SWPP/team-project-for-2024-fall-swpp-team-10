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
        previousButton.onClick.AddListener(GoToPreviousPage);
        nextButton.onClick.AddListener(GoToNextPage);
        ShowPage(currentPage);
        UpdateButtonStates();
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

    void ShowPage(int pageIndex)
    {
        for (int i = 0; i < tutorialPageGameObjects.Length; i++)
        {
            tutorialPageGameObjects[i].SetActive(i == pageIndex);
        }
        currentPage = pageIndex;
    }

    public void GoToPreviousPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
            UpdateButtonStates();
        }
    }

    public void GoToNextPage()
    {
        Debug.Log("Enetered next page");
        if (currentPage < tutorialPageGameObjects.Length - 1)
        {
            ShowPage(currentPage + 1);
            UpdateButtonStates();
        }
    }

    private void UpdateButtonStates()
    {
        // Disable previous button on first page
        if (previousButton != null)
            previousButton.gameObject.SetActive(currentPage > 0);

        // Disable next button on last page
        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPage < tutorialPageGameObjects.Length - 1);
    }
}
