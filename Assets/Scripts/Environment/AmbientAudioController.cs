using UnityEngine;

namespace ZenGarden.Environment
{
    /// <summary>
    /// Controla la ambientación sonora relajante del jardín japonés:
    /// sonido del agua constante, trinar sutil de pájaros y brisa del viento.
    /// </summary>
    public class AmbientAudioController : MonoBehaviour
    {
        [Header("Fuentes de Sonido")]
        [SerializeField] private AudioSource ambientNatureSource;
        [SerializeField] private AudioSource waterStreamSource;
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 0.6f;

        private void Start()
        {
            if (ambientNatureSource != null)
            {
                ambientNatureSource.loop = true;
                ambientNatureSource.volume = masterVolume * 0.7f;
                if (!ambientNatureSource.isPlaying) ambientNatureSource.Play();
            }

            if (waterStreamSource != null)
            {
                waterStreamSource.loop = true;
                waterStreamSource.volume = masterVolume * 0.8f;
                if (!waterStreamSource.isPlaying) waterStreamSource.Play();
            }
        }

        public void SetMasterVolume(float vol)
        {
            masterVolume = Mathf.Clamp01(vol);
            if (ambientNatureSource != null) ambientNatureSource.volume = masterVolume * 0.7f;
            if (waterStreamSource != null) waterStreamSource.volume = masterVolume * 0.8f;
        }
    }
}
