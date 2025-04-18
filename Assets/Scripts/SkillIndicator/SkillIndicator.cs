using UnityEngine;

public class SkillIndicator : MonoBehaviour
{
    private Material material;
    public float animationSpeed = 1.0f;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 更新Shader中的自定义时间值
        material.SetFloat("_CustomTime", Time.time * animationSpeed); // 修改为 _CustomTime
    }
}