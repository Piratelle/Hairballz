using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MovementController : NetworkBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    private Vector2 direction = Vector2.down;
    public float speed = 5f;

    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputRight = KeyCode.D;

    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererRight;
    private AnimatedSpriteRenderer activeSpriteRenderer;
    public AnimatedSpriteRenderer spriteRendererDeath;

    public AudioSource deathAudioSource; // Assign the AudioSource component in the Inspector
    public AudioClip deathSound; // Assign the death sound clip in the Inspector


    public NetworkVariable<int> animationState = new NetworkVariable<int>(0);


    private void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown;
    }

    private void Update()
    {
        if(IsOwner)
        {
            if (Input.GetKey(inputUp))
        {
            SetDirection(Vector2.up, spriteRendererUp);
            SetAnimationState(1);
        }
        else if (Input.GetKey(inputDown))
        {
            SetDirection(Vector2.down, spriteRendererDown);
            SetAnimationState(2);
        }
        else if (Input.GetKey(inputLeft))
        {
            SetDirection(Vector2.left, spriteRendererLeft);
            SetAnimationState(3);
        }
        else if (Input.GetKey(inputRight))
        {
            SetDirection(Vector2.right, spriteRendererRight);
            SetAnimationState(4);
        }
        else 
        {
            SetDirection(Vector2.zero, activeSpriteRenderer);
            SetAnimationState(0);
        }
        }

        if(IsClient)
        {
            if (Input.GetKey(inputUp))
        {
            SetDirection(Vector2.up, spriteRendererUp);
            SetAnimationState(1);
        }
        else if (Input.GetKey(inputDown))
        {
            SetDirection(Vector2.down, spriteRendererDown);
            SetAnimationState(2);
        }
        else if (Input.GetKey(inputLeft))
        {
            SetDirection(Vector2.left, spriteRendererLeft);
            SetAnimationState(3);
        }
        else if (Input.GetKey(inputRight))
        {
            SetDirection(Vector2.right, spriteRendererRight);
            SetAnimationState(4);
        }
        else 
        {
            SetDirection(Vector2.zero, activeSpriteRenderer);
            SetAnimationState(0);
        }
        }
        
        
    }

    private void FixedUpdate() 
    {
        Vector2 position = rigidbody.position;
        Vector2 translation = speed * Time.fixedDeltaTime * direction;

        rigidbody.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer) 
    {
        direction = newDirection;

        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

        activeSpriteRenderer = spriteRenderer;
        activeSpriteRenderer.idle = direction == Vector2.zero;
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

        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = true;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //GameManager.Instance.CheckWinState();
    }

    private void SetAnimationState(int newState)
    {
        if(!IsOwner)
        {
            return;
        }
        animationState.Value = newState;
    }

}

