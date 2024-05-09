// LoaderCallBack.cs
// handles Loading Scene transition

using UnityEngine;

public class LoaderCallBack : MonoBehaviour
{
    private bool isFirstUpdate = true;

    // Update is called once per frame
    private void Update()
    {
        if(isFirstUpdate)
        {
            isFirstUpdate = false;
            Loader.LoaderCallback();
        }
    }
}
