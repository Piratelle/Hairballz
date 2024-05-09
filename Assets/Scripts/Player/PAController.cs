using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEditor;
public class PAController : NetworkBehaviour
{
    /// <summary>
    /// //////////////////////////////
    /// 
    /// 
    /// </summary>
    /// 





    ///////////
    ///


    public float speed = 5f;
    public Sprite[] upAnimationSprites;
    public Sprite[] downAnimationSprites;
    public Sprite[] leftAnimationSprites;
    public Sprite[] rightAnimationSprites;

    public Sprite idleUpSprite;
    public Sprite idleDownSprite;
    public Sprite idleLeftSprite;
    public Sprite idleRightSprite;

    private SpriteRenderer spriteRenderer;
    private Vector2 input;
    private bool isMoving;
    private bool upFlag;
    private bool downFlag;
    private bool leftFlag;
    private bool rightFlag;

    public float animationTime = 0.25f;
    private int animationFrame=-1;

    public bool loop = true;
    public bool idle = true;


    void Start()
    {
        if(!IsOwner)
        {
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating(nameof(NextFrame), animationTime, animationTime);
    }

    private void NextFrame() 
    {
        if (upFlag) // Moving up
        {
            animationFrame++;

            if (loop && animationFrame >= upAnimationSprites.Length) 
            {
                animationFrame = 0;
            }

            if (idle)
            {
                spriteRenderer.sprite = idleUpSprite;
            }
            else if (animationFrame >= 0 && animationFrame < upAnimationSprites.Length) 
            {
                spriteRenderer.sprite = upAnimationSprites[animationFrame];
            }


        }
        else if (downFlag) // Moving down
        {
            animationFrame++;

            if (loop && animationFrame >= downAnimationSprites.Length) 
            {
                animationFrame = 0;
            }

            if (idle)
            {
                spriteRenderer.sprite = idleDownSprite;
            }
            else if (animationFrame >= 0 && animationFrame < downAnimationSprites.Length) 
            {
                spriteRenderer.sprite = downAnimationSprites[animationFrame];
            }
        }
        else if (leftFlag) // Moving left
        {
            animationFrame++;

            if (loop && animationFrame >= leftAnimationSprites.Length) 
            {
                animationFrame = 0;
            }

            if (idle)
            {
                spriteRenderer.sprite = idleLeftSprite;
            }
            else if (animationFrame >= 0 && animationFrame < leftAnimationSprites.Length) 
            {
                spriteRenderer.sprite = leftAnimationSprites[animationFrame];
            }
        }
        else if (rightFlag) // Moving right
        {
            animationFrame++;

            if (loop && animationFrame >= rightAnimationSprites.Length) 
            {
                animationFrame = 0;
            }

            if (idle)
            {
                spriteRenderer.sprite = idleRightSprite;
            }
            else if (animationFrame >= 0 && animationFrame < rightAnimationSprites.Length) 
            {
                spriteRenderer.sprite = rightAnimationSprites[animationFrame];
            }
        }

    
    }


    void Update()
    {
        if(!IsOwner)
        {
            return;
        }

        // Get player input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Normalize the input vector so diagonal movement isn't faster
        input.Normalize();

        // Check if the player is moving
        isMoving = input.magnitude > 0;

        if(isMoving)
        {
            if (input.x < 0)
            {
                leftFlag = true;
                rightFlag = false;
                upFlag = false;
                downFlag = false;            
            }
            else if (input.x > 0)
            {
                leftFlag = false;
                rightFlag = true;
                upFlag = false;
                downFlag = false;    
            }
            else if (input.y > 0)
            {
                leftFlag = false;
                rightFlag = false;
                upFlag = true;
                downFlag = false;    
            }
            else// (input.y < 0)
            {
                leftFlag = false;
                rightFlag = false;
                upFlag = false;
                downFlag = true;;    
            }
        }

        idle = !isMoving;

        

        // Update player position
        transform.Translate(input * speed * Time.deltaTime);

        // Update player animation
       // UpdateAnimation();
       
    }

}
