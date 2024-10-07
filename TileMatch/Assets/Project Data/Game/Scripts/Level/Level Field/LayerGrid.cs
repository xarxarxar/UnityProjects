using UnityEngine;

namespace Watermelon
{
    public class LayerGrid
    {
        private LevelElement[,] grid;

        public int Width => grid.GetLength(0);
        public int Height => grid.GetLength(1);

        public LevelElement this[ElementPosition pos]
        {
            get => grid[pos.X, pos.Y];
            set => grid[pos.X, pos.Y] = value;
        }

        public LevelElement this[int x, int y]
        {
            get => grid[x, y];
        }

        private GameObject layerObject;
        public GameObject LayerObject => layerObject;

        public LayerGrid(GameObject layerObject, int width, int height)
        {
            this.layerObject = layerObject;
            grid = new LevelElement[width, height];
        }

        public void Clear()
        {
            Object.Destroy(layerObject);
        }
    }
}