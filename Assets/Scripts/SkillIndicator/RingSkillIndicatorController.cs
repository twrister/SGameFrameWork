using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
// [RequireComponent(typeof(Renderer))]
public class RingSkillIndicatorController : MonoBehaviour
{
    public Renderer _renderer;
    
    [LabelText("颜色")]
    public Color color = Color.white;
    
    [LabelText("半径")]
    public float radius = 1f;
    
    [LabelText("圆心偏移")]
    public float centerOffset = 0f;
    
    [LabelText("渐变透明度")]
    [MinMaxSlider(0f, 1f, true)]
    public Vector2 fadeStartEndValue = new Vector2(0.2f, 0.5f);
    
    [LabelText("内圆比例")]
    [Range(0f, 1f)] public float innerRadius = 0.1f;
    
    [LabelText("角度")]
    [Range(0f, 360f)] public float angle = 45f;
    
    [LabelText("描边宽度")]
    [Range(0f, 0.2f)] public float borderThickness = 0.01f;
    
    [LabelText("向内渐变过渡距离")]
    [Range(0f, 0.5f)] public float fadeDistance = 0.1f;
    
    [LabelText("圆心向外的渐隐")]
    [MinMaxSlider(0f, 1f, true)]
    public Vector2 fadeFromCenter = new Vector2(0.5f, 1f);

    private MaterialPropertyBlock _propBlock;
    private float direction = 90f;
    private float outerRadius = 1f;
    private float fadeStartRate = 0.5f;
    private float fadeEndRate = 0.2f;
    private float centerFadeDistance = 0.5f;
    private float centerFadeEndRatio = 1.0f;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }
    
    void OnValidate()
    {
        // Update in edit mode as values change
        ApplyProperties();
    }

    private void ApplyProperties()
    {
        if (_renderer == null)
        {
            return;
        }

        if (_propBlock == null)
        {
            _propBlock = new MaterialPropertyBlock();
        }

        _renderer.transform.localScale = radius * 2f * Vector3.one;
        _renderer.transform.localPosition = centerOffset * Vector3.forward;
        
        fadeEndRate = fadeStartEndValue.x;
        fadeStartRate = fadeStartEndValue.y;
        centerFadeDistance = fadeFromCenter.x;
        centerFadeEndRatio = fadeFromCenter.y;
        
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", color);
        _propBlock.SetFloat("_FadeStartRate", fadeStartRate);
        _propBlock.SetFloat("_FadeEndRate", fadeEndRate);
        _propBlock.SetFloat("_InnerRadius", innerRadius);
        _propBlock.SetFloat("_OuterRadius", outerRadius);
        _propBlock.SetFloat("_Angle", angle);
        _propBlock.SetFloat("_Direction", direction);
        _propBlock.SetFloat("_BorderThickness", borderThickness);
        _propBlock.SetFloat("_FadeDistance", fadeDistance);
        _propBlock.SetFloat("_CenterFadeDistance", centerFadeDistance);
        _propBlock.SetFloat("_CenterFadeEndRatio", centerFadeEndRatio);
        _renderer.SetPropertyBlock(_propBlock);
    }
}