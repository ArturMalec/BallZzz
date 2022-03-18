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
    private bool isAllowToMove = true;


    private void Update()
    {
        if (isFlying)
        {
            GetComponent<Rigidbody2D>().velocity = MOVE_SPEED * (GetComponent<Rigidbody2D>().velocity.normalized);
        }

        if (Input.GetMouseButton(0) && !isFlying && !GameManager.Instance.IsInputBlocked)
        {          
            _DirectionArrow.SetActive(true);
            transform.Rotate(transform.rotation.x, transform.rotation.y, (Input.GetAxis("Mouse Y") * ROTATE_SPEED * Time.deltaTime), Space.World);

            if (transform.rotation.z <= -MAX_ROTATION)
            {
                _DirectionArrow.SetActive(false);
                isAllowToMove = false;
            }
            else if (transform.rotation.z >= MAX_ROTATION)
            {
                _DirectionArrow.SetActive(false);
                isAllowToMove = false;
            }
            else
            {
                _DirectionArrow.SetActive(true);
                isAllowToMove = true;
            }
        }
      
        if (Input.GetMouseButtonUp(0) && !GameManager.Instance.IsInputBlocked && isAllowToMove)
        {
            GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * MOVE_SPEED * Time.deltaTime, ForceMode2D.Impulse);
            _DirectionArrow.SetActive(false);
            isFlying = true;
            GameManager.Instance.IsGameStarted = true;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bottom")
        {
            isFlying = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            
            if (GameManager.Instance.IsGameStarted)
            {
                GameManager.Instance.OnBallBottomTouch?.Invoke();
            }           
        }
    }

}
