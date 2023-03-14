/******************************************************************************
 * File        : KLHManager.cs
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
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DUKE.Controls;
using DUKE.KLHData;
using DUKE.UI;


namespace DUKE {

    /// <summary>
    /// Different modes of the program.
    /// </summary>
    public enum KLHMode {
        SplashScreen,
        MainMenu,
        BuildingDrawer,
        Visualisation,
        DigitalTwins,
  
    }

    /// <summary>
    /// Usable languages of the program.
    /// </summary>
    public enum Language
    {
        Finnish,
        English
    }


    /// <summary>
    /// Controls the states of the game and relevant parameters.
    /// This object uses the Singleton pattern and is persistent 
    /// independent of the loading or unloading of Scenes.
    /// </summary>
    public class KLHManager : MonoBehaviour
    {
        #region Variables

        #region General Variables

        /// <summary>
        /// Public static instance of <typeparamref name="KLHManager"/>.
        /// </summary>
        public static KLHManager Current;
        [SerializeField] float fadeDuration;
        [SerializeField] KLHMode mode;
        [SerializeField] GameObject player;
        [SerializeField] float fadeValue;
        [SerializeField] UIPaletteSO defaultPalette;
        InputMode currentInputMode;

        #endregion
        #region Mode-specific GameObjects
        
        [Space(20f)]
        [SerializeField] MainMenuUI mainMenuUI;
        [SerializeField] BuildingDrawer buildingDrawer;
        [SerializeField] BuildingDrawerUI buildingDrawerUI;
        [SerializeField] GameObject buildingDrawerControls;
        [SerializeField] DataVisualisationUI visualisationUI;
        [SerializeField] GameObject visualisationSelectors;

        #endregion
        #region BuildingDrawer Variables

        [SerializeField] Building building;
        [SerializeField] List<Building> buildings;
        [SerializeField] List<Building> originalBuildings = null;
        [SerializeField] BuildingVolumeObject volumeObj;
        [SerializeField] bool editTemplates = false;
        [SerializeField] bool resetTemplates = false;

        #endregion
        #region Language variables

        [SerializeField] Language language;

        #endregion
        #region Audio Variables

        AudioLevel currentAudioLevel;

        #endregion

        #endregion


        #region Properties

        #region General Properties

        /// <summary>
        /// Current <typeparamref name="KLHMode"/>.
        /// </summary>
        public static KLHMode Mode {
            get { return Current.mode; } }

        /// <summary>
        /// Current <typeparamref name="Language"/>.
        /// </summary>
        public static Language Language { 
            get { return Current.language; } 
            set { Current.language = value; LanguageSet?.Invoke(); } }

        /// <summary>
        /// Static reference of the player.
        /// </summary>
        public static GameObject Player { 
            get { return TryCreatePlayer(); } }

        /// <summary>
        /// Currently active <typeparamref name="Camera"/>.
        /// </summary>
        public static Transform ActiveCamera { 
            get { return GetActiveCamera(); } }

        /// <summary>
        /// TRUE if the current <typeparamref name="InputMode"/> is <typeparamref name="Desktop"/>.
        /// </summary>
        public static bool IsDesktop { 
            get { return Current.currentInputMode == InputMode.Desktop; } }

        /// <summary>
        /// TRUE if the current <typeparamref name="InputMode"/> is <typeparamref name="VR"/>.
        /// </summary>
        public static bool IsVR { 
            get { return Current.currentInputMode == InputMode.VR; } }

        /// <summary>
        /// Currently active <typeparamref name="Input"/>.
        /// </summary>
        public static DUKE.Controls.Input ActiveInput { 
            get { return Player.GetComponentInChildren<DUKE.Controls.Input>(); } }

        /// <summary>
        /// Current opacity of <typeparamref name="FadeOrbs"/>.
        /// </summary>
        public static float FadeValue { 
            get { return Current.fadeValue; } 
            set { Current.fadeValue = value; FadeValueChanged?.Invoke(value); } }

        /// <summary>
        /// Duration of the fade.
        /// </summary>
        public static float FadeDuration { 
            get { return Current.fadeDuration; } }

        /// <summary>
        /// Currently selected <typeparamref name="Building"/>.
        /// </summary>
        public static Building Building { 
            get { return Current.building; } 
            set { Current.building = value; value.CalculateHeatingValues(); BuildingChanged?.Invoke(); } }

        /// <summary>
        /// List of all <typeparamref name="Buildings"/>.
        /// </summary>
        public static List<Building> Buildings { 
            get { return Current.buildings; } 
            set { Current.buildings = value; } }

        /// <summary>
        /// List of the original <typeparamref name="Building"/> templates.
        /// </summary>
        public static List<Building> OriginalBuildings { 
            get { return Current.originalBuildings; } 
            set { Current.originalBuildings = value; } }

        /// <summary>
        /// Template editing is allowed when set to TRUE.
        /// </summary>
        public static bool EditTemplates { 
            get { return Current.editTemplates; } }

        /// <summary>
        /// TRUE if the currently selected <typeparamref name="Building"/> has <typeparamref name="GrossArea"/> higher than 0 m2.
        /// </summary>
        public static bool BuildingIsDefined { 
            get { return Building.GrossArea > 0; } }

        #endregion
        #region Main Object Reference Properties

        /// <summary>
        /// Reference to the <typeparamref name="MainMenuUI"/>.
        /// </summary>
        public static MainMenuUI MainMenuUI { 
            get { return Current.mainMenuUI; } 
            set { Current.mainMenuUI = value; } }

        /// <summary>
        /// Reference to the <typeparamref name="BuildingDrawer"/>.
        /// </summary>
        public static BuildingDrawer BuildingDrawer { 
            get { return Current.buildingDrawer; } 
            set { Current.buildingDrawer = value; } }

        /// <summary>
        /// Reference to the <typeparamref name="BuildingDrawerUI"/>.
        /// </summary>
        public static BuildingDrawerUI BuildingDrawerUI { 
            get { return Current.buildingDrawerUI; } 
            set { Current.buildingDrawerUI = value; } }

        /// <summary>
        /// Reference to the controls panel object of <typeparamref name="BuildingDrawer"/>.
        /// </summary>
        public static GameObject BuildingDrawerControls { 
            get { return Current.buildingDrawerControls; } 
            set { Current.buildingDrawerControls = value; } }

        /// <summary>
        /// Reference to the <typeparamref name="DataVisualisationUI"/>.
        /// </summary>
        public static DataVisualisationUI VisualisationUI { 
            get { return Current.visualisationUI; }
            set { Current.visualisationUI = value; } }

        /// <summary>
        /// Reference to the <typeparamref name="BuildingVolumeObject"/>.
        /// </summary>
        public static BuildingVolumeObject VolumeObject { 
            get { return Current.volumeObj; } 
            set { Current.volumeObj = value; } }

        /// <summary>
        /// Reference to the parent object of <typeparamref name="AreaSelectors"/> for electricity and district heating values.
        /// </summary>
        public static GameObject VisualisationSelectors { 
            get { return Current.visualisationSelectors; }
            set { Current.visualisationSelectors = value; } }

        #endregion
        #region Root Object Properties

        /// <summary>
        /// Reference to the root object of MainMenu <typeparamref name="Scene"/>.
        /// </summary>
        public static GameObject RootMainMenu { 
            get; 
            set; }


        /// <summary>
        /// Reference to the root object of KLH_MAIN <typeparamref name="Scene"/>.
        /// </summary>
        public static GameObject RootKLHMain { 
            get; 
            set; }


        /// <summary>
        /// Reference to the root object of Digital_Twins_Full <typeparamref name="Scene"/>.
        /// </summary>
        public static GameObject RootDigitalTwins 
        { 
            get; 
            set; 
        }

        public static AudioLevel CurrentAudioLevel
        {
            get { return Current.currentAudioLevel; }
            set { Current.currentAudioLevel = value; }
        }

        #endregion

        #endregion


        #region Events

        /// <summary>
        /// Called when the <typeparamref name="Language"/> is updated.
        /// </summary>
        public static Action LanguageSet;

        /// <summary>
        /// Called when the <typeparamref name="KLHMode"/> is updated.
        /// </summary>
        public static Action ModeChanged;

        /// <summary>
        /// Called when the fade value is updated.
        /// </summary>
        public static Action<float> FadeValueChanged;

        /// <summary>
        /// Called when the <typeparamref name="Building"/> is updated.
        /// </summary>
        public static Action BuildingChanged;

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Attempt to instantiate a Player prefab if one does not already exist.
        /// </summary>
        /// <returns>Player prefab as a GameObject.</returns>
        public static GameObject TryCreatePlayer ()
        {
            Current.player =  GameObject.FindGameObjectWithTag("Player");

            if (null == Current.player) {

                if (Current.transform.childCount == 0) {

                    Current.player = Instantiate(Resources.Load("Prefabs/Controls/Player") as GameObject);
                    Current.player.transform.SetParent(Current.transform);

                } else {
                
                    Current.player = Current.transform.GetChild(0).gameObject;
                }
            }

            return Current.player;
        }

        /// <summary>
        /// Get the active Camera.
        /// </summary>
        /// <returns>Transform of the currently active Camera.</returns>
        public static Transform GetActiveCamera () 
        {
            if (IsDesktop)  { return Current.player.transform.GetChild(0).GetChild(0); } 
            else            { return Current.player.transform.GetChild(1).GetChild(0).GetChild(0); }
        }

        /// <summary>
        /// Set the language of text elements.
        /// Called through Clickable's UnityEvent.
        /// </summary>
        /// <param name="_lang">Index of selected language in the Language enum.</param>
        public static void SetLanguage(int _lang) 
        {
            Language = (Language)_lang;
        } 
        
        /// <summary>
        /// Set the language of text elements.
        /// </summary>
        /// <param name="_lang">Selected language.</param>
        public static void SetLanguage (Language _lang) 
        {
            Language = _lang;
        }

        /// <summary>
        /// Teleport Player to a specified location and rotation.
        /// </summary>
        /// <param name="_mode">Defined mode that specifies which scene the teleport target is searched from.</param>
        /// <param name="_fadeOut">Whether or not to use fading from black.</param>
        public static void TeleportPlayer (KLHMode _mode, bool _fadeOut = false) 
        {
            PlayerSpawnPoint[] spawnPoints = FindObjectsOfType<PlayerSpawnPoint>(true);

            for (int i = 0; i < spawnPoints.Length; i++) {

                PlayerSpawnPoint sp = spawnPoints[i];

                if (sp.Mode == _mode) { 
                    
                    Transform targetTransform = sp.transform;
                    SetPlayerPositionAndOrientation(targetTransform);          
                }
            }

            if (_fadeOut) { SceneController.DoFade(false); }
        }
        
        /// <summary>
        /// Teleport Player to a specified location and rotation.
        /// </summary>
        /// <param name="_targetTransform">Target Transform from which position and rotation are set to Player.</param>
        /// <param name="_fadeOut">Whether or not to use fading from black.</param>
        public static void TeleportPlayer (Transform _targetTransform, bool _fadeOut = false) 
        {
            SetPlayerPositionAndOrientation(_targetTransform);

            if (_fadeOut) { SceneController.DoFade(false); }
        }
        
        /// <summary>
        /// Teleport Player to a specified location.
        /// </summary>
        /// <param name="_targetPosition">Target position that is set to Player.</param>
        /// <param name="_fadeOut">Whether or not to use fading from black.</param>
        public static void TeleportPlayer (Vector3 _targetPosition, bool _fadeOut = false)
        {
            SetPlayerPosition(_targetPosition);

            if (_fadeOut) { SceneController.DoFade(false); }
        }



        /// <summary>
        /// Begin the transition to Splash_Screen.
        /// </summary>
        public static void ToSplashScreen () 
        {
            ResetBuildingTemplates();
            ToMode(KLHMode.SplashScreen);  
        }
        
        /// <summary>
        /// Begin the transition to Main_Menu.
        /// </summary>
        public static void ToMainMenu () 
        {       
            ToMode(KLHMode.MainMenu); 
        }
        
        /// <summary>
        /// Begin the transition to KLH_MAIN (BuildingDrawer).
        /// </summary>
        public static void ToBuildingDrawer () 
        {
            ToMode(KLHMode.BuildingDrawer); 
        }
        
        /// <summary>
        /// Begin the transition to KLH_MAIN (DataVisualisation).
        /// </summary>
        public static void ToVisualisation () 
        {
            ToMode(KLHMode.Visualisation); 
        }
    
        /// <summary>
        /// Begin the transition to Digital_Twins_Full.
        /// </summary>
        public static void ToDigitalTwins () 
        {
            ToMode(KLHMode.DigitalTwins); 
        }

        /// <summary>
        /// Reset the template Buildings list by instatiating each Building and saving them in a new list.
        /// </summary>
        public static void ResetBuildingTemplates () 
        {
            if (OriginalBuildings.Count != Buildings.Count) {

                OriginalBuildings = new List<Building>(Buildings);
            }

            Buildings = Current.CloneBuildings(Current.originalBuildings);
            Building = Buildings[0];
        }

        #endregion
        #region MonoBehaviour

        private void Awake () 
        {
            if (null == Current) { 
                
                Current = this;

                TranslatorSO.GetTranslationsFromFile();
                CurrentAudioLevel = AudioLevel.High;

                if (resetTemplates) { 

                    foreach (Building b in Buildings) {

                        b.DrawGrid = new DrawGrid(BuildingDrawer.MaxGridDimensions.x, BuildingDrawer.MaxGridDimensions.y);
                    }

                } else if (!EditTemplates) {

                    ResetBuildingTemplates();
                }

                Building = Buildings[0];
            }

            if (Current != this) {
    
                Destroy(gameObject); 
            }   

            Application.targetFrameRate = 300;
            QualitySettings.vSyncCount = -1;
            
            Current.currentInputMode = InputMode.Desktop;
        }

        void OnEnable()
        {
            BuildingDrawer.BuildingDrawerUpdated += OnBuildingDrawerUpdated;
            DetectInputDevices.InputModeChanged += UpdateInputMode;
        }

        void OnDisable()
        {
            BuildingDrawer.BuildingDrawerUpdated -= OnBuildingDrawerUpdated;
            DetectInputDevices.InputModeChanged -= UpdateInputMode;
        }

        #endregion
        #region Setup and Modes
        
        /// <summary>
        /// Fade to black, switch KLHMode and fade out.
        /// </summary>
        /// <param name="_mode"></param>
        /// <param name="_fade"></param>
        static void ToMode (KLHMode _mode, bool _fade = true) 
        {
            SceneController.DoFade<KLHMode, bool>(true, 1f, SwitchMode, _mode, _fade);
        }
    
        /// <summary>
        /// Change KLHMode and toggle relevant object (in)active.
        /// </summary>
        /// <param name="_mode"></param>
        public static void SwitchMode (KLHMode _mode, bool _fade) 
        {
            Current.mode = _mode;

            switch (Current.mode) {

                case KLHMode.SplashScreen:
                    AudioController.ChangeSnapshot(3f, 1f);
                    break;

                case KLHMode.MainMenu:
                    AudioController.ChangeSnapshot(3f, 1f);
                    RootMainMenu.SetActive(true);
                    Transform panelsParent = MainMenuUI.gameObject.transform.Find("Main menu scale/Canvas");
                    panelsParent.GetChild(0).gameObject.SetActive(true);
                    panelsParent.GetChild(1).gameObject.SetActive(false);

                    RootKLHMain.SetActive(false);
                    RootDigitalTwins.SetActive(false);
                    break;

                case KLHMode.BuildingDrawer:
                    AudioController.ChangeSnapshot(3f, 1f);
                    RootKLHMain.SetActive(true);
                    BuildingDrawer.gameObject.SetActive(true);
                    BuildingDrawerUI.gameObject.SetActive(true);
                    BuildingDrawerControls.SetActive(true);

                    BuildingDrawerUI.UpdateButtonStates();

                    VisualisationUI.gameObject.SetActive(false);
                    VisualisationSelectors.SetActive(false);
                    VolumeObject.gameObject.SetActive(false);
                    RootMainMenu.SetActive(false);
                    RootDigitalTwins.SetActive(false);
                    break;

                case KLHMode.Visualisation: 
                    AudioController.ChangeSnapshot(3f, 1f);
                    RootKLHMain.SetActive(true);
                    VisualisationUI.gameObject.SetActive(true);
                    VisualisationSelectors.SetActive(true);
                    VolumeObject.gameObject.SetActive(true);

                    VolumeObject.UpdateVolumeModel();

                    BuildingDrawer.gameObject.SetActive(false);
                    BuildingDrawerUI.gameObject.SetActive(false);
                    BuildingDrawerControls.SetActive(false); 
                    RootMainMenu.SetActive(false);
                    RootDigitalTwins.SetActive(false);
                    break;

                case KLHMode.DigitalTwins:
                    AudioController.ChangeSnapshot(3f, 0f);
                    RootDigitalTwins.SetActive(true);
                    RootKLHMain.SetActive(false);
                    RootMainMenu.SetActive(false);
                    break;

                default:
                    #if UNITY_EDITOR
                    Debug.LogError("KLHManager.SwitchMode(): invalid KLHMode.");
                    #endif
                    break;
            }

            ModeChanged?.Invoke();

            TeleportPlayer(Current.mode, _fade);
        }

        /// <summary>
        /// Update the current InputMode. This event is called from DetectInputDevices class whenever the mode changes.
        /// </summary>
        /// <param name="_newMode"></param>
        static void UpdateInputMode (InputMode _newMode) 
        {
            Current.currentInputMode = _newMode;
        }

        #endregion
        #region Building Methods

        /// <summary>
        /// Called when BuildingDrawer updates the building.
        /// </summary>
        protected static void OnBuildingDrawerUpdated () 
        {
            if (null != Current.building) { Current.building.CalculateHeatingValues(); }
            Building.CalculateDimensions();
        }

        /// <summary>
        /// Create duplicates out of original Building templates.
        /// </summary>
        /// <returns>A list of cloned Buildings.</returns>
        List<Building> CloneBuildings (List<Building> _originals)
        {
            List<Building> clones = new List<Building>();

            foreach (Building original in _originals) {

                if (original.DrawGrid.NullCells) {
                    
                    #if UNITY_EDITOR
                    Debug.LogError("KLHManager.CloneBuildings(): " + original.name+" has NULL cells. Replacing DrawGrid with a new instance.");
                    #endif
                    
                    original.DrawGrid = new DrawGrid(BuildingDrawer.MaxGridDimensions.x, BuildingDrawer.MaxGridDimensions.y);
                }

                clones.Add(Instantiate(original));
            }

            return clones;
        }

        #endregion
        #region Player Methods

        /// <summary>
        /// Update Player's position and rotation. 
        /// </summary>
        /// <param name="_target">Target transform from which the new position and rotation are taken.</param>
        static void SetPlayerPositionAndOrientation (Transform _target)
        {
            Player.transform.SetPositionAndRotation(_target.position, _target.rotation);
            Player.transform.GetChild(0).localPosition = Vector3.zero + Vector3.up * 1.7f;
            Player.transform.GetChild(0).localEulerAngles = Vector3.zero;
        }
        
        /// <summary>
        /// Update Player's position only.
        /// </summary>
        /// <param name="_position">Position to which the Player is moved.</param>
        static void SetPlayerPosition (Vector3 _position) 
        {
            Player.transform.SetPositionAndRotation(_position, Player.transform.rotation);
            Player.transform.GetChild(0).localPosition = Vector3.zero + Vector3.up * 1.7f;
            Player.transform.GetChild(0).localEulerAngles = Vector3.zero;
        }

        #endregion
        #region Static Help Methods

        /// <summary>
        /// Format an integer (often energy consumption [kWh]) into a string.
        /// </summary>
        /// <param name="_value">Integer value to be formatted.</param>
        /// <param name="_millionsAsM">Whether to display millions as M or full number.</param>
        /// <returns></returns>
        public static string FormatIntToString (int _value, bool _millionsAsM = false)
        {
            string s = "";
            bool subZero = false;

            if (_value < 0)
            {
                subZero = true;
                _value = Mathf.Abs(_value);
            }

            switch (_value) {

                case > 1000000:

                    if (_millionsAsM) 
                    {
                        s = (Mathf.RoundToInt(_value / 10000) / 100f).ToString();
                        s.Replace('.',',');
                        return (subZero ? "-" : "") + s + " M";    
                    } 
                    else 
                    {
                        s = _value.ToString();
                        int count = s.Length;
                        List<string> groups = new List<string>();
                        
                        for (int i = (count - 1); i >= 0; i -= 3) {

                            int startIndex = (i >= 2 ? i - 2 : 0);
                            int length = Mathf.Min(i + 1, 3);

                            groups.Add(s.Substring(startIndex, length));
                        }

                        s = "";

                        for (int i = groups.Count - 1; i >= 0; i--) 
                        {        
                            s += groups[i];

                            if (i > 0) { s += " "; }
                        }

                        return (subZero ? "-" : "") + s;
                    }

                case >= 1000:

                    s = _value.ToString();     
                    string firstX = s.Substring(0, s.Length - 3);
                    string last3 = s.Substring(s.Length - 3, 3);
                    return (subZero ? "-" : "") + firstX+" "+last3; 
            }

            return (subZero ? "-" : "") + _value.ToString();
        }

        /// <summary>
        /// Format a float into a string.
        /// </summary>
        /// <param name="_value">Float value to be formatted.</param>
        /// <param name="_decimals">The number of decimal points.</param>
        /// <returns>String form of the int value.</returns>
        public static string FormatFloatToString (float _value, int _decimals = 0)
        {
            int power = (int)Mathf.Pow(10,_decimals);
            float value = Mathf.RoundToInt(_value * power) / (float)power;

            return value.ToString();
        }

        /// <summary>
        /// Format a float into a string and add percent symbol to the end.
        /// </summary>
        /// <param name="_value">The float value (0...1) to be formatted.</param>
        /// <param name="_decimals">The number of decimal points.</param>
        /// <returns>String form of the float value.</returns>
        public static string FormatFloatToStringPercentage (float _value, int _decimals = 1)
        {
            int power = (int)Mathf.Pow(10,_decimals);
            float percent = Mathf.FloorToInt(_value * 100 * power) / (float)power;

            return percent.ToString() + " %";
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace