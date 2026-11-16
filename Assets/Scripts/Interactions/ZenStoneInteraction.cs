using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ZenGarden.Interactions
{
    /// <summary>
    /// Permite agarrar, mover y colocar piedras sagradas en el jardín Zen.
    /// Al colocarse en un círculo o pedestal ceremonial, despierta un aura armónica y partículas.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
    public class ZenStoneInteraction : MonoBehaviour
    {
        [Header("Efectos Zen")]
        [SerializeField] private ParticleSystem placementParticles;
        [SerializeField] private Transform targetZenCircle;
        [SerializeField] private float snapDistance = 0.7f;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip placementClip;

        private XRGrabInteractable grabInteractable;
        private Rigidbody rb;
        private bool isPlacedInCircle = false;
        private Vector3 initialPosition;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            initialPosition = transform.position;

            if (grabInteractable != null)
            {
                grabInteractable.selectExited.AddListener(OnReleased);
            }
        }

        private void OnDestroy()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectExited.RemoveListener(OnReleased);
            }
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            if (targetZenCircle != null)
            {
                float dist = Vector3.Distance(transform.position, targetZenCircle.position);
                if (dist <= snapDistance)
                {
                    PlaceInCircle();
                    return;
                }
            }
        }

        public void PlaceInCircle()
        {
            if (isPlacedInCircle) return;
            isPlacedInCircle = true;

            if (targetZenCircle != null)
            {
                transform.position = targetZenCircle.position + Vector3.up * 0.15f;
            }

            rb.isKinematic = true;

            if (placementParticles != null)
            {
                placementParticles.Play();
            }

            if (audioSource != null && placementClip != null)
            {
                audioSource.PlayOneShot(placementClip);
            }

            if (VRInteractionUI.Instance != null)
            {
                VRInteractionUI.Instance.RegisterDiscovery("Piedra Zen en Equilibrio");
                VRInteractionUI.Instance.NotifyInteraction("Piedra Zen colocada en el centro de contemplación.");
            }
        }
    }
}
