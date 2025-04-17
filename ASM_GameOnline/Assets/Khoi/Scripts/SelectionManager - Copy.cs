using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public TMP_InputField nameInputField;

    public Button buttonCharacter1;
    public Button buttonCharacter2;
    void Start()
    {
        buttonCharacter1.onClick.AddListener(() => OnButtonClick("Character1"));
        buttonCharacter2.onClick.AddListener(() => OnButtonClick("Character2"));
    }

    // Update is called once per frame
    void OnButtonClick(string playerClass)
    {
        //đọc tên người chơi nhâp từ input frield
        var playerName = nameInputField.text;
        //Lưu thông tin người chơi
        PlayerPrefs.SetString("PlayerName",playerName);
        PlayerPrefs.SetString("PlayerClass", playerClass);
        //loadscene
        SceneManager.LoadScene("Main");
    }
}
