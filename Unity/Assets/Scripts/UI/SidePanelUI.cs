/******************************************************************************
 * File        : SidePanelUI.cs
 * Version     : 1.0
 * Author      : Petteri Maljamäki (petteri.maljamaki@lapinamk.fi), Miika Puljujärvi (miika.puljujarvi@lapinamk.fi)
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
using DUKE.Controls;


namespace DUKE.UI {


    public class SidePanelUI : MonoBehaviour
    {
        #region Variables

        [SerializeField] Clickable[] languageButtons;
        [SerializeField] Clickable[] modeButtons;
        [SerializeField] Clickable generalInfoButton;
        [SerializeField] GameObject generalInfoPanel;
        [SerializeField] GameObject desktopInfoPanel;
        [SerializeField] GameObject VRInfoPanel;
        [SerializeField] Clickable[] audioClickables;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Set language. Called from inspector's UnityEvent.
        /// </summary>
        /// <param name="_lang"></param>
        public void SetLanguage (int _lang) 
        {
            KLHManager.Language = (Language)_lang;
        }

        /// <summary>
        /// Show or hide general info. Called from inspector's UnityEvent.
        /// </summary>
        public void ShowInfo () 
        {
            if (KLHManager.Mode == KLHMode.SplashScreen) { return; }

            generalInfoPanel.SetActive(generalInfoButton.IsToggled);
            desktopInfoPanel.SetActive(DetectInputDevices.InputMode == InputMode.Desktop);
            VRInfoPanel.SetActive(DetectInputDevices.InputMode == InputMode.VR);  
        }

        #endregion
        #region MonoBehaviour Methods

        private void OnEnable ()
        {
            KLHManager.LanguageSet += UpdateLanguageButtons;
            KLHManager.ModeChanged += UpdateModeButtons;
            DetectInputDevices.InputModeChanged += UpdateInputMode;

            UpdateButtons();

            for (int i = 0; i < modeButtons.Length; i++) {

                modeButtons[i].gameObject.SetActive(false);
                modeButtons[i].gameObject.SetActive(true);
            }
        }

        private void OnDisable ()
        {
            KLHManager.LanguageSet -= UpdateLanguageButtons;
            KLHManager.ModeChanged -= UpdateModeButtons;
            DetectInputDevices.InputModeChanged -= UpdateInputMode;

            if (null != generalInfoPanel) {

                if (generalInfoPanel.activeSelf) {

                    generalInfoButton.SetToggled(false, true);
                    ShowInfo();
                }
            }
        }

        #endregion
        #region Updating Methods

        /// <summary>
        /// Control the visual state of buttons depending on different variables.
        /// </summary>
        void UpdateButtons () 
        {
            UpdateLanguageButtons();
            UpdateModeButtons();
            UpdateAudioButtons();
        }

        /// <summary>
        /// Control the visual state of language buttons.
        /// </summary>
        void UpdateLanguageButtons ()
        {
            languageButtons[0].SetToggled(KLHManager.Language == Language.Finnish, true);
            languageButtons[1].SetToggled(KLHManager.Language == Language.English, true);
        }

        /// <summary>
        /// Control the visual state of mode buttons.
        /// </summary>
        void UpdateModeButtons ()
        {
            for (int i = 0; i < modeButtons.Length; i++) {

                modeButtons[i].SetToggled((int)KLHManager.Mode == i + 1, true);
            }
        }

        /// <summary>
        /// Toggle audio level buttons on/off depending on <typeparamref name="KLHManager"/>.<typeparamref name="CurrentAudioLevel"/>.
        /// </summary>
        void UpdateAudioButtons ()
        {
            if (null != audioClickables)
            {
                foreach (Clickable c in audioClickables)
                {
                    c.SetToggled(false, true);
                }

                audioClickables[(int)KLHManager.CurrentAudioLevel].SetToggled(true, true);
            }
        }

        /// <summary>
        /// Called when InputMode is changed.
        /// </summary>
        /// <param name="_newMode">New InputMode.</param>
        void UpdateInputMode (InputMode _newMode)
        {
            ShowInfo();
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace