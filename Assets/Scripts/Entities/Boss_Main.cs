using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Main : MonoBehaviour, I_Dmg_Able
{
    [SerializeField] private int Max_HP = 2000;
    [SerializeField] private int Current_HP = 2000;

    [SerializeField] private GameObject Shield;
    [SerializeField] private GameObject Attack;


    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float Max_Jump_Force = 17f;
    [SerializeField] private float Min_Jump_Force = 7f;
    [SerializeField] private float gravityMultiplier = 2f;
    private bool isGrounded;
    private bool Jumping;
    private bool Shield_Was_Last;
    [SerializeField] private Rigidbody Rigid_Body;
    [SerializeField] private Player_Main Player_Main;

    private void Start()
    {
        Reset_Boss();
    }

    public void Reset_Boss()
    {
        Jumping = false;
        isGrounded = true;
        Shield_Was_Last = true;
        Attack.SetActive(false);
        Shield.SetActive(false);

        gameObject.transform.position = new Vector3(0, 2.2f, 0);

        Current_HP = 2000;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        //wybiera losowy Atak je¿eli Nie ma aktywnego innego Ataku i jest w spoczynku
        if (!Jumping && !Shield.activeSelf && !Attack.activeSelf)
        {
            int Random_Value = Random.Range(0, 10);

            if (Random_Value <= 3 && Shield_Was_Last == false)
            {
                Shield.SetActive(true);
                Shield_Was_Last = true;
            }
            else
            {
               StartCoroutine(Jump_Attack());
               Shield_Was_Last = false;
            }
        }
        
    }

    IEnumerator Jump_Attack()
    {
        Jumping = true;
        isGrounded = false;

        int Jump_Force = (int)Random.Range(Min_Jump_Force, Max_Jump_Force);

        Rigid_Body.velocity = new Vector3(Rigid_Body.velocity.x, Jump_Force, Rigid_Body.velocity.z);

        yield return new WaitForSeconds(0.2f);

        while (isGrounded == false)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
            Rigid_Body.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
            yield return null;
        }

        Jumping = false;
        Attack.SetActive(true);
    }

    

    void I_Dmg_Able.Deal_Damage(int damage)
    {
        Current_HP -= damage;
        Debug.Log("Boss HP:" + Max_HP + "/" + Current_HP);
        if (Current_HP <= 0)
        {
            Boss_Dead();
        }
    }

    private void Boss_Dead()
    {
        Debug.Log("Boss is Dead. You Won!");
        StopAllCoroutines();
        Player_Main.Boss_Dead();
        gameObject.SetActive(false);


    }


}
