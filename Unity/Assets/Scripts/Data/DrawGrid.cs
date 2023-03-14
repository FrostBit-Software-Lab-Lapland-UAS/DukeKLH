/******************************************************************************
 * File        : DrawGrid.cs
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


namespace DUKE.KLHData {


    /// <summary>
    /// A 2D array of Cells with a defined width and depth.
    /// </summary>
    [System.Serializable]
    public class DrawGrid : ISerializationCallbackReceiver 
    {
        #region Variables

        [SerializeField] List<DrawCell> serializableCells;
        [SerializeField] DrawCell[,] cells;
        [SerializeField] int depth;
        [SerializeField] int width;

        #endregion


        #region Properties

        /// <summary>
        /// An array with each floor's <typeparamref name="DrawCell"/> count.
        /// </summary>
        public int[] LevelCellCount 
        { 
            get { return GetLevelCellCounts(); } 
        }
        
        /// <summary>
        /// TRUE if <typeparamref name="cells"/> has NULL <typeparamref name="DrawCells"/>. 
        /// </summary>
        public bool NullCells 
        { 
            get { return CellsAreNull(); } 
        }
        
        /// <summary>
        /// X-axis length of the <typeparamref name="cells"/> 2D array.
        /// </summary>
        public int Width 
        { 
            get { return width; } 
            set { width = value; } 
        }
        
        /// <summary>
        /// Y-axis length of the <typeparamref name="cells"/> 2D array.
        /// </summary>
        public int Depth 
        { 
            get { return depth; } 
            set { depth = value; } 
        }

        /// <summary>
        /// Public reference to <typeparamref name="cells"/>. 
        /// </summary>
        public DrawCell[,] Cells 
        { 
            get { return cells; } 
        }
       
        /// <summary>
        /// List of <typeparamref name="DrawCells"/> used for serializing the 2D array.
        /// </summary>
        public List<DrawCell> CellsList 
        { 
            get { return serializableCells; } 
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Empty constructor for <typeparamref name="DrawGrid"/>.
        /// </summary>
        public DrawGrid () { }

        /// <summary>
        /// Constructor for <typeparamref name="DrawGrid"/>.
        /// </summary>
        /// <param name="_width">Desired X-axis length.</param>
        /// <param name="_depth">Desired Y-axis length.</param>
        public DrawGrid (int _width, int _depth)
        {
            width = _width;
            depth = _depth;
            cells = new DrawCell[width, depth];

            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < depth; y++) 
                {
                    cells[x, y] = new DrawCell(x, y, 0);
                }
            }
        }

        /// <summary>
        /// Constructor for <typeparamref name="DrawGrid"/>.
        /// </summary>
        /// <param name="_otherGrid">Another <typeparamref name="DrawGrid"/> to be copied.</param>
        public DrawGrid (DrawGrid _otherGrid)
        {
            width = _otherGrid.Width;
            depth = _otherGrid.Depth;
            CopyCells(_otherGrid.Cells);
        }

        /// <summary>
        /// Constructor for <typeparamref name="DrawGrid"/>.
        /// </summary>
        /// <param name="_cells"><typeparamref name="DrawCell"/> 2D array to be copied.</param>
        public DrawGrid (DrawCell[,] _cells)
        {
            width = _cells.GetLength(0);
            depth = _cells.GetLength(1);
            CopyCells(_cells);
        }

        #endregion


        #region Methods

        #region Public Methods

        /// <summary>
        /// Copy cells from <paramref name="_otherCells"/> to this object's cells array.
        /// </summary>
        /// <param name="_otherCells">Another <typeparamref name="DrawCell"/> array from which the <typeparamref name="DrawCells"/> are copied.</param>
        protected void CopyCells (DrawCell[,] _otherCells)
        {
            if (width == 0 || depth == 0) 
            { 
                cells = new DrawCell[_otherCells.GetLength(0), _otherCells.GetLength(1)]; 
            }
            else
            { 
                cells = new DrawCell[width, depth]; 
            }

            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < depth; y++) 
                {
                    cells[x, y] = new DrawCell(x, y, _otherCells[x, y].Value);
                }
            }
        }

        /// <summary>
        /// Get a <typeparamref name="DrawCell"/> at coordinates <paramref name="_v2Pos"/>.
        /// </summary>
        /// <param name="_v2Pos">Two-dimensional position with which the <typeparamref name="DrawCell"/> is selected.</param>
        /// <returns>A <typeparamref name="DrawCell"/> at the specified coordinates.</returns>
        public DrawCell GetCell (Vector2Int _v2Pos)
        {
            int x = _v2Pos.x;
            int y = _v2Pos.y;

            return GetCell(x, y);
        }

        /// <summary>
        /// Get a <typeparamref name="DrawCell"/> at coordinates (<paramref name="_x"/>,<paramref name="_y"/>).
        /// </summary>
        /// <param name="_x">Grid coordinate point of the <typeparamref name="DrawCell"/> on X-axis.</param>
        /// <param name="_y">Grid coordinate point of the <typeparamref name="DrawCell"/> on Y-axis.</param>
        /// <returns>A <typeparamref name="DrawCell"/> at the specified coordinates.</returns>
        public DrawCell GetCell (int _x, int _y)
        {
            if (_x >= Width || _y >= Depth || _x < 0 || _y < 0) 
            { 
                return null; 
            } 
            else 
            { 
                return cells[_x, _y]; 
            }
        }

        /// <summary>
        /// Calculate the height difference (Value) of neighbouring cells.
        /// The order of sides is: [0] = +Z, [1] = +X, [2] = -Z, [3] = -X.
        /// Values answer the question "How much taller is THIS cell compared to it's neighbours?".
        /// For optimization purposes this method should only be provided cells with a Value higher than 0.
        /// </summary>
        /// <param name="_c">Origin <typeparamref name="DrawCell"/> from which the height differences of surrounding <typeparamref name="DrawCells"/> are calculated.</param>
        /// <param name="_includeDiagonals">TRUE if the returned array should contain diagonal connections as well.</param>
        /// <returns>Array of integers containing the height difference between <paramref name="_c"/> and its neighbouring <typeparamref name="DrawCell"/>s.</returns>
        public int[] CellNeighbourElevationDifference (DrawCell _c, bool _includeDiagonals = false)
        {
            DrawCell cN = GetCell(_c.X, _c.Y + 1);
            DrawCell cE = GetCell(_c.X + 1, _c.Y);
            DrawCell cS = GetCell(_c.X, _c.Y - 1);
            DrawCell cW = GetCell(_c.X - 1, _c.Y);

            int[] array;

            if (_includeDiagonals) 
            {
                DrawCell cNE = GetCell(_c.X + 1, _c.Y + 1);
                DrawCell cSE = GetCell(_c.X + 1, _c.Y - 1);
                DrawCell cSW = GetCell(_c.X - 1, _c.Y - 1);
                DrawCell cNW = GetCell(_c.X - 1, _c.Y + 1);

                array = new int[8] 
                {
                    (cN == null ? _c.Value : _c.Value - cN.Value),
                    (cE == null ? _c.Value : _c.Value - cE.Value),
                    (cS == null ? _c.Value : _c.Value - cS.Value),
                    (cW == null ? _c.Value : _c.Value - cW.Value),
                    (cNE == null ? _c.Value : _c.Value - cNE.Value),
                    (cSE == null ? _c.Value : _c.Value - cSE.Value),
                    (cSW == null ? _c.Value : _c.Value - cSW.Value),
                    (cNW == null ? _c.Value : _c.Value - cNW.Value)
                };
            } 
            else 
            {
                array = new int[4] 
                {
                    (cN == null ? _c.Value : _c.Value - cN.Value),
                    (cE == null ? _c.Value : _c.Value - cE.Value),
                    (cS == null ? _c.Value : _c.Value - cS.Value),
                    (cW == null ? _c.Value : _c.Value - cW.Value)
                };
            }

            return array;
        }

        /// <summary>
        /// Check whether the selected cell and another cell next to it are at different elevations.
        /// </summary>
        /// <param name="_c">The selected cell.</param>
        /// <param name="_dir">The direction from the selected cell to the other cell.</param>
        /// <returns>TRUE if <typeparamref name="DrawCell"/> <paramref name="_c"/> and its neighbouring <typeparamref name="DrawCell"/> at the direction specified with <paramref name="_dir"/> are at different elevations.</returns>
        public bool IsEdge (DrawCell _c, Vector2Int _dir)
        {
            if (null == _c) { return false; }

            int otherX = _c.X + _dir.x;
            int otherY = _c.Y + _dir.y;

            DrawCell otherCell = GetCell(otherX, otherY);

            if (null == otherCell) 
            {
                #if UNITY_EDITOR
                Debug.LogError("DrawGrid.IsEdge(): OtherCell is null. Returning true.");
                #endif

                return true;
            } 
            else 
            {
                return _c.Value != otherCell.Value;
            }
        }
        
        /// <summary>
        /// Get the count of cells of each height (0-9).
        /// </summary>
        /// <returns>The count of cells at levels between 0 and 9.</returns>
        protected int[] GetLevelCellCounts () 
        {
            int[] array = new int[10];

            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < depth; y++) 
                {
                    array[cells[x,y].Value]++;
                }
            }

            return array;
        }

        #endregion
        #region Dimension Calculations

        /// <summary>
        /// Calculate the gross area of KLHManager.Building in square metres.
        /// </summary>
        /// <returns>Gross area of <typeparamref name="KLHManager"/>.<typeparamref name="Building"/> in square metres.</returns>
        public float CalculateGrossArea ()
        {
            float total = 0;

            for (int x = 0; x < Width; x++) 
            {
                for (int y = 0; y < Depth; y++) 
                {
                    DrawCell c = GetCell(x, y);
                    if (c.Value > 0) { total += c.Value; }
                }
            }

            return total;
        }

        /// <summary>
        /// Calculate the horizontal surface area of KLHManager.Building in square metres.
        /// </summary>
        /// <returns>Horizontal surface area of <typeparamref name="KLHManager"/>.<typeparamref name="Building"/> (roof and ground-facing floor) in square metres.</returns>
        public float CalculateHorizontalArea ()
        {
            float total = 0;

            for (int x = 0; x < Width; x++) 
            {
                for (int y = 0; y < Depth; y++) 
                {
                    DrawCell c = GetCell(x, y);
                    if (c.Value > 0) { total++; }
                }
            }

            return total;
        }

        /// <summary>
        /// Calculate the vertical surface area of KLHManager.Building in square metres.
        /// </summary>
        /// <returns>Vertical surface area of <typeparamref name="KLHManager"/>.<typeparamref name="Building"/> (walls) in square metres.</returns>
        public float CalculateWallArea (float _storyHeight)
        {
            float total = 0f;

            for (int x = 0; x < Width; x++) 
            {
                for (int y = 0; y < Depth; y++) 
                {
                    DrawCell c = GetCell(x, y);
                    int[] neighbourDiff = CellNeighbourElevationDifference(c);

                    for (int i = 0; i < 4; i++) 
                    {
                        int valDiff = neighbourDiff[i];
                        if (valDiff > 0) { total += valDiff * _storyHeight; }
                    }
                }
            }

            return total;
        }

        /// <summary>
        /// Calculate the total surface area of KLHManager.Building in square metres.
        /// </summary>
        /// <param name="_storyHeight"></param>
        /// <returns>Total surface area of <typeparamref name="KLHManager"/>.<typeparamref name="Building"/> in square metres.</returns>
        public float CalculateEnvelopeArea (float _storyHeight)
        {
            return CalculateWallArea(_storyHeight) + 2 * CalculateHorizontalArea();
        }

        /// <summary>
        /// Calculate the total volume of KLHManager.Building in cubic metres.
        /// </summary>
        /// <param name="_storyHeight"></param>
        /// <returns>Total volume of <typeparamref name="KLHManager"/>.<typeparamref name="Building"/> in cubic metres.</returns>
        public float CalculateVolume (float _storyHeight)
        {
            float total = 0f;

            for (int x = 0; x < Width; x++) 
            {
                for (int y = 0; y < Depth; y++) 
                {
                    DrawCell c = GetCell(x, y);
                    total += c.Value * _storyHeight;
                }
            }

            return total;
        }

        #endregion
        #region Overrides and Interfaces

        public void OnBeforeSerialize ()
        {
            if (null != cells && null == serializableCells) 
            {
                Serialize2DArrayToList();
            }
        }

        public void OnAfterDeserialize ()
        {
            if (null != serializableCells && null == cells) 
            {
                SerializeListTo2DArray();
            }
        }

        #endregion
        #region Serialization

        /// <summary>
        /// Serialize the current DrawGrid as a list.
        /// </summary>
        public void SaveGrid ()
        {
            Serialize2DArrayToList();
        }

        /// <summary>
        /// Convert the serialized list to a grid.
        /// </summary>
        protected void SerializeListTo2DArray ()
        {
            cells = new DrawCell[Width, Depth];

            for (int i = 0; i < serializableCells.Count; i++) 
            {
                DrawCell c = serializableCells[i];

                if (c.X < Width && c.Y < Depth) 
                {
                    cells[c.X, c.Y] = c;
                }
            }
        }

        /// <summary>
        /// Convert the grid to a list for serialization.
        /// </summary>
        protected void Serialize2DArrayToList ()
        {        
            serializableCells = new List<DrawCell>();

            for (int x = 0; x < Width; x++) 
            {
                for (int y = 0; y < Depth; y++) 
                {
                    DrawCell c = cells[x, y];
                    serializableCells.Add(c);
                }
            }
        }

        /// <summary>
        /// Check if the <typeparamref name="DrawGrid"/>'s grid contains NULL <typeparamref name="DrawCell"/>s.
        /// </summary>
        /// <returns>TRUE if a NULL <typeparamref name="DrawCell"/> is found in the grid.</returns>
        protected bool CellsAreNull ()
        {
            if (null == cells)
            { 
                return true; 
            } 
            else if (cells.Length == 0) 
            { 
                return true; 
            } 
            else 
            {
                for (int x = 0; x < width; x++) 
                {
                    for (int y = 0; y < depth; y++) 
                    {
                        if (null == cells[x, y]) { return true; }
                    }
                }
            }

            return false;
        }

        #endregion

        #endregion


    } /// End of Class


} /// End of Namespace