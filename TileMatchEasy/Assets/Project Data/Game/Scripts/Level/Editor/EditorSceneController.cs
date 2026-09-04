#pragma warning disable 649

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    public class EditorSceneController
    {
        private static readonly int OVERLAY_STRENGTH_ID = Shader.PropertyToID("_OverlayStrength");
        public GameObject[] prefabs;

        public Vector2Int EvenLayerSize;
        public Vector2Int OddLayerSize;

        private Dictionary<Vector3Int, GameObject> matchables; //z - layer index
        private List<GameObject> layerGameobjects;
        private GameObject tempGameObject;
        private Material enabledMaterial;
        private Material disabledMaterial;
        private List<int> enabledPrefabIndexes;
        private List<int> disabledPrefabIndexes;
        public int setsAmount;

        private GameObject parentGameObject;

        public EditorSceneController(GameObject parentGameObject)
        {
            this.parentGameObject = parentGameObject;

            matchables = new Dictionary<Vector3Int, GameObject>();
            layerGameobjects = new List<GameObject>();
            enabledPrefabIndexes = new List<int>();
            disabledPrefabIndexes = new List<int>();
        }

        public Vector3 GetPosition(ElementPosition elementPosition, int arraySize)
        {
            if ((arraySize + 1 - elementPosition.LayerId) % 2 == 0)
            {
                return new Vector3(-EvenLayerSize.x / 2f + elementPosition.X + 0.5f, -EvenLayerSize.y / 2f + 0.5f + elementPosition.Y, elementPosition.LayerId);
            }
            else
            {
                return new Vector3(-OddLayerSize.x / 2f + elementPosition.X + 0.5f, -OddLayerSize.y / 2f + 0.5f + elementPosition.Y, elementPosition.LayerId);
            }
        }


        public void Clear()
        {
            for (int i = parentGameObject.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(parentGameObject.transform.GetChild(i).gameObject);
            }

            matchables.Clear();
            layerGameobjects.Clear();
            enabledPrefabIndexes.Clear();
            disabledPrefabIndexes.Clear();
        }

        public void SetState(GameObject layer, bool state)
        {
            if (!layer.activeSelf)
            {
                layer.SetActive(true);
            }

            var tintColor = (Color.white * (state ? 1f : 0.7f)).SetAlpha(1f);
            SpriteRenderer[] spriteRenderers = layer.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = tintColor;
            }
        }

        public void UpdateCells(int layer, int xIndex, int yIndex, bool value)
        {
            if(matchables.TryGetValue(new Vector3Int(xIndex,yIndex,layer), out tempGameObject))
            {
                if(tempGameObject.activeSelf == value)
                {
                    return;
                }
                else
                {
                    tempGameObject.SetActive(value);
                }
            }
            else
            {
                Debug.LogWarning("Object not found");
            }
        }

        public void InitCells(int layer, int xIndex, int yIndex, bool value, int arraySize)
        {
            if(layer == layerGameobjects.Count)
            {
                GameObject newLayer = new GameObject();
                newLayer.transform.SetParent(parentGameObject.transform);
                layerGameobjects.Add(newLayer);
            }

            GameObject newPrefab = null;

            if (value)
            {
                if(enabledPrefabIndexes.Count == 0)
                {
                    setsAmount -= prefabs.Length;

                    if((setsAmount > 0) && (setsAmount < prefabs.Length))//this whole thing makes it that our preview level is solvable
                    {
                        for (int i = 0; i < setsAmount; i++)
                        {
                            enabledPrefabIndexes.Add(i);
                            enabledPrefabIndexes.Add(i);
                            enabledPrefabIndexes.Add(i);
                        }
                    }
                    else 
                    {
                        setsAmount -= prefabs.Length;

                        for (int i = 0; i < prefabs.Length; i++)
                        {
                            enabledPrefabIndexes.Add(i);
                            enabledPrefabIndexes.Add(i);
                            enabledPrefabIndexes.Add(i);
                        }
                    }
                }

                int randomIndex = UnityEngine.Random.Range(0, enabledPrefabIndexes.Count);
                newPrefab = UnityEngine.Object.Instantiate(prefabs[enabledPrefabIndexes[randomIndex]], layerGameobjects[layer].transform) as GameObject;
                enabledPrefabIndexes.RemoveAt(randomIndex);
            }
            else
            {
                if (disabledPrefabIndexes.Count == 0)
                {
                    for (int i = 0; i < prefabs.Length; i++)
                    {
                        disabledPrefabIndexes.Add(i);
                        disabledPrefabIndexes.Add(i);
                        disabledPrefabIndexes.Add(i);
                    }
                }

                int randomIndex = UnityEngine.Random.Range(0, disabledPrefabIndexes.Count);
                newPrefab = UnityEngine.Object.Instantiate(prefabs[disabledPrefabIndexes[randomIndex]], layerGameobjects[layer].transform) as GameObject;
                disabledPrefabIndexes.RemoveAt(randomIndex);
            }

            if (!value)
            {
                newPrefab.SetActive(false);
            }

            newPrefab.transform.position = GetPosition(new ElementPosition(xIndex, yIndex, layer), arraySize);
            SetSortingOrder(newPrefab, -layer * 100 + xIndex - yIndex);

            matchables.Add(new Vector3Int(xIndex, yIndex, layer), newPrefab);
        }

        public void SetSortingOrder(GameObject prefab, int order)
        {
            SpriteRenderer[] spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();

            if (spriteRenderers.Length != 2)
            {
                Debug.LogError("Prefab changed so we need to change this method.");
            }

            SpriteRenderer iconSpriteRenderer;
            SpriteRenderer backgroundSpriteRenderer;

            if (spriteRenderers[0].gameObject.name.Equals("Icon"))
            {
                iconSpriteRenderer = spriteRenderers[0];
                backgroundSpriteRenderer = spriteRenderers[1];
            }
            else
            {
                iconSpriteRenderer = spriteRenderers[1];
                backgroundSpriteRenderer = spriteRenderers[0];
            }

            iconSpriteRenderer.sortingOrder = order * 3 - 1;
            backgroundSpriteRenderer.sortingOrder = order * 3 - 2;
        }

        public void UpdateSelectedLayerIndex(int selectedLayerIndex)
        {
            for (int i = 0; i < layerGameobjects.Count; i++)
            {
                if (i == selectedLayerIndex)
                {
                    SetState(layerGameobjects[i], true);
                }
                else if (i > selectedLayerIndex)
                {
                    SetState(layerGameobjects[i], false);
                }
                else
                {
                    layerGameobjects[i].SetActive(false);
                }
            }
        }

        public void InitMaterials(GameObject prefab)
        {
            enabledMaterial = new Material(prefab.GetComponentInChildren<SpriteRenderer>().sharedMaterial);
            disabledMaterial = new Material(prefab.GetComponentInChildren<SpriteRenderer>().sharedMaterial);
            enabledMaterial.SetFloat(OVERLAY_STRENGTH_ID, 0);
            disabledMaterial.SetFloat(OVERLAY_STRENGTH_ID, 0.3f);
        }

        public void Destroy()
        {
            UnityEngine.Object.DestroyImmediate(parentGameObject);

            UnityEngine.Object.DestroyImmediate(enabledMaterial);
            UnityEngine.Object.DestroyImmediate(disabledMaterial);
        }
    }
}
