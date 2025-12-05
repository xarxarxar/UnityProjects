using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace UnityEngine.UI
{
    public class CutOutMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material material = new Material(base.materialForRendering);
                material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return material;
            }
        }
    }
}

