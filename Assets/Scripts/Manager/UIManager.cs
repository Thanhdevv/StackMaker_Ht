using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance ; 

    [SerializeField] private GameObject Victory;
    [SerializeField] private TMP_Text ScoreText;
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowVictory(int score)
    {
        
        ScoreText.text = score.ToString();
        Victory.SetActive(true);
    }

    public void HideVictory()
    {
        Victory.SetActive(false);
    }
}
