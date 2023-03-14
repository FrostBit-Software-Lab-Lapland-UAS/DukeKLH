/******************************************************************************
 * File        : MainMenuUI.cs
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
using UnityEngine.UI;
using TMPro;
using DUKE.Controls;
using DUKE.KLHData;

namespace DUKE.UI {


    /// <summary>
    /// Control and process user input within Main Menu.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        #region Variables

        #region General Variables

        [SerializeField] Transform mainPanel;
        [SerializeField] Transform consumptionPanel;
        [SerializeField] DUKE.Controls.InputField kwhInputField;
        [SerializeField] ZoneMap zoneMap;
        [SerializeField] AreaSelector floorCountSelector;
        [SerializeField] RectTransform eraButtonsArea;
        [SerializeField] RectTransform floorIndicatorLine;
        [SerializeField] TextMeshProUGUI floorCountNumber;

        #endregion
        #region Selected Parameter Variables

        [Space(20)]
        [Header("Building Estimation Values")]
        [SerializeField] int totalKWH;
        [SerializeField] int constructionYear;
        [SerializeField] BuildingZone selectedZone;
        [SerializeField] int floorCount = 1;

        #endregion
        
        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Set the total heat consumption. Called when InputField is updated.
        /// </summary>
        /// <param name="_total"></param>
        public void SetTotalKWH (int _total)
        {
            totalKWH = _total;
        }

        /// <summary>
        /// Set the BuildingZone of KLHManager's Building. Called when ZoneMap is updated.
        /// </summary>
        /// <param name="_zone">Specified BuildingZone.</param>
        public void SelectZone (BuildingZone _zone)
        {
            KLHManager.Buildings[0].Zone = selectedZone = _zone;
        }
        
        /// <summary>
        /// Set the BuildingZone of KLHManager's Building.
        /// </summary>
        /// <param name="_index">Specified BuildingZone's index.</param>
        public void SelectZone (int _index)
        {
            KLHManager.Buildings[0].Zone = selectedZone = (BuildingZone)_index;
        }

        /// <summary>
        /// Set the construction year of KLHManager's Building. 
        /// </summary>
        /// <param name="_year">Specified construction year.</param>
        public void SelectConstructionYear (int _year)
        {
            KLHManager.Buildings[0].ConstructionYear = constructionYear = _year;
        }

        /// <summary>
        /// Create an estimation of a Building's dimensions based on total heat consumption, 
        /// floor count, construction year and zone.
        /// </summary>
        public void EsimateBuilding ()
        {
            KLHManager.Building = KLHManager.Buildings[0];
            BuildingEstimator.UpdateBuildingGrid(KLHManager.Buildings[0], totalKWH, floorCount, constructionYear, selectedZone);
            FindObjectOfType<BuildingDrawer>(true).SetBuilding(KLHManager.Buildings[0]);
            //FindObjectOfType<BuildingVolumeObject>().UpdateVolumeModel();     
        }

        #endregion
        #region MonoBehaviour Methods

        private void OnEnable ()
        {
            KLHManager.MainMenuUI = this;

            consumptionPanel.gameObject.SetActive(false);
            kwhInputField.IntValueChanged += SetTotalKWH;
            zoneMap.ZoneSelected += SelectZone;
            floorCountSelector.OnAreaClicked += OnFloorCountSelectorClicked;
        }
        
        private void OnDisable ()
        {
            kwhInputField.IntValueChanged -= SetTotalKWH;
            zoneMap.ZoneSelected -= SelectZone;
            floorCountSelector.OnAreaClicked -= OnFloorCountSelectorClicked;
        }

        #endregion
        #region Main Panel Methods

        /// <summary>
        /// Called through <typeparamref name="Clickable's"/> <typeparamref name="OnClick"/>.
        /// Transition to KLH_MAIN with the selected <typeparamref name="Building"/>.
        /// </summary>
        /// <param name="_index">Defines which option was selected.</param>
        public void GoToDrawerWithBuilding (int _index)
        {
            _index = Mathf.Clamp(_index, 0, KLHManager.Buildings.Count - 1);

            KLHManager.Building = KLHManager.Buildings[_index];
            KLHManager.ToBuildingDrawer();
        }

        /// <summary>
        /// Show/hide HeatConsumptionPanel.
        /// </summary>
        public void ToggleHeatConsumptionPanel ()
        {
            bool val = !consumptionPanel.gameObject.activeSelf;
            consumptionPanel.gameObject.SetActive(val);
            mainPanel.gameObject.SetActive(!val);

            if (consumptionPanel.gameObject.activeSelf) {
                
                KLHManager.Building = KLHManager.Buildings[0];
                FindObjectOfType<ZoneMap>().SelectZone((int)KLHManager.Building.Zone);
                
                Clickable[] eraButtons = eraButtonsArea.GetComponentsInChildren<Clickable>();

                for (int i = 0; i < eraButtons.Length; i++) {

                    eraButtons[i].SetToggled(i == KLHManager.Building.ConstructionEra, true);
                }
            }
        }

        #endregion
        #region Heat Consumption Input Panel Methods

        /// <summary>
        /// Called when FloorCount's AreaSelector is updated.
        /// Only y-axis is read.
        /// </summary>
        /// <param name="_ratio">Ratio of coordinates within the area.</param>
        void OnFloorCountSelectorClicked (Vector2 _ratio)
        {
            RectTransform selectorRT = floorCountSelector.GetComponent<RectTransform>();
            float rawHeight = _ratio.y * selectorRT.rect.height - 50f;

            for (int i = 1; i < selectorRT.GetChild(0).childCount; i++) {

                Image img = selectorRT.GetChild(0).GetChild(i).GetComponent<Image>();
                img.color = new Color(
                    img.color.r,
                    img.color.g,
                    img.color.b,
                    (rawHeight > i * 100f ? 1f : 0.25f));
            }

            floorCount = Mathf.Clamp(Mathf.FloorToInt(rawHeight / 100f) + 1, 1, 9);
            floorCountNumber.text = floorCount.ToString();

            floorIndicatorLine.localPosition = new Vector3(
                floorIndicatorLine.localPosition.x,
                (floorCount * 100f) + 45f,
                floorIndicatorLine.localPosition.z);

        }

        #endregion
       
        #endregion


    } /// End of Class


} /// End of Namespace