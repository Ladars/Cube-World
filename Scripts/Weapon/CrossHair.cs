using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public float speed;
    public LayerMask targetMask;
    public SpriteRenderer _crossHair;
    public Color HighLightColour;
    public Color TeleportColour;
    Color OriginalColour;

    private Player player;
    private void Start()
    {
        Cursor.visible = false;
        OriginalColour = _crossHair.color;
        player = FindObjectOfType<Player>();
      
    }
    private void Update()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
        TeleportCursorHighLight();
    }
    public void DetectTargets(Ray ray) //准心检测到敌人，更换准心颜色 Change the colour of crosshair when it detect enemy
    {
        if (Physics.Raycast(ray, 1000, targetMask))
        {
            _crossHair.color = HighLightColour;
            _crossHair.transform.localScale = new Vector3(8, 8, 8);
        }
        else
        {
            _crossHair.color = OriginalColour;
            _crossHair.transform.localScale = new Vector3(4, 4, 4);
        }
    }
    private void TeleportCursorHighLight()
    {       
        if (Time.timeScale == 0.1f&&player.canTeleport ==true)
        {
            _crossHair.color = TeleportColour;
            _crossHair.transform.localScale = new Vector3(6, 6, 6);
        }
        if (Time.timeScale == 0.1f && player.canTeleport == false)
        {
            _crossHair.color = OriginalColour;
            _crossHair.transform.localScale = new Vector3(4, 4, 4);
        }
    }
}
