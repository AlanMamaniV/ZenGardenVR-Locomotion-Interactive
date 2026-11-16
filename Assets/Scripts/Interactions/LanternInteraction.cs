using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ZenGarden.Interactions
{
    /// <summary>
    /// Farol de piedra o madera tradicional japonés (Tōrō).
    /// Permite encender y apagar su fuego cálido interior mediante selección a distancia o directa.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class LanternInteraction : MonoBehaviour
    {
        [Header("Componentes de Iluminación")]
        [SerializeField] private Light lanternLight;
        [SerializeField] private Renderer fireFlameRenderer;
        [SerializeField] private ParticleSystem flameParticles;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip toggleClip;

        [Header("Colores")]
        [SerializeField] private Color litColor = new Color(1f, 0.75f, 0.35f, 1f); // Dorado cálido
        [SerializeField] private float lightIntensity = 2.2f;

        private XRSimpleInteractable simpleInteractable;
        private bool isLit = false;
        private Material flameMat;

        private void Awake()
        {
            simpleInteractable = GetComponent<XRSimpleInteractable>();

            if (fireFlameRenderer != null)
            {
                flameMat = fireFlameRenderer.material;
            }

            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.AddListener(OnSelected);
            }

            // Iniciar apagado o en estado inicial
            ApplyState(false);
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
            ToggleLantern();
        }

        [ContextMenu("Toggle Lantern")]
        public void ToggleLantern()
        {
            isLit = !isLit;
            ApplyState(isLit);

            if (audioSource != null && toggleClip != null)
            {
                audioSource.PlayOneShot(toggleClip);
            }

            if (VRInteractionUI.Instance != null)
            {
                string status = isLit ? "Farol Tōrō Encendido" : "Farol Tōrō Apagado";
                VRInteractionUI.Instance.NotifyInteraction(status);

                if (isLit)
                {
                    VRInteractionUI.Instance.RegisterDiscovery("Luz del Farol Tōrō");
                }
            }
        }

        private void ApplyState(bool lit)
        {
            if (lanternLight != null)
            {
                lanternLight.enabled = lit;
                lanternLight.color = litColor;
                lanternLight.intensity = lightIntensity;
            }

            if (flameMat != null)
            {
                if (lit)
                {
                    flameMat.SetColor("_EmissionColor", litColor * 2.5f);
                    flameMat.EnableKeyword("_EMISSION");
                }
                else
                {
                    flameMat.SetColor("_EmissionColor", Color.black);
                }
            }

            if (flameParticles != null)
            {
                if (lit) flameParticles.Play();
                else flameParticles.Stop();
            }
        }
    }
}
