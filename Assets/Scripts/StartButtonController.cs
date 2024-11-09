using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class StartButtonController : MonoBehaviour
{
    [SerializeField] TMP_InputField playerName;
    Button btnStart;
    string validPrev;

    // Start is called before the first frame update
    void Start()
    {
        btnStart = gameObject.GetComponent<Button>();
        playerName.onValueChanged.AddListener(delegate { ValidatePlayerName(); });
        btnStart.onClick.AddListener(SetPlayerName);
        validPrev = "";
    }

    // Update is called once per frame
    void Update()
    {
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

    public void SetPlayerName()
    {
        GameManager.inst.SetPlayerName(GameObject.FindWithTag("PlayerName").GetComponent<TextMeshProUGUI>().text);
    }
}
