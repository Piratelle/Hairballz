using UnityEngine;
using Unity.Netcode;

public class AnimatedSpriteRenderer : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;


    private MovementController movementController;


    public Sprite idleSprite;
    public Sprite[] animationSprites;

    public float animationTime = 0.25f;
    private int animationFrame;

    public bool loop = true;
    public bool idle = true;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementController = GetComponent<MovementController>();

    }

    private void OnEnable() 
    {
        spriteRenderer.enabled = true;
        movementController.animationState.OnValueChanged += OnAnimationStateChanged;
    }

    private void OnDisable() 
    {
        spriteRenderer.enabled = false;
        movementController.animationState.OnValueChanged -= OnAnimationStateChanged;
    }

    private void Start() 
    {
        InvokeRepeating(nameof(NextFrame), animationTime, animationTime);
    }

    override public void OnDestroy()
    {
        movementController.animationState.OnValueChanged -= OnAnimationStateChanged;
    }

    private void OnAnimationStateChanged(int oldState, int newState)
    {
        switch(newState)
        {
            case 0:
                idle = true;
                break;
            case 1 :
                idle = false;
                animationFrame = 0;
                break;
            case 2 :
                idle = false;
                animationFrame = 0;
                break;
            case 3:
                idle = false;
                animationFrame = 0;
                break;
            case 4 :
                idle = false;
                animationFrame = 0;
                break;
            default:
                break;

        }
        //throw new NotImplementedException();
    }


    private void NextFrame() 
    {

        animationFrame++;

        if (loop && animationFrame >= animationSprites.Length) 
        {
            animationFrame = 0;
        }

        if (idle)
        {
            spriteRenderer.sprite = idleSprite;
        }
        else if (animationFrame >= 0 && animationFrame < animationSprites.Length) 
        {
            spriteRenderer.sprite = animationSprites[animationFrame];
        }
    }


}
