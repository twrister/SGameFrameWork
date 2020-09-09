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
half4 ApplyHsvEffect(half4 color, half param)
{
	fixed4 param1 = tex2D(_ParamTex, float2(0.25, param));
	fixed4 param2 = tex2D(_ParamTex, float2(0.75, param));
    fixed3 targetHsv = param1.rgb;

    fixed3 targetRange = param1.w;
    fixed3 hsvShift = param2.xyz - 0.5;
	half3 hsv = RgbToHsv(color.rgb);
	half3 range = abs(hsv - targetHsv);
	half diff = max(max(min(1-range.x, range.x), min(1-range.y, range.y)/10), min(1-range.z, range.z)/10);

	fixed masked = step(diff, targetRange);
	color.rgb = HsvToRgb(hsv + hsvShift * masked);

	return color;
}