using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    GameManager gameManager;
    CharacterController controller;       

    [Header("Movement")]
    [SerializeField]float moveSpeed = 3f;
    [SerializeField] float sprintMultiplier = 1.6f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float jumpHeight = 0.7f;    

    [Header("Stamina")]
    public float stamina = 100f;
    [SerializeField] float maxStamina = 100f;

    [Header("Crouch")]
    [SerializeField] float crouchHeight = 1.0f;
    float normalHeight;
    float baseMoveSpeed;
    public bool isCrouching = false;

    [Header("Footsteps")]
    [SerializeField] AudioSource footstepSource;  
    [SerializeField] AudioClip[] footstepClips;
    [SerializeField] float walkStepDelay = 0.5f;
    [SerializeField] float sprintStepDelay = 0.3f;
    float currentFootstepVolume = 0.5f;

    Coroutine footstepCoroutine;

    [Header("Heartbeat")]
    [SerializeField] AudioSource heartbeatSource;
    [SerializeField] AudioClip[] heartbeatClips;

    [Header("Breathing")]
    [SerializeField] AudioClip breathingClip;


    [SerializeField] Vector3 velocity;   

    void Start()
    {
        controller = GetComponent<CharacterController>();       

        normalHeight = controller.height;
        baseMoveSpeed = moveSpeed;        

        gameManager = GameManager.Instance;

        StartCoroutine(Stamina());
        StartCoroutine(HeartBeatSound());
    }
    void Update()
    {
        Move();
        Crouch();
        DropItem();
        Jump();
        HandleFootsteps();
        FallDeath();        
    }
    // ================= MOVE =================
    void Move()
    {
        float horizontalMove = 0;
        float verticalMove = 0;

        if (Input.GetKey(KeyCode.W))
        {
            verticalMove += 1;
        }            
        if (Input.GetKey(KeyCode.S))
        {
            verticalMove -= 1;
        }           
        if (Input.GetKey(KeyCode.D))
        {
            horizontalMove += 1;
        }            
        if (Input.GetKey(KeyCode.A))
        {
            horizontalMove -= 1;
        }          

        Vector3 move = transform.right * horizontalMove + transform.forward * verticalMove;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(move * moveSpeed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

    }
    void Crouch()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (!isCrouching)
            {
                // Çömel
                controller.height = crouchHeight;
                controller.center = new Vector3(0, crouchHeight / 2f, 0);
                moveSpeed = baseMoveSpeed * 0.5f;
                isCrouching = true;
            }
        }
        else
        {
            if (isCrouching)
            {
                // Üstte engel var mı kontrolü
                float checkDistance = normalHeight - crouchHeight; // 1 birim yukarı
                if (!Physics.Raycast(transform.position + Vector3.up * crouchHeight, Vector3.up, checkDistance))
                {
                    // Kalk
                    controller.height = normalHeight;
                    controller.center = new Vector3(0, normalHeight / 2f, 0);
                    moveSpeed = baseMoveSpeed;
                    isCrouching = false;
                }
            }
        }
    }
    void DropItem()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Envanterden secili itemi al
            var inventory = UI_itemController.instance;
            if (inventory == null) return;

            //secili item al
            itemdata item = inventory.GetSelectedItem();
            if (item == null) return;

            Transform cam = Camera.main.transform;

            RaycastHit hit;
            Vector3 dropPos;

            // Kameradan ileri doğru ışın at
            if (Physics.Raycast(cam.position, cam.forward, out hit, 3f))
            {
                // Bir şeye çarptıysa onun ÜSTÜNE bırak
                dropPos = hit.point + Vector3.up * 0.3f;
            }
            else
            {
                // Hiçbir şeye çarpmadıysa → aşağıya ray at → yeri bul
                if (Physics.Raycast(cam.position, Vector3.down, out hit, 10f))
                {
                    dropPos = hit.point + Vector3.up * 0.3f;
                }
                else
                {
                    // En kötü ihtimal → oyuncunun ayağının dibine
                    dropPos = transform.position + transform.forward * 1.5f;
                }
            }

            // Item prefabını spawn et
            GameObject go = Instantiate(item.itemprefab, dropPos, Quaternion.identity);            

            // WorldItem componentine item datasını ata
            WorldItem wi = go.GetComponent<WorldItem>();
            if (wi != null)
                wi.data = item;

            inventory.RemoveSelectedItem();
        }
    }
    void Jump()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }     
    void FallDeath()
    {
        if(velocity.y < -15f && controller.isGrounded)
        {
            UI_PlayerUI1.Instance.DeathScreen.SetActive(true);            
        }
    }
    IEnumerator Stamina()
    {
        while (true)
        {
            if (Time.timeScale == 0f)
            {
                yield return null;
                continue;
            }

            // sprint yapma ve eger stamina varsa
            if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && !isCrouching)
            {
                moveSpeed = baseMoveSpeed * sprintMultiplier;
                stamina--;

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                moveSpeed = isCrouching ? baseMoveSpeed * 0.5f : baseMoveSpeed;

                if (stamina < maxStamina)
                {
                    // eger stamina sıfırsa 5 saniye bekle
                    if (stamina <= 0)
                    {
                        heartbeatSource.PlayOneShot(breathingClip);
                        yield return new WaitForSeconds(5f);                        
                        stamina = 5f;
                    }
                    else
                    {
                        stamina++;
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                else
                {
                    yield return null;
                }

            }
        }
    }
    void HandleFootsteps()
    {
        bool isMoving = (Input.GetKey(KeyCode.W) ||Input.GetKey(KeyCode.A) ||Input.GetKey(KeyCode.S) ||Input.GetKey(KeyCode.D));                                              

        if (isMoving && controller.isGrounded && !isCrouching)
        {
            if (footstepCoroutine == null)
                footstepCoroutine = StartCoroutine(FootstepSound());
        }
        else
        {
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }
    }
    IEnumerator FootstepSound()
    {
        while (true)
        {
            if (!controller.isGrounded)
            {
                yield return null;
                continue;
            }

            if (footstepClips.Length > 0)
            {
                AudioClip clip = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
                footstepSource.PlayOneShot(clip, currentFootstepVolume);
            }


            float delay = Input.GetKey(KeyCode.LeftShift)? sprintStepDelay: walkStepDelay;              
                
            yield return new WaitForSeconds(delay);
        }
    }
    IEnumerator HeartBeatSound()
    {
        float maxDistance = 13f;

        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, maxDistance);
            float closestDistance = maxDistance;
            bool enemyNearby = false;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    enemyNearby = true;
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < closestDistance)
                        closestDistance = dist;
                }
            }

            if (enemyNearby)
            {
                float heartbeatVolume = Mathf.Clamp01(1f - (closestDistance / maxDistance));
               
                currentFootstepVolume = 1f - heartbeatVolume * 0.7f;

                heartbeatSource.PlayOneShot(heartbeatClips[Random.Range(0, heartbeatClips.Length)],heartbeatVolume);                                                    
            }
            else
            {
                currentFootstepVolume = 0.5f;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}