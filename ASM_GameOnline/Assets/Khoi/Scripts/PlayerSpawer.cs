using UnityEngine;
using Fusion;

// class này dùng để spawn player vào game trong network
public class PlayerSpawer2D : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab2D; // Sử dụng prefab 2D
    // khi vào mạng thì tạo nhân vật cho người chơi
    public void PlayerJoined(PlayerRef player)
    {
        // kiểm tra xem người này có phải là người chơi đang chơi không
        if (player == Runner.LocalPlayer)
        {
            //tạo nhân vật ở vị trí (0, 2, 0) - chuyển thành 2D là (0, 2)
            // gọi APU lấy thông tin Player
            var position2D = new Vector2(0, 2); // Sử dụng Vector2 cho vị trí 2D
            var position3D = new Vector3(position2D.x, position2D.y, 0); // Tạo Vector3 từ Vector2 với Z = 0
            // spawn nhân vật ở vị trí này
            Runner.Spawn(PlayerPrefab2D, position3D, Quaternion.identity, Runner.LocalPlayer, (runner, obj) =>
            {
                var playersetup = obj.GetComponent<PlayerSetup>(); // Có thể cần PlayerSetup2D
                if (playersetup != null) playersetup.SetupCamera();

                var bullet = obj.GetComponent<PlayerGun>(); // Có thể cần PlayerGun2D
                if (bullet != null)
                {
                    bullet.networkRunner = runner;
                }
            }
            );
        };


    }
}