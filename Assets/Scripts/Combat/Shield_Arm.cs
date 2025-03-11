using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield_Arm : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private float Shield_Up_Time;
    private void OnEnable()
    {
        if (Player != null)
        {
            transform.LookAt(Player);
            StartCoroutine(Timer(Shield_Up_Time));
        }
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false); 
    }


}
