using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const float ROTATE_SPEED = 500f;
    private const float MOVE_SPEED = 8f;
    private const float MAX_ROTATION = .6f;

    [SerializeField] GameObject _DirectionArrow;

    private bool isFlying = false;

    private void Update()
    {
        if (isFlying)
        {
            GetComponent<Rigidbody2D>().velocity = MOVE_SPEED * (GetComponent<Rigidbody2D>().velocity.normalized);
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
            GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * MOVE_SPEED * Time.deltaTime, ForceMode2D.Impulse);
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
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (GameManager.Instance.IsGameStarted)
            {
                GameManager.Instance.Touches++;
                GameManager.Instance.OnBallTouchedGround?.Invoke();
                
                if (!GameManager.Instance.IsFirstBallTouchedGround)
                {
                    GameManager.Instance.OnNewLevelCall?.Invoke();
                    GameManager.Instance.IsFirstBallTouchedGround = true;
                    GameManager.MainBall = this;
                }
                else
                {
                    transform.position = GameManager.MainBall.transform.position;
                }
                
            }
        }
    }
}
