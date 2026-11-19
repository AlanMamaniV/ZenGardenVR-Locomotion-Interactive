using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

namespace ZenGarden.NPC
{
    /// <summary>
    /// Controlador del personaje virtual animado "Zen Guardian" (Kachujin) en el pabellón de contemplación.
    /// Responde interactivamente al jugador mediante animaciones corporales, seguimiento de mirada (gaze/head tracking),
    /// micro-expresiones faciales y gestos humanos (saludo, asentimiento, diálogo).
    /// </summary>
    public class ZenGuardianController : MonoBehaviour
    {
        [Header("Animación y Rig")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform headBone;
        [SerializeField] private Transform neckBone;
        [SerializeField] private Transform eyeLeftBone;
        [SerializeField] private Transform eyeRightBone;

        [Header("Seguimiento de la Mirada (Head & Eye Tracking)")]
        [SerializeField] private Transform playerHeadTarget;
        [SerializeField] private float gazeTrackingSpeed = 5.0f;
        [SerializeField] private float bodyRotationSpeed = 3.0f;
        [SerializeField] private float maxGazeAngle = 120.0f;
        [SerializeField] private float proximityDistance = 35.0f;
        [SerializeField] private float intimateDistance = 3.5f;

        [Header("Animación Facial y Parpadeo Autónomo")]
        [SerializeField] private SkinnedMeshRenderer faceRenderer;
        [SerializeField] private float blinkIntervalMin = 2.5f;
        [SerializeField] private float blinkIntervalMax = 5.5f;
        [SerializeField] private float blinkDuration = 0.15f;

        [Header("Audio y Reacciones")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip greetingClip;
        [SerializeField] private AudioClip wisdomClip;
        [SerializeField] private AudioClip bowClip;

        [Header("Interactables del Entorno Observados")]
        [SerializeField] private XRBaseInteractable zenStoneInteractable;
        [SerializeField] private XRBaseInteractable lanternInteractable;
        [SerializeField] private XRBaseInteractable[] allLanterns;
        [SerializeField] private XRBaseInteractable bonsaiInteractable;

        [Header("Diálogo / Subtítulos en World Space")]
        [SerializeField] private GameObject speechBubbleRoot;
        [SerializeField] private TextMeshProUGUI speechText;

        // Parámetros del Animator Controller
        private static readonly int AnimStateHash = Animator.StringToHash("State");
        private static readonly int TriggerWaveHash = Animator.StringToHash("TriggerWave");
        private static readonly int TriggerNodHash = Animator.StringToHash("TriggerNod");
        private static readonly int TriggerTalkHash = Animator.StringToHash("TriggerTalk");
        private static readonly int TriggerShakeHandsHash = Animator.StringToHash("TriggerShakeHands");

        // Estados del personaje
        public enum GuardianState
        {
            MeditatingIdle = 0,
            WavingGreeting = 1,
            TalkingWisdom = 2,
            NoddingRespect = 3,
            OfferingHandshake = 4
        }

        private GuardianState currentState = GuardianState.MeditatingIdle;
        private Quaternion initialHeadLocalRot;
        private Quaternion initialNeckLocalRot;
        private float nextBlinkTime;
        private bool isBlinking = false;
        private float blinkTimer = 0f;
        private bool playerInRange = false;
        private bool hasGreetedPlayer = false;
        private float stateTimer = 0f;

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();

            // Auto-detectar huesos de cabeza y cuello si no están asignados
            if (animator != null && animator.isHuman)
            {
                if (headBone == null) headBone = animator.GetBoneTransform(HumanBodyBones.Head);
                if (neckBone == null) neckBone = animator.GetBoneTransform(HumanBodyBones.Neck);
            }

            // Si es rig genérico (mixamo), buscar por jerarquía de transform
            if (headBone == null)
            {
                headBone = FindDeepChild(transform, "mixamorig:Head");
                if (headBone == null) headBone = FindDeepChild(transform, "Head");
            }
            if (neckBone == null)
            {
                neckBone = FindDeepChild(transform, "mixamorig:Neck");
                if (neckBone == null) neckBone = FindDeepChild(transform, "Neck");
            }

            if (headBone != null) initialHeadLocalRot = headBone.localRotation;
            if (neckBone != null) initialNeckLocalRot = neckBone.localRotation;

            nextBlinkTime = Time.time + Random.Range(blinkIntervalMin, blinkIntervalMax);
        }

        private void Start()
        {
            FindPlayerTarget();

            // Asegurar enlace de huesos
            EnsureBones();

            // Suscribirse a eventos de interacción del mundo para reaccionar humanamente
            HookEnvironmentInteractions();

            // Ocultar burbuja de diálogo inicial
            if (speechBubbleRoot != null) speechBubbleRoot.SetActive(false);
        }

        private void FindPlayerTarget()
        {
            if (playerHeadTarget != null) return;

            // 1. Intentar por MainCamera
            Camera cam = Camera.main;
            if (cam != null)
            {
                playerHeadTarget = cam.transform;
                return;
            }

            // 2. Intentar buscar por tag
            GameObject camGO = GameObject.FindGameObjectWithTag("MainCamera");
            if (camGO != null)
            {
                playerHeadTarget = camGO.transform;
                return;
            }

            // 3. Buscar cualquier Camera activa en la escena
            Camera anyCam = FindAnyObjectByType<Camera>();
            if (anyCam != null)
            {
                playerHeadTarget = anyCam.transform;
            }
        }

        private void EnsureBones()
        {
            if (headBone == null)
            {
                headBone = FindDeepChild(transform, "mixamorig:Head");
                if (headBone == null) headBone = FindDeepChild(transform, "Head");
            }
            if (neckBone == null)
            {
                neckBone = FindDeepChild(transform, "mixamorig:Neck");
                if (neckBone == null) neckBone = FindDeepChild(transform, "Neck");
            }
            if (headBone != null && initialHeadLocalRot == Quaternion.identity)
            {
                initialHeadLocalRot = headBone.localRotation;
            }
            if (neckBone != null && initialNeckLocalRot == Quaternion.identity)
            {
                initialNeckLocalRot = neckBone.localRotation;
            }
        }

        private void Update()
        {
            if (playerHeadTarget == null)
            {
                FindPlayerTarget();
            }

            CheckPlayerProximity();
            UpdateBodyRotation();
            UpdateBlinking();
            UpdateTimers();
        }

        private void LateUpdate()
        {
            // El seguimiento de mirada se calcula en LateUpdate después de las animaciones corporales
            UpdateGazeTracking();
        }

        /// <summary>
        /// Rota suavemente todo el cuerpo del personaje hacia el jugador si está en rango
        /// </summary>
        private void UpdateBodyRotation()
        {
            if (playerHeadTarget == null || !playerInRange) return;

            Vector3 lookDirection = playerHeadTarget.position - transform.position;
            lookDirection.y = 0; // Mantener rotación en el plano horizontal (Yaw)

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetBodyRot = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetBodyRot, Time.deltaTime * bodyRotationSpeed);
            }
        }

