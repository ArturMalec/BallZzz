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
    public static Ball MainBall;
    public Action OnNewLevelCall;
    public Action OnGameEnd;
    public Action OnBallTouchedGround;
    public Action OnRingCollect;
    public Action OnNewBallCollect;
    private List<Ball> ballsList = new List<Ball>();
    private int level = 0;
    private int rings = 0;
    private int touches = 0;
    private bool isFirstBallTouchedGround = false;
    private bool isGameStarted = false;
    private bool isInputBlocked = false;

    public int Level { get { return level; } set { level = value; } }
    public int Touches { get { return touches; } set { touches = value; } }
    public bool IsGameStarted { get { return isGameStarted; } set { isGameStarted = value; } }
    public bool IsInputBlocked { get { return isInputBlocked; } private set { isInputBlocked = value; } }
    public bool IsFirstBallTouchedGround { get { return isFirstBallTouchedGround; } set { isFirstBallTouchedGround = value; } }

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
        OnNewLevelCall += InstantiateLevel;
        OnNewLevelCall += test;
        OnGameEnd += EndGame;
        OnRingCollect += CollectRing;
        OnBallTouchedGround += test;
        OnNewBallCollect += CollectBall;

        if (PlayerPrefs.HasKey("CollectedRings"))
        {
            rings = PlayerPrefs.GetInt("CollectedRings");
        }

        _CollectRingsText.text = "= " + rings.ToString();
        InstantiateNewBall(_StartSpawnPoint);
    }

    private void Update()
    {
        
    }

    private void EndGame()
    {
        _EndScoreText.text = "YOUR SCORE:\n" + (Level - 1).ToString();
        _EndGamePopUp.SetActive(true);
        isInputBlocked = true;
    }

    private void test()
    {
        Debug.Log("Ilosc kulek: " + ballsList.Count.ToString());
        Debug.Log("Ilosc dotkniec: " + Touches.ToString());


    }

    private void CollectRing()
    {
        rings++;
        _CollectRingsText.text = "= " + rings.ToString();
        PlayerPrefs.SetInt("CollectedRings", rings);
    }

    private void CollectBall()
    {
        StartCoroutine(WaitForInstantiateNewBall());
    }

    IEnumerator WaitForInstantiateNewBall()
    {
        yield return new WaitUntil(() => Touches == ballsList.Count);
        InstantiateNewBall(MainBall.transform);
    }

    public void InstantiateNewBall(Transform trs)
    {
        Ball ball = Instantiate(_BallPrefab, _BallsContainer);
        ball.transform.position = trs.position;
        ball.transform.rotation = trs.rotation;
        ballsList.Add(ball);
    }


    public void InstantiateLevel()
    {
        StartCoroutine(WaitForAllTouches());
    }

    IEnumerator WaitForAllTouches()
    {
        if (ballsList.Count > 0)
        {
            isInputBlocked = true;
            yield return new WaitUntil(() => Touches == ballsList.Count);
            isInputBlocked = false;
        }

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
            instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)].TransformToRingOrBall(Block.BlockTransformsTypes.collectionRing);
        }
        if (Level > 2)
        {
            instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)].TransformToRingOrBall(Block.BlockTransformsTypes.newBall);
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
