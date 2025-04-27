using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject settingsMenu; 
    [SerializeField] private Slider volumeSlider;    

    private void Awake()
    {
        
    }

    void Start()
    {
        // Đảm bảo rằng các button đã được gán
        if (startGameButton != null)
            startGameButton.onClick.AddListener(OnClickStartGameButton);

        if (settingButton != null)
            settingButton.onClick.AddListener(OnClickSettingButton);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnClickExitButton);

        // Set âm lượng ban đầu
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
            AudioListener.volume = volumeSlider.value;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void OnClickStartGameButton()
    {
        if (startGameButton != null)
        {
            Invoke(nameof(LoadScene), 1);
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene("Selection");
    }

    public void OnClickSettingButton()
    {
        settingsMenu.SetActive(true);

    }

    public void OnClickBackButton()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false); 
        }
    }
    public void OnClickExitButton()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // Nếu đang chạy trên bản build, thoát ứng dụng
                Application.Quit();
        #endif
    }

    public void OnVolumeChanged(float value)
    {
        // Lưu âm lượng khi thay đổi
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }

    void Update()
    {
        // Thực hiện các hành động khác nếu cần
    }
}
