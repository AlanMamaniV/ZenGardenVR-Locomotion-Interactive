using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ZenGarden.Interactions
{
    /// <summary>
    /// Gestiona los eventos de selección e inspección de objetos del jardín Zen,
    /// comunicándose con la UI World Space para mostrar nombres, descripciones y acciones disponibles.
    /// </summary>
    public class ObjectSelectionManager : MonoBehaviour
    {
        [Header("Datos de Selección")]
        [SerializeField] private string objectName = "Elemento Zen";
        [TextArea(2, 4)]
        [SerializeField] private string objectDescription = "Descripción del elemento tradicional japonés.";
        [TextArea(1, 3)]
        [SerializeField] private string availableActions = "• Apunta para observar\n• Usa Grip para interactuar";

        private XRBaseInteractable interactable;

        private void Awake()
        {
            interactable = GetComponent<XRBaseInteractable>();
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
            if (VRInteractionUI.Instance != null)
            {
                VRInteractionUI.Instance.ShowObjectInfo(objectName, objectDescription, availableActions);
            }
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            if (VRInteractionUI.Instance != null)
            {
                VRInteractionUI.Instance.ShowDefaultPrompt();
            }
        }
    }
}
