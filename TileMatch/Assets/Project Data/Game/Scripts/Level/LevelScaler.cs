using UnityEngine;

namespace Watermelon
{
    public class LevelScaler: MonoBehaviour
    {
        [SerializeField] Offset levelFieldOffset;

        public Vector2 LevelFieldCenter { get; private set; }
        public Vector2 LevelFieldSize { get; private set; }

        public static Vector2 TileSize { get; private set; }
        public static Vector2 OddLevelSize { get; private set; }
        public static Vector2 EvenLevelSize { get; private set; }

        public static Vector2 SlotSize { get; private set; }

        private void Awake()
        {
            float cameraHeight = Camera.main.orthographicSize * 2;
            float cameraWidth = cameraHeight * Camera.main.aspect;

            float leftBorder = (levelFieldOffset.left - 0.5f) * cameraWidth;
            float rightBorder = (0.5f - levelFieldOffset.right) * cameraWidth;

            float bottomBorder = (levelFieldOffset.bottom - 0.5f) * cameraHeight;
            float topBorder = (0.5f - levelFieldOffset.top) * cameraHeight;

            LevelFieldCenter = new Vector2((leftBorder + rightBorder) / 2, (bottomBorder + topBorder) / 2);
            LevelFieldSize = new Vector2(rightBorder - leftBorder, topBorder - bottomBorder);

            var dataTileSize = GameController.Data.TileSize;

            SlotSize = new Vector2(LevelFieldSize.x / 8, LevelFieldSize.x / 8);

            if (dataTileSize != Vector2.one)
            {
                if (dataTileSize.x > dataTileSize.y)
                {
                    SlotSize = new Vector2(SlotSize.x, SlotSize.y / GameController.Data.TileSize.x * GameController.Data.TileSize.y);
                }
                else
                {
                    SlotSize = new Vector2(SlotSize.x / GameController.Data.TileSize.y * GameController.Data.TileSize.x, SlotSize.y);
                }
            }
        }

        public void Recalculate()
        {
            var biggerLayerSize = LevelController.IsEvenLayerBigger ? LevelController.EvenLayerSize : LevelController.OddLayerSize;

            var tileSizeX = LevelFieldSize.x / biggerLayerSize.x;
            var tileSizeY = LevelFieldSize.y / biggerLayerSize.y;

            var refTileSize = Vector2.one * Mathf.Clamp(Mathf.Min(tileSizeX, tileSizeY), 0, GameController.Data.MaxTileSize);

            var dataTileSize = GameController.Data.TileSize;

            if(dataTileSize == Vector2.one)
            {
                TileSize = refTileSize;
            } else
            {
                if(dataTileSize.x > dataTileSize.y)
                {
                    TileSize = new Vector2(refTileSize.x, refTileSize.y / GameController.Data.TileSize.x * GameController.Data.TileSize.y);
                } else
                {
                    TileSize = new Vector2(refTileSize.x / GameController.Data.TileSize.y * GameController.Data.TileSize.x, refTileSize.y);
                }
            }

            OddLevelSize = new Vector2(TileSize.x * LevelController.OddLayerSize.x, TileSize.y * LevelController.OddLayerSize.y);
            EvenLevelSize = new Vector2(TileSize.x * LevelController.EvenLayerSize.x, TileSize.y * LevelController.EvenLayerSize.y);
        }

        public static Vector3 GetPosition(ElementPosition elementPosition)
        {
            int layerID = elementPosition.LayerId;

            var halfSize = TileSize / 2f;

            if ((LevelController.Level.AmountOfLayers + 1 - layerID) % 2 == 0)
            {
                return new Vector3(-EvenLevelSize.x / 2f + elementPosition.X * TileSize.x + halfSize.x, -EvenLevelSize.y / 2f + halfSize.y + elementPosition.Y * TileSize.y, layerID);
            }
            else
            {
                return new Vector3(-OddLevelSize.x / 2f + elementPosition.X * TileSize.x + halfSize.x, -OddLevelSize.y / 2f + halfSize.y + elementPosition.Y * TileSize.y, layerID);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.DrawWireCube(LevelFieldCenter, LevelFieldSize);
            }
        }

        [System.Serializable]
        public struct Offset
        {
            [Range(0, 1)] public float left;
            [Range(0, 1)] public float right;
            [Range(0, 1)] public float top;
            [Range(0, 1)] public float bottom;
        }
    }
}