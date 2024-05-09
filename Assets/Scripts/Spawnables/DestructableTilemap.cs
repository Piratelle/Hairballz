using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableTilemap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Explosion")
        {
            Destroy(this.gameObject);
        }
    }
}
