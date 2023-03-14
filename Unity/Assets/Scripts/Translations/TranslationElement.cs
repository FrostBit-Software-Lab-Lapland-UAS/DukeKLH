/******************************************************************************
 * File        : TranslationElement.cs
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
using TMPro;


namespace DUKE {


    /// <summary>
    /// Translate TextMeshPro or TextMeshProUGUI component's text that is attached to this GameObject.
    /// </summary>
    public class TranslationElement : MonoBehaviour
    {
        [SerializeField] string id;



        /// <summary>
        /// Identification string used for finding the translation.
        /// </summary>
        public string ID {get {return id;}}



        #region Methods

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            KLHManager.LanguageSet += SetTranslationText;

            SetTranslationText();
        }

        private void OnDisable()
        {
            KLHManager.LanguageSet -= SetTranslationText;
        }

        #endregion
        #region Translation Methods

        /// <summary>
        /// Attempt to find a text component and set its text by referencing <paramref name="id"/>.
        /// </summary>
        public void SetTranslationText()
        {
            string text = TranslatorSO.GetTranslationById(id);

            if (text != "TranslationMissing") {

                if(TryGetComponent(out TextMeshProUGUI tmpUgui)) {

                    tmpUgui.text = text;
                    tmpUgui.rectTransform.localPosition += new Vector3(1,0,0);
                    tmpUgui.rectTransform.localPosition += new Vector3(-1,0,0);

                } else if (TryGetComponent(out TextMeshPro tmp)) {

                    tmp.text = text;
                    tmp.rectTransform.localPosition += new Vector3(1,0,0);
                    tmp.rectTransform.localPosition += new Vector3(-1,0,0);
                }
            }       
        }

        /// <summary>
        /// Update ID.
        /// </summary>
        /// <param name="_newID">New ID which will replace the old one.</param>
        public void UpdateId(string _newID) 
        {
            id = _newID;
            SetTranslationText();  
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace