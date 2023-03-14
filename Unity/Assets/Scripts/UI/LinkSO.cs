/******************************************************************************
 * File        : LinkSO.cs
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DUKE.UI {


    /// <summary>
    /// Function as a dummy object and link UnityEvents to static (Singleton) classes' methods.
    /// </summary>
    [CreateAssetMenu(fileName = "Link", menuName = "ScriptableObjects/Create Link instance", order = 0)]
    public class LinkSO : ScriptableObject
    { 
        #region KLHManager

        /// <summary>
        /// Set the language.
        /// </summary>
        /// <param name="_lang"><typeparamref name="Language"/> enum index.</param>
        public void SetLanguage (int _lang) 
        {
            KLHManager.Language = (Language)_lang;
        }

        /// <summary>
        /// Begin the transition to Splash_Screen.
        /// </summary>
        public void ToSplashScreen () 
        {
            KLHManager.ToSplashScreen();
        }
        
        /// <summary>
        /// Begin the transition to Main_Menu.
        /// </summary>
        public void ToMainMenu () 
        {
            KLHManager.ToMainMenu();
        }
        
        /// <summary>
        /// Begin the transition to KLH_MAIN (BuildingDrawer).
        /// </summary>
        public void ToBuildingDrawer () 
        {
            KLHManager.ToBuildingDrawer();     
        }

        /// <summary>
        /// Begin the transition to KLH_MAIN (DataVisualisation).
        /// </summary>
        /// <param name="_buildingIndex">Index of the selected <typeparamref name="Building"/>.</param>
        public void ToBuildingDrawer (int _buildingIndex) 
        {
            _buildingIndex = Mathf.Clamp(_buildingIndex, 0, KLHManager.Buildings.Count - 1);
            KLHManager.Building = KLHManager.Buildings[_buildingIndex];
            KLHManager.ToBuildingDrawer();     
        }

        /// <summary>
        /// Begin the transition to KLH_MAIN (DataVisualisation).
        /// </summary>    
        public void ToVisualisation () 
        {
            KLHManager.ToVisualisation();
        }   

        /// <summary>
        /// Begin the transition to Digital_Twins_Full.
        /// </summary>    
        public void ToDigitalTwins () 
        {
            KLHManager.ToDigitalTwins();     
        }

        #endregion


        #region SceneController

        /// <summary>
        /// Start the unloading of Scenes.
        /// </summary>
        public void Reset () 
        {
            SceneController.Reset();
        }

        #endregion


        #region AudioController

        public void ChangeSnapshot(float _duration, float _musicLevel) {
            AudioController.ChangeSnapshot(_duration, _musicLevel);
        }

        public void ChangeMasterVolume(float _volume) {
            AudioController.ChangeMasterVolume(_volume);
        }
        public void PlayClickStart()
        {
            AudioController.PlayClickStart();
        }

        public void PlayClickEnd()
        {
            AudioController.PlayClickEnd();
        }

        #endregion


        #region General Methods

        /// <summary>
        /// Set <paramref name="_t"/> as the last sibling.
        /// </summary>
        /// <param name="_t"><typeparamref name="Transform"/> to reorder within its parent.</param>
        public void SetAsLastSibling (Transform _t)
        {
            _t.SetAsLastSibling();
        }

        /// <summary>
        /// Set <paramref name="_t"/> as the first sibling.
        /// </summary>
        /// <param name="_t"><typeparamref name="Transform"/> to reorder within its parent.</param>
        public void SetAsFirstSibling (Transform _t)
        {
            _t.SetAsFirstSibling();
        }

        /// <summary>
        /// Quit the application.
        /// </summary>
        public void ExitGame ()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();      
            #endif
        }


        #endregion





    } /// End of Class


} /// End of Namespace