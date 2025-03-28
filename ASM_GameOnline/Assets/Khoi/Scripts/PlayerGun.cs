using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public NetworkRunner networkRunner;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (networkRunner is not null && networkRunner.LocalPlayer.IsRealPlayer)
            {
                var bullet = networkRunner.Spawn(bulletPrefab, firePoint.position, firePoint.rotation);
                var bulletDirection = firePoint.forward;
                bullet.GetComponent<Rigidbody>().AddForce(bulletDirection * 20f, ForceMode.Impulse);
            }
        }
    }
}