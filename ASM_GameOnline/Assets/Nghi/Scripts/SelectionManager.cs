using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public TMP_InputField nameInputField;

    public Button buttonMale;
    public Button buttonFemale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonFemale.onClick.AddListener(() => OnButtonClick("Female"));
        buttonMale.onClick.AddListener(() => OnButtonClick("Male"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClick(string playerClass)
    {
        //đọc tên người chơi từ Input Field 
        var playerName = nameInputField.text;
        //lưu thông tin nguời chơi 
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetString("PlayerClass", playerClass);
        SceneManager.LoadScene("Main");
    }
}
