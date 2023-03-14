/******************************************************************************
 * File        : UISettingsElement.cs
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
using UnityEngine.UI;
using TMPro;


namespace DUKE.UI {


    /// <summary>
    /// Read UISettingsSO's color values etc. and set the values of relevant components.
    /// </summary>
    public class UISettingsElement : MonoBehaviour
    {
        #region Variables

        UISettingsProvider settingsProvider;
        [SerializeField] UIColorMode uiColorMode;
        [SerializeField] Color customColor;

        #endregion


        #region Properties

        /// <summary>
        /// <typeparamref name="UISettingsProvider"/> providing the color palette to this instance.
        /// </summary>
        public UISettingsProvider SettingsProvider { 
            get { return TryFindSettingsProvider(); } }
        
        /// <summary>
        /// <typeparamref name="UIPaletteSO"/> of <paramref name="SettingsProvider"/>.
        /// </summary>
        public UIPaletteSO UIPalette { 
            get { return SettingsProvider.UIPalette; } }
       
        /// <summary>
        /// Currently selected <typeparamref name="UIColorMode"/>.
        /// </summary>
        public UIColorMode UIColorMode { 
            get { return uiColorMode; } }
        
        /// <summary>
        /// Current <typeparamref name="Color"/>.
        /// </summary>
        public Color Color { 
            get { return (uiColorMode == UIColorMode.Custom) 
                ? customColor
                : UIPalette.GetColor(uiColorMode); } }
        
        /// <summary>
        /// TRUE if <paramref name="SettingsProvider"/> is found.
        /// </summary>
        public bool HasSettingsProvider { 
            get { return null != SettingsProvider; } }
        
        /// <summary>
        /// TRUE if <typeparamref name="UIPalette"/> is found.
        /// </summary>
        public bool HasPalette { 
            get {return null != UIPalette; } }
        
        /// <summary>
        /// TRUE if a <typeparamref name="TextMeshProUGUI"/> component is is found.
        /// </summary>
        public bool HasTextComponent { 
            get { return TryGetComponent(out TextMeshProUGUI _); } }
        
        /// <summary>
        /// TRUE if an <typeparamref name="Image"/> component is found.
        /// </summary>
        public bool HasImageComponent { 
            get { return TryGetComponent(out Image _); } }

        #endregion


        #region Methods

        /// <summary>
        /// Attempt to find UISettings from hierarchy.
        /// </summary>
        /// <returns>True if UISettingsProvider was found.</returns>
        UISettingsProvider TryFindSettingsProvider ()
        {
            if (null == settingsProvider) { 

                settingsProvider = GetComponentInParent<UISettingsProvider>(); 
            }
            
            return settingsProvider;
        }

        /// <summary>
        ///  set the visuals according to the defined settings.
        /// </summary>
        public void SetVisuals ()
        {
            if (!HasSettingsProvider)   { return; }     
            if (!HasPalette)            { return; }

            SetColor();

            if (TryGetComponent(out DUKE.Controls.Clickable clickable)) {

                clickable.SetUISettings(this);
            }       
        }

        /// <summary>
        /// Set the color of the element.
        /// </summary>
        void SetColor ()
        {
            /// Only set the graphic's color if UIColor.Off is not selected.
            if (uiColorMode != UIColorMode.Off) {

                if (TryGetComponent(out Image img))             { img.color = Color; }
                if (TryGetComponent(out TextMeshProUGUI text))  { text.color = Color; }
            }
        }

        /// <summary>
        /// Set <paramref name="uiColorMode"/> manually. Used in inspector's UnityEvent.
        /// </summary>
        /// <param name="_colorMode"><typeparamref name="UIColorMode"/> selection.</param>
        public void SetUIColorMode (string _colorMode)
        {
            switch (_colorMode)
            {
                default: Debug.LogWarning("'" + _colorMode + "' is not a valid UIColorMode."); break;
                case "Primary": uiColorMode = UIColorMode.Primary; break;
                case "Secondary": uiColorMode = UIColorMode.Secondary; break;
                case "Tertiary": uiColorMode = UIColorMode.Tertiary; break;
                case "AccentA":
                case "Accent A": uiColorMode = UIColorMode.AccentA; break;
                case "AccentB":
                case "Accent B": uiColorMode = UIColorMode.AccentB; break;
            }

            SetColor();
        }

        #endregion


    } /// End of Class


} /// End of Namespace