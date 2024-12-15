using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

public class StartButtonController : MonoBehaviour
{
    [SerializeField] TMP_InputField playerName;
    Button btnStart;
    string validPrev;
    float elapsedTimeSinceLastMotion;
    float timeToPlayIntro = 30;

    // Start is called before the first frame update
    void Start()
    {
        btnStart = gameObject.GetComponent<Button>();
        playerName.onValueChanged.AddListener(delegate { ValidatePlayerName(); });
        validPrev = "";
        elapsedTimeSinceLastMotion = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && btnStart.interactable)
        {
            GameManager.inst.SetPlayerName();
            GameManager.inst.LoadStageSelection();
        }

        // if any key pressed or mouse moved, reset the elapsed time
        if (Input.anyKeyDown || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            elapsedTimeSinceLastMotion = 0;
        }
        else
        {
            elapsedTimeSinceLastMotion += Time.deltaTime;
        }

        // if user motion not detected for a long time, play the intro
        if (elapsedTimeSinceLastMotion >= timeToPlayIntro)
        {
            GameManager.inst.LoadIntro();
        }
    }

    private void ValidatePlayerName()
    {
        playerName.text = playerName.text.Trim().ToUpper();
        //playerName.text = playerName.text.ToUpper();
        btnStart.interactable = playerName.text != "" && Regex.IsMatch(playerName.text, @"^[a-zA-Z0-9]{1,13}$");
        if (!btnStart.interactable && playerName.text != "")
            playerName.text = validPrev;
        else
            validPrev = playerName.text;
    }
}
