using UnityEngine;

public class ModelSelectable : MonoBehaviour
{
    public ModelInfoData info;

    private static ModelSelectable currentlySelected;

    private ModelRootController rootController;
    private MeshRenderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);
        rootController = GetComponentInParent<ModelRootController>();
    }

    public void Select()
    {
        // Deselect previous
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        currentlySelected = this;

        if (rootController != null)
        {
            rootController.FocusOn(renderers);
        }
    }

    public void Deselect()
    {
        if (rootController != null)
        {
            rootController.ResetTransparency();
        }

        currentlySelected = null;
    }

    public static void ClearSelection()
    {
        if (currentlySelected != null)
        {
            currentlySelected.Deselect();
        }
    }
}