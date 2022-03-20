using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedBall : MonoBehaviour
{
    private Vector2 impulseForce = new Vector2(0f, 3f);
    private Rigidbody2D rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(impulseForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bottom")
        {
            rigidBody.gravityScale = 0f;
            rigidBody.velocity = Vector2.zero;
            StartCoroutine(LerpToMainBall());
        }
    }

    IEnumerator LerpToMainBall()
    {
        float lerp = 0;
        float animationTime = .2f;

        yield return new WaitUntil(() => GameManager.Instance.CheckForAllBallsTouchedGround());
        do
        {
            lerp += Time.deltaTime / animationTime;
            transform.position = Vector3.Lerp(transform.position, GameManager.Instance.MainBall.transform.position, lerp);
            yield return null;
        }
        while (lerp < 1);

        Destroy(gameObject);
    }

}
