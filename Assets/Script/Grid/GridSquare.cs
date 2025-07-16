using DG.Tweening;
using UnityEngine;

public class GridSquare : MonoBehaviour
{
    public SpriteRenderer Hoversprite;
    public SpriteRenderer activeSprite;
    public SpriteRenderer normalSprite;
    
    public bool Selected { get; set; }
    public int Index;
    public bool SquareOccupied { get; set; }
    public Rigidbody2D rb;
    private Vector3 startPosition;

    private void OnEnable()
    {
        GameEvents.GameOver += GameOverAction;
        GameEvents.RestartGameAction += ResetSquare;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= GameOverAction;
        GameEvents.RestartGameAction -= ResetSquare;
    }




    private void Start()
    {
        Selected = false;
        SquareOccupied = false;
        Hoversprite.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.localPosition;
        //ResetSquare();
    }
    public bool CanUseThisSquare()
    {
        return Hoversprite.gameObject.activeSelf;
    }
    public void ActivateSquare()
    {
        Hoversprite.gameObject.SetActive(false);
        activeSprite.gameObject.SetActive(true);
        activeSprite.gameObject.transform.localScale = Vector3.one*1.25f;
        Selected = true;
        SquareOccupied = true;
    }
    public void DeActivateSquare()
    {
        activeSprite.gameObject.SetActive(false);
    }
    public void ClearOccupied()
    {
        Selected = false;
        SquareOccupied = false; 
    }
    public void PlaceShapeOnBoard(Sprite spriteShape)
    {
        ActivateSquare();
        activeSprite.sprite= spriteShape;
    }
    public void GameOverAction()
    {
        ClearOccupied();
        TweenHelper.DoLocalScale(activeSprite.gameObject.transform, Vector3.zero, 0.5f, Ease.InBack, () =>
        {
            DeActivateSquare();
        });

    }
    public void ResetSquare()
    {
        ClearOccupied();
        DeActivateSquare();
    }

    public void SetSprite(bool setFirstSprite) 
    {   
        normalSprite.sprite = setFirstSprite ? Hoversprite.sprite : activeSprite.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!SquareOccupied ) 
        {
            var col = collision.GetComponent<ShapeSquare>();
            if (!col.isInsideAnySquare)
            {
                Selected = true;
                Hoversprite.gameObject.SetActive(true);
            }
        }
        else if(collision.GetComponent<ShapeSquare>() != null) 
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
            
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;
        if (!SquareOccupied)
        {
            var col = collision.GetComponent<ShapeSquare>();
            if (!col.isInsideAnySquare)
            {
                Hoversprite.gameObject.SetActive(true);
            }
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (!SquareOccupied)
        {
            collision.GetComponent<ShapeSquare>().isInsideAnySquare = false;
            Selected = false;
            Hoversprite.gameObject.SetActive(false);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupied();
        }
    }

}







