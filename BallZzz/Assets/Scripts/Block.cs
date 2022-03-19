using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField] Text _BlockLifesText;
    [SerializeField] Image _CollectionRingImage;
    [SerializeField] Image _NewBallImage;
    [SerializeField] ParticleSystem _ParticleSystem;
    [SerializeField] Color32 _EasyLevelColor;
    [SerializeField] Color32 _MediumLevelColor;
    [SerializeField] Color32 _HardLevelColor;
    [SerializeField] Color32 _ExtremeLevelColor;

    private int lifes;

    public enum BlockTransformsTypes { collectionRing = 0, newBall = 1 }

    private BlockTransformsTypes transformType;
    public int Lifes { get { return lifes; } set { lifes = value; } }

    private void Start()
    {
        _BlockLifesText.text = Lifes.ToString();
        ColorManagment();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ball")
        {
            Lifes--;
            _BlockLifesText.text = Lifes.ToString();
            ColorManagment();
            if (Lifes <= 0)
            {
                MakeObjectInvisible(true);
            }
        }       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "EndLine")
        {
            GameManager.Instance.OnGameEnd?.Invoke();
        }

        if (collision.gameObject.tag == "Ball")
        {
            MakeObjectInvisible();
            switch (transformType)
            {
                case BlockTransformsTypes.collectionRing:
                    GameManager.Instance.OnRingCollect?.Invoke();
                    break;
                case BlockTransformsTypes.newBall:
                    GameManager.Instance.OnNewBallCollect?.Invoke(transform.position);
                    break;
            }                 
        }

    }

    private void ColorManagment()
    {
        if (Lifes <= 2)
            GetComponent<Image>().color = _EasyLevelColor;
        else if (Lifes > 2 && Lifes <= 4)
            GetComponent<Image>().color = _MediumLevelColor;
        else if (Lifes > 4 && Lifes <= 6)
            GetComponent<Image>().color = _HardLevelColor;
        else
            GetComponent<Image>().color = _ExtremeLevelColor;
    }

    public void MakeObjectInvisible(bool playParticles = false)
    {
        GetComponent<Image>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        _CollectionRingImage.enabled = false;
        _NewBallImage.enabled = false;
        _BlockLifesText.enabled = false;

        if (playParticles)
        {
            _ParticleSystem.Play();
            GameManager.Instance.Audio.Play();
        }       
    }

    public void TransformToRingOrBall(BlockTransformsTypes type)
    {
        switch (type)
        {
            case BlockTransformsTypes.collectionRing:
                _CollectionRingImage.enabled = true;
                break;
            case BlockTransformsTypes.newBall:
                _NewBallImage.enabled = true;
                break;
        }

        GetComponent<Image>().enabled = false;
        _BlockLifesText.enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().isTrigger = true;
        transformType = type;
    }
}
