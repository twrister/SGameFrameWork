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