using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    private void Awake()
    {
        playButton.onClick.AddListener(()=>{
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(()=>{
            Application.Quit();
        });
    }

    //Time.timeScale = 1.0f;


    
}
