// Destructable.cs
// handles power-up spawning when tiles are destroyed

using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Destructable : NetworkBehaviour
{
    public float destructionTime = 1f;
    [Range(0f,1f)]
    public float itemSpawnChance = 0.2f;
    public GameObject[] spawnableItems;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Explosion")
        {
            Destroy(gameObject);
        }
    }

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

    /*public override void OnDestroy()
    {
        if(spawnableItems.Length > 0 && Random.value < itemSpawnChance)
        {
            int randomIndex = Random.Range(0, spawnableItems.Length);
            Instantiate(spawnableItems[randomIndex], transform.position, Quaternion.identity);
        }
        base.OnDestroy();
    }*/
}
