using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour//主要用于调用Gun脚本的方法
{
    public Transform weaponHold;
    public Gun[] allGuns;
    Gun equippedGun;

    private void Start()
    {
       
    }
    public void EquipGun(Gun gunToEquip) //把枪的预制体实例到weaponHold物体上
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
         
        equippedGun = Instantiate(gunToEquip,weaponHold.position,weaponHold.rotation) ;
        equippedGun.transform.parent = weaponHold;
    }
    public void EquipGun(int weaponIndex)
    {
        EquipGun(allGuns[weaponIndex]);
    }
    public void OnTriggerHold()
    {
        if(equippedGun != null)
        {
            equippedGun.OntriggerHold();
        }
    }
    public void OntriggerRelease()
    {
        equippedGun.OntriggerRelease();
    }
    public float GunHeight()
    {     
        return weaponHold.position.y;
    }
    public void Aim(Vector3 aimPoint)
    {
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }
    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }
}
