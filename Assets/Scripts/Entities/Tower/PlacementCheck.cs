using UnityEngine;
using System.Collections.Generic;

public class PlacementCheck : MonoBehaviour
{
    [Header("To Set:")]
    public LayerMask validLayers;
    public LayerMask invalidLayers;
    public Material validPlacement;
    public Material invalidPlacement;
    private Renderer previewRenderer;

    private readonly List<Collider> touchingValid = new List<Collider>();
    private readonly List<Collider> touchingInvalid = new List<Collider>();

    public bool IsValidPlacement => touchingValid.Count > 0 && touchingInvalid.Count == 0;

    void Start()
    {
        previewRenderer = GetComponentInChildren<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;

        if (((1 << layer) & validLayers) != 0)
        {
            if (!touchingValid.Contains(other))
                touchingValid.Add(other);
        }
        else if (((1 << layer) & invalidLayers) != 0)
        {
            if (!touchingInvalid.Contains(other))
                touchingInvalid.Add(other);
        }
        UpdateMaterial();
    }
    void OnTriggerExit(Collider other)
    {
        if (touchingValid.Contains(other))
            touchingValid.Remove(other);
        if (touchingInvalid.Contains(other))
            touchingInvalid.Remove(other);
        UpdateMaterial();
    }
    void UpdateMaterial()
    {
        previewRenderer.material = IsValidPlacement ? validPlacement : invalidPlacement;
    }
}