using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathChecker : MonoBehaviour
{
    //other
    public bool PlayerIsDeath;
    Player player;
    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    
}
