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

    private Image image;
    private BoxCollider2D boxCollider;
    private AudioSource audio;
    private int lifes;
    private bool isVisible = true;
    private BlockTransformsTypes transformType;

    public enum BlockTransformsTypes { collectionRing = 0, newBall = 1 }
    public int Lifes { get { return lifes; } set { lifes = value; } }
    public bool IsVisible { get { return isVisible; } }

    private void Awake()
    {
        image = GetComponent<Image>();
        boxCollider = GetComponent<BoxCollider2D>();
        audio = GetComponent<AudioSource>();      
    }

    private void Start()
    {
        _BlockLifesText.text = lifes.ToString();
        ColorManagment();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ball")
        {
            audio.Play();
            lifes--;
            _BlockLifesText.text = lifes.ToString();
            ColorManagment();
            if (lifes <= 0)
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
        if (lifes <= 4)
            image.color = _EasyLevelColor;
        else if (lifes > 4 && lifes <= 8)
            image.color = _MediumLevelColor;
        else if (lifes > 8 && lifes <= 12)
            image.color = _HardLevelColor;
        else
            image.color = _ExtremeLevelColor;
    }

    public void MakeObjectInvisible(bool playParticles = false)
    {
        image.enabled = false;
        boxCollider.enabled = false;
        _CollectionRingImage.enabled = false;
        _NewBallImage.enabled = false;
        _BlockLifesText.enabled = false;
        isVisible = false;

        if (playParticles)
        {
            _ParticleSystem.Play();            
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

        image.enabled = false;
        _BlockLifesText.enabled = false;
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
        transformType = type;
        isVisible = false;
    }
}
