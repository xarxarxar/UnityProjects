using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Watermelon
{
    public class LevelRepresentation
    {
        private LayersMatrix layers;
        public LayersMatrix Layers => layers;

        private List<TileBehavior> tiles;
        public List<TileBehavior> Tiles => tiles;

        private LevelData level;

        public LevelRepresentation(LevelData level, GameObject layersParent)
        {
            this.level = level;

            tiles = new List<TileBehavior>();
            layers = new LayersMatrix(level, layersParent);
        }


        /// <param name="availableTilesData">unique tile datas in current level</param>
        /// <returns>Complete list of tile datas of current level, evenly distributed</returns>
        private TileData[] PrepareInitialTiles(TileData[] availableTilesData)
        {
            // Helps keep track of the amount of already included tiles
            Dictionary<TileData, int> objectsInLevelAmount = new Dictionary<TileData, int>();

            var initialTilesData = new List<TileData>();

            int tilesDataLeft = level.GetAmountOfFilledCells();

            // The current maximum amount of any specific tile inside initialTilesData
            int maxAmount = 1;

            while (tilesDataLeft > 0)
            {
                TileData tileData = availableTilesData.GetRandomItem();

                // Sellecting the most appropriate tile data
                if (objectsInLevelAmount.ContainsKey(tileData))
                {
                    // This tile data have already been added to the list. Trying to add data that isn't the one with max amount of already added
                    for (int i = 0; i < availableTilesData.Length; i++)
                    {
                        TileData testTileData = availableTilesData[i];
                        if (testTileData != tileData)
                        {
                            if (objectsInLevelAmount.ContainsKey(testTileData))
                            {
                                if (objectsInLevelAmount[testTileData] < maxAmount)
                                {
                                    tileData = testTileData;
                                }
                            }
                            else
                            {
                                tileData = testTileData;
                                objectsInLevelAmount.Add(tileData, 1);
                            }

                        }
                    }

                    int amount = objectsInLevelAmount[tileData];
                    amount++;

                    if (maxAmount < amount)
                        maxAmount = amount;
                    objectsInLevelAmount[tileData] = amount;
                }
                else
                {
                    // This is the first time we're adding this tile data to the list
                    objectsInLevelAmount.Add(tileData, 1);
                    if (maxAmount == 0)
                        maxAmount = 1;
                }

                initialTilesData.Add(tileData);

                tilesDataLeft -= 3;
            }

            return initialTilesData.OrderBy(x => UnityEngine.Random.value).ToArray();
        }

        public void RelinkTiles()
        {
            foreach (LayerGrid layer in layers.Layers)
            {
                int width = layer.Width;
                int height = layer.Height;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        layer[x, y].LinkTile(null);
                    }
                }
            }

            foreach (TileBehavior tile in tiles)
            {
                ElementPosition elementPosition = tile.ElementPosition;

                layers.Layers[elementPosition.LayerId][elementPosition].LinkTile(tile);
            }

            UpdateStates(true);
        }

        public void UpdateStates(bool withAnimation = false)
        {
            foreach(TileBehavior tile in tiles)
            {
                tile.SetState(IsTileUnconcealed(tile), withAnimation);
            }
        }

        public void RemoveObject(TileBehavior tile)
        {
            tiles.Remove(tile);

            layers[tile.ElementPosition] = null;
        }

        public void AddObject(TileBehavior tile)
        {
            tiles.Add(tile);

            layers[tile.ElementPosition] = tile;
        }

        private TileBehavior SpawnTile(TileData tileData, ElementPosition elementPosition)
        {
            TileBehavior tile = tileData.Pool.GetPooledObject().GetComponent<TileBehavior>();
            tile.Initialise(tileData, elementPosition);
            tile.transform.SetParent(layers[elementPosition.LayerId].LayerObject.transform);
            tile.transform.localPosition = LevelScaler.GetPosition(tile.ElementPosition);
            tile.transform.localScale = Vector3.one;
            tile.SetScale(LevelScaler.TileSize);

            layers[tile.ElementPosition] = tile;

            // Figuring out is object is Active
            tile.SetState(IsTileUnconcealed(tile), false);

            // Add tile to global tiles list
            tiles.Add(tile);

            return tile;
        }

        public void SpawnObjects(TileData[] availableObjects)
        {
            TileData[] initialTilesData = PrepareInitialTiles(availableObjects);
            List<EffectData> effectDatas = new List<EffectData>();

            List<TileSpawnData> emptyTiles = new List<TileSpawnData>();
            for (int i = 0; i < level.AmountOfLayers; i++)
            {
                Layer layer = level.GetLayer(i);

                bool isEven = (level.AmountOfLayers - i - 1) % 2 == 0;

                Vector2Int size = isEven ? LevelController.EvenLayerSize : LevelController.OddLayerSize;

                for (int y = size.y - 1; y >= 0; y--)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        CellData cellData = layer[y].GetCell(x);

                        if (cellData.IsFilled)
                        {
                            TileSpawnData tileSpawnData = new TileSpawnData();
                            tileSpawnData.ElementPosition = new ElementPosition(x, y, i);
                            tileSpawnData.CellData = cellData;

                            tileSpawnData.LayerIndex = i;
                            tileSpawnData.Layer = layer;
                            tileSpawnData.LayerSize = size;

                            emptyTiles.Add(tileSpawnData);
                        }
                    }
                }
            }

            for (int i = 0; i < initialTilesData.Length; i++)
            {
                // Get random tile from the lower layer
                TileSpawnData firstTileSpawnData = emptyTiles.OrderBy(x => UnityEngine.Random.value).OrderBy(x => x.LayerIndex).FirstOrDefault();

                // Remove first tile from the list
                emptyTiles.Remove(firstTileSpawnData);

                // Spawn first tile
                TileBehavior firstElementBehavior = SpawnTile(initialTilesData[i], firstTileSpawnData.ElementPosition);

                // Activate effect
                if (firstTileSpawnData.CellData.Effect != TileEffectType.None)
                {
                    effectDatas.Add(new EffectData(firstElementBehavior, firstTileSpawnData.CellData.Effect));
                }

                float totalWeight = 0;

                // Recalculate empty tiles weights
                foreach (TileSpawnData emptyTile in emptyTiles)
                {
                    emptyTile.RecalculateWeight(firstTileSpawnData.LayerIndex);

                    totalWeight += emptyTile.RandomWeight;
                }

                // Spawn additional elements
                for(int a = 0; a < 2; a++)
                {
                    float randomValue = UnityEngine.Random.Range(0, totalWeight);
                    float currentWeight = 0;

                    TileSpawnData selectedTileData = null;
                    foreach (TileSpawnData emptyTile in emptyTiles)
                    {
                        currentWeight += emptyTile.RandomWeight;

                        if (currentWeight >= randomValue)
                        {
                            selectedTileData = emptyTile;

                            break;
                        }
                    }

                    if(selectedTileData != null)
                    {
                        // Remove additional tile from the list
                        emptyTiles.Remove(selectedTileData);

                        // Decreese total weight
                        totalWeight -= selectedTileData.RandomWeight;

                        // Spawn additional tile
                        TileBehavior additionalElementBehavior = SpawnTile(initialTilesData[i], selectedTileData.ElementPosition);

                        // Activate effect
                        if (selectedTileData.CellData.Effect != TileEffectType.None)
                        {
                            effectDatas.Add(new EffectData(additionalElementBehavior, selectedTileData.CellData.Effect));
                        }
                    }
                }
            }

            // Apply effects
            foreach(EffectData effectData in effectDatas)
            {
                TileEffect effect = LevelController.GetTileEffect(effectData.EffectType);
                if (effect != null)
                {
                    effect.ApplyEffect(effectData.TileBehavior);
                }
            }
        }

        public List<TileBehavior> GetSortedTiles()
        {
            List<TileBehavior> sortedTiles = new List<TileBehavior>();
            for (int i = level.AmountOfLayers - 1; i >= 0; i--)
            {
                Layer layer = level.GetLayer(i);

                bool isEven = (level.AmountOfLayers - i - 1) % 2 == 0;

                var size = isEven ? LevelController.EvenLayerSize : LevelController.OddLayerSize;

                for (int y = size.y - 1; y >= 0; y--)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        CellData cellData = layer[y].GetCell(x);

                        if (cellData.IsFilled)
                        {
                            sortedTiles.Add(layers[i][x, y]);
                        }
                    }
                }
            }

            return sortedTiles;
        }

        public void SpawnObjects(PreloadedLevelData preloadedLevelData)
        {
            preloadedLevelData.Initialise();

            List<EffectData> effectDatas = new List<EffectData>();
            PreloadedLevelData.Tile[] preloadTiles = preloadedLevelData.Tiles;
            foreach(PreloadedLevelData.Tile tile in preloadTiles)
            {
                TileData tileData = tile.TileData;
                ElementPosition elementPosition = tile.ElementPosition;

                TileBehavior tileBehavior = tileData.Pool.GetPooledObject().GetComponent<TileBehavior>();
                tileBehavior.Initialise(tileData, elementPosition);
                tileBehavior.transform.SetParent(layers[elementPosition.LayerId].LayerObject.transform);
                tileBehavior.transform.localPosition = LevelScaler.GetPosition(tile.ElementPosition);
                tileBehavior.transform.localScale = Vector3.one;
                tileBehavior.SetScale(LevelScaler.TileSize);

                layers[tile.ElementPosition] = tileBehavior;

                // Figuring out is object is Active
                tileBehavior.SetState(IsTileUnconcealed(tileBehavior), false);

                // Activate effect
                if (tile.EffectType != TileEffectType.None)
                {
                    effectDatas.Add(new EffectData(tileBehavior, tile.EffectType));
                }

                tiles.Add(tileBehavior);
            }

            // Apply effects
            foreach (EffectData effectData in effectDatas)
            {
                TileEffect effect = LevelController.GetTileEffect(effectData.EffectType);
                if (effect != null)
                {
                    effect.ApplyEffect(effectData.TileBehavior);
                }
            }
        }

        public bool IsThereAnyTilesOnLayer(int layerId)
        {
            var size = ((level.AmountOfLayers - layerId - 1) % 2) == 0 ? LevelController.EvenLayerSize : LevelController.OddLayerSize;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (layers[new ElementPosition(x, y, layerId)].State)
                        return true;
                }
            }

            return false;
        }


        public bool IsTileUnconcealed(ElementPosition tilePos)
        {
            if (tilePos.LayerId == 0)
                return true;
            var layerIdFromBottom = LevelController.Level.AmountOfLayers - tilePos.LayerId - 1;
            bool isEven = layerIdFromBottom % 2 == 0;

            for (int i = tilePos.LayerId - 1; i >= 0; i--)
            { 
                var thislayerIdFromBottom = LevelController.Level.AmountOfLayers - i - 1;

                bool isLayerEven = thislayerIdFromBottom % 2 == 0;

                var position = new ElementPosition(tilePos, i);

                if (isEven == isLayerEven)
                {
                    // if there is something directly above object, it is not available

                    if (layers[position].State)
                        return false;
                }
                else
                {
                    var size = isLayerEven ? LevelController.EvenLayerSize : LevelController.OddLayerSize;

                    bool sizeIsBigger;
                    if (isLayerEven)
                    {
                        sizeIsBigger = LevelController.IsEvenLayerBigger;
                    }
                    else
                    {
                        sizeIsBigger = !LevelController.IsEvenLayerBigger;
                    }

                    position = new ElementPosition(sizeIsBigger ? position + 1 : position - 1, i);

                    // Checking if there is something partly above the object. If there is - it is not available
                    // Should check 4 times because every odd lever is bigger and shifted a little bit

                    if (position.X != -1 && position.Y != -1 && position.X != size.x && position.Y != size.y && layers[position].State)
                        return false;

                    if (sizeIsBigger)
                    {
                        var leftNeighbourPos = position.LeftNeighbourPos;
                        if (leftNeighbourPos.X != -1 && leftNeighbourPos.Y != size.y && layers[leftNeighbourPos].State)
                            return false;

                        var topNeighbourPos = position.UpNeighbourPos;
                        if (topNeighbourPos.X != size.x && topNeighbourPos.Y != -1 && layers[topNeighbourPos].State)
                            return false;

                        var topLeftNeighbourPos = topNeighbourPos.LeftNeighbourPos;
                        if (topLeftNeighbourPos.X != -1 && topLeftNeighbourPos.Y != -1 && layers[topLeftNeighbourPos].State)
                            return false;
                    }
                    else
                    {
                        var rightNeighbourPos = position.RightNeighbourPos;
                        if (rightNeighbourPos.X != size.x && rightNeighbourPos.Y != -1 && layers[rightNeighbourPos].State)
                            return false;

                        var bottomNeighbourPos = position.BottomNeighbourPos;

                        if (bottomNeighbourPos.X != -1 && bottomNeighbourPos.Y != size.y && layers[bottomNeighbourPos].State)
                            return false;

                        var bottomRightNeighbourPos = bottomNeighbourPos.RightNeighbourPos;
                        if (bottomRightNeighbourPos.X != size.x && bottomRightNeighbourPos.Y != size.y && layers[bottomRightNeighbourPos].State)
                            return false;
                    }
                }
            }

            return true;
        }

        public bool IsTileExists(ElementPosition elementPosition)
        {
            int layerId = elementPosition.LayerId;
            int width = layers[layerId].Width;
            int height = layers[layerId].Height;

            if (elementPosition.X >= 0 && elementPosition.X < width && elementPosition.Y >= 0 && elementPosition.Y < height)
            {
                return layers[elementPosition].State;
            }

            return false;
        }

        public void Clear()
        {
            for(int i = 0; i < tiles.Count; i++)
            {
                tiles[i].Clear();
            }

            layers.Clear();
        }

        private class EffectData
        {
            public TileBehavior TileBehavior;
            public TileEffectType EffectType;

            public EffectData(TileBehavior tileBehavior, TileEffectType effectType)
            {
                TileBehavior = tileBehavior;
                EffectType = effectType;
            }
        }

        private class TileSpawnData
        {
            public ElementPosition ElementPosition;
            public CellData CellData;

            public int LayerIndex;
            public Layer Layer;
            public Vector2Int LayerSize;

            public float RandomWeight;

            public void RecalculateWeight(int baseLayerIndex)
            {
                int layerDiff = LayerIndex - baseLayerIndex;

                RandomWeight = Mathf.Clamp(3 - layerDiff, 0, int.MaxValue);
            }
        }
    }
}