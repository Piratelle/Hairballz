// ItemPickup.cs
// handles power-up acquistion and activation

using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
    public AudioSource powerupAudioSource;
    public AudioClip powerupSound; 

    public enum ItemType 
    {
        ExtraBomb,
        BlastRadius,
        SpeedIncrease,
        Flare,
    }

    public ItemType type;

    void Start() {
        powerupAudioSource = GetComponent<AudioSource>();
        powerupSound = GetComponent<AudioClip>();
        powerupAudioSource.clip = powerupSound;
        this.powerupAudioSource.Play();
    }

    private void OnItemPickup(GameObject player)
    {
        // Play sound


        switch (type) 
        {
            case ItemType.ExtraBomb:
                player.GetComponent<BombController>().AddBomb();
                break;

            case ItemType.BlastRadius:
                player.GetComponent<BombController>().IncrementExplosionRadius();
                break;

            case ItemType.SpeedIncrease:
                player.GetComponent<PlayerController>().IncrementSpeed();
                break;

            case ItemType.Flare:
                StartCoroutine(LightItUp());
                break;

        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) 
        {
            Debug.Log("This is a regular log message.");
            OnItemPickup(other.gameObject);
        }
    }

    private IEnumerator LightItUp()
    {
        GameObject fogOfWar = GameObject.FindGameObjectWithTag("FogOfWar");
        if (fogOfWar != null)
        {
            fogOfWar.SetActive(false);
            yield return new WaitForSeconds(3);
            fogOfWar.SetActive(true);
        }
    }
}
