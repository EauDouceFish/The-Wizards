using UnityEngine;

public class ShaderUI : MonoBehaviour
{
    [SerializeField] private bool enableAdditionalLights = true;

    void Start()
    {
        ToggleShaderKeywords();
    }

    void Update()
    {
        //ToggleShaderKeywords();
    }

    private void ToggleShaderKeywords()
    {
        if (enableAdditionalLights)
        {
            Shader.EnableKeyword("_AdditionalLights");
        }
        else
        {
            Shader.DisableKeyword("_AdditionalLights");
        }
    }

    public void SetAdditionalLights(bool enable)
    {
        enableAdditionalLights = enable;
        ToggleShaderKeywords();
    }
}
