using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ZenGarden.Interactions
{
    /// <summary>
    /// Maneja el resaltado visual y respuesta táctil/visual al apuntar con XR Ray o Direct Interactor.
    /// </summary>
    [RequireComponent(typeof(XRBaseInteractable))]
    public class ObjectHighlight : MonoBehaviour
    {
        [Header("Configuración de Resaltado")]
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.6f, 1f); // Tono ámbar/dorado cálido zen
        [SerializeField] private float highlightIntensity = 1.3f;

        private MaterialPropertyBlock propBlock;
        private XRBaseInteractable interactable;
        private bool isHighlighted = false;

        private void Awake()
        {
            propBlock = new MaterialPropertyBlock();
            interactable = GetComponent<XRBaseInteractable>();

            if (targetRenderers == null || targetRenderers.Length == 0)
            {
                targetRenderers = GetComponentsInChildren<Renderer>();
            }

            if (interactable != null)
            {
                interactable.hoverEntered.AddListener(OnHoverEnter);
                interactable.hoverExited.AddListener(OnHoverExit);
            }
        }

        private void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.hoverEntered.RemoveListener(OnHoverEnter);
                interactable.hoverExited.RemoveListener(OnHoverExit);
            }
        }

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            SetHighlight(true);
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            SetHighlight(false);
        }

        public void SetHighlight(bool enable)
        {
            if (isHighlighted == enable) return;
            isHighlighted = enable;

            if (targetRenderers == null) return;

            foreach (var rend in targetRenderers)
            {
                if (rend == null) continue;
                rend.GetPropertyBlock(propBlock);

                if (enable)
                {
                    propBlock.SetColor("_EmissionColor", highlightColor * highlightIntensity);
                }
                else
                {
                    propBlock.SetColor("_EmissionColor", Color.black);
                }

                rend.SetPropertyBlock(propBlock);
            }
        }
    }
}
