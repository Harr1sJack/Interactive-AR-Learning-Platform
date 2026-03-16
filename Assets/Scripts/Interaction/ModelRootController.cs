using UnityEngine;

public class ModelRootController : MonoBehaviour
{
    private MeshRenderer[] allRenderers;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        allRenderers = GetComponentsInChildren<MeshRenderer>(true);
        propertyBlock = new MaterialPropertyBlock();

        ConvertAllMaterialsToTransparent();
    }

    public void FocusOn(MeshRenderer[] targetRenderers)
    {
        foreach (var r in allRenderers)
        {
            float alpha = IsTarget(r, targetRenderers) ? 1f : 0.05f;
            ApplyAlpha(r, alpha);
        }
    }

    public void ResetTransparency()
    {
        foreach (var r in allRenderers)
        {
            ApplyAlpha(r, 1f);
        }
    }

    private bool IsTarget(MeshRenderer r, MeshRenderer[] targets)
    {
        foreach (var t in targets)
        {
            if (r == t)
                return true;
        }
        return false;
    }

    private void ApplyAlpha(MeshRenderer renderer, float alpha)
    {
        renderer.GetPropertyBlock(propertyBlock);

        var materials = renderer.sharedMaterials;

        for (int i = 0; i < materials.Length; i++)
        {
            var mat = materials[i];
            if (mat == null)
                continue;

            if (mat.HasProperty("_BaseColor"))
            {
                Color baseColor = mat.GetColor("_BaseColor");
                baseColor.a = alpha;
                propertyBlock.SetColor("_BaseColor", baseColor);
            }
            else if (mat.HasProperty("_Color"))
            {
                Color baseColor = mat.GetColor("_Color");
                baseColor.a = alpha;
                propertyBlock.SetColor("_Color", baseColor);
            }
        }

        renderer.SetPropertyBlock(propertyBlock);
    }

    private void ConvertAllMaterialsToTransparent()
    {
        foreach (var r in allRenderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat == null)
                    continue;

                if (mat.HasProperty("_Surface"))
                {
                    mat.SetFloat("_Surface", 1); 
                    mat.SetFloat("_Blend", 0);
                    mat.SetFloat("_ZWrite", 0);

                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
            }
        }
    }
}