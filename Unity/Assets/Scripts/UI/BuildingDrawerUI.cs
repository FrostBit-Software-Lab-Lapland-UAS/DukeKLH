/******************************************************************************
 * File        : BuildingDrawerUI.cs
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
using TMPro;
using DUKE.Controls;
using DUKE.KLHData;


namespace DUKE.UI {


    /// <summary>
    /// Display BuildingDrawer's generated data numerically.
    /// </summary>
    public class BuildingDrawerUI : MonoBehaviour
    {
        #region Variables

        [SerializeField] BuildingDrawer buildingDrawer;


        [Space(10f)]
        [SerializeField] TextMeshProUGUI grossAreaObj;
        [SerializeField] TextMeshProUGUI grossVolumeObj;
        [SerializeField] TextMeshProUGUI envelopeObj;
        [SerializeField] TextMeshProUGUI conductionObj;
        [SerializeField] TextMeshProUGUI ventilationObj;
        [SerializeField] TextMeshProUGUI airInfiltrationObj;
        [SerializeField] TextMeshProUGUI heatedWaterObj;
        [SerializeField] TextMeshProUGUI internalSourcesObj;
        [SerializeField] TextMeshProUGUI totalHeatLossObj;
        [SerializeField] TextMeshProUGUI conductionPercentObj;
        [SerializeField] TextMeshProUGUI ventilationPercentObj;
        [SerializeField] TextMeshProUGUI airInfiltrationPercentObj;
        [SerializeField] TextMeshProUGUI heatedWaterPercentObj;
        [SerializeField] TextMeshProUGUI internalSourcesPercentObj;
        [SerializeField] TextMeshProUGUI totalPercentObj;


        [Space(10f)]
        [SerializeField] RectTransform eraButtonsPanel;
        [SerializeField] RectTransform presetButtonsPanelOnDrawingPlane;
        [SerializeField] ZoneMap zoneMap;


        [SerializeField] Transform heatLossInfoButtonsParent;
        [SerializeField] Transform heatLossInfoPanelsParent;
        [SerializeField] GameObject infoConduction;
        [SerializeField] GameObject infoVentilation;
        [SerializeField] GameObject infoAirInfiltration;
        [SerializeField] GameObject infoHeatedWater;
        [SerializeField] GameObject infoInternalSources;


        [SerializeField] GameObject warningBuildingSize;
        [SerializeField] Vector3Int warningSizeLimits;
        [SerializeField] GameObject warningFloorCount;
        [SerializeField] int warningFloorCountLimit;

        #endregion


        #region Properties

        /// <summary>
        /// UI panel containing dimension values.
        /// </summary>
        protected GameObject DimensionValuesPanel 
        { 
            get { return grossAreaObj.transform.parent.parent.gameObject; } 
        }

        /// <summary>
        /// UI panel containing energy consumption values.
        /// </summary>
        protected GameObject EnergyConsumptionPanel 
        { 
            get { return conductionObj.transform.parent.parent.gameObject; } 
        }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Called when ZoneMap is updated.
        /// </summary>
        /// <param name="_zone"></param>
        public void ZoneSelected (BuildingZone _zone)
        {
            UpdateBuildingCalculations();
        }

        /// <summary>
        /// Called through Clickable's UnityEvent. Set the Building's construction year.
        /// </summary>
        /// <param name="_year">Selected year.</param>
        public void YearSelected (int _year)
        {
            KLHManager.Building.ConstructionYear = _year;
            UpdateBuildingCalculations();
        }

        /// <summary>
        /// Update the calculation of the building's dimensions and heat loss.
        /// </summary>
        public void UpdateBuildingCalculations ()
        {
            UpdateButtonStates();

            // Toggle panels off and on to set the values curved after an unknown reason causes them to become flat.
            DimensionValuesPanel.SetActive(false);
            EnergyConsumptionPanel.SetActive(false);

            KLHManager.Building.CalculateHeatingValues();

            grossAreaObj.text = KLHManager.FormatIntToString(KLHManager.Building.GrossArea) + " m2";
            grossVolumeObj.text = KLHManager.FormatIntToString(KLHManager.Building.GrossVolume) + " m3";
            envelopeObj.text = KLHManager.FormatIntToString(KLHManager.Building.EnvelopeArea) + " m2";
            
            /// Clamp values to prevent negative numbers:
            int conduction = Mathf.Clamp(KLHManager.Building.HeatConsumptionDueToConduction, 0, int.MaxValue);
            int ventilation = Mathf.Clamp(KLHManager.Building.HeatConsumptionDueToVentilation, 0, int.MaxValue);
            int airInfiltration = Mathf.Clamp(KLHManager.Building.HeatConsumptionDueToAirInfiltration, 0, int.MaxValue);
            int heatedWater = Mathf.Clamp(KLHManager.Building.HeatConsumptionDueToWaterHeating, 0, int.MaxValue);
            int internalSources = Mathf.Clamp(KLHManager.Building.HeatGainDueToInternalSources, 0, int.MaxValue) * -1;
            int total = Mathf.Clamp(KLHManager.Building.TotalHeatConsumption, 0, int.MaxValue);
            string suffix = " kWh/a";

            conductionObj.text = KLHManager.FormatIntToString(conduction) + suffix;
            ventilationObj.text = KLHManager.FormatIntToString(ventilation) + suffix;
            airInfiltrationObj.text = KLHManager.FormatIntToString(airInfiltration) + suffix;
            heatedWaterObj.text = KLHManager.FormatIntToString(heatedWater) + suffix;
            internalSourcesObj.text = KLHManager.FormatIntToString(internalSources) + suffix;
            totalHeatLossObj.text = KLHManager.FormatIntToString(total) + suffix;

            if (total == 0)
            {
                conductionPercentObj.text = "0.00 %";
                ventilationPercentObj.text = "0.00 %";
                airInfiltrationPercentObj.text = "0.00 %";
                heatedWaterPercentObj.text = "0.00 %";
                internalSourcesPercentObj.text = "0.00 %";
                totalPercentObj.text = "0.00 %";
            }
            else
            {
                float actualTotal = conduction + ventilation + airInfiltration + heatedWater;
                int decimals = 2;
                
                conductionPercentObj.text = KLHManager.FormatFloatToStringPercentage(conduction / actualTotal, decimals);
                ventilationPercentObj.text = KLHManager.FormatFloatToStringPercentage(ventilation / actualTotal, decimals);
                airInfiltrationPercentObj.text = KLHManager.FormatFloatToStringPercentage(airInfiltration / actualTotal, decimals);
                heatedWaterPercentObj.text = KLHManager.FormatFloatToStringPercentage(heatedWater / actualTotal, decimals);
                internalSourcesPercentObj.text = KLHManager.FormatFloatToStringPercentage(internalSources / actualTotal, decimals);
                totalPercentObj.text = KLHManager.FormatFloatToStringPercentage(1f, 2);
            }


            
            DimensionValuesPanel.SetActive(true);       
            EnergyConsumptionPanel.SetActive(true);     

            bool buildingSizeTooSmall = 
                (KLHManager.Building.GrossArea < warningSizeLimits.x || 
                KLHManager.Building.GrossVolume < warningSizeLimits.y || 
                KLHManager.Building.EnvelopeArea < warningSizeLimits.z);

            warningBuildingSize.SetActive(buildingSizeTooSmall);
            warningFloorCount.SetActive(KLHManager.Building.HighestLevel < warningFloorCountLimit);
        }

        /// <summary>
        /// Called through Clickable's UnityEvent.
        /// Toggle info boxes on/off depending on the provided string.
        /// </summary>
        /// <param name="_info">String containing the target and state separated by a dot.</param>
        public void ToggleInfoBox (string _info) 
        {
            if (!_info.Contains('.')) {

                #if UNITY_EDITOR
                Debug.LogError("BuildingDrawerUI.ToggleInfoBox(): Use '.' to separate target and state (conduction.off).");
                #endif
            }

            string[] s  =_info.Split('.');

            if (s.Length < 2) { return; }

            bool on = s[1] == "on";

            switch (s[0]) {

                case "conduction":      infoConduction.SetActive(on); break;
                case "ventilation":     infoVentilation.SetActive(on); break;
                case "airInfiltration": infoAirInfiltration.SetActive(on); break;
                case "heatedWater":     infoHeatedWater.SetActive(on); break;
                case "internalSources": infoInternalSources.SetActive(on); break;
            } 
        }
        
        /// <summary>
        /// Called through Clickable's UnityEvent. 
        /// Toggle info boxes on/off depending on the Clickable's IsToggled state.
        /// </summary>
        /// <param name="_infoNumber">Index of the Clickable within their parent.</param>
        public void ToggleInfoBox (int _infoNumber) 
        {
            foreach (Transform t in heatLossInfoPanelsParent) {

                t.gameObject.SetActive(false);
            }

            bool isToggled = heatLossInfoButtonsParent.GetChild(_infoNumber).GetComponent<Clickable>().IsToggled;
            
            heatLossInfoPanelsParent.GetChild(_infoNumber).gameObject.SetActive(isToggled);
        }

        #endregion
        #region MonoBehaviour Methods

        private void OnEnable ()
        {
            KLHManager.BuildingDrawerUI = this;

            BuildingDrawer.BuildingDrawerUpdated += UpdateBuildingCalculations;
            zoneMap.ZoneSelected += ZoneSelected;

            UpdateBuildingCalculations();
        }
        
        private void OnDisable ()
        {
            BuildingDrawer.BuildingDrawerUpdated -= UpdateBuildingCalculations;
            zoneMap.ZoneSelected -= ZoneSelected;
        }

        #endregion
        #region Button Methods

        /// <summary>
        /// Update button states based on source building's settings.
        /// </summary>
        public void UpdateButtonStates ()
        {
            UpdateEraButtons();
            UpdatePresetButtons();
        }

        /// <summary>
        /// Update era buttons' states.
        /// </summary>
        void UpdateEraButtons ()
        {
            Clickable[] buttons = eraButtonsPanel.GetComponentsInChildren<Clickable>();

            for (int i = 0; i < buttons.Length; i++) 
            {
                buttons[i].SetToggled(KLHManager.Building.ConstructionEra == i, true);
            }
        }

        /// <summary>
        /// Update preset buttons' states.
        /// </summary>
        void UpdatePresetButtons ()
        {
            Clickable[] buttons = presetButtonsPanelOnDrawingPlane.GetComponentsInChildren<Clickable>();

            for (int i = 0; i < buttons.Length; i++) {

                buttons[i].SetToggled(KLHManager.Building == KLHManager.Buildings[i], true);
            }
        }

        #endregion
        
        #endregion


    } /// End of Class
    

} /// End of Namespace