using UnityEngine;

public class TestExplosion : MonoBehaviour
{
    public TestAnimatedSpriteRenderer start;
    public TestAnimatedSpriteRenderer middle;
    public TestAnimatedSpriteRenderer end;


    public void SetActiveRenderer(TestAnimatedSpriteRenderer renderer) 
    {
        start.enabled = renderer == start;
        middle.enabled = renderer == middle;
        end.enabled = renderer == end;
    }

    public void SetDirection(Vector2 direction) 
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void DestroyAfter(float seconds) 
    {
        Destroy(gameObject, seconds);
    }

}
