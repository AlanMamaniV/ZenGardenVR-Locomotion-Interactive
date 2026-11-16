using UnityEngine;

namespace ZenGarden.Environment
{
    /// <summary>
    /// Controlador dinámico del estanque y fuentes del jardín Zen.
    /// Maneja ondulación de materiales y respuestas visuales del agua.
    /// </summary>
    public class WaterController : MonoBehaviour
    {
        [Header("Material de Agua")]
        [SerializeField] private Renderer waterRenderer;
        [SerializeField] private Vector2 waveSpeed = new Vector2(0.04f, 0.02f);
        [SerializeField] private Color deepWaterColor = new Color(0.12f, 0.38f, 0.45f, 0.8f);

        private Material waterMat;
        private Vector2 currentOffset = Vector2.zero;

        private void Start()
        {
            if (waterRenderer != null)
            {
                waterMat = waterRenderer.material;
                if (waterMat != null)
                {
                    waterMat.SetColor("_BaseColor", deepWaterColor);
                }
            }
        }

        private void Update()
        {
            if (waterMat != null)
            {
                currentOffset += waveSpeed * Time.deltaTime;
                waterMat.SetTextureOffset("_BaseMap", currentOffset);
            }
        }
    }
}
