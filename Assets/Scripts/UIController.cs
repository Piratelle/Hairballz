using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public int bombup;
    public int speedup;
    public int blastup;
    public TextMeshProUGUI bombupText;
    public TextMeshProUGUI blastupText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateBombCountUI(int bombsRemaining, int totalBombs)
    {
        bombupText.text = bombsRemaining.ToString();
    }

    public void UpdateExplosionRadiusUI(int radius)
    {
        blastupText.text = radius.ToString();
    }
}