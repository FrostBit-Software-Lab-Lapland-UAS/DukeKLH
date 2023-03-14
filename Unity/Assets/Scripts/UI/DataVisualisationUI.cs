/******************************************************************************
 * File        : DataVisualisationUI.cs
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
using DUKE.Controls;
using TMPro;
using System;
using DUKE.UI;
using DUKE.KLHData;


namespace DUKE {

    /// <summary>
    /// Different types of <typeparamref name="Graph"/>.
    /// </summary>
    public enum GraphType {
        TotalKWH,
        AnnualCost,
        AnnualCO2,
        TwentyYearCost
    }


    /// <summary>
    /// Controls the UI components of Data Visualisation UI.
    /// </summary>
    public class DataVisualisationUI : MonoBehaviour
    {
        #region Variables

        #region General Variables

        [SerializeField] Color[] renewableColors;
        [SerializeField] Color[] renewableInvestementColors;
        [SerializeField] Color[] districtColors;
        [SerializeField] Color[] districtInvestmentColors;
        [SerializeField] Color disabledColor;
        [SerializeField] AreaSelector costSelector;
        [SerializeField] AreaSelector co2Selector;
        [SerializeField] Clickable[] costPresetButtons;
        [SerializeField] Clickable[] co2PresetButtons;
        [SerializeField] TextMeshProUGUI textCost;
        [SerializeField] TextMeshProUGUI textCO2;
        [SerializeField] GameObject AreaSelectorsContainer;
        [SerializeField] GameObject districtDeviceWarningPanel;
        [SerializeField] GameObject defineBuildingWarningPanel;


        private bool isCostUpdated = false;
        private bool isCO2Updated = false;
        private bool includeDistrictDeviceCost = false;

        #endregion
        #region Graph Variables

        [SerializeField] GraphArea annualCO2GraphArea;
        [SerializeField] GraphArea annualCostGraphArea;
        [SerializeField] GraphArea twentyYearsCostGraphArea;
        [SerializeField] RectTransform graphInfoPanel;
        [SerializeField] Color graphInfoPanelBackgroundColor;


        Graph kwhDistrictGraph;
        Graph kWhRenewableGraph;
        Graph anCostDistrictGraph;
        Graph anCostRenewableGraph;
        Graph anCO2DistrictGraph;
        Graph anCO2RenewableGraph;
        Graph twentyYearsDistrictGraph;
        Graph twentyYearsRenewableGraph;
        Graph twentyYearsDHInvestmentCostsGraph;
        Graph twentyYearsREInvestmentCostsGraph;
    
        #endregion
        
        #endregion


        #region Properties
        
        /// <summary>
        /// Graph info panel.
        /// </summary>
        public RectTransform GraphInfoPanel { 
            get { return graphInfoPanel; } }
        
        /// <summary>
        /// <typeparamref name="Color"/> array containing hybrid device colors.
        /// </summary>
        public Color[] HybridDeviceColors { 
            get { return renewableColors; } }
        
        /// <summary>
        /// <typeparamref name="Color"/> array containing district heating colors.
        /// </summary>
        public Color[] DistrictColors { 
            get { return districtColors; } }
        
        /// <summary>
        /// <typeparamref name="Color"/> of disabled objects.
        /// </summary>
        public Color DisabledColor { 
            get { return disabledColor; } }
        
        /// <summary>
        /// TRUE when building floor count warning is active.
        /// </summary>
        public bool FloorCountWarning { 
            get { return transform.parent.Find("Warning Panel - Building Floor Count").gameObject.activeSelf; } }

        /// <summary>
        /// String array containing device names for district heating.
        /// </summary>
        public string[] DistrictNames { 
            get { return new string[4] { 
                "device_District", 
                "device_District", 
                "device_District", 
                "device_District"}; } }
        
        /// <summary>
        /// String array containing device names for hybrid devices.
        /// </summary>
        public string[] RenewableNames { 
            get { return new string[4] { 
                "device_District", 
                "device_Geothermal", 
                "device_Airsource", 
                "device_HeatRecovery"}; } }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Update all GraphAreas.
        /// </summary>
        public void UpdateGraphAreas () 
        {
            SetupGraphs();
            ToggleHeatRecoveryBars();
        }

        /// <summary>
        /// Set the info text of a Graph.
        /// </summary>
        /// <param name="_area">GraphArea to which the panel belongs.</param>
        /// <param name="_text">Text to be displayed.</param>
        public void SetGraphInfoText (GraphArea _area, string _text) 
        {
            if (_area.gameObject.activeSelf) {

                TextMeshProUGUI textObj = _area.transform.Find("Graph Info Panel").Find("Text - Graph Info").GetComponent<TextMeshProUGUI>();
                textObj.text = _text;
            }
        }

        /// <summary>
        /// Set the value of CostSelector and update affected Graphs. Called through Clickable's UnityEvent.
        /// </summary>
        /// <param name="_caseIndex">Index of the case.</param>
        public void SetCostValues (int _caseIndex) 
        {
            Vector2 ratio = _caseIndex switch {
                0 => new Vector2(0.15f, 0.10f),
                1 => new Vector2(0.10f, 0.50f),
                2 => new Vector2(0.60f, 0.05f),
                _ => new Vector2(0.15f, 0.10f)
            };

            UpdateCostWithoutToggle(ratio);
            costSelector.SetPointerPosition(ratio);
            annualCostGraphArea.UpdateGraphs();
            twentyYearsCostGraphArea.UpdateGraphs();
        }

        /// <summary>
        /// Set the value of CO2Selector and update affected Graphs. Called through Clickable's UnityEvent.
        /// </summary>
        /// <param name="_caseIndex">Index of the case.</param>
        public void SetCO2Values (int _caseIndex) 
        {
            Vector2 ratio = _caseIndex switch {
                0 => new Vector2(0.2225f, 0.4425f),
                1 => new Vector2(0.05f, 0.60f),
                2 => new Vector2(0.60f, 0.05f),
                _ => new Vector2(0.2225f, 0.4425f)
            };
            
            UpdateCo2WithoutToggle(ratio);
            co2Selector.SetPointerPosition(ratio);
            annualCO2GraphArea.UpdateGraphs();
        }

        /// <summary>
        /// Start a Coroutine to toggle Graph Info Panel on/off.
        /// </summary>
        /// <param name="_on">Whether to toggle the panel on or off.</param>
        public void ToggleGraphInfoPanel (bool _on) 
        {
            StartCoroutine(SetGraphInfoPanelActivity(_on));
        }

        /// <summary>
        /// Start a Coroutine to toggle Graph Info Panel on/off.
        /// </summary>
        /// <param name="_on">Whether to toggle the panel on or off.</param>
        /// <param name="_gInfo">GraphInfo with which the panel contents are filled.</param>
        public void ToggleGraphInfoPanel (bool _on, GraphInfo _gInfo)
        {
            graphInfoPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = _gInfo.name + (_gInfo.extraInfo == "" ? "" : "\n" + _gInfo.extraInfo );
            graphInfoPanel.GetChild(2).GetComponent<TextMeshProUGUI>().text = KLHManager.FormatIntToString((int)_gInfo.value) + " "+_gInfo.unit;
            graphInfoPanel.GetChild(3).GetComponent<TextMeshProUGUI>().text = _gInfo.Percent(2).ToString() + " %";
        
            ToggleGraphInfoPanel(_on);
        }

        /// <summary>
        /// Called through Clickable's UnityEvent.
        /// Toggle the cost of district heating investment between 0 and the calculated value.
        /// </summary>
        public void ToggleDistrictDeviceCost ()
        {
            includeDistrictDeviceCost = !includeDistrictDeviceCost;

            SetupGraph(twentyYearsCostGraphArea);

            districtDeviceWarningPanel.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>().enabled = includeDistrictDeviceCost;
            districtDeviceWarningPanel.transform.GetComponentInChildren<Clickable>().SetToggled(includeDistrictDeviceCost, true);
        }

        #endregion
        #region MonoBehaviour

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (isCostUpdated) {

                SetupGraph(annualCostGraphArea);
                SetupGraph(twentyYearsCostGraphArea);
                isCostUpdated = false;
            }
            if (isCO2Updated) {

                SetupGraph(annualCO2GraphArea);
                isCO2Updated = false;
            }
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            KLHManager.VisualisationUI = this;
            KLHManager.VisualisationSelectors = AreaSelectorsContainer;

            if (null != costSelector)   { costSelector.OnAreaClicked += UpdateCost; }
            if (null != co2Selector)    { co2Selector.OnAreaClicked += UpdateCo2; }

            KLHMath.ElectricityCostChanged += UpdateCostText;
            KLHMath.ElectricityCO2Changed += UpdateCO2Text;
            KLHMath.DistrictHeatingCostChanged += UpdateCostText;
            KLHMath.DistrictHeatingCO2Changed += UpdateCO2Text;

            districtDeviceWarningPanel.SetActive(true);

            if (KLHManager.BuildingIsDefined) {

                defineBuildingWarningPanel.SetActive(false);
                
                StartCoroutine(PreStartGraphAreas());
            
            } else {

                defineBuildingWarningPanel.SetActive(true);

                annualCostGraphArea.gameObject.SetActive(false);
                annualCO2GraphArea.gameObject.SetActive(false);
                twentyYearsCostGraphArea.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            districtDeviceWarningPanel.SetActive(false);

            if (null != costSelector)   { costSelector.OnAreaClicked -= UpdateCost; }
            if (null != co2Selector)    { co2Selector.OnAreaClicked -= UpdateCo2; }

            KLHMath.ElectricityCostChanged -= UpdateCostText;
            KLHMath.ElectricityCO2Changed -= UpdateCO2Text;
            KLHMath.DistrictHeatingCostChanged -= UpdateCostText;
            KLHMath.DistrictHeatingCO2Changed -= UpdateCO2Text;

            defineBuildingWarningPanel.SetActive(false);
        }

        #endregion
        #region Cost Updating

        /// <summary>
        /// Update the cost values of KLHMath.
        /// </summary>
        /// <param name="_ratio">Ratio between the defined min and max. X is electricity and Y is district heating.</param>
        void UpdateCost(Vector2 _ratio)
        {
            KLHMath.GetElectricityCost(_ratio.x);
            KLHMath.GetDistrictHeatingCost(_ratio.y);
            KLHManager.Building.UpdateHeatingDevices();
            isCostUpdated = true;     

            foreach (Clickable c in costPresetButtons)
            {
                c.SetToggled(false, true);
            }
        }

        /// <summary>
        /// Update the cost values of KLHMath. Do not toggle preset button off.
        /// </summary>
        /// <param name="_ratio">Ratio between the defined min and max. X is electricity and Y is district heating.</param>
        void UpdateCostWithoutToggle (Vector2 _ratio)
        {
            KLHMath.GetElectricityCost(_ratio.x);
            KLHMath.GetDistrictHeatingCost(_ratio.y);
            KLHManager.Building.UpdateHeatingDevices();
            isCostUpdated = true;   
        }

        /// <summary>
        /// Update the CO2 values of KLHMath.
        /// </summary>
        /// <param name="_ratio">Ratio between the defined min and max. X is electricity and Y is district heating.</param>
        void UpdateCo2(Vector2 _ratio)
        {
            KLHMath.GetElectricityCo2EmissionFactor(_ratio.x);
            KLHMath.GetDistrictHeatingCo2(_ratio.y);
            KLHManager.Building.UpdateHeatingDevices();
            isCO2Updated = true;   

            foreach (Clickable c in co2PresetButtons)
            {
                c.SetToggled(false, true);
            }  
        }

        /// <summary>
        /// Update the CO2 values of KLHMath. Do not toggle preset button off.
        /// </summary>
        /// <param name="_ratio">Ratio between the defined min and max. X is electricity and Y is district heating.</param>
        void UpdateCo2WithoutToggle (Vector2 _ratio)
        {
            KLHMath.GetElectricityCo2EmissionFactor(_ratio.x);
            KLHMath.GetDistrictHeatingCo2(_ratio.y);
            KLHManager.Building.UpdateHeatingDevices();
            isCO2Updated = true;    
        }

        /// <summary>
        /// Update CostSelector's text info.
        /// </summary>
        void UpdateCostText() 
        {
            textCost.text = TranslatorSO.GetTranslationById("elPrice") + " " + FormatCostNumber(KLHMath.ElectricityCost) + " €/kWh \n" 
                + TranslatorSO.GetTranslationById("dhPrice") + " " + FormatCostNumber(KLHMath.DistrictHeatingCost) + " €/kWh";
        }

        /// <summary>
        /// Update CostSelector's text info.
        /// </summary>
        void UpdateCO2Text() 
        {
            textCO2.text = TranslatorSO.GetTranslationById("elCo2") + " " + (int)KLHMath.ElectricityCO2EmssionFactor + " g/kWh \n" 
                + TranslatorSO.GetTranslationById("dhCo2") + " " + (int)KLHMath.DistrictHeatingCO2EmissionFactor + " g/kWh";
        }

        #endregion
        #region Graph Methods

        /// <summary>
        /// Setup the Graphs of every GraphArea.
        /// </summary>
        void SetupGraphs() 
        {     
            KLHManager.Building.UpdateHeatingDevices();

            ///SetupGraph(kwhGraphArea, GraphType.TotalKWH);
            SetupGraph(annualCO2GraphArea);
            SetupGraph(annualCostGraphArea);
            SetupGraph(twentyYearsCostGraphArea);  
        }

        /// <summary>
        /// Setup the Graphs of a single GraphArea. 
        /// </summary>
        /// <param name="_parentArea">GraphArea within which the Graphs are created or updated.</param>
        /// <param name="_type"></param>
        void SetupGraph (GraphArea _parentArea) 
        {
            GraphType gType = _parentArea.name switch {
                "Graph Area - Annual CO2" => GraphType.AnnualCO2,
                "Graph Area - Annual Cost" => GraphType.AnnualCost,
                "Graph Area - 20 Year Cost" => GraphType.TwentyYearCost,
                _ => GraphType.TotalKWH 
            };

            if (!_parentArea.gameObject.activeSelf) { return; }

            GraphDataset districtDataset = CreateDataset(_parentArea, gType, true);
            GraphDataset renewableDataset = CreateDataset(_parentArea, gType, false);

            _parentArea.DataCount = districtDataset.Data.Count;
            _parentArea.DataValueRange = new Vector2(0, FindMaxValue(new List<GraphDataset>{districtDataset, renewableDataset}));
        
            Graph districtGraph = GetGraph(gType, true);
            Graph renewableGraph = GetGraph(gType, false);

                        
            switch (gType) {

                default:
                case GraphType.TotalKWH:
                    if (kwhDistrictGraph == null)   { kwhDistrictGraph = CreateGraphComponent(_parentArea, districtDataset, true);}
                    else                            { kwhDistrictGraph.Dataset = districtDataset; }

                    if (kWhRenewableGraph == null) { kWhRenewableGraph = CreateGraphComponent(_parentArea, renewableDataset, false);}
                    else { kWhRenewableGraph.Dataset = renewableDataset; }
                    break;

                case GraphType.AnnualCO2:
                    if (anCO2DistrictGraph == null) { anCO2DistrictGraph = CreateGraphComponent(_parentArea, districtDataset, true);}
                    else { anCO2DistrictGraph.Dataset = districtDataset; }

                    if (anCO2RenewableGraph == null) { anCO2RenewableGraph = CreateGraphComponent(_parentArea, renewableDataset, false);}
                    else { anCO2RenewableGraph.Dataset = renewableDataset; }
                    break;

                case GraphType.AnnualCost:
                    if (anCostDistrictGraph == null) { anCostDistrictGraph = CreateGraphComponent(_parentArea, districtDataset, true);}
                    else { anCostDistrictGraph.Dataset = districtDataset; }

                    if (anCostRenewableGraph == null) { anCostRenewableGraph = CreateGraphComponent(_parentArea, renewableDataset, false);}
                    else { anCostRenewableGraph.Dataset = renewableDataset; }
                    break;
                
                case GraphType.TwentyYearCost:
                    if (twentyYearsDistrictGraph == null) { twentyYearsDistrictGraph = CreateGraphComponent(_parentArea, districtDataset, true);}
                    else { twentyYearsDistrictGraph.Dataset = districtDataset; }

                    if (twentyYearsRenewableGraph == null) { twentyYearsRenewableGraph = CreateGraphComponent(_parentArea, renewableDataset, false);}
                    else { twentyYearsRenewableGraph.Dataset = renewableDataset; }
                    break;      
            } 

            if  (_parentArea == twentyYearsCostGraphArea) {

                GraphDataset districtInvestmentDataset = CreateDataset(_parentArea, gType, true, true);
                GraphDataset renewableInvestmentDataset = CreateDataset(_parentArea, gType, false, true);

                _parentArea.DataValueRange = new Vector2(0, FindMaxValue(new List<GraphDataset>{districtDataset, renewableDataset, districtInvestmentDataset, renewableInvestmentDataset}));

                if (twentyYearsDHInvestmentCostsGraph == null) { twentyYearsDHInvestmentCostsGraph = CreateGraphComponent(_parentArea, districtInvestmentDataset, true, true);}
                    else { twentyYearsDHInvestmentCostsGraph.Dataset = districtInvestmentDataset; }

                if (twentyYearsREInvestmentCostsGraph == null) { twentyYearsREInvestmentCostsGraph = CreateGraphComponent(_parentArea, renewableInvestmentDataset, false, true);}
                    else { twentyYearsREInvestmentCostsGraph.Dataset = renewableInvestmentDataset; }

                if (null != twentyYearsDHInvestmentCostsGraph) { twentyYearsDHInvestmentCostsGraph.UpdateGraph(); }
                if (null != twentyYearsREInvestmentCostsGraph) { twentyYearsREInvestmentCostsGraph.UpdateGraph(); }
            }

            if (null != districtGraph)  { districtGraph.UpdateGraph(); }
            if (null != renewableGraph) { renewableGraph.UpdateGraph(); }       

            _parentArea.UpdateBackgroundGridVisuals();  
        }

        /// <summary>
        /// Create a GraphDataset for a Graph.
        /// </summary>
        /// <param name="_parentArea">GraphArea to which the Graph belongs.</param>
        /// <param name="_type">The type of the dataset.</param>
        /// <param name="_district">Whether the dataset is district or renewable.</param>
        /// <returns></returns>
        GraphDataset CreateDataset (GraphArea _parentArea, GraphType _type, bool _district, bool _isInvestmentOrMaintentance=false) 
        {
            List<DataPoint> newData = new List<DataPoint>();

            for (int i = 0; i < KLHManager.Building.HeatingMethods.Length; i++) {

                float value = 0;
                string name = _district ? DistrictNames[i] : RenewableNames[i];
                string extraInfo = "";

                HeatingMethod heatingMethod = KLHManager.Building.HeatingMethods[i];
                HeatingMethod districtHeatingMethod = KLHManager.Building.HeatingMethods[0];

                switch (_type) {

                    case GraphType.AnnualCO2:
                        value = _district
                        ? heatingMethod.AnnualDHCO2Amount
                        : heatingMethod.AnnualRECO2Amount;

                        extraInfo = "annualCO2";
                    break;

                    case GraphType.AnnualCost:
                        value = _district
                        ? heatingMethod.AnnualDHOperatingCost
                        : heatingMethod.AnnualREOperatingCost;
                        extraInfo = "annualCost";
                    break;

                    case GraphType.TwentyYearCost:

                            if (_district) {

                                if (_isInvestmentOrMaintentance)    { value = (includeDistrictDeviceCost ? districtHeatingMethod.InvestmentCost : 0) + districtHeatingMethod.CalculateLongtermMaintenanceCosts(20); }                         
                                else                                { value = heatingMethod.CalculateLongtermDistrictEnergyCost(20); }

                            } else if (i > 0) {

                                if (_isInvestmentOrMaintentance)    { value = heatingMethod.InvestmentCost + heatingMethod.CalculateLongtermMaintenanceCosts(20); } 
                                else                                { value = heatingMethod.CalculateLongtermRenewableEnergyCost(20); }
                            
                            } else {

                                value = 0;
                            }
                        
                        extraInfo = _isInvestmentOrMaintentance ? "device_Investment" : "device_Energy";

                    break;
                }
                        
                newData.Add(new DataPoint(name, value, i, extraInfo));
            }

            return new GraphDataset(newData, "");
        }

        /// <summary>
        /// Create a new Graph.
        /// </summary>
        /// <param name="_parentArea">GraphArea to which the Graph belongs.</param>
        /// <param name="_dataset">Dataset of the Graph.</param>
        /// <param name="_district">Whether the Graph is district or renewable.</param>
        /// <returns></returns>
        Graph CreateGraphComponent (GraphArea _parentArea, GraphDataset _dataset, bool _district, bool _isInvestmentOrMaintenance=false) 
        {
            GameObject graphObj = Instantiate(Resources.Load("Prefabs/UI elements/Bars and Graphs/New Graph") as GameObject);
            Graph newGraph = graphObj.GetComponent<Graph>();

            Color[] colors = _district 
            ? (_isInvestmentOrMaintenance ? districtInvestmentColors : districtColors)
            : (_isInvestmentOrMaintenance ? renewableInvestementColors : renewableColors);

            newGraph.InitializeGraph("Graph - ("+_parentArea.name+")",_parentArea, _dataset, colors); 
            
            _parentArea.AddGraph(newGraph);

            return newGraph;
        }
        
        /// <summary>
        /// Get an existing Graph with defined <param>_type</param> and <param>_district</param>.
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_district"></param>
        /// <returns></returns>
        Graph GetGraph(GraphType _type, bool _district, bool _isInvestmentOrMaintenance=false) 
        {
            switch (_type) {

                default:
                case GraphType.TotalKWH:
                    return _district ? kwhDistrictGraph : kWhRenewableGraph;

                case GraphType.AnnualCO2:
                    return _district ? anCO2DistrictGraph : anCO2RenewableGraph;

                case GraphType.AnnualCost:
                    return _district ? anCostDistrictGraph : anCostRenewableGraph;

                case GraphType.TwentyYearCost:
                    return _district 
                    ? (_isInvestmentOrMaintenance ? twentyYearsDHInvestmentCostsGraph : twentyYearsDistrictGraph) 
                    : (_isInvestmentOrMaintenance ? twentyYearsREInvestmentCostsGraph : twentyYearsRenewableGraph);
            }
        }



        /// <summary>
        /// Get the maximum float value. 
        /// Calculate datasets together by matching index and find the highest combined value.
        /// </summary>
        /// <param name="_datasets">List of datasets.</param>
        /// <returns></returns>
        float FindMaxValue (List<GraphDataset> _datasets) 
        {
            float top = 0f;

            for (int i = 0; i < _datasets[0].DataPointCount; i++) {

                float val = 0;

                for (int j = 0; j < _datasets.Count; j++) {

                    val += _datasets[j].Data[i].Value;
                }

                if (val > top) { top = val; }
            }

            return top;
        }

        /// <summary>
        /// Rounds the float and adds trailing zeros if needed.
        /// </summary>
        /// <param name="_costAsFloat"></param>
        /// <returns></returns>
        string FormatCostNumber(float _costAsFloat) 
        {
            string costToString = Math.Round(_costAsFloat, 2).ToString();

            if (costToString.Length == 1)       { costToString += ".00"; }
            else if (costToString.Length == 3)  { costToString += "0"; }
        
            return costToString;
        }

        /// <summary>
        /// Toggle heat recovery bar active or inactive (change color to grey and disable highlightable).
        /// </summary>
        /// <param name="_on">Whether to enable or disable heat recovery bar.</param>
        void ToggleHeatRecoveryBars ()
        {
            /// Set active/inactive boolean based on max floor count.
            bool on = !transform.parent.Find("Warning Panel - Building Floor Count").gameObject.activeSelf;//KLHManager.Building.MaximumLevel >= 3;
            
            /// Index (3) is the fourth column containing heat recovery data.  
            ToggleSingleBar(anCostDistrictGraph, 3, on, false);
            ToggleSingleBar(anCostRenewableGraph, 3, on, true);
            ToggleSingleBar(anCO2DistrictGraph, 3, on, false);
            ToggleSingleBar(anCO2RenewableGraph, 3, on, true);
            ToggleSingleBar(twentyYearsDistrictGraph, 3, on, false);
            ToggleSingleBar(twentyYearsRenewableGraph, 3, on, true);
            ToggleSingleBar(twentyYearsDHInvestmentCostsGraph, 3, on, false);
            ToggleSingleBar(twentyYearsREInvestmentCostsGraph, 3, on, true);
        }

        /// <summary>
        /// Toggle the visuals of a single bar active or inactive (change color to grey and disable highlightable).
        /// </summary>
        /// <param name="_graph">Graph containing the bar.</param>
        /// <param name="_index">Index of the bar within Data list of <paramref name="_graph"/>.</param>
        /// <param name="_on">Whether to set the bar as active or inactive.</param>
        void ToggleSingleBar (Graph _graph, int _index, bool _on, bool _isRenewable)
        {
            if (_graph.DataCount <= _index) { return; }

            Color col = _on ? (_isRenewable ? HybridDeviceColors[_index] : DistrictColors[_index]) : DisabledColor;
            Transform bar = _graph.transform.GetChild(0).GetChild(_index).GetChild(1);
            bar.GetComponent<MeshRenderer>().material.SetColor("_Color", col);
            bar.GetComponent<MeshRenderer>().material.SetColor("_EmissiveColor", col);
            bar.GetComponent<Highlightable>().IsInteractable = _on;
        }



        IEnumerator SetGraphInfoPanelActivity (bool _on)
        {
            Image img = graphInfoPanel.GetComponent<Image>();
            img.color = Color.clear;

            graphInfoPanel.gameObject.SetActive(_on);
            
            graphInfoPanel.GetComponent<ContentSizeFitter>().enabled = false;

            yield return new WaitForEndOfFrame();
            
            graphInfoPanel.GetComponent<ContentSizeFitter>().enabled = true;
            img.color = graphInfoPanelBackgroundColor;
        }

        #endregion
        #region Coroutines
        
        /// <summary>
        /// Make sure GraphAreas render correctly by double-toggling them on / off.
        /// </summary>
        /// <returns></returns>
        IEnumerator PreStartGraphAreas ()
        {
            annualCostGraphArea.gameObject.SetActive(true);
            annualCO2GraphArea.gameObject.SetActive(true);
            twentyYearsCostGraphArea.gameObject.SetActive(true);

            UpdateGraphAreas();

            yield return new WaitForEndOfFrame();

            annualCostGraphArea.gameObject.SetActive(false);
            annualCO2GraphArea.gameObject.SetActive(false);
            twentyYearsCostGraphArea.gameObject.SetActive(false);

            yield return new WaitForEndOfFrame();

            annualCostGraphArea.gameObject.SetActive(true);
            annualCO2GraphArea.gameObject.SetActive(true);
            twentyYearsCostGraphArea.gameObject.SetActive(true);

            UpdateGraphAreas();
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace