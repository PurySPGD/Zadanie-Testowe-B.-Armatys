using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int Dmg = 10;
    [SerializeField] private float lifetime = 5; 
    [SerializeField] private float speed = 10;   

    private Rigidbody Rigid_Body;
    [SerializeField] private SphereCollider Projectil_Colider;
    void OnEnable ()
    {
        Rigid_Body = GetComponent<Rigidbody>();
        if (Rigid_Body != null)
        {
            Rigid_Body.velocity = transform.forward * speed;
        }

        StartCoroutine(Life_Timer(lifetime));

        Dmg = Player_Main.DMG;
    }
    IEnumerator Life_Timer(float time)
    {
        yield return new WaitForSeconds(time);
        DeactivateProjectile();
    }

    void DeactivateProjectile()
    {
        if (gameObject.activeSelf)
        {
            StopAllCoroutines();

            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            gameObject.transform.localRotation = new Quaternion(0, 0, 0,0);

            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name);
        DeactivateProjectile();
    }

    void OnTriggerEnter(Collider other)
    {
        DeactivateProjectile();

        if (other.gameObject.tag == "Boss")
        {
            Debug.Log("Boss trafiony");
            I_Dmg_Able IDmgAble = other.GetComponent<I_Dmg_Able>();
            if (IDmgAble != null)
            {
                IDmgAble.Deal_Damage(Dmg);
            }
        }
    }
}
