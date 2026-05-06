using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FlowerSway : MonoBehaviour
{
    public float swayAmount = 10f;
    public float swaySpeed = 2f;

    private MaterialPropertyBlock propBlock;
    private Renderer rend;
    private float randomOffset;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        rend = GetComponent<Renderer>();
        randomOffset = Random.Range(0f, 100f);
        UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties()
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_RandomOffset", randomOffset);
        propBlock.SetColor("_Color", Color.white);
        rend.SetPropertyBlock(propBlock);
    }

    public void SetColor(Color newColor)
    {
        if (propBlock == null) propBlock = new MaterialPropertyBlock();
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", newColor);
        rend.SetPropertyBlock(propBlock);
    }
}