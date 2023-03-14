/******************************************************************************
 * File        : UISettingsSO.cs
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
using TMPro;
using UnityEngine;


namespace DUKE.UI {

    /// <summary>
    /// Color modes.
    /// </summary>
    public enum UIColorMode {
        Off,
        Primary,
        Secondary,
        Tertiary,
        AccentA,
        AccentB,
        Custom
    }


    [CreateAssetMenu(fileName = "New UI Settings", menuName = "ScriptableObjects/Create UI Settings")]
    public class UIPaletteSO : ScriptableObject
    {
        #region Variables

        [SerializeField] Color primaryColor = new Color (1, 1, 1, 1); 
        [SerializeField] Color secondaryColor = new Color (1, 1, 1, 1);
        [SerializeField] Color tertiaryColor = new Color (1, 1, 1, 1);
        [SerializeField] Color accentColorA = new Color (1, 1, 1, 1);
        [SerializeField] Color accentColorB = new Color (1, 1, 1, 1);

        #endregion


        #region Properties

        /// <summary>
        /// Primary <typeparamref name="Color"/> (as per <typeparamref name="UIColorMode"/>).
        /// </summary>
        public Color PrimaryColor { 
            get { return primaryColor; } }
                
        /// <summary>
        /// Secondary <typeparamref name="Color"/> (as per <typeparamref name="UIColorMode"/>).
        /// </summary>
        public Color SecondaryColor { 
            get { return secondaryColor; } }
               
        /// <summary>
        /// Tertiary <typeparamref name="Color"/> (as per <typeparamref name="UIColorMode"/>).
        /// </summary>
        public Color TertiaryColor { 
            get { return tertiaryColor; } }
               
        /// <summary>
        /// Accent <typeparamref name="Color"/> A (as per <typeparamref name="UIColorMode"/>).
        /// </summary>
        public Color AccentColorA { 
            get { return accentColorA; } }
        
        /// <summary>
        /// Accent <typeparamref name="Color"/> B (as per <typeparamref name="UIColorMode"/>).
        /// </summary>
        public Color AccentColorB { 
            get { return accentColorB; } } 

        #endregion


        #region Methods

        /// <summary>
        /// Get the desired <typeparamref name="Color"/> based on the provided <paramref name="_uiColor"/>.
        /// </summary>
        /// <param name="_uiColor"><typeparamref name="UIColorMode"/> enum value.</param>
        /// <returns><typeparamref name="Color"/> corresponding to the <paramref name="_uiColor"/>.</returns>
        public Color GetColor (UIColorMode _uiColor)
        {
            return _uiColor switch {
                UIColorMode.Primary     => PrimaryColor,
                UIColorMode.Secondary   => SecondaryColor,
                UIColorMode.Tertiary    => TertiaryColor,
                UIColorMode.AccentA     => AccentColorA,
                UIColorMode.AccentB     => AccentColorB,
                _                   => Color.magenta
            };
        }

        #endregion


    } /// End of Class


} /// End of Namespace