using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class LayersMatrix
    {
        private List<LayerGrid> layers;
        public List<LayerGrid> Layers => layers;

        public LevelElement this[ElementPosition pos]
        {
            get => layers[pos.LayerId][pos];
            set => layers[pos.LayerId][pos] = value;
        }

        public int Count => layers.Count;

        public LayerGrid this[int layerId] => layers[layerId];

        public LayersMatrix(LevelData level, GameObject layersParent)
        {
            layers = new List<LayerGrid>(level.AmountOfLayers);

            for (int i = 0; i < level.AmountOfLayers; i++)
            {
                GameObject layerGameObject = new GameObject(string.Format("Layer {0}", i.ToString("00")));
                layerGameObject.transform.SetParent(layersParent.transform);
                layerGameObject.transform.ResetLocal();

                var size = (LevelController.Level.AmountOfLayers - i - 1) % 2 == 0 ? LevelController.EvenLayerSize : LevelController.OddLayerSize;
                LayerGrid layerRepresentation = new LayerGrid(layerGameObject, size.x, size.y);

                layers.Add(layerRepresentation);
            }
        }

        public void Clear()
        {
            foreach(LayerGrid layer in layers)
            {
                layer.Clear();
            }

            layers.Clear();
        }
    }
}