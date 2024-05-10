using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public int bombup = 1;
    public int speedup = 1;
    public int blastup = 1;
    public TextMeshProUGUI bombupText;
    public TextMeshProUGUI speedupText;
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

    public void AddItem(ItemPickup.ItemType type)
    {
        switch (type)
        {
            case ItemPickup.ItemType.ExtraBomb:
                bombup++;
                UpdateItemCountUI(bombupText, bombup);
                break;
            case ItemPickup.ItemType.BlastRadius:
                blastup++;
                UpdateItemCountUI(blastupText, blastup);
                break;
            case ItemPickup.ItemType.SpeedIncrease:
                speedup++;
                UpdateItemCountUI(speedupText, speedup);
                break;
        }
    }

    void UpdateItemCountUI(TextMeshProUGUI uiText, int count)
    {
        uiText.text = count.ToString();
    }
}