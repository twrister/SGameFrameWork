using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class AStarMapData
    {
        public int EdgeLen { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public int GridCount { get { return MapWidth * MapHeight; } }

        int[] blockArray;
        public int[] showArray;

        public AStarMapData(int width, int height, int edge = 50)
        {
            MapWidth = width;
            MapHeight = height;
            EdgeLen = edge;
            blockArray = new int[width * height];
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
                return blockArray[index];
            }
            set
            {
                if (index < 0 || index >= MapHeight * MapWidth) return;
                blockArray[index] = value;
            }
        }

        public bool IsBlock(int index)
        {
            if (index < 0 || index >= GridCount) return false;
            return blockArray[index] != 0;
        }
    }
}