using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int MAX_BLOCKS_IN_ROW = 7;

    [SerializeField] Ball _BallPrefab;
    [SerializeField] Block _BlockPrefab;
    [SerializeField] Transform _BlocksContainer;
    [SerializeField] Transform _BallsContainer;
    [SerializeField] Transform _StartSpawnPoint;
    [SerializeField] GameObject _EndGamePopUp;
    [SerializeField] Text _ScoreText;
    [SerializeField] Text _EndScoreText;
    [SerializeField] Text _CollectRingsText;

    public static GameManager Instance;
    public Action OnBallBottomTouch;
    public Action OnGameEnd;
    public Action OnRingCollect;
    private int level = 0;
    private int rings = 0;
    private bool isGameStarted = false;
    private bool isInputBlocked = false;

    public int Level { get { return level; } set { level = value; } }
    public bool IsGameStarted { get { return isGameStarted; } set { isGameStarted = value; } }
    public bool IsInputBlocked { get { return isInputBlocked; } private set { isInputBlocked = value; } }

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InstantiateLevel();
        OnBallBottomTouch += InstantiateLevel;
        OnGameEnd += EndGame;
        OnRingCollect += CollectRing;

        if (PlayerPrefs.HasKey("CollectedRings"))
        {
            rings = PlayerPrefs.GetInt("CollectedRings");
        }

        _CollectRingsText.text = "= " + rings.ToString();
        Ball firstBall = Instantiate(_BallPrefab, _BallsContainer);
        firstBall.transform.position = _StartSpawnPoint.position;
        firstBall.transform.localScale = _StartSpawnPoint.localScale;
    }

    private void EndGame()
    {
        _EndScoreText.text = "YOUR SCORE:\n" + (Level - 1).ToString();
        _EndGamePopUp.SetActive(true);
        isInputBlocked = true;
    }

    private void CollectRing()
    {
        rings++;
        _CollectRingsText.text = "= " + rings.ToString();
        PlayerPrefs.SetInt("CollectedRings", rings);
    }


    public void InstantiateLevel()
    {
        Level++;
        _ScoreText.text = "SCORE: " + Level.ToString();
        List<Block> instantiantedBlocks = new List<Block>();

        for (int i = 0; i < MAX_BLOCKS_IN_ROW; i++)
        {
            Block block = Instantiate(_BlockPrefab, _BlocksContainer);
            block.Lifes = Level;
            instantiantedBlocks.Add(block);
        }

        int blocksToRemove = Level % 2 == 0 ? 2 : 3;

        for (int i = 0; i < blocksToRemove; i++)
        {
            instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)].MakeObjectInvisible();
        }
        if (Level % 2 == 0)
        {
            instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)].TransformToCollectionRing();
        }
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    

    

}
