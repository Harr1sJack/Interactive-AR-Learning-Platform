using UnityEngine;

public class ModelSelectable : MonoBehaviour
{
    public ModelInfoData info;

    private static ModelSelectable currentlySelected;

    private MeshRenderer[] renderers;

    // Property Block (efficient per-renderer overrides)
    private MaterialPropertyBlock propertyBlock;

    // Shader property IDs (faster than strings)
    private static readonly int ColorID = Shader.PropertyToID("_Color");
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    [SerializeField]
    private Color highlightColor = Color.softYellow;

    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);
        propertyBlock = new MaterialPropertyBlock();
    }

    public void Select()
    {
        // Deselect previous
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        currentlySelected = this;

        ApplyHighlight();
    }

    public void Deselect()
    {
        ResetHighlight();
        currentlySelected = null;
    }

    public static void ClearSelection()
    {
        if (currentlySelected != null)
        {
            currentlySelected.Deselect();
        }
    }

    private void ApplyHighlight()
    {
        foreach (var renderer in renderers)
        {
            renderer.GetPropertyBlock(propertyBlock);

            // Apply color (URP/HDRP or Standard)
            if (renderer.sharedMaterial.HasProperty(BaseColorID))
            {
                propertyBlock.SetColor(BaseColorID, highlightColor);
            }
            else if (renderer.sharedMaterial.HasProperty(ColorID))
            {
                propertyBlock.SetColor(ColorID, highlightColor);
            }

            // Optional: subtle emission for AR visibility
            if (renderer.sharedMaterial.HasProperty(EmissionID))
            {
                propertyBlock.SetColor(EmissionID, highlightColor * 0.6f);
            }

            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void ResetHighlight()
    {
        foreach (var renderer in renderers)
        {
            // Clearing property block restores original material appearance
            renderer.SetPropertyBlock(null);
        }
    }
}