        /// <summary>
        /// Detecta la distancia del jugador y desencadena respuestas de comportamiento humano
        /// </summary>
        private void CheckPlayerProximity()
        {
            if (playerHeadTarget == null) return;

            float dist = Vector3.Distance(transform.position, playerHeadTarget.position);

            if (dist <= proximityDistance)
            {
                if (!playerInRange)
                {
                    playerInRange = true;
                    // Primera vez que se acerca: Saludar amablemente con la mano
                    if (!hasGreetedPlayer)
                    {
                        hasGreetedPlayer = true;
                        TriggerGreeting();
                    }
                }

                // Si está muy cerca (zona íntima en el pabellón de té) y en Idle, ofrecer apretón de manos o hablar
                if (dist <= intimateDistance && currentState == GuardianState.MeditatingIdle && stateTimer > 6.0f)
                {
                    TriggerWisdomDialog();
                }
            }
            else
            {
                playerInRange = false;
                if (currentState != GuardianState.MeditatingIdle && stateTimer > 4.0f)
                {
                    ReturnToIdle();
                }
            }
        }

        /// <summary>
        /// Rotación procedural de cuello y cabeza orientada hacia la vista del jugador (Gaze Tracking)
        /// </summary>
        private void UpdateGazeTracking()
        {
            if (playerHeadTarget == null || headBone == null || !playerInRange) return;

            Vector3 dirToPlayer = playerHeadTarget.position - headBone.position;
            if (dirToPlayer.sqrMagnitude < 0.001f) return;

            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            // Seguir con la mirada si el jugador está en el campo visual amplio
            if (angle <= maxGazeAngle)
            {
                Quaternion targetLookRot = Quaternion.LookRotation(dirToPlayer, Vector3.up);

                // Distribuir la rotación: 35% cuello, 65% cabeza
                if (neckBone != null)
                {
                    neckBone.rotation = Quaternion.Slerp(neckBone.rotation, targetLookRot, Time.deltaTime * gazeTrackingSpeed * 0.5f);
                }

                headBone.rotation = Quaternion.Slerp(headBone.rotation, targetLookRot, Time.deltaTime * gazeTrackingSpeed);
            }
        }

        /// <summary>
        /// Simulación de parpadeo biológico humano mediante blendshapes o sutil ajuste facial
        /// </summary>
        private void UpdateBlinking()
        {
            if (Time.time >= nextBlinkTime && !isBlinking)
            {
                isBlinking = true;
                blinkTimer = 0f;
            }

            if (isBlinking)
            {
                blinkTimer += Time.deltaTime;
                float progress = blinkTimer / blinkDuration;

                if (faceRenderer != null)
                {
                    // Buscar índice de blendshape de parpadeo (Blink o EyesClosed)
                    int blinkIndex = faceRenderer.sharedMesh.GetBlendShapeIndex("EyesClosed");
                    if (blinkIndex < 0) blinkIndex = faceRenderer.sharedMesh.GetBlendShapeIndex("Blink");
                    if (blinkIndex < 0) blinkIndex = faceRenderer.sharedMesh.GetBlendShapeIndex("blink");

                    if (blinkIndex >= 0)
                    {
                        float weight = Mathf.Sin(progress * Mathf.PI) * 100f;
                        faceRenderer.SetBlendShapeWeight(blinkIndex, weight);
                    }
                }

                if (blinkTimer >= blinkDuration)
                {
                    isBlinking = false;
                    nextBlinkTime = Time.time + Random.Range(blinkIntervalMin, blinkIntervalMax);
                }
            }
        }

