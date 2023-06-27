using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIDoTween : MonoBehaviour
{
    [Header("GunUI")]
    public Transform[] gunUI;
    public Transform Flash;
    public Image flashEffect;
    // public static UIDoTween instance;
    // Start is called before the first frame update
    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        if (instance != null)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //    DontDestroyOnLoad(gameObject);
    //}
    public void GunDoShakeEffect(int index) //枪械UI抖动效果
    {
        gunUI[index].DOShakeScale(1f, 0.5f, 5);
    }
    public void GunDoShakeEffect() //枪械UI抖动效果
    {
        Flash.DOShakeScale(1f, 0.5f, 5);
    }
    public void FlashAlphaEffect(float alpha)
    {
        flashEffect.DOFade(alpha, 0.5f);
    }    
}
