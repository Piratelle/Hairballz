/* Originally from David, modified by Jonah
 * Controls player movement
 * Changes:
 * - Start -> OnNetworkSpawn, 
 * - Changed sprite animation process to animator; modularity more practical for networking (video: https://www.youtube.com/watch?v=hkaysu1Z-N8)
 * - Condensed input/direction checking
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MovementController : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject camHolder;
    private Vector2 direction = Vector2.down;
    public float speed = 5f;

    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputRight = KeyCode.D;

    public Animator animator;
    public bool idle;
    public bool isDead;

    public AudioSource deathAudioSource; // Assign the AudioSource component in the Inspector
    public AudioClip deathSound; // Assign the death sound clip in the Inspector

    public override void OnNetworkSpawn() 
    {
        base.OnNetworkSpawn();
        if (!IsOwner) {
            this.enabled = false;
            this.camHolder.SetActive(false);
            return;
        }

        Initialize();
    }

    // public void Start() {
    //     if (rb == null) {
    //         this.enabled = true;
    //         Initialize();
    //     }
    // }

    private void Initialize() {
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        // attach main camera to player if owner (can also just set positions in update if works better)
        Camera.main.transform.SetParent(rb.transform);
    }

    private void Update()
    {
        if (Input.GetKey(inputUp)) direction = Vector2.up;
        else if (Input.GetKey(inputDown)) direction =  Vector2.down;
        else if (Input.GetKey(inputLeft)) direction = Vector2.left;
        else if (Input.GetKey(inputRight)) direction = Vector2.right;
        else direction = Vector2.zero;
    }

    private void FixedUpdate() 
    {
        Vector2 pos = rb.position;
        Vector2 translation = speed * Time.fixedDeltaTime * direction;

        rb.MovePosition(pos + translation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            DeathSequence();
        }
    }

    private void DeathSequence()
    {
        enabled = false;
        GetComponent<BombController>().enabled = false;

        // Play the death sound
        if (deathAudioSource != null && deathSound != null)
        {
            deathAudioSource.clip = deathSound;
            deathAudioSource.Play();
        }

        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //GameManager.Instance.CheckWinState();
    }

}

