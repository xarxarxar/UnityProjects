using UnityEngine;

namespace Watermelon
{ 
    public class BackgroundBehavior : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        public void Awake()
        {
            var camera = Camera.main;
            transform.position = camera.transform.position + camera.transform.forward * (camera.farClipPlane - 0.01f);
            transform.forward = camera.transform.forward;

            var spriteSize = spriteRenderer.sprite.textureRect.size;
            var spriteAspect = spriteSize.x / spriteSize.y;

            var cameraHeight = camera.orthographicSize * 2;
            var cameraWidth = cameraHeight * camera.aspect;

            if (Camera.main.aspect > spriteAspect)
            {
                spriteRenderer.size = new Vector2(cameraWidth, cameraWidth / spriteAspect);
            } else
            {
                spriteRenderer.size = new Vector2(cameraHeight * spriteAspect, cameraHeight);
            }
        }
    }
}