        private void UpdateTimers()
        {
            stateTimer += Time.deltaTime;

            // Retorno automático a Idle tras finalizar animaciones de gestos
            if (currentState != GuardianState.MeditatingIdle && stateTimer > 4.5f)
            {
                ReturnToIdle();
            }
        }

        // ================= REACCIONES HUMANAS =================

        /// <summary>
        /// El guardián saluda con la mano (Waving) al ver llegar al jugador al pabellón
        /// </summary>
        public void TriggerGreeting()
        {
            currentState = GuardianState.WavingGreeting;
            stateTimer = 0f;

            if (animator != null)
            {
                animator.SetInteger(AnimStateHash, (int)GuardianState.WavingGreeting);
                animator.SetTrigger(TriggerWaveHash);
            }

            ShowSpeechBubble("¡Bienvenido al Pabellón de Contemplación, viajero de Kioto!", 4.0f);

            if (audioSource != null && greetingClip != null)
            {
                audioSource.PlayOneShot(greetingClip);
            }
        }

        /// <summary>
        /// El guardián asiente respetuosamente con la cabeza (Head Nod) cuando el jugador realiza una acción zen
        /// </summary>
        public void TriggerNodRespect(string reason = "")
        {
            currentState = GuardianState.NoddingRespect;
            stateTimer = 0f;

            if (animator != null)
            {
                animator.SetInteger(AnimStateHash, (int)GuardianState.NoddingRespect);
                animator.SetTrigger(TriggerNodHash);
            }

            string text = string.IsNullOrEmpty(reason) 
                ? "Siento la calma en tus acciones. Bien hecho." 
                : $"Tu devoción es digna: {reason}.";
            
            ShowSpeechBubble(text, 3.5f);

            if (audioSource != null && bowClip != null)
            {
                audioSource.PlayOneShot(bowClip);
            }
        }

        /// <summary>
        /// El guardián conversa activamente (Talking) gesticulando con las manos y el cuerpo
        /// </summary>
        public void TriggerWisdomDialog()
        {
            currentState = GuardianState.TalkingWisdom;
            stateTimer = 0f;

            if (animator != null)
            {
                animator.SetInteger(AnimStateHash, (int)GuardianState.TalkingWisdom);
                animator.SetTrigger(TriggerTalkHash);
            }

            ShowSpeechBubble("El jardín no busca perfección exterior, sino serenidad en el alma.", 4.5f);

            if (audioSource != null && wisdomClip != null)
            {
                audioSource.PlayOneShot(wisdomClip);
            }
        }

        public void ReturnToIdle()
        {
            currentState = GuardianState.MeditatingIdle;
            stateTimer = 0f;

            if (animator != null)
            {
                animator.SetInteger(AnimStateHash, (int)GuardianState.MeditatingIdle);
            }

            if (speechBubbleRoot != null)
            {
                speechBubbleRoot.SetActive(false);
            }
        }

        private void ShowSpeechBubble(string message, float duration)
        {
            if (speechBubbleRoot != null)
            {
                speechBubbleRoot.SetActive(true);
                if (speechText != null) speechText.text = message;
            }
        }

        private void HookEnvironmentInteractions()
        {
            // Reaccionar cuando el jugador interactúa con la Piedra Zen
            if (zenStoneInteractable != null)
            {
                zenStoneInteractable.selectEntered.AddListener((args) => TriggerNodRespect("has colocado la roca en armonía"));
            }

            // Reaccionar cuando el jugador enciende o apaga el farol principal
            if (lanternInteractable != null)
            {
                lanternInteractable.selectEntered.AddListener((args) => TriggerNodRespect("la llama sagrada ilumina el jardín"));
            }

            // Buscar y enlazar TODOS los faroles en la escena automáticamente
            var foundLanterns = FindObjectsByType<ZenGarden.Interactions.LanternInteraction>(FindObjectsSortMode.None);
            foreach (var lantern in foundLanterns)
            {
                var interactable = lantern.GetComponent<XRBaseInteractable>();
                if (interactable != null && interactable != lanternInteractable)
                {
                    interactable.selectEntered.AddListener((args) => TriggerNodRespect("la luz de otro farol guía tu espíritu"));
                }
            }

            // Reaccionar cuando el jugador contempla el bonsái
            if (bonsaiInteractable != null)
            {
                bonsaiInteractable.selectEntered.AddListener((args) => TriggerNodRespect("la paciencia del bonsái es eterna"));
            }
        }

        private static Transform FindDeepChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                Transform result = FindDeepChild(child, name);
                if (result != null) return result;
            }
            return null;
        }
    }
}
