using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const float ROTATE_SPEED = 500f;
    private const float MOVE_SPEED = 8f;
    private const float MAX_ROTATION = .6f;

    [SerializeField] GameObject _DirectionArrow;

    private Rigidbody2D rigidBody;
    private bool isFlying = false;
    private bool isInPlayField = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isFlying)
        {
            rigidBody.velocity = MOVE_SPEED * (rigidBody.velocity.normalized);
        }

        if (Input.GetMouseButton(0) && !isFlying && !GameManager.Instance.IsInputBlocked)
        {
            DisableArrow(false);
            transform.Rotate(transform.rotation.x, transform.rotation.y, (Input.GetAxis("Mouse Y") * ROTATE_SPEED * Time.deltaTime), Space.World);

            if (transform.rotation.z <= -MAX_ROTATION)
            {
                DisableArrow(true);
                GameManager.Instance.IsAllowToMove = false;
            }
            else if (transform.rotation.z >= MAX_ROTATION)
            {
                DisableArrow(true);
                GameManager.Instance.IsAllowToMove = false;
            }
            else
            {
                DisableArrow(false);
                GameManager.Instance.IsAllowToMove = true;
            }
        }
    }

    public void LaunchBall()
    {
        if (GameManager.Instance.IsAllowToMove)
        {
            rigidBody.AddRelativeForce(Vector2.up * MOVE_SPEED * Time.deltaTime, ForceMode2D.Impulse);           
            isFlying = true;
        }      
    }

    /// <summary>
    /// Disable or enable direction arrow
    /// </summary>
    /// <param name="state"></param>
    public void DisableArrow(bool state)
    {
        if (state)
            _DirectionArrow.SetActive(false);
        else
            _DirectionArrow.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bottom")
        {
            isFlying = false;
            rigidBody.velocity = Vector2.zero;
            
            if (GameManager.Instance.IsGameStarted)
            {
                GameManager.Instance.Touches++;
                
                if (!GameManager.Instance.IsFirstBallTouchedGround)
                {
                    GameManager.Instance.OnNewLevelCall?.Invoke();
                    GameManager.Instance.IsFirstBallTouchedGround = true;
                    GameManager.Instance.MainBall = this;
                }
                else
                {                   
                    StartCoroutine(LerpToMainBall());
                }              
            }
        }

        if (collision.gameObject.tag == "EndLine") // Adding some gravity to prevent ball blocking in Y pos
        {
            if (!isInPlayField)
            {
                rigidBody.gravityScale = .1f;
                isInPlayField = true;
            }
            else
            {
                rigidBody.gravityScale = 0f;
                isInPlayField = false;
            }
        }
    }

    IEnumerator LerpToMainBall()
    {
        float lerp = 0;
        float animationTime = .1f;
        do
        {
            lerp += Time.deltaTime / animationTime;
            transform.position = Vector3.Lerp(transform.position, GameManager.Instance.MainBall.transform.position, lerp);
            yield return null;
        } 
        while (lerp < 1);
    }
}
