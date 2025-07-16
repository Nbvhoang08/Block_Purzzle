using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _mainSprite;
    public bool isInsideAnySquare { get; set; }
    private void Start()
    {
        _spriteRenderer.gameObject.SetActive(false);
        isInsideAnySquare = false;
    }
    public void DeactivateShape()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
        isInsideAnySquare = false;
    }
    public void SetSprite(Sprite sprite) 
    {
       _mainSprite.sprite = sprite;
    }

    public void ActivateShape()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(true);
        isInsideAnySquare = false;
    }
    public void SetOccupied()
    {
        _spriteRenderer.gameObject.SetActive(true);
    }
    public void UnSetOccupied() 
    {
        _spriteRenderer.gameObject.SetActive(false);

    }
}
