using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedBall : MonoBehaviour
{
    private Vector2 ImpulseForce = new Vector2(0f, 3f);

    private void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(ImpulseForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bottom")
        {
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
            transform.position = Vector3.Lerp(transform.position, GameManager.MainBall.transform.position, lerp);
            yield return null;
        }
        while (lerp < 1);

        Destroy(gameObject);
    }

}
