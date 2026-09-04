using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines : MonoBehaviour
{
    SpriteMask Mask;
    Color[] Colors;

    int Width, Height;

    void Start()
    {
        //Get objects
        Mask = GameObject.Find("Mask").GetComponent<SpriteMask>();

        //Extract color data once
        Colors = Mask.sprite.texture.GetPixels();

        //Store mask dimensionns
        Width = Mask.sprite.texture.width;
        Height = Mask.sprite.texture.height;

        ClearMask();
    }

    void ClearMask()
    {
        //set all color data to transparent
        for (int i = 0; i < Colors.Length; ++i)
        {
            Colors[i] = new Color(1, 1, 1, 1);
        }

        Debug.Log("lengggggg:" + Colors.Length);

        Mask.sprite.texture.SetPixels(Colors);
        Mask.sprite.texture.Apply(false);
    }

    //This function will draw a circle onto the texture at position cx, cy with radius r
    public void DrawOnMask(int cx, int cy, int r)
    {
        int px, nx, py, ny, d;

        for (int x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));

            for (int y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                var tx = py * Width + px;
                var ty = py * Width + nx;
                var zx = ny * Height + px;
                var zy = ny * Height + nx;

                Debug.Log("tx:" + tx + " ty:" + ty + " zx:" + zx + " zy:" + zy);

                if (Colors.Length > tx && tx >= 0)
                    Colors[tx] = new Color(1, 1, 1, 0);
                if (Colors.Length > ty && ty >= 0)
                    Colors[ty] = new Color(1, 1, 1, 0);
                if (Colors.Length > zx && zx >= 0)
                    Colors[zx] = new Color(1, 1, 1, 0);
                if (Colors.Length > zy && zy >= 0)
                    Colors[zy] = new Color(1, 1, 1, 0);
            }
        }

        Mask.sprite.texture.SetPixels(Colors);
        Mask.sprite.texture.Apply(false);
    }


    void Update()
    {

        //Get mouse coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Check if mouse button is held down
        if (Input.GetMouseButton(0))
        {
            //Check if we hit the collider
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                //Normalize to the texture coodinates
                // int y = (int)((0.5 - (Mask.transform.position - mousePosition).y) * Height);
                // int x = (int)((0.5 - (Mask.transform.position - mousePosition).x) * Width);

                int y = (int)((0.5 - (Mask.transform.position - mousePosition).y) * Height);
                int x = (int)((0.5 - (Mask.transform.position - mousePosition).x) * Width);

                // int x = (int)mousePosition.x;
                // int y = (int)mousePosition.y;

                //Draw onto the mask
                DrawOnMask(x, y, 20);
            }
        }
    }
}