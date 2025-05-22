using UnityEngine;

[ExecuteAlways]
public class DecalController : MonoBehaviour
{
    public Material decalMaterial;

    public Vector2 decalCenter = new Vector2(0.5f, 0.5f);

    public Vector2 decalScale = new Vector2(0.2f, 0.2f);

    [Range(0, 360)]
    public float decalRotation = 0f;

    void Update()
    {
        if (decalMaterial == null) return;

        decalMaterial.SetVector("_DecalCenter", new Vector4(decalCenter.x / 100, decalCenter.y / 100, 0, 0));
        decalMaterial.SetVector("_DecalScale", new Vector4(decalScale.x / 100, decalScale.y / 100, 0, 0));
        decalMaterial.SetFloat("_DecalRotation", decalRotation);
    }
}
