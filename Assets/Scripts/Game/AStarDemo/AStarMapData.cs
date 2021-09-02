using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class AStarMapData
    {
        public const int NORMAL = 0;
        public const int BLOCK = 1;
        public const int REACHED = 2;
        public const int FRONTIER = 3;

        public int edgeLen { get; private set; }
        public int mapWidth { get; private set; }
        public int mapHeight { get; private set; }
        public int[] gridArray;

        public AStarMapData(int width, int height, int edge = 50)
        {
            mapWidth = width;
            mapHeight = height;
            edgeLen = edge;
            gridArray = new int[width * height];
        }

        public int this[int index]
        {
            get
            {
                return gridArray[index];
            }
            set
            {
                if (index < 0 || index >= mapHeight * mapWidth) return;
                gridArray[index] = value;
            }
        }
    }
}