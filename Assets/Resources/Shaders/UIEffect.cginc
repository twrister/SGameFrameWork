// Apply tone effect.
fixed4 ApplyToneEffect(fixed4 color, fixed factor)
{
    #ifdef GRAYSCALE
    color.rgb = lerp(color.rgb, Luminance(color.rgb), factor);

    #elif SEPIA
    color.rgb = lerp(color.rgb, Luminance(color.rgb) * half3(1.07, 0.74, 0.43), factor);

    #elif NEGA
    color.rgb = lerp(color.rgb, 1 - color.rgb, factor);
    #endif

    return color;
}

half3 RgbToHsv(half3 c) {
	half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
	half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

	half d = q.x - min(q.w, q.y);
	half e = 1.0e-10;
	return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

half3 HsvToRgb(half3 c) {
	c = half3(c.x, clamp(c.yz, 0.0, 1.0));
	half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, clamp(p.xyz - K.xxx, 0.0, 1.0), c.y);
}

// Apply Hsv effect.
half4 ApplyHsvEffect(half4 color, fixed4 param1, fixed4 param2)
{
    fixed3 targetHsv = param1.rgb;			// 目标颜色的HSV值
    fixed targetRange = param1.w;			// 与目标颜色的偏差范围,(0~1),越小越接近目标颜色
    
    fixed3 hsvShift = param2.xyz - 0.5;		// HSV偏移(-0.5~0.5)
	half3 hsv = RgbToHsv(color.rgb);		// 源颜色的HSV值
	half3 range = abs(hsv - targetHsv);		// 源颜色与目标颜色，HSV各值的绝对值偏差

	// 求出偏差值H,S,V中最最大的，作为源颜色与目标色的偏差值，其中S,V要缩小10倍
	half diff = max(max(min(1-range.x, range.x), min(1-range.y, range.y)/10), min(1-range.z, range.z)/10);

	// 目标偏差比实际偏差大的，加上HSV偏移
	fixed masked = step(diff, targetRange);
	color.rgb = HsvToRgb(hsv + hsvShift * masked);

	return color;
}