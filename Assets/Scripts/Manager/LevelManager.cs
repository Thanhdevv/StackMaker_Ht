using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public List<Level> levels;
    public Level CurrentLevel;
    public PlayerController PlayerPrefab;
    public PlayerController Player;
    [SerializeField] private int currentLeverIndex;
    [SerializeField] private CameraFollow camera;
    public Transform posStart;

 

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

    private void Start()
    {
        SpawnPlayer();
        UpdateLeverVisibility();
    }

    private void SpawnPlayer()
    {
        if (Player == null)
        {
            Player = Instantiate(PlayerPrefab);
        }
        if (camera != null && Player != null)
        {
            camera.SetTarget(Player.transform);
        }
    }

    public void NextLevel()
    {
        if (currentLeverIndex < levels.Count - 1)
        {
            currentLeverIndex++;
            UpdateLeverVisibility();
            MovePlayerToPosStart();
            UIManager.Instance.HideVictory();
            Player.ResetAnimator();
            Player.OnInit();
        }
        else
        {
           
        }
    }

    private void UpdateLeverVisibility()
    {
        if (CurrentLevel != null)
        {
            CurrentLevel.gameObject.SetActive(false);
        }

        if (currentLeverIndex < levels.Count)
        {
            CurrentLevel = levels[currentLeverIndex];
            CurrentLevel.gameObject.SetActive(true);
            posStart = CurrentLevel.StartPos;
            Debug.Log("CurrentLevel: " + CurrentLevel.name);
        }
    }

    private void MovePlayerToPosStart()
    {
        if (posStart == null) return;
        if (Player == null) return;
        Player.SetStartPosition(posStart.position);
    }

    public void Replay()
    {
        SpawnPlayer();
        Player.OnInit();
        Player.ResetLevel();
        UpdateLeverVisibility();
        MovePlayerToPosStart();
        UIManager.Instance.HideVictory();
    }


}
