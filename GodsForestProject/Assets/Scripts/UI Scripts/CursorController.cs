
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public static CursorController instance;
    public SpriteRenderer cursorImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    private void OnEnable()
    {
        if (Cursor.visible)
        { Cursor.visible = false; }
        try
        {
            CursorManager.instance.LoadCursorData(GameManager.instance.cursorInfo);
        }
        catch
        {

        }
    }

    public void SetCursorImage(Sprite newCursor, Color cursorColor)
    {
        cursorImage.sprite = newCursor;
        cursorImage.color = cursorColor;
    }
    void FixedUpdate()
    {
        if (Camera.main != null) 
        { 
        Vector2 targetPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = targetPos + new Vector2(0, 0f); 
        }
    }
}
