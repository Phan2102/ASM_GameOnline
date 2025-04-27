using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class RankingManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI rankingText;
    private List<PlayerProperties> players;

    private void Start()
    {
        players = new List<PlayerProperties>();
    }

    // Thêm người chơi vào danh sách bảng xếp hạng
    public void AddPlayer(PlayerProperties player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            UpdateRanking();
        }
    }

    // Cập nhật bảng xếp hạng
    private void UpdateRanking()
    {
        // Sắp xếp theo số kill giảm dần
        players.Sort((p1, p2) => p2.killCount.CompareTo(p1.killCount));

        string ranking = "";
        for (int i = 0; i < players.Count; i++)
        {
            ranking += $"{i + 1}. {players[i].NetworkedName} - Kills: {players[i].killCount}\n";
        }

        rankingText.text = ranking;
    }

    // Cập nhật bảng xếp hạng khi có sự thay đổi về số kill
    public void OnKillCountChanged()
    {
        UpdateRanking();
    }
}
