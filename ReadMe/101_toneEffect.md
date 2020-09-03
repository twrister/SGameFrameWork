## Tone Effect
#### 灰度 grayscale
![](Pic/100_shanks.png)
![](Pic/101_grayscale.png)  
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

#### 上色 sepia
![](Pic/100_shanks.png)
![](Pic/101_sepia.png)  
在灰度的基础上，乘上一种颜色(这里是棕色)
```
col.rgb = dot(col.rgb, half3(0.22, 0.707, 0.071)) * half3(1.07, 0.74, 0.43);
```

#### 反色 nagate
![](Pic/100_shanks.png)
![](Pic/101_nagate.png)  
反色就是每个通道都取反，1减通道本身可得反色。
```
color.rgb = lerp(color.rgb, 1 - color.rgb, _Factor);
```