using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ZenGarden.Interactions
{
    /// <summary>
    /// Bonsái ceremonial de cerezo o pino japonés.
    /// Al seleccionarse, activa una suave lluvia de pétalos o esporas zen y despliega su mensaje de contemplación.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class BonsaiInteraction : MonoBehaviour
    {
        [Header("Efectos")]
        [SerializeField] private ParticleSystem cherryBlossomParticles;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip chimeClip;
        [SerializeField] private Transform treeModel;

        private XRSimpleInteractable simpleInteractable;
        private bool isInteracted = false;
        private float swaySpeed = 1.5f;

        private void Awake()
        {
            simpleInteractable = GetComponent<XRSimpleInteractable>();

            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.AddListener(OnBonsaiSelect);
            }
        }

        private void OnDestroy()
        {
            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.RemoveListener(OnBonsaiSelect);
            }
        }

        private void Update()
        {
            // Sutil animación orgánica de balanceo con la brisa
            if (treeModel != null)
            {
                float rotZ = Mathf.Sin(Time.time * swaySpeed) * 0.8f;
                treeModel.localRotation = Quaternion.Euler(0, 0, rotZ);
            }
        }

        private void OnBonsaiSelect(SelectEnterEventArgs args)
        {
            TriggerBonsai();
        }

        [ContextMenu("Trigger Bonsai")]
        public void TriggerBonsai()
        {
            if (cherryBlossomParticles != null)
            {
                cherryBlossomParticles.Play();
            }

            if (audioSource != null && chimeClip != null)
            {
                audioSource.PlayOneShot(chimeClip);
            }

            if (VRInteractionUI.Instance != null)
            {
                VRInteractionUI.Instance.NotifyInteraction("Bonsái tradicional: La armonía entre paciencia y naturaleza.");
                if (!isInteracted)
                {
                    isInteracted = true;
                    VRInteractionUI.Instance.RegisterDiscovery("Espíritu del Bonsái");
                }
            }
        }
    }
}
