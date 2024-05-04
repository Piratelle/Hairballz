// Might not need this, implemented as part of movement controller
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }


    void LateUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10f);
    }
}