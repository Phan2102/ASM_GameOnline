using UnityEngine;
using TMPro;
using Fusion;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform rankingPanel;
    [SerializeField] private GameObject rankingEntryPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            leaderboardPanel.SetActive(true);
            UpdateRanking();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            leaderboardPanel.SetActive(false);
        }

        if (Time.frameCount % 60 == 0) 
            UpdateRanking();
    }

    public void UpdateRanking()
    {
        foreach (Transform child in rankingPanel)
        {
            Destroy(child.gameObject);
        }

        // Lấy danh sách player hiện tại
        foreach (var player in FindObjectsOfType<PlayerProperties>())
        {
            var entry = Instantiate(rankingEntryPrefab, rankingPanel);
            var text = entry.GetComponent<TextMeshProUGUI>();
            //text.text = $"{player.NetworkedName} - {player.KillCount} kills";
        }

    }

}
