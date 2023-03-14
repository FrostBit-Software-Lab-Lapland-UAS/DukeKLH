/******************************************************************************
 * File        : InputField.cs
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
using UnityEngine.UI;
using DUKE.UI;
using UnityEngine.InputSystem;


namespace DUKE.Controls {


    /// <summary>
    /// Content types of <typeparamref name="InputField"/>.
    /// </summary>
    public enum FieldContentType 
    {
        Integer,
        Float,
        String
    }


    /// <summary>
    /// Input alphanumeric data with VR  controller or keyboard.
    /// </summary>
    public class InputField : Interactable 
    {
        #region Variables

        #region General Variables

        [Space(20f)]
        [Header("Field Settings")]
        [SerializeField] TMPro.TextMeshProUGUI textObj;


        [Space(10f)]
        [SerializeField] FieldContentType contentType;
        [SerializeField] int maximumFieldCharacters = 10;
        [SerializeField] string content;
        [SerializeField] string prefix = "";
        [SerializeField] string suffix = ""; 
        [SerializeField, HideInInspector] Collider col;
        bool fieldActive = false;
        bool updateEventSubscribedTo = false;
        CurvedUIElement cObject;
        VRThumbDial vrThumbDial;


        [Space(10f)]
        [Header("Button Settings")]
        [SerializeField] bool isCurvedUI = false;
        [SerializeField] bool clearContentOnActivate = false;

        #endregion
        #region Color Variables

        [Space(10f)]
        [Header("Color Settings")]
        [SerializeField] Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        [SerializeField] Color backgroundActiveColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        [Space(5f)]
        [SerializeField] Color borderColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] Color borderActiveColor = new Color(1f, 0.5f, 0.25f, 1f);
        [Space(5f)]
        [SerializeField] Color textColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] Color textActiveColor = new Color(1f, 0.5f, 0.25f, 1f);

        #endregion

        #endregion


        #region Events

        /// <summary>
        /// Called when content value (<typeparamref name="float"/>) changes.
        /// </summary>
        public Action<float> FloatValueChanged;
        
        /// <summary>
        /// Called when content value (<typeparamref name="int"/>) changes.
        /// </summary>
        public Action<int> IntValueChanged;
        
        /// <summary>
        /// Called when content value (<typeparamref name="string"/>) changes.
        /// </summary>
        public Action<string> StringValueChanged;

        #endregion


        #region Methods

        #region MonoBehaviour Methods

        protected override void Start ()
        {
            base.Start();

            if (null == col) { SetupColliderForUIElement(); }

            UpdateColors();
        }
        
        protected override void OnEnable ()
        {
            base.OnEnable();

            SubscribeToEvents();

            textObj.gameObject.SetActive(false);
            textObj.gameObject.SetActive(true);
        }
        
        protected override void OnDisable ()
        {
            base.OnDisable();

            UnsubscribeFromEvents();
        }

        #endregion
        #region Override Methods

        public override bool ValidateInput(Input _input)
        {
            if (fieldActive)
            {
                return true;
            }

            return base.ValidateInput(_input);
        }

        /// <summary>
        /// Enable interaction if <typeparamref name="HasInteractionMode"/> returns TRUE.
        /// </summary>
        /// <param name="_source"></param>
        public override void BeginInteraction (Input _source)
        {
            base.BeginInteraction(_source);

            if (source.IsVR)    { vrInput.TriggerDown += ToggleFieldEditMode; } 
            else                { mkbInput.OnLMBDown += ToggleFieldEditMode; }

            ToggleFieldEditMode();
        }
        
        /// <summary>
        /// Let <typeparamref name="source"/> <typeparamref name="Input"/> know this interaction should end.
        /// </summary>
        protected override void EndInteraction ()
        {
            if (source.IsVR)    { vrInput.TriggerDown -= ToggleFieldEditMode; } 
            else                { mkbInput.OnLMBDown -= ToggleFieldEditMode; }

            base.EndInteraction();
        }
        
        /// <summary>
        /// Update loop for VR.
        /// </summary>
        protected override void VRInteractionUpdate ()
        {
            base.VRInteractionUpdate();

            textObj.text = prefix + content + suffix;

            if (!fieldActive)
            {
                if (sourceInteractionMode == InteractionMode.Raycast) 
                {
                    if (null == vrInput.PointerTarget)              { EndInteraction(); } 
                    else if (vrInput.PointerTarget != transform)    { EndInteraction(); }
                } 
                else if (sourceInteractionMode == InteractionMode.Overlap) 
                {
                    if (null == vrInput.OverlapTarget)              { EndInteraction(); } 
                    else if (vrInput.OverlapTarget != transform)    { EndInteraction(); }
                }
            }
        }

        /// <summary>
        /// Update loop for desktop.
        /// </summary>
        protected override void DesktopInteractionUpdate ()
        {
            base.DesktopInteractionUpdate();

            textObj.text = prefix + content + suffix;

            if (!fieldActive)
            {
                if (null == mkbInput.PointerTarget)              { EndInteraction(); } 
                else if (mkbInput.PointerTarget != transform)    { EndInteraction(); }
            }
        }

        #endregion
        #region Setup and Event Methods

        /// <summary>
        /// Create a BoxCollider for the UI element if it doesn't have one yet.
        /// This method automates a tedious part of the manual UI setup, but is not the perfect solution for complex element shapes etc.
        /// </summary>
        void SetupColliderForUIElement ()
        {
            RectTransform rTrn;
            if (!transform.TryGetComponent(out col) && transform.TryGetComponent(out rTrn)) 
            {
                Image img;
                CurvedUIElement cObj;
                if (transform.TryGetComponent(out img) && transform.TryGetComponent(out cObj)) 
                {
                    isCurvedUI = true;

                    MeshCollider mCol;
                    if (!transform.TryGetComponent(out mCol)) mCol = gameObject.AddComponent<MeshCollider>();

                    Mesh m = cObj.CurvedMesh;
                    mCol.sharedMesh = m;
                    col = mCol;
                } 
                else 
                {
                    BoxCollider bCol = gameObject.AddComponent<BoxCollider>();
                    bCol.size = new Vector3(rTrn.rect.width, rTrn.rect.height, 1f);
                    col = bCol;
                }
            }
            SubscribeToEvents();
        }
        
        /// <summary>
        /// Get the up-to-date curved Mesh from CurvedUIElement.
        /// </summary>
        /// <param name="_mesh"></param>
        protected virtual void UpdateCollisionMesh (Mesh _mesh)
        {
            if (!isCurvedUI) { return; }
            if (null == col) { return; }

            MeshCollider mCol = col as MeshCollider;
            mCol.sharedMesh = _mesh;
        }
        
        /// <summary>
        /// Add listeners to relevant <typeparamref name="Actions"/>.
        /// </summary>
        protected override void SubscribeToEvents ()
        {
            if (!isCurvedUI) { return; }
            if (updateEventSubscribedTo) { return; }
            if (null == cObject) { cObject = GetComponent<CurvedUIElement>(); }

            cObject.CurvedMeshUpdated += UpdateCollisionMesh;
            updateEventSubscribedTo = true;
        }
        
        /// <summary>
        /// Remove previously added <typeparamref name="Action"/> listeners.
        /// </summary>
        protected override void UnsubscribeFromEvents ()
        {
            if (!isCurvedUI) { return; }
            if (!updateEventSubscribedTo) { return; }

            cObject.CurvedMeshUpdated -= UpdateCollisionMesh;
            updateEventSubscribedTo = false;
        }
        
        /// <summary>
        /// Send InputField's content as the type defined by 'contentType'.
        /// </summary>
        void SendEvent ()
        {
            switch (contentType) 
            {
                default:
                case FieldContentType.String:
                    StringValueChanged?.Invoke(prefix + content + suffix);
                    break;

                case FieldContentType.Integer:
                    IntValueChanged?.Invoke(ContentToInt());
                    break;

                case FieldContentType.Float:
                    FloatValueChanged?.Invoke(ContentToFloat());
                    break;
            }
        }

        #endregion
        #region Input Editing Methods

        /// <summary>
        /// Begin input feeding to the field.
        /// </summary>
        void BeginInput ()
        {
            if (source.IsVR) 
            {
                if (source.transform.TryGetComponent(out vrThumbDial))  
                {  
                    vrThumbDial.NumberSelected += VREdit;     
                } 
                else 
                {   
                    #if UNITY_EDITOR
                    Debug.Log("Couldn't find VRThumbDial."); 
                    #endif
                }
            } 
            else 
            {
                mkbInput.SetMode(DesktopInputMode.AlphanumericInput);
                Keyboard.current.onTextInput += MKBEdit;
            }

            if (clearContentOnActivate) 
            {
                content = "";
                SetTextContent("");
            }
        }
        
        /// <summary>
        /// End input feeding to the field.
        /// </summary>
        void EndInput ()
        {
            RemoveHeadingZeros();

            if (source.IsVR) 
            {
                if (null != vrThumbDial) { vrThumbDial.NumberSelected -= VREdit; }
            } 
            else 
            {
                Keyboard.current.onTextInput -= MKBEdit;
                mkbInput.SetMode(DesktopInputMode.Default);
            }

            SendEvent();
            EndInteraction();
        }
        
        /// <summary>
        /// Edit (input) loop for VR.
        /// </summary>
        /// <param name="_val"></param>
        void VREdit (int _val)
        {
            if (_val >= 0) 
            {
                Write(_val);
            } 
            else 
            {
                if (_val == -1)      { Erase(); } 
                else if (_val == -3) { Write(","); }
            }

            textObj.textInfo.textComponent.havePropertiesChanged = true;
        }
       
        /// <summary>
        /// Edit (input) loop for mouse and keyboard.
        /// </summary>
        /// <param name="_c"></param>
        void MKBEdit (char _c)
        {
            if (Keyboard.current.backspaceKey.isPressed)  { Erase(); } 
            else if (Keyboard.current.enterKey.isPressed) { ToggleFieldEditMode(); } 
            else                                          { Write(FilterChar(_c)); }

            textObj.textInfo.textComponent.havePropertiesChanged = true;
        }
        
        /// <summary>
        /// Add a character to the field.
        /// </summary>
        /// <param name="_char"></param>
        void Write (char _char)
        {
            if (maximumFieldCharacters > content.Length && _char != '�') 
            { 
                content += _char.ToString(); 
            }
        }
       
        /// <summary>
        /// Add a number to the field.
        /// </summary>
        /// <param name="_int"></param>
        void Write (int _int)
        {
            if (maximumFieldCharacters > content.Length) 
            { 
                content += _int.ToString(); 
            }
        }
        
        /// <summary>
        /// Add a string to the field.
        /// </summary>
        /// <param name="_string"></param>
        void Write (string _string)
        {
            if (maximumFieldCharacters > content.Length) 
            { 
                content += _string; 
            }
        }
       
        /// <summary>
        /// Remove an entry or all entriest from the field.
        /// </summary>
        /// <param name="_index">Erase everything if -1. Otherwise remove the latest entry only.</param>
        void Erase (int _index = -1)
        {
            int index = _index == -1
                ? content.Length - 1
                : Mathf.Clamp(_index, 0, content.Length - 1);

            if (index >= 0) { SetTextContent(content.Remove(index)); } 
            else            { SetTextContent(""); }
        }
        
        /// <summary>
        /// Check which FieldContentType the provided character belongs to.
        /// </summary>
        /// <param name="_c">The character to be checked.</param>
        /// <returns></returns>
        char FilterChar (char _c)
        {
            if (contentType == FieldContentType.Integer || contentType == FieldContentType.Float) 
            {
                switch (_c) 
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return _c;

                    default:
                        if (contentType == FieldContentType.Float && (_c == '.' || _c == ',')) { return ','; }
                        else return '�';
                }      
            } 
            else 
            {        
                return _c; 
            }
        }

        #endregion
        #region Input Control Methods

        /// <summary>
        /// Toggle Field editing.
        /// </summary>
        void ToggleFieldEditMode ()
        {
            fieldActive = !fieldActive;

            if (source.IsVR) 
            { 
                vrInput.GetComponent<VRThumbDial>().SetDialActive(fieldActive); 
            }

            UpdateColors();

            if (fieldActive) 
            { 
                BeginInput(); 
            } 
            else 
            { 
                EndInput(); 
            }
        }
        
        /// <summary>
        /// Remove zeros from the front of text.
        /// </summary>
        void RemoveHeadingZeros ()
        {
            string s = content;
            int safefail = 100;

            if (s.Length >= 1) 
            {
                while (s[0] == '0' && safefail > 0 && s.Length > 1) 
                {
                    s = s.Remove(0, 1);
                    safefail--;

                    if (s == "") { break; }
                }

                #if UNITY_EDITOR
                if (safefail <= 0) { Debug.Log("Safefail 0."); }
                #endif
            } 
            else 
            {
                switch (contentType) 
                {
                    case FieldContentType.Float: s = "0"; break;
                    case FieldContentType.Integer: s = "0"; break;
                    case FieldContentType.String: s = ""; break;
                }
            }

            SetTextContent(s);
        }
        
        /// <summary>
        /// Set the text of the field's textObj.
        /// </summary>
        /// <param name="_s">String to be displayed by textObj.</param>
        void SetTextContent (string _s)
        {
            content = _s;
            textObj.text = prefix + content + suffix;
        }
        
        /// <summary>
        /// Convert the content to an integer value.
        /// </summary>
        /// <returns></returns>
        int ContentToInt ()
        {
            char[] chars = content.ToCharArray();
            string cleanString = "";
            bool roundUp = false;

            for (int i = 0; i < chars.Length; i++) 
            {
                char c = chars[i];

                switch (c) 
                {
                    case '0':
                        cleanString += "0";
                        break;

                    case '1':
                        cleanString += "1";
                        break;

                    case '2':
                        cleanString += "2";
                        break;

                    case '3':
                        cleanString += "3";
                        break;

                    case '4':
                        cleanString += "4";
                        break;

                    case '5':
                        cleanString += "5";
                        break;

                    case '6':
                        cleanString += "6";
                        break;

                    case '7':
                        cleanString += "7";
                        break;

                    case '8':
                        cleanString += "8";
                        break;

                    case '9':
                        cleanString += "9";
                        break;

                    /*
                    case ',':
                    case '.':

                        if (i < chars.Length - 1) 
                        {
                            char nextChar = chars[i + 1];
                            int nextInt = int.Parse(nextChar.ToString());

                            roundUp = nextInt >= 5;
                        }

                        i = chars.Length;

                        break;
                    */

                    default:
                        break;
                }
            }

            return cleanString == "" ? 0 : (int.Parse(cleanString) + (roundUp ? 1 : 0));
        }
        
        /// <summary>
        /// Convert the content to a float value.
        /// </summary>
        /// <returns></returns>
        float ContentToFloat ()
        {
            char[] chars = content.ToCharArray();
            string cleanString = "";

            for (int i = 0; i < chars.Length; i++) 
            {
                char c = chars[i];

                switch (c) 
                {
                    case '0':
                        cleanString += "0";
                        break;

                    case '1':
                        cleanString += "1";
                        break;

                    case '2':
                        cleanString += "2";
                        break;

                    case '3':
                        cleanString += "3";
                        break;

                    case '4':
                        cleanString += "4";
                        break;

                    case '5':
                        cleanString += "5";
                        break;

                    case '6':
                        cleanString += "6";
                        break;

                    case '7':
                        cleanString += "7";
                        break;

                    case '8':
                        cleanString += "8";
                        break;

                    case '9':
                        cleanString += "9";
                        break;

                    case '.':
                    case ',':
                        cleanString += ".";
                        break;

                    default:
                        cleanString += "";
                        break;
                }
            }

            return cleanString == "" ? 0f : float.Parse(cleanString);
        }

        #endregion
        #region Color Control Methods

        /// <summary>
        /// Update the colors of visual elements.
        /// </summary>
        void UpdateColors ()
        {
            transform.Find("Background").GetComponent<Image>().color = fieldActive ? backgroundActiveColor : backgroundColor;
            transform.GetComponent<Image>().color = fieldActive ? borderActiveColor : borderColor;
            textObj.color = fieldActive ? textActiveColor : textColor;
        }

        #endregion
        
        #endregion


    } /// End of Class


} /// End of Namespace