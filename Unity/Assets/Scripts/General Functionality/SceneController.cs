/******************************************************************************
 * File        : SceneController.cs
 * Version     : 1.0
 * Author      : Miika Puljujärvi (miika.puljujarvi@lapinamk.fi)
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace DUKE {


    /// <summary>
    /// Controls the loading and unloading of Scenes specific to KLH.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        # region Variables

        [SerializeField] static SceneController current;
        [SerializeField, HideInInspector] bool currentlyFading = false;

        #endregion


        # region Properties

        /// <summary>
        /// Public static instance of <typeparamref name="SceneController"/>.
        /// </summary>
        public static SceneController Current { 
            get {return current;} 
            set {current = value;} }

        /// <summary>
        /// Currently active <typeparamref name="Scene"/>.
        /// </summary>
        public string CurrentScene { 
            get { return SceneManager.GetActiveScene().name; } }
       
        /// <summary>
        /// TRUE if Splash_Screen has completed loading.
        /// </summary>
        static bool SplashScreenLoadCompleted {
            get; 
            set;}
        
        /// <summary>
        /// TRUE if MainMenu has completed loading.
        /// </summary>
        static bool MainMenuLoadCompleted {
            get; 
            set;}
        
        /// <summary>
        /// TRUE if KLH_MAIN has completed loading.
        /// </summary>
        static bool KLHMainLoadCompleted {
            get; 
            set;}
        
        /// <summary>
        /// TRUE if Digital_Twins_Full has completed loading.
        /// </summary>
        static bool DigitalTwinsLoadCompleted {
            get; 
            set;}

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        private void Awake () {

            if (null == Current)    { Current = this; }    
        }

        private void Start () {

            Reset();
        }

        #endregion
        #region Scene Loading Methods

        /// <summary>
        /// Start the loading of Scenes.
        /// </summary>
        public static void Begin () 
        {
            DoFade<bool>(true, 1f, LoadMainScenes, true);
        }

        /// <summary>
        /// Start the unloading of Scenes.
        /// </summary>
        public static void Reset () 
        {
            DoFade<bool>(true, 1f, LoadMainScenes, false);
        }

        /// <summary>
        /// Start the loading or unloading of Scenes.
        /// </summary>
        /// <param name="_load"></param>
        static void LoadMainScenes (bool _load) 
        {
            Current.StartCoroutine(Current.LoadMainScenesAsync(_load));
        }

        /// <summary>
        /// Load or unload a specific Scene.
        /// </summary>
        /// <param name="_name">Name of the Scene to be loaded or unloaded.</param>
        /// <param name="_load">Whether to load or unload the Scene.</param>
        /// <returns></returns>
        static AsyncOperation LoadScene (string _name, bool _load) 
        {
            AsyncOperation newOp = null;

            if (_load && !IsSceneLoaded(_name)) { newOp = SceneManager.LoadSceneAsync(_name, LoadSceneMode.Additive); }
            else if (IsSceneLoaded(_name))      { newOp = SceneManager.UnloadSceneAsync(_name); }

            return newOp;
        }



        /// <summary>
        /// Load/unload Splash_Screen.
        /// </summary>
        /// <param name="_load">Whether to load or unload the Scene.</param>   
        public static void LoadSplash (bool _load) {

            AsyncOperation op = LoadScene("Splash_Screen", _load);
            if (null != op) { op.completed += OnSplashScreenLoadCompleted; }    
        }
        
        /// <summary>
        /// Load/unload Main_Menu.
        /// </summary>
        /// <param name="_load">Whether to load or unload the Scene.</param>
        public static void LoadMainMenu (bool _load) {

            AsyncOperation op = LoadScene("Main_Menu", _load);
            if (null != op) { op.completed += OnMainMenuLoadCompleted; }     
        }
        
        /// <summary>
        /// Load/unload KLH_MAIN.
        /// </summary>
        /// <param name="_load">Whether to load or unload the Scene.</param>
        public static void LoadKLHMain (bool _load) {
            
            AsyncOperation op = LoadScene("KLH_MAIN", _load);
            if (null != op) { op.completed += OnKLHMainLoadCompleted; } 
        }
        
        /// <summary>
        /// Load/unload Digital_Twins_Full.
        /// </summary>
        /// <param name="_load">Whether to load or unload the Scene.</param>    
        public static void LoadDigitalTwins (bool _load) {
            
            AsyncOperation op = LoadScene("Digital_Twins_Full", _load);
            if (null != op) { op.completed += OnDigitalTwinsLoadCompleted; } 
        }



        /// <summary>
        /// Called when Splash_Screen has been loaded/unloaded.
        /// </summary>
        /// <param name="_op">The operation which controls the loading/unloading.</param>
        static void OnSplashScreenLoadCompleted (AsyncOperation _op)
        {
            SplashScreenLoadCompleted = true;
        }
        
        /// <summary>
        /// Called when Main_Menu has been loaded/unloaded.
        /// </summary>
        /// <param name="_op">The operation which controls the loading/unloading.</param>
        static void OnMainMenuLoadCompleted (AsyncOperation _op) 
        {
            MainMenuLoadCompleted = true;
        }
        
        /// <summary>
        /// Called when KLH_MAIN has been loaded/unloaded.
        /// </summary>
        /// <param name="_op">The operation which controls the loading/unloading.</param>
        static void OnKLHMainLoadCompleted (AsyncOperation _op) 
        {
            KLHMainLoadCompleted = true;       
        }
        
        /// <summary>
        /// Called when Digital_Twins_Full has been loaded/unloaded.
        /// </summary>
        /// <param name="_op">The operation which controls the loading/unloading.</param>    
        static void OnDigitalTwinsLoadCompleted (AsyncOperation _op) 
        {
            DigitalTwinsLoadCompleted = true;
        }
        


        /// <summary>
        /// Check whether a Scene is loaded or not.
        /// </summary>
        /// <param name="_sceneName">The name of the Scene to be checked.</param>
        /// <returns></returns>
        public static bool IsSceneLoaded (string _sceneName) {
            
            for (int i = 0; i < SceneManager.sceneCount; i++) {

                if (SceneManager.GetSceneAt(i).name == _sceneName) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Check whether a Scene is loaded or not.
        /// </summary>
        /// <param name="_scene">The Scene to be checked.</param>
        /// <returns></returns>
        public static bool IsSceneLoaded (Scene _scene) {

            return _scene.isLoaded;
        }

        #endregion
        #region Coroutine Methods

        /// <summary>
        /// Start a Fade coroutine.
        /// </summary>
        /// <param name="_increase">Whether to increase or decrease opacity (TRUE = fade to black).</param>
        /// <param name="_duration">Duration of the animation in seconds.</param>
        /// <param name="_method">A method that will be called when the coroutine finishes.</param>
        public static void DoFade (bool _increase, float _duration = 1f, Action _method = null) {

            if (null == _method)    { Current.StartCoroutine(Current.Fade(_increase, _duration)); }
            else                    { Current.StartCoroutine(Current.Fade(_increase, _duration, _method)); }       
        }
        
        /// <summary>
        /// Start a Fade coroutine.
        /// </summary>
        /// <param name="_increase">Whether to increase or decrease opacity (TRUE = fade to black).</param>
        /// <param name="_duration">Duration of the animation in seconds.</param>
        /// <param name="_method">A method that will be called when the coroutine finishes.</param>
        /// <param name="_val">Parameter of the method.</param>
        public static void DoFade<T> (bool _increase, float _duration, Action<T> _method, T _val) {

            Current.StartCoroutine(Current.FadeWithGenericMethod(_increase, _duration, _method, _val));
        }
        
        /// <summary>
        /// Start a Fade coroutine.
        /// </summary>
        /// <param name="_increase">Whether to increase or decrease opacity (TRUE = fade to black).</param>
        /// <param name="_duration">Duration of the animation in seconds.</param>
        /// <param name="_method">A method that will be called when the coroutine finishes.</param>
        /// <param name="_val1">First arameter of the method.</param>
        /// <param name="_val2">Second parameter of the method.</param>
        public static void DoFade<T,U> (bool _increase, float _duration, Action<T,U> _method, T _val1, U _val2) {

            Current.StartCoroutine(Current.FadeWithGenericMethod(_increase, _duration, _method, _val1, _val2));
        }



        /// <summary>
        /// Use BaseFade to fade to or from black and call a parameterless method after completion.
        /// </summary>
        /// <param name="_increase"></param>
        /// <param name="_duration"></param>
        /// <param name="_method"></param>
        /// <returns></returns>
        IEnumerator Fade (bool _increase, float _duration, Action _method = null) { 

            if (currentlyFading) { yield return new WaitUntil (() => currentlyFading = false); } 

            Current.StartCoroutine(Current.BaseFade(_increase, _duration));

            yield return new WaitUntil (() => currentlyFading == false);

            if (null != _method) { _method(); }
        }

        /// <summary>
        /// Use BaseFade to fade to or from black and call a method with a single parameter after completion.
        /// </summary>
        /// <param name="_increase"></param>
        /// <param name="_duration"></param>
        /// <param name="_method"></param>
        /// <returns></returns>
        IEnumerator FadeWithGenericMethod<T> (bool _increase, float _duration, Action<T> _method, T _methodVal) {

            if (currentlyFading) { yield return new WaitUntil (() => currentlyFading = false); } 

            Current.StartCoroutine(Current.BaseFade(_increase, _duration));

            yield return new WaitUntil (() => currentlyFading == false);

            if (null != _method) { _method(_methodVal); }  
        }
        
        /// <summary>
        /// Use BaseFade to fade to or from black and call a method with two parameters after completion.
        /// </summary>
        /// <param name="_increase"></param>
        /// <param name="_duration"></param>
        /// <param name="_method"></param>
        /// <returns></returns>
        IEnumerator FadeWithGenericMethod<T,U> (bool _increase, float _duration, Action<T,U> _method, T _methodVal1, U _methodVal2) {

            if (currentlyFading) { yield return new WaitUntil (() => currentlyFading = false); } 
            

            Current.StartCoroutine(Current.BaseFade(_increase, _duration));

            yield return new WaitUntil (() => currentlyFading == false);

            if (null != _method) { _method(_methodVal1, _methodVal2); }  
        }

        /// <summary>
        /// Fade to or from black gradually only.
        /// </summary>
        /// <param name="_increase">Wheter or not to increace opacity (TRUE = fade to black).</param>
        /// <param name="_duration">Duration of the animation in seconds.</param>   
        IEnumerator BaseFade (bool _increase, float _duration) {

            _duration = KLHManager.FadeDuration;
            currentlyFading = true;

            List<DUKE.Controls.Input> disabledInputs = new List<DUKE.Controls.Input>();
            GameObject[] fades = GameObject.FindGameObjectsWithTag("Fade");
            float ratio = 0f;

            /// Disable Inputs temporarily and store the references.
            foreach (DUKE.Controls.Input i in FindObjectsOfType<DUKE.Controls.Input>()) {

                if (i.InputActive) { 
                    
                    disabledInputs.Add(i); 
                    i.DisableInput(); 
                }
            }

            while (ratio < 1f) {

                ratio = Mathf.Clamp01(ratio + (Time.deltaTime / _duration));
                KLHManager.FadeValue = _increase ? ratio : 1f - ratio;

                yield return new WaitForEndOfFrame();
            }

            /// Enable Inputs that were disabled.
            foreach (DUKE.Controls.Input i in disabledInputs) {

                i.EnableInput(); 
            }

            currentlyFading = false;
        }

        /// <summary>
        /// Load Scenes.
        /// </summary>
        /// <param name="_load">TRUE = Splash_Screen is unloaded and other Scenes are loaded.
        /// FALSE = Splash_Screen is loaded and other Scenes are unloaded.</param>
        /// <returns></returns>
        IEnumerator LoadMainScenesAsync (bool _load) {

            SplashScreenLoadCompleted = false;
            MainMenuLoadCompleted = false;
            KLHMainLoadCompleted = false;
            DigitalTwinsLoadCompleted = false;

            LoadSplash(!_load);
            LoadMainMenu(_load);
            LoadKLHMain(_load);
            LoadDigitalTwins(_load);

            if (_load) {

                yield return new WaitUntil(() => 
                SplashScreenLoadCompleted == true &&
                MainMenuLoadCompleted == true &&
                KLHMainLoadCompleted == true &&
                DigitalTwinsLoadCompleted == true);

            } else {

                yield return new WaitUntil(() => 
                SplashScreenLoadCompleted == true);
            }

            /// Transport Player to the proper spot before fading from black.
            if (_load)  { KLHManager.SwitchMode(KLHMode.MainMenu, true); }
            else        { KLHManager.SwitchMode(KLHMode.SplashScreen, true); }
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace