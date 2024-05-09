using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;
using UnityEngine.EventSystems;
using System;

public class NewBombController : NetworkBehaviour
{
    public event EventHandler OnplayerPlacedBomb;

    [Header("Bomb")]
    public GameObject bombPrefeb;
    public KeyCode inputKey = KeyCode.Space;
    public float bombFuseTime = 3f;
    public int bombAmmount = 1;
    private int bombsRemaining;


    private void OnEnable() 
    {
        bombsRemaining = bombAmmount;
    }

    private void Update() 
    {
        if(!IsOwner)
        {
            return;
        }
       
        if (IsLocalPlayer && bombsRemaining > 0 && Input.GetKeyDown(inputKey)) 
        {

            //StartCoroutine(PlaceBomb());
            PlaceBomb();
        }
    }

    public void PlaceBomb()
    {
        PlaceBombServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaceBombServerRpc()
    {
        GameObject bomb = Instantiate(bombPrefeb, transform.position, Quaternion.identity);

        PlaceBombClientRpc();
    }


    [ClientRpc]
    private void PlaceBombClientRpc()
    {
        OnplayerPlacedBomb?.Invoke(this,EventArgs.Empty);
    }


   

    private void OnTriggerExit2D(Collider2D other) //removes trigger so players can push the bomb around
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb")) 
        {
            other.isTrigger = false;
        }
    }

    public void AddBomb()
    {
        bombAmmount++;
        bombsRemaining++;
    }

}

