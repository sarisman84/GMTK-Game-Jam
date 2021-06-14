using System;
using UnityEngine;

namespace Managers
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _player;
        private void Awake()
        {
            _player = gameObject.AddComponent<AudioSource>();
        }

        public void Play(AudioClip selectedLevelMusicClip, float volume = 0.5f)
        {
            if (_player.clip == selectedLevelMusicClip) return;
            _player.clip = selectedLevelMusicClip;
            _player.volume = volume;
            _player.loop = true;
            _player.Play();
        }
    }
}