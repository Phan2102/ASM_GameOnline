using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    private void Awake()
    {
        startGameButton = GameObject.Find("StartGame Button").GetComponent<Button>();
    }
    void Start()
    {
        //Invoke(nameof(LoadScene), 2);//Chờ 2 giây rồi mới chuyển Scene
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

    // Update is called once per frame
    void Update()
    {

    }
}
