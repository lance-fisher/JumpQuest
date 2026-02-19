using UnityEngine;
using System.Collections.Generic;

namespace JumpQuest
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("SFX Clips")]
        public AudioClip CoinClip;
        public AudioClip JumpClip;
        public AudioClip FinishClip;
        public AudioClip CheckpointClip;
        public AudioClip JumpPadClip;
        public AudioClip ButtonClip;

        [Header("Music")]
        public AudioClip MusicClip;
        public float MusicVolume = 0.4f;
        public float SfxVolume = 0.8f;

        private AudioSource musicSource;
        private AudioSource sfxSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = MusicVolume;
            musicSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = SfxVolume;
            sfxSource.playOnAwake = false;
        }

        public void PlaySFX(string name)
        {
            AudioClip clip = GetClip(name);
            if (clip != null)
                sfxSource.PlayOneShot(clip);
        }

        public void PlayMusic()
        {
            if (MusicClip != null && !musicSource.isPlaying)
            {
                musicSource.clip = MusicClip;
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        private AudioClip GetClip(string name)
        {
            switch (name)
            {
                case "coin": return CoinClip;
                case "jump": return JumpClip;
                case "finish": return FinishClip;
                case "checkpoint": return CheckpointClip;
                case "jumppad": return JumpPadClip;
                case "button": return ButtonClip;
                default: return null;
            }
        }
    }
}
