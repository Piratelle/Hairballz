/* Originally from David, modified by Jonah
 * Player controller script
 * - Handles movement
 * - Death sequence
 */


using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject camHolder;
    private Vector2 direction = Vector2.down;
    [SerializeField] private float speed = 5f;

    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputRight = KeyCode.D;

    public Animator animator;
    public bool idle;
    private bool isDead;

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
        deathAudioSource = GetComponent<AudioSource>();
        deathSound = GetComponent<AudioClip>();
        rb = GetComponent<Rigidbody2D>();
        // attach main camera to player if owner (can also just set positions in update if works better)
        Camera.main.transform.SetParent(rb.transform);
    }

    private void Update()
    {
        // Input handling
        if (Input.GetKey(inputUp)) direction = Vector2.up;
        else if (Input.GetKey(inputDown)) direction =  Vector2.down;
        else if (Input.GetKey(inputLeft)) direction = Vector2.left;
        else if (Input.GetKey(inputRight)) direction = Vector2.right;
        else direction = Vector2.zero;
    }

    private void FixedUpdate() 
    {
        // Movement calc
        Vector2 pos = rb.position;
        Vector2 translation = speed * Time.fixedDeltaTime * direction;

        rb.MovePosition(pos + translation);
    }
    #region Death Handling
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            // TODO: Send client id to score tracker etc
            DeathSequenceServerRpc();
        }
    }

    // Might not need to be an RPC
    [ServerRpc(RequireOwnership = false)]
    private void DeathSequenceServerRpc()
    {
        this.enabled = false;
        GetComponent<BombController>().enabled = false;

        // Death animation

        // Play the death sound
        if (deathAudioSource != null && deathSound != null)
        {
            deathAudioSource.clip = deathSound;
            this.deathAudioSource.Play();
        }

        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);

        // TODO: check win conditions, respawn if necessary

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //GameManager.Instance.CheckWinState();
    }
    #endregion

    public float GetSpeed() {
        return this.speed;
    }

    public void IncrementSpeed() {
        this.speed++;
    }

}

