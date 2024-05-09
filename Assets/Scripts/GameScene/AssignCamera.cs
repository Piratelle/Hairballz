using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AssignCamera : NetworkBehaviour
{

    public Transform player;
    public GameObject playerCamera;
    // Start is called before the first frame update
    //void Start()
    //{
      //  player = NetworkManager.LocalClient.PlayerObject.transform;
    //}

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)
        {
            playerCamera.SetActive(false);
        }
    }
}
