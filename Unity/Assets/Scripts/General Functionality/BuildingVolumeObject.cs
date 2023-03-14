/******************************************************************************
 * File        : BuildingVolumeObject.cs
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
using TMPro;


namespace DUKE {


    public class BuildingVolumeObject : MonoBehaviour
    {
        #region Variables

        [SerializeField] float maximumHeight;


        [Space(20)]
        [SerializeField] Transform disc;
        [SerializeField] Transform meshContainer;
        [SerializeField] Transform outlineContainer;
        [SerializeField] MeshFilter[] levelObjects;
        [SerializeField] MeshFilter[] outlineObjects;
        [SerializeField] Material meshMaterial;
        [SerializeField] Material outlineMaterial;
        [SerializeField] Color outlineColor;
        [SerializeField] Color meshColor;
        [SerializeField] TextMeshProUGUI totalKWHTextObj;
        [SerializeField] TextMeshProUGUI grossAreaTextObj;
        [SerializeField] TextMeshProUGUI grossVolumeTextObj;
        [SerializeField] TextMeshProUGUI yearAndZoneTextObj;

        #endregion


        #region Methods

        void Awake ()
        {
            meshMaterial = new Material(meshMaterial);
            meshMaterial.SetColor("_EmissiveColor", meshColor);

            outlineMaterial = new Material(outlineMaterial);
            outlineMaterial.color = outlineColor;
        }

        void OnEnable ()
        {
            KLHManager.VolumeObject = this;
            BuildingDrawer.BuildingMeshesUpdated += UpdateVolumeModel;

            UpdateVolumeModel();
            UpdateTextObjects();
        }

        void OnDisable ()
        {
            BuildingDrawer.BuildingMeshesUpdated -= UpdateVolumeModel;
        }

        /// <summary>
        /// Copy the <typeparamref name="Meshes"/> from <typeparamref name="BuildingDrawer"/>. Rescale the model to fit the disc.
        /// </summary>
        public void UpdateVolumeModel ()
        {
            float hrzScale = disc.localScale.x / (BuildingDrawer.Bounds.maxCornerDistance + 0.2f);
            float topFloor = 0;

            meshContainer.localScale = Vector3.one * hrzScale;

            for (int i = 0; i < levelObjects.Length; i++) {
                
                Mesh lvlMesh = MeshUtility.CopyMesh(BuildingDrawer.LevelMeshes[i]);    
                MeshUtility.OffsetMeshOrigin(lvlMesh, BuildingDrawer.Bounds.originPoint);
                lvlMesh.RecalculateBounds();
                lvlMesh.Optimize();
        
                levelObjects[i].mesh = lvlMesh;
                levelObjects[i].GetComponent<MeshRenderer>().material = meshMaterial;
                
                Mesh outMesh = MeshUtility.CopyMesh(BuildingDrawer.OutlineMeshes[i]);
                MeshUtility.OffsetMeshOrigin(outMesh, BuildingDrawer.Bounds.originPoint);
                outMesh.RecalculateBounds();
                outMesh.Optimize();

                outlineObjects[i].mesh = outMesh;
                outlineObjects[i].GetComponent<MeshRenderer>().material = outlineMaterial;
                
                if (lvlMesh.vertexCount > 0 ) { topFloor = i + 1; }
            }

            float limit = 10f / topFloor;

            if (limit < meshContainer.localScale.x) {

                meshContainer.localScale = Vector3.one * limit;
            }    

            outlineContainer.localScale = meshContainer.localScale;
        }

        /// <summary>
        /// Update the text objects circling the <typeparamref name="Mesh"/>.
        /// </summary>
        void UpdateTextObjects ()
        {
            totalKWHTextObj.text = KLHManager.FormatIntToString(KLHManager.Building.TotalHeatConsumption)+" kWh";
            grossAreaTextObj.text = KLHManager.FormatIntToString(KLHManager.Building.GrossArea)+" m2";
            grossVolumeTextObj.text = KLHManager.FormatIntToString(KLHManager.Building.GrossVolume)+" m3";
            yearAndZoneTextObj.text = KLHManager.Building.ConstructionYear.ToString() + "  |  Zone " + KLHManager.Building.Zone.ToString();
        }

        #endregion


    } /// End of Class


} /// End of Namespace