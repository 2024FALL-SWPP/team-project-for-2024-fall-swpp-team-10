using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialScreenManager : MonoBehaviour
{
    [Header("Tutorial Pages")]
    public GameObject[] supernaturalTutorialPages;  // Array for Supernatural tutorial pages
    public GameObject[] dittoTutorialPages;         // Array for Ditto tutorial pages
    public Button previousButton;
    public Button nextButton;

    private GameObject[] currentTutorialPages;
    private int currentPage = 0;
    public Sprite[] stageBackgrounds;
    public GameObject background;

    private Image backgroundImage;
    void Start()
    {
        SetupTutorialPages();
        ShowPage(currentPage);
        UpdateButtonStates();
    }

    private void SetupTutorialPages()
    {
        // Get the current stage from GameManager
        int currentStage = GameManager.inst.GetStage();

        // Set the active tutorial pages based on stage
        if (currentStage == 1) // Supernatural
        {
            currentTutorialPages = supernaturalTutorialPages;
            // Deactivate Ditto pages
            foreach (var page in dittoTutorialPages)
            {
                page.SetActive(false);
            }
        }
        else // Ditto
        {
            currentTutorialPages = dittoTutorialPages;
            // Deactivate Supernatural pages
            foreach (var page in supernaturalTutorialPages)
            {
                page.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        backgroundImage = background.GetComponent<Image>();
        backgroundImage.sprite = stageBackgrounds[GameManager.inst.GetStage() - 1];
        GameManager.inst.SetSelected(false);
        Time.timeScale = 1;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void ShowPage(int pageIndex)
    {
        for (int i = 0; i < currentTutorialPages.Length; i++)
        {
            currentTutorialPages[i].SetActive(i == pageIndex);
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
        if (currentPage < currentTutorialPages.Length - 1)
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
            nextButton.gameObject.SetActive(currentPage < currentTutorialPages.Length - 1);
    }
}

