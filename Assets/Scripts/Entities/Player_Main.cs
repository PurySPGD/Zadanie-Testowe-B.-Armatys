using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Main : MonoBehaviour, I_Dmg_Able
{
    [SerializeField] private int HP_Max = 100;
    public static int DMG = 5;


    [SerializeField] private int Current_HP;
    [SerializeField] private Slider HP_Bar;

    [SerializeField] private float Move_Speed = 5f;
    [SerializeField] private float Jump_Force = 8.5f;
    [SerializeField] private float Gravity_Multiplyer = 2f;

    [SerializeField] private float Mouse_Sensitivity = 600f;
    [SerializeField] private Transform Player_Camera_Transform;

    private Rigidbody Rigid_Body;
    private float xRotation = 0f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 15;
    private Queue<GameObject> projectilePool = new Queue<GameObject>();

    [SerializeField] private Camera Player_Camera;

    [SerializeField] private GameObject Main_Menu_UI;
    [SerializeField] private GameObject EQ_UI;
    [SerializeField] private GameObject Gameplay_UI;
    [SerializeField] private GameObject Boss;
    [SerializeField] private GameObject Win_Screen;

    // public Button resumeButton;     

    private bool isPaused = false;
    private bool First_Load = false;

    void Start()
    {
        Rigid_Body = GetComponent<Rigidbody>();
        Rigid_Body.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Current_HP = HP_Max;
        Debug.Log("Player HP: " + Current_HP);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectilePool.Enqueue(obj);
        }

        PauseGame();
        First_Load = true;
    }

    public void Update_Stats(int DMG_Value)
    {
        DMG = DMG_Value;
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!Inventory.Data_Lodaed)
        {
            return;
        }

        Time.timeScale = 1f; 
        Gameplay_UI.SetActive(true);
        Main_Menu_UI.SetActive(false);
        EQ_UI.SetActive(false);

        isPaused = false;
        First_Load = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (Win_Screen.activeSelf || !Inventory.Data_Lodaed || First_Load)
        {
            return;
        }

        // Sprawdzanie które Menu trzeba otworzyæ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Main_Menu_UI.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                Main_Menu_UI.SetActive(true);
                EQ_UI.SetActive(false);
                Gameplay_UI.SetActive(false);

                PauseGame(); 
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (EQ_UI.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                Main_Menu_UI.SetActive(false);
                EQ_UI.SetActive(true);
                Gameplay_UI.SetActive(false);

                PauseGame();
            }
        }

        if (isPaused)
        {
            return;
        }

        MovePlayer();
        Check_Jump();
        RotateCamera();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetProjectile();
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        Vector3 newVelocity = new Vector3(moveDirection.x * Move_Speed, Rigid_Body.velocity.y, moveDirection.z * Move_Speed);

        Rigid_Body.velocity = newVelocity;
    }

    void Check_Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Rigid_Body.velocity = new Vector3(Rigid_Body.velocity.x, Jump_Force, Rigid_Body.velocity.z);
        }

        if (!isGrounded)
        {
            Rigid_Body.AddForce(Vector3.down * Gravity_Multiplyer, ForceMode.Acceleration);
        }
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * Mouse_Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Mouse_Sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Player_Camera_Transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public GameObject GetProjectile()
    {
        GameObject obj = projectilePool.Dequeue();
        projectilePool.Enqueue(obj);

        Vector3 spawnPosition = transform.position + transform.forward * 1.5f;

        obj.transform.position = spawnPosition;
        obj.transform.rotation = Player_Camera_Transform.transform.rotation;
        obj.SetActive(true);

        return obj;
    }
    void I_Dmg_Able.Deal_Damage(int DMG)
    {
        Current_HP -= DMG;
        HP_Bar.value = (float)Current_HP / 100;
        Debug.Log("Player took damage! Current HP: " + Current_HP);

        if (Current_HP <= 0)
        {
            Debug.Log("Player has died!");
            Reset_Player();
        }
    }
   
    public void Reset_Player()// Po œmierci soft reset
    {

        gameObject.transform.position = new Vector3(0, 1, -10);
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Current_HP = 100;
        HP_Bar.value = (float)Current_HP / 100;

        Main_Menu_UI.SetActive(true);
        EQ_UI.SetActive(false);
        Gameplay_UI.SetActive(false);

        PauseGame();

        Boss.SetActive(true);
        Boss.GetComponent<Boss_Main>().Reset_Boss();

    }

    public void Boss_Dead()
    {
        StartCoroutine(Victory(5f));
    }

    private IEnumerator Victory(float waitTime)
    {
        Win_Screen.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        Win_Screen.SetActive(false);

        Reset_Player();
    }
}
