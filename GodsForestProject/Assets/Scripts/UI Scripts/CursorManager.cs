using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public List<Sprite> cursorList = new List<Sprite>();
    public List<Slider> sliderList = new List<Slider>();
    public Image displayTexture;
    float red, green, blue, opacity;
    int currentIndex = 0;
    public float[] cursorData = new float[5];
    public GameObject cursorSettingPanel;
    public static CursorManager instance;

    private void Awake()
    {
        if (instance == null)
        { instance = this; }
        else
        { Destroy(gameObject); }
    }

    void Update()
    {
        if (cursorSettingPanel.activeSelf)
        {
            red = sliderList[0].value; 
            green = sliderList[1].value; 
            blue = sliderList[2].value; 
            opacity = sliderList[3].value;

            displayTexture.color = new Color(red / 255.0f, green / 255.0f, blue / 255.0f, opacity / 255.0f);
        }
    }

    public void CycleCursorTexture(bool next)
    {     
        if (next)
        {
            if(currentIndex < cursorList.Count - 1)
            {
                currentIndex++;
                displayTexture.sprite = cursorList[currentIndex];
            }
            else
            {
                currentIndex = 0;
                displayTexture.sprite = cursorList[currentIndex];
            }
        }
        else
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                displayTexture.sprite = cursorList[currentIndex];
            }
            else
            {
                currentIndex = cursorList.Count - 1;
                displayTexture.sprite = cursorList[currentIndex];
            }
        }
    }

    public void SendCursorData()
    {
        if (CursorController.instance != null)
        { CursorController.instance.SetCursorImage(displayTexture.sprite, displayTexture.color); }
        cursorData = new float[] { currentIndex, displayTexture.color.r,displayTexture.color.g, displayTexture.color.b, displayTexture.color.a};
        GameManager.instance.cursorInfo = cursorData;
    }

    public void SendCursorData(Color cursorColor, Sprite cursorSprite)
    {
        if (CursorController.instance != null)
        { CursorController.instance.SetCursorImage(cursorSprite, cursorColor); }
    }

    public void SetCustomCursor()
    {
        //Texture2D cursorTexture = displayTexture.sprite.texture;
        
    }
    public void LoadCursorData(float[] saveData)
    {
        try
        {
            if (saveData.Length == 5 && saveData[4] > 0)
            {
                currentIndex = (int)saveData[0];
                red = saveData[1];
                green = saveData[2];
                blue = saveData[3];
                opacity = saveData[4];

                var savedColor = new Color(red, green, blue, opacity);
                var savedTexture = cursorList[currentIndex];

                SendCursorData(savedColor, savedTexture);
            }
            else
            {
                SendCursorData(new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f), cursorList[3]);
            }
        }
        catch
        {
            SendCursorData(new Color(255.0f/255.0f, 255.0f / 255.0f, 255.0f / 255.0f), cursorList[3]);
        }
    }

    public void Accept()
    {
        SendCursorData();
    }
}
