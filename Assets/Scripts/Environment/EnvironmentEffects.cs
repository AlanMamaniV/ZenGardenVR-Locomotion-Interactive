using UnityEngine;

namespace ZenGarden.Environment
{
    /// <summary>
    /// Maneja efectos atmosféricos del jardín:
    /// balanceo suave de bambú y vegetación con el viento, y emisión de pétalos de sakura.
    /// </summary>
    public class EnvironmentEffects : MonoBehaviour
    {
        [Header("Efectos Atmosféricos")]
        [SerializeField] private ParticleSystem fallingSakuraPetals;
        [SerializeField] private Transform[] foliageRoots;
        [SerializeField] private float windFrequency = 1.2f;
        [SerializeField] private float windStrength = 1.1f;

        private void Start()
        {
            if (fallingSakuraPetals != null && !fallingSakuraPetals.isPlaying)
            {
                fallingSakuraPetals.Play();
            }
        }

        private void Update()
        {
            if (foliageRoots != null)
            {
                float angle = Mathf.Sin(Time.time * windFrequency) * windStrength;
                for (int i = 0; i < foliageRoots.Length; i++)
                {
                    if (foliageRoots[i] != null)
                    {
                        foliageRoots[i].localRotation = Quaternion.Euler(0, 0, angle * (i % 2 == 0 ? 1 : -1));
                    }
                }
            }
        }
    }
}
