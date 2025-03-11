using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{
    [SerializeField] private int DMG;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Gracz trafiony");
            I_Dmg_Able IDmgAble = other.GetComponent<I_Dmg_Able>();
            if (IDmgAble != null)
            {
                IDmgAble.Deal_Damage(DMG);
            }
        }
    }
}
