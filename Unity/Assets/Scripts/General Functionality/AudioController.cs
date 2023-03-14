/******************************************************************************
 * File        : AudioController.cs
 * Version     : 1.0
 * Author      : Miika Puljujärvi (miika.puljujarvi@lapinamk.fi), Petteri Maljamäki (petteri.maljamaki@lapinamk.fi)
 * Copyright   : Lapland University of Applied Sciences
 * Licence     : MIT-Licence
 * 
 * Copyright (c) 2022 Lapland University of Applied Sciences
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DUKE.Controls;


namespace DUKE {


    public enum AudioLevel
    {
        Off,
        Low,
        High
    }


    /// <summary>
    /// Control the audio of the program. Offer public static methods to activate audio clips and music.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        #region Variables

        [SerializeField] AudioMixer audioMixer;
        [SerializeField] AudioSource vrSoundSource;
        [SerializeField] AudioSource vrMusicSource;
        [SerializeField] AudioSource desktopSoundSource;
        [SerializeField] AudioSource desktopMusicSource;


        [Space(10f)]
        [SerializeField, Range(0,1)] float soundVolume;
        float soundVolumeMultiplier = 1f;
        [SerializeField, Range(0,1)] float musicVolume;
        float musicVolumeMultiplier = 1f;
        [SerializeField] float smoothVolumeTransitionLength = 3f;
        [SerializeField] float currentBGMTime;
        
        
        [Space(10f)]
        [SerializeField] AudioClip backgroundMusic;
        [SerializeField] AudioClip clickSound;
        [SerializeField] AudioClip clickStart;
        [SerializeField] AudioClip clickEnd;
        [SerializeField] AudioClip toggle;
        [SerializeField] AudioClip teleport;
        [SerializeField] AudioClip modeSwitchSound;

        #endregion


        #region Properties

        /// <summary>Public static instance of <typeparamref name="AudioController"/>.</summary>
        public static AudioController Current 
        { 
            get; 
            private set; 
        }
        
        /// <summary>Source audio.</summary>
        public static AudioSource SoundSource 
        { 
            get; 
            private set; 
        }

        /// <summary>Source audio.</summary>
        public static AudioSource MusicSource 
        { 
            get; 
            private set; 
        }

        

        /// <summary>Current volume level for UI and other sound effects.</summary>
        public static float SoundVolume 
        {
            get { return SoundBaseVolume * SoundVolumeAdjustment; }     
        }

        /// <summary>Base volume level for UI and other sound effects.</summary>
        public static float SoundBaseVolume
        {
            get { return Current.soundVolume; } 
            set { Current.soundVolume = Mathf.Clamp01(value); }
        }

        /// <summary>Adjust multiplier for UI and other sound effects.</summary>
        public static float SoundVolumeAdjustment
        {
            get { return Current.soundVolumeMultiplier; }
            set { Current.soundVolumeMultiplier = Mathf.Clamp01(value); }
        }

        /// <summary>Current volume level for background music.</summary>
        public static float MusicVolume 
        {
            get { return MusicBaseVolume * MusicVolumeAdjustment; }     
        }
        
        /// <summary>Base volume level for background music.</summary>
        public static float MusicBaseVolume
        {
            get { return Current.musicVolume; } 
            set { Current.musicVolume = Mathf.Clamp01(value); }
        }

        /// <summary>Adjust multiplier for background music.</summary>
        public static float MusicVolumeAdjustment
        {
            get { return Current.musicVolumeMultiplier; }
            set { Current.musicVolumeMultiplier = Mathf.Clamp01(value); }
        }

        /// <summary>Master <typeparamref name="AudioMixer"/>.</summary>
        protected static AudioMixer Audiomixer 
        { 
            get { return Current.audioMixer; }
        }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>Set the balance between <paramref name="music_hi"/> and <paramref name="music_low"/> <typeparamref name="AudioMixerSnapshot">.</summary>
        /// <param name="_duration">Duration of the transition from snapshot to another.</param>
        /// <param name="_musicLevel">Amount of music_hi -> 1 = full, 0 = silent.</param>
        public static void ChangeSnapshot(float _duration, float _musicLevel)
        {
            AudioMixerSnapshot[] snapshotList = new  AudioMixerSnapshot[2] 
            {
                Audiomixer.FindSnapshot("snapshot_music_hi"),
                Audiomixer.FindSnapshot("snapshot_music_low")
            };

            float[] levels = new float[2] 
            {
                _musicLevel, 
                1-_musicLevel
            };

            Audiomixer.TransitionToSnapshots(snapshotList, levels, _duration);
        }

        /// <summary>Change the master volume level.</summary>
        /// <param name="_volume"></param>
        public static void ChangeMasterVolume(float _volume)
        {
            Audiomixer.SetFloat("masterVol", _volume);
            KLHManager.CurrentAudioLevel = _volume switch 
            {
                > -10 => AudioLevel.High,
                > -60 => AudioLevel.Low,
                _ => AudioLevel.Off
            };
        }

        /// <summary>Play the <typeparamref name="AudioClip"/> selected for click start instances.</summary>
        public static void PlayClickStart ()
        {
            SoundSource.volume = Current.soundVolume;
            SoundSource.PlayOneShot(Current.clickStart);          
        }

        /// <summary>Play the <typeparamref name="AudioClip"/> selected for click end instances.</summary>
        public static void PlayClickEnd ()
        {
            SoundSource.volume = Current.soundVolume;
            SoundSource.PlayOneShot(Current.clickEnd);          
        }

        /// <summary>Play the <typeparamref name="AudioClip"/> selected for toggle instances.</summary>
        public static void PlayToggle () 
        {
            SoundSource.volume = Current.soundVolume;
            SoundSource.PlayOneShot(Current.toggle);          
        }

        /// <summary>Play the <typeparamref name="AudioClip"/> selected for teleport instances.</summary>
        public static void PlayTeleport () 
        {
            SoundSource.volume = Current.soundVolume;
            SoundSource.PlayOneShot(Current.teleport);          
        }

        /// <summary>Start the transition of the selected <paramref name="_source"/> to the desired volume level.</summary>
        /// <param name="_source">Selected <typeparamref name="AudioSource"/>.</param>
        /// <param name="_targetVolume">Desired volume level.</param>
        /// <param name="_duration">Duration of the transition between current volume level and <paramref name="_targetVolume"/>.</param>
        public static void DoSourceVolumeTransition (AudioSource _source, float _targetVolume, float _duration = -1)
        {
            if (_duration == -1) 
            { 
                _duration = Current.smoothVolumeTransitionLength; 
            }

            _targetVolume = Mathf.Clamp01(_targetVolume);
            Current.StartCoroutine(Current.SmoothVolumeLevel(_targetVolume, _duration));
        }

        /// <summary>Play <paramref name="modeSwitchSound"/> once.</summary>
        public static void PlayModeSwitchSound ()
        {
            SoundSource.volume = Current.soundVolume;
            SoundSource.PlayOneShot(Current.modeSwitchSound);          
        }

        #endregion 
        #region MonoBehaviour Methods

        void Awake ()
        {
            if (!Current) 
            { 
                Current = this; 
                DontDestroyOnLoad(this.gameObject);
            }
        }

        void Start ()
        {
            SetAudioSource(InputMode.Desktop);
            StartCoroutine(PlayMusic(true, currentBGMTime));
        }

        void Update ()
        {
            SoundSource.volume = SoundVolume;
            MusicSource.volume = MusicVolume;
            Current.currentBGMTime = MusicSource.time;
        }

        void OnEnable ()
        {
            DetectInputDevices.InputModeChanged += SetAudioSource;
        }

        void OnDisable ()
        {
            DetectInputDevices.InputModeChanged -= SetAudioSource;
        }

        #endregion
        #region Audio Methods

        /// <summary>Set the AudioSource based on the current InputMode.</summary>
        /// <param name="_newMode">Define which AudioSource (VR or desktop) is used.</param>
        void SetAudioSource (InputMode _newMode)
        {
            if (_newMode == InputMode.Desktop) 
            { 
                SoundSource = Current.desktopSoundSource; 
                MusicSource = Current.desktopMusicSource;
            }
            else if (_newMode == InputMode.VR) 
            { 
                SoundSource = Current.vrSoundSource; 
                MusicSource = Current.vrMusicSource;
            } 

            StartCoroutine(PlayMusic(true, currentBGMTime));
        }

        /// <summary>Toggle music on/off.</summary>
        /// <param name="_on">Whether to play or stop the music.</param>
        /// <param name="_musicVolume">Multiply the base audio volume by this value.</param>
        protected static IEnumerator PlayMusic (bool _on, float _time)
        {
            yield return new WaitForEndOfFrame();

            MusicSource.volume = MusicVolume;

            if (_on) 
            {
                MusicSource.clip = Current.backgroundMusic;
                MusicSource.time = _time;
                MusicSource.Play();
            } 
            else 
            {
                MusicSource.Stop();
            }
        }

        #endregion
        #region Coroutines

        /// <summary>Adjust volume smoothly.</summary>
        /// <param name="_targetVolume">Target volume level.</param>
        /// <param name="_duration">Duration of the transition to <paramref name="_targetVolume"/>.</param>
        /// <returns></returns>
        IEnumerator SmoothVolumeLevel (float _targetVolume, float _duration, bool _isSound = true)
        {
            float vol = _isSound ? soundVolume : musicVolume;

            bool increase = vol < _targetVolume;
            int multiplier = increase ? 1 : -1;
            float startVolume = vol;

            Vector2 minMax = new Vector2(
                increase ? startVolume : _targetVolume,
                increase ? _targetVolume : startVolume
            );

            while (vol != _targetVolume) 
            {
                vol = Mathf.Clamp((vol + Time.deltaTime / _duration * multiplier), minMax.x, minMax.y);
                SoundSource.volume = vol;

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace