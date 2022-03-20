using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int MAX_BLOCKS_IN_ROW = 7;

    #region SerializeFields
    [SerializeField] Ball _BallPrefab;
    [SerializeField] Block _BlockPrefab;
    [SerializeField] CollectedBall _CollectedBallPrefab;
    [SerializeField] Transform _BlocksContainer;
    [SerializeField] Transform _BallsContainer;
    [SerializeField] Transform _StartSpawnPoint;
    [SerializeField] Transform _TextSwitchSidesPoint;
    [SerializeField] GameObject _EndGamePopUp;
    [SerializeField] Text _ScoreText;
    [SerializeField] Text _EndScoreText;
    [SerializeField] Text _CollectRingsText;
    [SerializeField] Text _AmountText;
    [SerializeField] Text _BestScoreText;
    #endregion

    #region PublicVariables
    public static GameManager Instance;
    public Action OnNewLevelCall;
    public Action OnGameEnd;
    public Action OnRingCollect;
    public Action<Vector2> OnNewBallCollect;
    #endregion

    #region PrivateVariables
    private List<Ball> ballsList = new List<Ball>();
    private Ball mainBall;
    private int level = 0;
    private int rings = 0;
    private int touches = 0;
    private int bestScore = 0;
    private bool isFirstBallTouchedGround = false;
    private bool isGameStarted = false;
    private bool isInputBlocked = false;
    private bool isAllowToMove = true;
    #endregion

    #region Properties
    public int Touches { get { return touches; } set { touches = value; } }
    public bool IsGameStarted { get { return isGameStarted; } set { isGameStarted = value; } }
    public bool IsInputBlocked { get { return isInputBlocked; } set { isInputBlocked = value; } }
    public bool IsFirstBallTouchedGround { get { return isFirstBallTouchedGround; } set { isFirstBallTouchedGround = value; } }
    public bool IsAllowToMove { get { return isAllowToMove; } set { isAllowToMove = value; } }
    public Ball MainBall { get { return mainBall; } set { mainBall = value; } }
    #endregion

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
        OnGameEnd += EndGame;
        OnRingCollect += CollectRing;
        OnNewBallCollect += CollectBall;

        if (PlayerPrefs.HasKey("CollectedRings"))
        {
            rings = PlayerPrefs.GetInt("CollectedRings");
        }

        if (PlayerPrefs.HasKey("BestScore"))
        {
            bestScore = PlayerPrefs.GetInt("BestScore");
        }

        _BestScoreText.text = bestScore.ToString();
        _CollectRingsText.text = rings.ToString();
        mainBall = InstantiateNewBall(_StartSpawnPoint);
        StickAmountTextToBall(mainBall.transform);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && !isInputBlocked && isAllowToMove)
        {          
            touches = 0;
            isFirstBallTouchedGround = false;

            for (int i = 0; i < ballsList.Count; i++)
            {
                ballsList[i].DisableArrow(true);
            }

            StartCoroutine(WaitForLaunchBall());
        }
    }

    IEnumerator WaitForLaunchBall()
    {
        int ballsToLaunch = ballsList.Count;
        isInputBlocked = true;
        for (int i = 0; i < ballsList.Count; i++)
        {
            _AmountText.text = "x" + ballsToLaunch.ToString();
            ballsList[i].LaunchBall();
            ballsToLaunch--;
            yield return new WaitForSeconds(.1f);         
        }
        _AmountText.enabled = false;
        isGameStarted = true;       
    }
    private void EndGame()
    {
        int score = level - 1;
        _EndScoreText.text = "YOUR SCORE:\n" + score.ToString();
        _AmountText.enabled = false;
        _EndGamePopUp.SetActive(true);
        isInputBlocked = true;

        if (score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }       
    }
    private void CollectRing()
    {
        rings++;
        _CollectRingsText.text = rings.ToString();
        PlayerPrefs.SetInt("CollectedRings", rings);
    }

    private void CollectBall(Vector2 position)
    {
        CollectedBall cBall  = Instantiate(_CollectedBallPrefab, _BallsContainer);
        cBall.transform.position = position;
        StartCoroutine(WaitForInstantiateNewBall());
    }

    IEnumerator WaitForInstantiateNewBall()
    {
        yield return new WaitUntil(() => CheckForAllBallsTouchedGround());
        InstantiateNewBall(mainBall.transform);
        _AmountText.text = "x" + ballsList.Count.ToString();
    }

    private void StickAmountTextToBall(Transform trs)
    {
        _AmountText.enabled = true;
        _AmountText.transform.position = trs.transform.position;
        float offset = 100f;
        if (trs.localPosition.x > _TextSwitchSidesPoint.localPosition.x)
        {
            _AmountText.rectTransform.anchoredPosition = new Vector2(_AmountText.rectTransform.anchoredPosition.x - offset, _AmountText.rectTransform.anchoredPosition.y + offset);
        }
        else
        {
            _AmountText.rectTransform.anchoredPosition = new Vector2(_AmountText.rectTransform.anchoredPosition.x + offset, _AmountText.rectTransform.anchoredPosition.y + offset);
        }       
        _AmountText.text = "x" + ballsList.Count.ToString();
    }

    public Ball InstantiateNewBall(Transform trs)
    {
        Ball ball = Instantiate(_BallPrefab, _BallsContainer);
        ball.transform.position = trs.position;
        ball.transform.rotation = trs.rotation;      
        ballsList.Add(ball);
        return ball;
    }

    public bool CheckForAllBallsTouchedGround()
    {
        if (touches == ballsList.Count)
            return true;
        else
            return false;
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
            yield return new WaitUntil(() => CheckForAllBallsTouchedGround());
            isInputBlocked = false;
            StickAmountTextToBall(mainBall.transform);
        }

        level++;
        _ScoreText.text = level.ToString();
        List<Block> instantiantedBlocks = new List<Block>();

        for (int i = 0; i < MAX_BLOCKS_IN_ROW; i++)
        {
            Block block = Instantiate(_BlockPrefab, _BlocksContainer);
            block.Lifes = level;
            instantiantedBlocks.Add(block);
        }
      
        int blocksToRemove = level % 2 == 0 ? 2 : 3;

        for (int i = 0; i < blocksToRemove; i++)
        {
            instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)].MakeObjectInvisible();
        }
        if (level % 2 == 0)
        {
            instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)].TransformToRingOrBall(Block.BlockTransformsTypes.collectionRing);
        }
        if (level > 2)
        {
            instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)].TransformToRingOrBall(Block.BlockTransformsTypes.newBall);
        }

        int index = 0;

        while (index < 1)
        {
            Block block = instantiantedBlocks[UnityEngine.Random.Range(0, instantiantedBlocks.Count)];
            if (block.IsVisible)
            {
                block.Lifes *= 2;
                index++;
            }
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
