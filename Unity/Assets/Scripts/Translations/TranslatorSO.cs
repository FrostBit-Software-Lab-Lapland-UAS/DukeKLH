/******************************************************************************
 * File        : TranslatorSO.cs
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
using System.Linq;
using System;


namespace DUKE {


    /// <summary>
    /// Contains every translation of a specific ID.
    /// </summary>
    [Serializable]
    public struct TextTranslation
    {
        public string id;
        public string[] translationsArray;

        public TextTranslation(string _id, string[] _translations)
        {
            id = _id;
            translationsArray = _translations;
        }
    }


    /// <summary>
    /// A ScriptableObject which contains an array of TextTranslations.
    /// This object uses the Singleton pattern and can be referenced as a static object.
    /// </summary>
    [CreateAssetMenu(fileName = "Translator", menuName = "ScriptableObjects/Create Translator Instance", order = 0)]
    public class TranslatorSO : ScriptableObject
    {
        #region Variables

        private static TranslatorSO current;
        [SerializeField] TextTranslation[] textTranslations;
        [SerializeField] TextAsset translationsCSVFile;

        #endregion


        #region Properties

        /// <summary>
        /// Public static instance of <typeparamref name="TranslatorSO"/>.
        /// </summary>
        public static TranslatorSO Current { get { return GetInstance(); } }

        #endregion


        #region Methods

        /// <summary>
        /// Get the static instance.
        /// </summary>
        public static TranslatorSO GetInstance ()
        {
            if (null == current) { current = Resources.Load("ScriptableObjects/Translator") as TranslatorSO; }

            return current;
        }

        #region Translation Methods
        
        /// <summary>
        /// Convert CSV file of translations to TextTranslation instances.
        /// </summary>
        public static void GetTranslationsFromFile()
        {
            if (null == Current.translationsCSVFile) {

                #if UNITY_EDITOR
                Debug.LogError("TranslatorSO.GetTranslationsFromFile(): Translation source file is NULL.");
                #endif

            } else {

                string[] lines = Current.translationsCSVFile.text.Split("\n"[0]);
                List<TextTranslation> newTranslations = new List<TextTranslation>();
                
                for (int i = 1; i < lines.Length; i++) {

                    lines[i] = lines[i].Replace(" _ ", System.Environment.NewLine);

                    List<string> textSegments = lines[i].Split(';').ToList();
                    string id = textSegments[0];

                    if (id != "") {

                        textSegments.RemoveAt(0);
                        newTranslations.Add(new TextTranslation(id, textSegments.ToArray()));
                    }
                }

                Current.textTranslations = newTranslations.ToArray();
            }
        }

        /// <summary>
        /// Find a specifit TextTranslation with the corresponding ID.
        /// </summary>
        /// <param name="_id">ID of the required TextTranslation.</param>
        /// <returns>Translation of the specified TextTranslation with KLHManager's current language.</returns>
        public static string GetTranslationById(string _id)
        {
            if (null != Current.textTranslations) {

                for (int i = 0; i < Current.textTranslations.Length; i++) {

                    if (Current.textTranslations[i].id == _id) {

                        int languageIndex = Application.isPlaying ? (int)KLHManager.Language : 0;

                        return Current.textTranslations[i].translationsArray[languageIndex];
                    }
                }
            }

            return "TranslationMissing";
        }

        /// <summary>
        /// Get a list of every ID.
        /// </summary>
        /// <returns></returns>
        public static string[] GetTranslationIdList() 
        {
            if (!Application.isPlaying) { GetTranslationsFromFile(); }
        
            string[] allTranslationIds = new string[Current.textTranslations.Length];
            
            for (int i = 0; i < Current.textTranslations.Length; i++)  {

                allTranslationIds[i] = Current.textTranslations[i].id;
            }

            return allTranslationIds;
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace