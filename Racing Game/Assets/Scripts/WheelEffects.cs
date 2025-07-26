using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    // This component requires an AudioSource to function
    [RequireComponent(typeof(AudioSource))]
    public class WheelEffects : MonoBehaviour
    {
        // Prefab for the visual trail left by skidding
        public Transform SkidTrailPrefab;

        // Static parent object for detached skid trails (for cleanup)
        public static Transform skidTrailsDetachedParent;

        // Particle system for emitting tire smoke
        public ParticleSystem skidParticles;

        // Property to indicate whether the wheel is currently skidding
        public bool skidding { get; private set; }

        // Property to indicate whether skid audio is currently playing
        public bool PlayingAudio { get; private set; }

        // Private references to components
        private AudioSource m_AudioSource;
        private Transform m_SkidTrail;
        private WheelCollider m_WheelCollider;

        private void Start()
        {
            // Find the particle system in the car's hierarchy
            skidParticles = transform.root.GetComponentInChildren<ParticleSystem>();

            // Warn if no particle system is found, otherwise stop it at start
            if (skidParticles == null)
            {
                Debug.LogWarning("No particle system found on car to generate smoke particles", gameObject);
            }
            else
            {
                skidParticles.Stop();
            }

            // Cache the WheelCollider and AudioSource components
            m_WheelCollider = GetComponent<WheelCollider>();
            m_AudioSource = GetComponent<AudioSource>();
            PlayingAudio = false;

            // Create a parent object to hold detached skid trails if it doesn't already exist
            if (skidTrailsDetachedParent == null)
            {
                skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
            }
        }

        // Call this method to emit a single smoke particle and start skid trail if not already started
        public void EmitTyreSmoke()
        {
            // Position the smoke particles just below the wheel
            skidParticles.transform.position = transform.position - transform.up * m_WheelCollider.radius;
            skidParticles.Emit(1);

            // Start the skid trail coroutine if not already skidding
            if (!skidding)
            {
                StartCoroutine(StartSkidTrail());
            }
        }

        // Plays the skidding audio
        public void PlayAudio()
        {
            m_AudioSource.Play();
            PlayingAudio = true;
        }

        // Stops the skidding audio
        public void StopAudio()
        {
            m_AudioSource.Stop();
            PlayingAudio = false;
        }

        // Coroutine to start the skid trail effect
        public IEnumerator StartSkidTrail()
        {
            skidding = true;

            // Instantiate the skid trail prefab
            m_SkidTrail = Instantiate(SkidTrailPrefab);

            // Wait until the skid trail is ready
            while (m_SkidTrail == null)
            {
                yield return null;
            }

            // Parent the skid trail to the wheel and position it correctly
            m_SkidTrail.parent = transform;
            m_SkidTrail.localPosition = -Vector3.up * m_WheelCollider.radius;
        }

        // Ends the skid trail effect and detaches the trail object
        public void EndSkidTrail()
        {
            // Only stop if skidding is currently happening
            if (!skidding)
            {
                return;
            }

            skidding = false;

            // Detach the skid trail and schedule it for destruction
            m_SkidTrail.parent = skidTrailsDetachedParent;
            Destroy(m_SkidTrail.gameObject, 10);
        }
    }
}
