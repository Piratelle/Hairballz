/* Originally from David, modified by Jonah
 * Modified by Erin to combine David's PlayerMovement and MovementController
 * Player controller script
 * - Handles movement
 * - Death sequence
 */ 

using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    #region Variables
    //private NetworkVariable<int> playerNum = new NetworkVariable<int>(0);
    //private Color[] playerColors = { Color.white, Color.red, Color.blue, Color.yellow };

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject camHolder;
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.down;

    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputRight = KeyCode.D;

    public bool idle;
    private bool isDead;

    public AudioSource deathAudioSource;
    //public AudioClip deathSound;
    #endregion

    #region Initialization
    public override void OnNetworkSpawn() 
    {
        base.OnNetworkSpawn();
        if (!IsOwner) {
            enabled = false;
            camHolder.SetActive(false);
            return;
        }
        Initialize();
    }

    private void Initialize() {
        // get components
        deathAudioSource = GetComponent<AudioSource>();
        //deathSound = GetComponent<AudioClip>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; // fix for annoying collision-breaking bug of doom

        // attach main camera to player if owner (can also just set positions in update if works better)
        Camera.main.transform.SetParent(rb.transform);

        // determine player data here!
    }
    #endregion

    #region Movement
    private void Update()
    {
        // Input handling
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        // Update animator
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);
    }

    private void FixedUpdate() 
    {
        // Movement calc
        Vector2 pos = rb.position;
        Vector2 translation = speed * Time.fixedDeltaTime * direction;

        rb.MovePosition(pos + translation);
    }
    #endregion

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
        enabled = false;
        GetComponent<BombController>().enabled = false;

        // Death animation

        // Play the death sound
        if (deathAudioSource != null) // && deathSound != null)
        {
            //deathAudioSource.clip = deathSound;
            deathAudioSource.Play();
        }
        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);

        // TODO: check win conditions, respawn if necessary
    }
    #endregion

    #region Accessors & Mutators
    public float GetSpeed() {
        return speed;
    }

    public void IncrementSpeed() {
        speed++;
    }

    public int GetPlayerNum() {
        //return playerNum.Value;
        return 0;
    }
    #endregion

}

