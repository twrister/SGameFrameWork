using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class PathFindingMapData
    {
        public int EdgeLen { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public int GridCount { get { return MapWidth * MapHeight; } }

        int[] gridArray;
        public int[] showArray;

        public PathFindingMapData(int width, int height, int edge = 50)
        {
            MapWidth = width;
            MapHeight = height;
            EdgeLen = edge;
            gridArray = new int[width * height];
            ClearShowArray();
        }

        public void ClearShowArray()
        {
            showArray = new int[MapWidth * MapHeight];
        }

        public int this[int index]
        {
            get
            {
                return gridArray[index];
            }
            set
            {
                if (index < 0 || index >= MapHeight * MapWidth) return;
                gridArray[index] = value;
            }
        }

        public bool IsBlock(int index)
        {
            if (index < 0 || index >= GridCount) return false;
            return gridArray[index] == PathFindingGridView.BLOCK;
        }
    }
}