using UnityEngine;
using TMPro;

namespace ZenGarden.Interactions
{
    /// <summary>
    /// Interfaz gráfica World Space con estética japonesa minimalista para mostrar
    /// información contextual del objeto seleccionado, instrucciones y progreso de exploración.
    /// </summary>
    public class VRInteractionUI : MonoBehaviour
    {
        public static VRInteractionUI Instance { get; private set; }

        [Header("Elementos de Texto")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI actionText;
        [SerializeField] private TextMeshProUGUI counterText;
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Configuración")]
        [SerializeField] private int totalDiscoverableObjects = 4;
        private int discoveredCount = 0;
        private float notificationTimer = 0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            UpdateCounterDisplay();
            ShowDefaultPrompt();
        }

        private void Update()
        {
            if (notificationTimer > 0)
            {
                notificationTimer -= Time.deltaTime;
                if (notificationTimer <= 0 && notificationText != null)
                {
                    notificationText.text = "";
                }
            }
        }

        public void ShowDefaultPrompt()
        {
            if (titleText != null) titleText.text = "JARDÍN ZEN DE KIOTO";
            if (descriptionText != null) descriptionText.text = "Explora los senderos de piedra, contempla la naturaleza y conecta con la armonía del jardín.";
            if (actionText != null) actionText.text = "• Teletransporte: Apunta al sendero o arena\n• Interactuar: Apunta con el rayo y presiona Trigger/Grip";
        }

        public void ShowObjectInfo(string objectName, string description, string actions)
        {
            if (titleText != null) titleText.text = objectName.ToUpper();
            if (descriptionText != null) descriptionText.text = description;
            if (actionText != null) actionText.text = actions;
        }

        public void NotifyInteraction(string message)
        {
            if (notificationText != null)
            {
                notificationText.text = "✦ " + message;
                notificationTimer = 4.5f;
            }
        }

        public void RegisterDiscovery(string itemName)
        {
            discoveredCount++;
            UpdateCounterDisplay();
            NotifyInteraction("Descubierto: " + itemName);
        }

        private void UpdateCounterDisplay()
        {
            if (counterText != null)
            {
                counterText.text = $"Elementos en Armonía: {discoveredCount} / {totalDiscoverableObjects}";
            }
        }
    }
}
