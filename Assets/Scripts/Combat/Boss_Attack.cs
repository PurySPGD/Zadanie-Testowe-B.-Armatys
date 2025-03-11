using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Attack : MonoBehaviour
{
    [SerializeField] private float expansionSpeed = 4.0f;
    [SerializeField] private float maxSize = 10.0f;
    [SerializeField] private float edgeThickness = 0.1f;   
    private float currentSize;
    [SerializeField] private float Starting_Size = 0.5f;
    [SerializeField] private int Attack_DMG = 20;


    void Start()
    {
        currentSize = Starting_Size;
    }

    void Update()
    {
        if (currentSize < maxSize)
        {
            currentSize += expansionSpeed * Time.deltaTime;
            this.gameObject.transform.localScale = new Vector3(currentSize, edgeThickness, currentSize);
        }
        else
        {
            currentSize = Starting_Size;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Gracz trafiony");
            I_Dmg_Able IDmgAble = other.GetComponent<I_Dmg_Able>();
            if (IDmgAble != null)
            {
                IDmgAble.Deal_Damage(Attack_DMG);
            }
        }
    }
}
