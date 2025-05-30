// TattooPreview.cs
// Attach this component to the SkinnedMeshRenderer (or MeshRenderer) that uses the TattooOverlayRotatable shader.
// It exposes editable fields for tattoo center / scale / rotation and automatically pushes them to the material.
// In Scene view it draws a yellow rectangle gizmo so you can roughly see where the tattoo will appear.
// A custom editor (inside the #if UNITY_EDITOR block) adds position / scale / rotation handles for interactive editing.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class TattooPreview : MonoBehaviour
{
    [Header("Material that uses the TattooOverlayRotatable shader")]
    public Material targetMaterial;

    [Header("Tattoo Parameters (UV space 0‑1)")]
    [Tooltip("Center of the tattoo in UV space (0‑1). x = U, y = V")]
    public Vector2 center = new Vector2(0.5f, 0.5f);

    [Tooltip("Scale of the tattoo in UV space. (1,1) means full‑quad; 0.1 is 10 % of UV size")]
    public Vector2 scale = new Vector2(0.2f, 0.2f);

    [Tooltip("Rotation of the tattoo in degrees (clockwise)")]
    public float rotation = 0f;

    private static readonly int CenterID = Shader.PropertyToID("_TattooCenter");
    private static readonly int ScaleID = Shader.PropertyToID("_TattooScale");
    private static readonly int RotateID = Shader.PropertyToID("_TattooRotation");

    void LateUpdate()
    {
        if (targetMaterial == null) return;
        targetMaterial.SetVector(CenterID, new Vector4(center.x, center.y, 0, 0));
        targetMaterial.SetVector(ScaleID, new Vector4(scale.x, scale.y, 0, 0));
        targetMaterial.SetFloat(RotateID, rotation);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (targetMaterial == null) return;
        // Approximate preview: we draw a rectangle in world space using the bounds center projected to the mesh
        var rend = GetComponent<Renderer>();
        if (rend == null) return;
        var bounds = rend.bounds;

        // Use object local right/up as preview axes (not 100 % accurate but gives rough idea)
        Vector3 right = transform.right * bounds.size.magnitude * scale.x;
        Vector3 up = transform.up * bounds.size.magnitude * scale.y;

        // Apply rotation in world space around bounds center
        Quaternion q = Quaternion.AngleAxis(-rotation, transform.forward);
        right = q * right;
        up = q * up;

        Vector3 worldCenter = bounds.center + transform.forward * 0.01f; // small offset to avoid z‑fighting

        Vector3 p0 = worldCenter - right * 0.5f - up * 0.5f;
        Vector3 p1 = worldCenter + right * 0.5f - up * 0.5f;
        Vector3 p2 = worldCenter + right * 0.5f + up * 0.5f;
        Vector3 p3 = worldCenter - right * 0.5f + up * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);
    }

    // Custom inspector + scene handles for intuitive editing
    [CustomEditor(typeof(TattooPreview))]
    public class TattooPreviewEditor : Editor
    {
        void OnSceneGUI()
        {
            var tp = (TattooPreview)target;
            var rend = tp.GetComponent<Renderer>();
            if (rend == null) return;

            var bounds = rend.bounds;
            Vector3 worldCenter = bounds.center;

            // Draw position handle in world space (moves along mesh local plane)
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(worldCenter, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tp, "Move Tattoo Center");
                // Project movement to UV space approx: offset along local XY proportionally
                Vector3 delta = tp.transform.InverseTransformVector(newPos - worldCenter);
                tp.center += new Vector2(delta.x, delta.y); // simplistic – adjust as needed
            }
        }
    }
#endif
}
