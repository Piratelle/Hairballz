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
    #region Variables
    //private int playerNum;
    private NetworkVariable<int> playerNum = new NetworkVariable<int>(0);
    // Network variable for playercount
    private Color[] playerColors = { Color.white, Color.red, Color.blue, Color.yellow };

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

    public AudioSource deathAudioSource;
    public AudioClip deathSound;
    #endregion

    #region Initialization
    public override void OnNetworkSpawn() 
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            playerNum.Value = ClientConnectionHandler.GetPlayerNum(OwnerClientId) + 1;
            Debug.Log("I'm Player #" + playerNum.Value);
        }
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
        rb.bodyType = RigidbodyType2D.Dynamic; // fix for annoying collision-breaking bug of doom
        // attach main camera to player if owner (can also just set positions in update if works better)
        Camera.main.transform.SetParent(rb.transform);

        // Set player color
        GetComponent<SpriteRenderer>().color = playerColors[playerNum.Value - 1];
        if (!IsHost) {
            // playerNum = playerCount;
            // GetComponent<SpriteRenderer>().color = color.red;
        } else {
            // Is host
            //playerNum = 1;
        }
    }
    #endregion

    #region Movement
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

    #region Accessors & Mutators
    public float GetSpeed() {
        return speed;
    }

    public void IncrementSpeed() {
        speed++;
    }

    public int GetPlayerNum() {
        return playerNum.Value;
    }
    #endregion

}

