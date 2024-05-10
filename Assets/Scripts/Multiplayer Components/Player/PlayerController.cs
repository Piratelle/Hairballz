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
    private NetworkVariable<int> playerNum = new NetworkVariable<int>(0, writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject camHolder;
    [SerializeField] private AudioSource deathAudioSource;
    //[SerializeField] private AudioClip deathSound;
    [SerializeField] private float speed = 5f;

    //public KeyCode inputUp = KeyCode.W;
    //public KeyCode inputLeft = KeyCode.A;
    //public KeyCode inputDown = KeyCode.S;
    //public KeyCode inputRight = KeyCode.D;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.down;
    private bool isDead;

    //public bool idle;
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
        //Initialize();
    }

    private void Start() {
        // get components
        deathAudioSource = GetComponent<AudioSource>();
        //deathSound = GetComponent<AudioClip>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; // fix for annoying collision-breaking bug of doom

        // attach main camera to player if owner (can also just set positions in update if works better)
        Camera.main.transform.SetParent(rb.transform);

        // learn player details
        playerNum.Value = GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId);
        PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);

        // set player color
        Color color = GameMultiplayer.Instance.GetPlayerColor(playerData.colorId);
        Debug.Log("Color detected: " + color);
        //SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //spriteRenderer.color = color;

        // set spawn position
        transform.position = GameManager.Instance.Level.GetSpawnPoint(playerNum.Value);
    }
    #endregion

    #region Movement
    private void Update()
    {
        // Input handling
        //if (Input.GetKey(inputUp)) direction = Vector2.up;
        //else if (Input.GetKey(inputDown)) direction = Vector2.down;
        //else if (Input.GetKey(inputLeft)) direction = Vector2.left;
        //else if (Input.GetKey(inputRight)) direction = Vector2.right;
        //else direction = Vector2.zero;
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
            Bomb bomb = other.GetComponentInParent<Bomb>();
            if (bomb != null)
            {
                int bombOwner = bomb.GetPlayerNum();
                if (bombOwner != playerNum.Value)
                {
                    UpdateScoreServerRpc(bombOwner);
                }
            }
            DeathSequenceServerRpc();
        }
    }

    [ServerRpc]
    private void UpdateScoreServerRpc(int scoringPlayer)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.IncrementScore(scoringPlayer);
        }
        else
        {
            Debug.LogError("ScoreManager instance not found!");
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
        return playerNum.Value;
    }
    #endregion

}

