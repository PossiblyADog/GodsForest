using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffCooldownManager : MonoBehaviour
{
    public Image LMB_image, RMB_image;
    public Slider LMB_slider, RMB_slider;
    public List<Sprite> cdIcons = new List<Sprite>();


    public static StaffCooldownManager instance;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

    }
    public void SetPanelData(string lmbImage, string rmbImage)
    {

        foreach(Sprite s in cdIcons)
        {
            if(s.name == lmbImage)
            {
                LMB_image.sprite = s;
            }
            else if(s.name == rmbImage)
            {
                RMB_image.sprite = s;
            }
        }

    }

    public void SetLMB_CD(float cd)
    {
        LMB_slider.maxValue = cd*60;
        LMB_slider.value = 0;

    }

    public void SetRMB_CD(float cd)
    {
        RMB_slider.maxValue = cd*60;
        RMB_slider.value = 0;
    }
    void Update()
    {
       if(LMB_slider.value < LMB_slider.maxValue)
       {
           LMB_slider.value++;
       }

       if(RMB_slider.value < RMB_slider.maxValue)
       {
           RMB_slider.value++;
       }

    }
}
