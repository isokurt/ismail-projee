namespace EasyDoorSystem
{
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.Events;
    

    [RequireComponent(typeof(AudioSource))]
    public class EasyDoor : MonoBehaviour
    {
        
       
        public enum MovementType { Rotation, Position, Both }

        
        Collider doorCollider;

        [Header("Door Settings")]
        [SerializeField] private MovementType movementType = MovementType.Rotation;
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float autoCloseDelay = 0f;

        [Header("Interaction")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private float detectionRange = 3f;

        [Header("Key Item Requirement")]
        [SerializeField] private bool requiresItem = false;     // Bu kapý item istiyor mu?
        [SerializeField] private itemdata requiredItem;         // Gerekli item (Inspector'dan verilecek)
        [SerializeField] private bool consumeItemOnUse = false; // Kullanýlýnca silinsin mi?
        

        [Header("Transform Targets")]
        [SerializeField] private Vector3 closedRotation;
        [SerializeField] private Vector3 openedRotation;
        [SerializeField] private Vector3 closedPosition;
        [SerializeField] private Vector3 openedPosition;

        [Header("Audio")]
        [SerializeField] private AudioClip doorOpenSound;
        [SerializeField] private AudioClip doorCloseSound;
        [SerializeField][Range(0, 1)] private float audioVolume = 0.8f;
        [SerializeField] private AudioClip lockedSound; // Kapý kilitliyse çalacak ses (opsiyonel)

        [Header("Events")]
        public UnityEvent OnDoorOpening;
        public UnityEvent OnDoorClosed;
        
        

        public bool IsOpen { get; private set; }
        public bool IsMoving { get; private set; }

        private AudioSource audioSource;
        private Transform playerTransform;
        private Coroutine movementCoroutine;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
            doorCollider = GetComponent<Collider>();
           

            FindPlayer();
        }

        public void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        private void Update()
        {
            if (Time.timeScale == 0f)
                return;

            if (!playerTransform)
            {
                FindPlayer();
                return;
            }

            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= detectionRange)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    TryToggleDoor();                   
                }
            }
        }
       
        // ===================== ITEM KONTROLLÜ KAPI =====================

        void TryToggleDoor()
        {
            if (requiresItem && consumeItemOnUse &&
               !UI_itemController.instance.HasItem(requiredItem))
                return;

            if (CanOpenDoor())
            {
                if (requiresItem && consumeItemOnUse && UI_itemController.instance != null)
                {
                    UI_itemController.instance.RemoveAmount(requiredItem, 1);

                }

                ToggleDoor();
            }
            else
            {
                // Kapý kilitli
                PlaySound(lockedSound);
                Debug.Log("Kapý kilitli! Gerekli item yok veya yanlýþ item seçili.");
            }
        }

        
        bool CanOpenDoor()
        {
            if (!requiresItem)
                return true;

            if (UI_itemController.instance == null)
                return false;

            return UI_itemController.instance.HasItem(requiredItem);
        }


        // ===================== NORMAL KAPI SÝSTEMÝ =====================

        public void ToggleDoor()
        {
            if (IsOpen) CloseDoor();
            else OpenDoor();
        }

        public void OpenDoor()
        {
            if (IsOpen || IsMoving) return;

            if (doorCollider != null)
                doorCollider.enabled = false;
          

            MoveDoor(openedPosition, openedRotation, true);
            PlaySound(doorOpenSound);
            OnDoorOpening.Invoke();
        }


        public void CloseDoor()
        {
            if (!IsOpen || IsMoving) return;

            MoveDoor(closedPosition, closedRotation, false);
            PlaySound(doorCloseSound);
            OnDoorClosed.Invoke();

            if (doorCollider != null)
                doorCollider.enabled = true;
            

        }
       

        private void MoveDoor(Vector3 targetPosition, Vector3 targetRotation, bool opening)
        {
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);

            movementCoroutine = StartCoroutine(AnimateDoor(
                movementType != MovementType.Rotation ? targetPosition : transform.localPosition,
                movementType != MovementType.Position ? targetRotation : transform.localEulerAngles,
                opening
            ));
        }

        private System.Collections.IEnumerator AnimateDoor(Vector3 targetPos, Vector3 targetRot, bool opening)
        {
            IsMoving = true;
            Quaternion startRot = transform.localRotation;
            Vector3 startPos = transform.localPosition;
            Quaternion targetQuaternion = Quaternion.Euler(targetRot);

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * 1f;

                if (movementType != MovementType.Position)
                {
                    transform.localRotation = Quaternion.Slerp(startRot, targetQuaternion, progress * rotationSpeed);
                }

                if (movementType != MovementType.Rotation)
                {
                    transform.localPosition = Vector3.Lerp(startPos, targetPos, progress * movementSpeed);
                }

                yield return null;
            }

            if (movementType != MovementType.Position)
                transform.localRotation = targetQuaternion;

            if (movementType != MovementType.Rotation)
                transform.localPosition = targetPos;

            IsOpen = opening;
            IsMoving = false;

            if (autoCloseDelay > 0 && IsOpen)
                Invoke(nameof(CloseDoor), autoCloseDelay);
        }

        private void PlaySound(AudioClip clip)
        {
            if (!clip || !audioSource) return;

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.volume = audioVolume;
            audioSource.Play();
        }

        // Gizmo (Scene ekranýnda mesafe gösterir)
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }

        public void SaveCurrentState(bool saveAsOpen)
        {
            if (saveAsOpen)
            {
                openedRotation = transform.localEulerAngles;
                openedPosition = transform.localPosition;
            }
            else
            {
                closedRotation = transform.localEulerAngles;
                closedPosition = transform.localPosition;
            }
        }
    }
}

