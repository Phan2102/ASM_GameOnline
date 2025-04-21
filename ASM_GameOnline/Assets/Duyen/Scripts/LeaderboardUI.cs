using UnityEngine;
using TMPro;
using System.Linq;
using static Unity.Collections.Unicode;
using static UnityEngine.EventSystems.EventTrigger;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform entryParent;
    [SerializeField] private GameObject entryPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            leaderboardPanel.SetActive(true);
            UpdateLeaderboard();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            leaderboardPanel.SetActive(false);
        }
    }

    public void UpdateLeaderboard()
    {
        /*if (player.Object.InputAuthority == Runner.LocalPlayer)
        {
            entry.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }


        foreach (Transform child in entryParent)
        {
            Destroy(child.gameObject);
        }

        var players = FindObjectsOfType<PlayerNetwork>()
                      .OrderByDescending(p => p.KillCount);

        foreach (var player in players)
        {
            GameObject entry = Instantiate(entryPrefab, entryParent);
            entry.GetComponent<TextMeshProUGUI>().text = $"{player.PlayerName} - {player.KillCount} kills";
        }*/
    }
}
