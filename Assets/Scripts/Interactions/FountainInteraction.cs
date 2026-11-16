using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ZenGarden.Interactions
{
    /// <summary>
    /// Fuente de bambú tradicional japonesa (Tsukubai / Shishi-odoshi).
    /// Permite abrir o cerrar el flujo del chorro de agua y su apacible sonido ambiental.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class FountainInteraction : MonoBehaviour
    {
        [Header("Componentes de Agua")]
        [SerializeField] private ParticleSystem waterStreamParticles;
        [SerializeField] private ParticleSystem waterRippleParticles;
        [SerializeField] private AudioSource waterAudioSource;
        [SerializeField] private Transform bambooLever;

        private XRSimpleInteractable simpleInteractable;
        private bool isWaterFlowing = true;
        private float leverAngle = 0f;

        private void Awake()
        {
            simpleInteractable = GetComponent<XRSimpleInteractable>();

            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.AddListener(OnSelected);
            }

            ApplyFlowState(true);
        }

        private void OnDestroy()
        {
            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.RemoveListener(OnSelected);
            }
        }

        private void OnSelected(SelectEnterEventArgs args)
        {
            ToggleWater();
        }

        [ContextMenu("Toggle Water")]
        public void ToggleWater()
        {
            isWaterFlowing = !isWaterFlowing;
            ApplyFlowState(isWaterFlowing);

            if (VRInteractionUI.Instance != null)
            {
                string status = isWaterFlowing ? "Fuente Tsukubai: Flujo de agua purificadora activo" : "Fuente Tsukubai en calma y reposo";
                VRInteractionUI.Instance.NotifyInteraction(status);

                if (isWaterFlowing)
                {
                    VRInteractionUI.Instance.RegisterDiscovery("Agua Sagrada Tsukubai");
                }
            }
        }

        private void ApplyFlowState(bool flow)
        {
            if (waterStreamParticles != null)
            {
                if (flow) waterStreamParticles.Play();
                else waterStreamParticles.Stop();
            }

            if (waterRippleParticles != null)
            {
                if (flow) waterRippleParticles.Play();
                else waterRippleParticles.Stop();
            }

            if (waterAudioSource != null)
            {
                waterAudioSource.mute = !flow;
            }

            if (bambooLever != null)
            {
                bambooLever.localRotation = Quaternion.Euler(flow ? 15f : -10f, 0, 0);
            }
        }
    }
}
