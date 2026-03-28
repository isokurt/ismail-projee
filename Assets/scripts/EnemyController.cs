using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private NavMeshAgent agent;
    public Animator animator;    
    float currentFootstepVolume = 0.7f;
    Transform player;

    int difficulty = 2;
    Vector3 lastPosition;
    float stuckTimer = 0f;
    bool isMoving = false;

    [Header("Vision Settings")]
    float viewDistance;
    float viewAngle;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask playerMask;

    [Header("Door Settings")]
    float doorCheckDistance = 2f;
    [SerializeField] LayerMask doorMask; // Door layer

    [Header("FootSteps")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] footstepClips;
    Coroutine footstepCoroutine;

    [Header("Detection sounds")]
    [SerializeField] AudioSource detectionAudio;
    [SerializeField] AudioClip playerDetected;
    [SerializeField] AudioClip soundDetected;
    int detected = 0;

    private enum EnemyState
    {
        Patrol,
        InvestigateSound,
        ChasePlayer
    }

    private EnemyState currentState = EnemyState.Patrol;
    private Vector3 soundPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        difficulty = PlayerPrefs.GetInt("difficulty", 2);
        StartCoroutine(StateMachine());        
        lastPosition = transform.position;        
        player = GameObject.FindGameObjectWithTag("Player").transform;        

    }

    void Update()
    {
        isMoving = agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;
        HandleFootsteps();
        PlayerDetection();
        SoundDetection();
        CheckForDoor();
        CheckStuck();        
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case EnemyState.Patrol:
                    yield return StartCoroutine(Patrol());
                    break;

                case EnemyState.InvestigateSound:
                    Difficulty();
                    agent.SetDestination(soundPosition);

                    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    {
                        yield return new WaitForSeconds(2f);                        
                        currentState = EnemyState.Patrol;
                    }
                    break;

                case EnemyState.ChasePlayer:
                    break;
            }

            yield return null;
        }
    }

    IEnumerator Patrol()
    {       
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 randomDir = Random.insideUnitSphere * 50f + transform.position;
            NavMeshHit hit;            

            if (NavMesh.SamplePosition(randomDir, out hit, 50f, NavMesh.AllAreas))
            {
                Difficulty();
                animations();
                HandleFootsteps();
                detected = 0;
                agent.SetDestination(hit.position);
                isMoving = true;
                yield return new WaitForSeconds(5f);
                
            }
        }
    }

    // ================= PLAYER VISION =================
    //void PlayerDetection()
    //{
    //    Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance, playerMask);

    //    foreach (var hit in hits)
    //    {
    //        if (!hit.CompareTag("Player")) continue;

    //        Vector3 dirToPlayer = (hit.transform.position - transform.position).normalized;
    //        float angle = Vector3.Angle(transform.forward, dirToPlayer);

    //        if (angle < viewAngle / 2f)
    //        {
    //            float distance = Vector3.Distance(transform.position, hit.transform.position);

    //            if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToPlayer, distance, obstacleMask))
    //            {
    //                Difficulty();
    //                animations();
    //                HandleFootsteps();
    //                agent.SetDestination(hit.transform.position);
    //                isMoving = true;
    //                currentState = EnemyState.ChasePlayer;
    //                return;
    //            }
    //        }
    //    }

    //    if (currentState == EnemyState.ChasePlayer)
    //        currentState = EnemyState.Patrol;
    //}
    void PlayerDetection()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance, playerMask);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            Vector3 enemyEyePos = transform.position + Vector3.up * 1.6f;
            Vector3 playerEyePos = hit.transform.position + Vector3.up * 1.6f;

            Vector3 dirToPlayer = (playerEyePos - enemyEyePos).normalized;
            float distance = Vector3.Distance(enemyEyePos, playerEyePos);
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            // Görüş açısı kontrolü
            if (angle > viewAngle / 2f)
                continue;

            // 🔥 DUVAR ENGEL KONTROLÜ (SADECE obstacleMask)
            if (Physics.Raycast(enemyEyePos, dirToPlayer, out RaycastHit hitInfo, distance, obstacleMask))
            {
                // Eğer ray önce duvara çarptıysa oyuncu görünmez
                continue;
            }

            // Eğer buraya geldiyse oyuncu net görünüyor
            Difficulty();
            animations();
            if (!detectionAudio.isPlaying && detected == 0)
            {
                detected = 1;
                audioSource.PlayOneShot(playerDetected);
            }
            agent.SetDestination(hit.transform.position);
            currentState = EnemyState.ChasePlayer;
            return;
        }

        if (currentState == EnemyState.ChasePlayer)
            currentState = EnemyState.Patrol;
    }


     //================= SOUND =================
    void SoundDetection()
    {
        if (currentState == EnemyState.ChasePlayer)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, 100f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Sound"))
            {
                animations();
                HandleFootsteps();
                if (!detectionAudio.isPlaying && detected == 0)
                {
                    detected = 1;
                    audioSource.PlayOneShot(soundDetected);
                }
                soundPosition = hit.transform.position;
                currentState = EnemyState.InvestigateSound;
                isMoving = true;
                return;
            }
        }
    }
    

    // ================= DOOR SYSTEM =================
    void CheckForDoor()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 1.2f, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, doorCheckDistance, doorMask))
        {
            EasyDoorSystem.EasyDoor door = hit.collider.GetComponent<EasyDoorSystem.EasyDoor>();

            if (door != null && !door.IsOpen && !door.IsMoving)
            {
                door.OpenDoor(); 
            }
        }
    }

    // ================= DIFFICULTY =================
    public void Difficulty()
    {
        if (difficulty == 1) // EASY
        {
            agent.speed = currentState == EnemyState.Patrol ? 2f :
                          currentState == EnemyState.ChasePlayer ? 4f : 6f;

            viewDistance = 15f;
            viewAngle = 120f;            
        }
        else if (difficulty == 2) // NORMAL
        {
            agent.speed = currentState == EnemyState.Patrol ? 3f :
                          currentState == EnemyState.ChasePlayer ? 5f : 8f;

            viewDistance = 20f;
            viewAngle = 150f;            
        }
        else if (difficulty == 3) // HARD
        {
            agent.speed = currentState == EnemyState.Patrol ? 4f :
                          currentState == EnemyState.ChasePlayer ? 6f : 10f;

            viewDistance = 30f;
            viewAngle = 200f;            
        }

       
    }

    void CheckStuck()
    {
        float movedDistance = Vector3.Distance(transform.position, lastPosition);

        if (movedDistance < 0.05f && agent.hasPath)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer > 3f)
            {
                Debug.Log("ENEMY STUCK! New path...");

                agent.ResetPath();

                Vector3 newPos = transform.position + Random.insideUnitSphere * 50f;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(newPos, out hit, 50f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }

                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    void animations()
    {
        if (animator == null) { Debug.LogWarning("Animator null"); return; }

        if (currentState == EnemyState.Patrol)
        {
            animator.SetBool("iswalking", true);
            animator.SetBool("isrunning", false);
            animator.SetBool("isidle", false);
        }
        else if (currentState == EnemyState.ChasePlayer)
        {
            animator.SetBool("iswalking", false);
            animator.SetBool("isrunning", true);
            animator.SetBool("isidle", false);
        }
        else if (currentState == EnemyState.InvestigateSound)
        {
            animator.SetBool("iswalking", false);
            animator.SetBool("isrunning", true);
            animator.SetBool("isidle", false);
        }
        else
        {
            animator.SetBool("iswalking", false);
            animator.SetBool("isrunning", false);
            animator.SetBool("isidle", true);
        }        
       
      
    }
    void HandleFootsteps()
    {       

        if (isMoving)
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
            if (!isMoving)
            {
                footstepCoroutine = null;
                yield break;
            }

            if (footstepClips.Length > 0)
            {
                AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
                audioSource.PlayOneShot(clip, currentFootstepVolume);
            }

            float delay = 0.5f;

            if (currentState == EnemyState.ChasePlayer)
                delay = 0.3f;
            else if (currentState == EnemyState.InvestigateSound)
                delay = 0.3f;

            yield return new WaitForSeconds(delay);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            UI_PlayerUI1.Instance.DeathScreen.SetActive(true);
            Destroy(gameObject);
        }
    }

    //void OnDrawGizmosSelected()
    //{
    //    Görüş mesafesi çemberi
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, viewDistance);

    //    Görüş açısı çizgileri
    //   Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
    //    Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
    //    Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);

    //    İleri yön çizgisi
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(transform.position, transform.position + transform.forward * viewDistance);
    //}

}

