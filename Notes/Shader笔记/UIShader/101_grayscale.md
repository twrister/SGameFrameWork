#### 灰度 grayscale

实现
```
#define unity_ColorSpaceLuminance half4(0.22, 0.707, 0.071, 0.0)
inline half Luminance(half3 rgb)
{
    return dot(rgb, unity_ColorSpaceLuminance.rgb);
}

float dot(float3 a, float3 b)
{
  return a.x*b.x + a.y*b.y + a.z*b.z;
}

fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);

    col.rgb = lerp(col.rgb, Luminance(col.rgb), _Factor);

    return col;
}
```
简化
```
col.rgb = dot(col.rgb, half3(0.22, 0.707, 0.071));
```
> dot(col.rgb, half3(0.22, 0.707, 0.071) 求出该片元的明度值，rgb通道都用明度值，视觉上就是置灰