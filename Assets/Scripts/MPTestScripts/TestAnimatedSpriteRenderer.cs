using UnityEngine;

public class TestAnimatedSpriteRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite idleSprite;
    public Sprite[] animationSprites;

    public float animationTime = 0.25f;
    private int animationFrame;

    public bool loop = true;
    public bool idle = true;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void OnEnable() 
    {
        spriteRenderer.enabled = true;
    }

    private void OnDisable() 
    {
        spriteRenderer.enabled = false;
    }

    private void Start() 
    {
        InvokeRepeating(nameof(NextFrame), animationTime, animationTime);
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
