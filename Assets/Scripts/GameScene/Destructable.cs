using System.Collections;
using Unity.Netcode;
using UnityEngine;


public class Destructable : NetworkBehaviour
{
    public float destructionTime = 1f;
    [Range(0f,1f)]
    public float itemSpawnChance = 0.2f;
    public GameObject[] spawnableItems;



    private void Start()
    {
        Debug.LogWarning("Bousta DestroyFunction!!!!!!!!!!!!1");
        //Destroy (gameObject, destructionTime);
        StartCoroutine(DestructableTimer(NetworkObject));
    }

    private IEnumerator DestructableTimer(NetworkObject instanceNetworkObject2)
    {
        yield return new WaitForSeconds(destructionTime);//explosion countdown
        instanceNetworkObject2.DontDestroyWithOwner = true;
        instanceNetworkObject2.Despawn();
    
    }

   /* private void OnDestroy()
    {
        if(spawnableItems.Length > 0 && Random.value < itemSpawnChance)
        {
            int randomIndex = Random.Range(0, spawnableItems.Length);
            Instantiate(spawnableItems[randomIndex], transform.position, Quaternion.identity);
        }
    }*/
}
