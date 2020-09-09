## HSV Modifier

HSV是Hue(色调), Saturation(饱和度), Value(亮度)的缩写  

##### step
实现一个step函数，对于x中大于或等于参考向量a中对应分量的每个分量返回1，否则返回0。

```
float3 step(float3 a, float3 x)
{
    return x >= a;
}
```