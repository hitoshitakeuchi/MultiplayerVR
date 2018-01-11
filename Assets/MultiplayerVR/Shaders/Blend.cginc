
// Blend

// --------------------------------------------------------------------------
// AddSub
fixed4 fragAddSub (fixed4 a, fixed4 b, float intensity) {
	half4 toAdd = b * intensity;
	return a + toAdd;
}

// --------------------------------------------------------------------------
// Multiply
fixed4 fragMultiply (fixed4 a, fixed4 b, float intensity) {
	half4 toBlend = b * intensity;
	return a * toBlend;
}

// --------------------------------------------------------------------------
// Screen
fixed4 fragScreen (fixed4 a, fixed4 b, float intensity) {
	half4 toBlend =  b * intensity;
	return 1-(1-toBlend)*(1-a);
}

// --------------------------------------------------------------------------
// Overlay
fixed4 fragOverlay (fixed4 a, fixed4 b, float intensity) {
	half4 m = b;
	half4 color = a;

	// overlay blend mode
	float3 check = step(half3(0.5, 0.5, 0.5), color.rgb);
	float3 result = (float3)0.0;

	result = check * (half3(1,1,1) - ((half3(1,1,1) - 2*(color.rgb-0.5)) * (1-m.rgb)));
	result += (1-check) * (2*color.rgb) * m.rgb;

	return half4(lerp(color.rgb, result.rgb, intensity), color.a);
}

// --------------------------------------------------------------------------
// AlphaBlend
fixed4 fragAlphaBlend (fixed4 a, fixed4 b, float intensity) {
	half4 toAdd = b;
	return lerp(a, toAdd, toAdd.a * intensity);
}

// --------------------------------------------------------------------------
// SoftLight
float softLight (float a, float b) {
	return (b < 0.5) ? 
		(2.0 * a * b + a * a * (1.0 - 2.0 * b)) : 
		(sqrt(a) * (2.0 * b - 1.0) + 2.0 * a * (1.0 - b));
}

fixed4 fragSoftLight (fixed4 a, fixed4 b, float intensity) {
	return float4(softLight(a.r, b.r), softLight(a.g, b.g), softLight(a.b, b.g), softLight(a.a, b.a)) * intensity +
		   b * (1.0 - intensity);
}

// --------------------------------------------------------------------------
// HardLight
// SoftLight
float hardLight (float a, float b) {
	return (b < 0.5) ? 
		(2.0 * a * b) :
		(fixed4(1,1,1,1) - 2.0 * (fixed4(1,1,1,1) - b) * (fixed4(1,1,1,1) - a));	
}

fixed4 fragHardLight (fixed4 a, fixed4 b, float intensity) {
	return float4(hardLight(a.r, b.r), hardLight(a.g, b.g), hardLight(a.b, b.g), hardLight(a.a, b.a)) * intensity +
		   a * (1.0 - intensity);
}

// --------------------------------------------------------------------------
// Darken
fixed4 fragDarken (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		min(b, a),
		intensity
	);
}

// --------------------------------------------------------------------------
// Lighten
fixed4 fragLighten (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		max(b, a),
		intensity
	);
}

// --------------------------------------------------------------------------
// Difference
fixed4 fragDifference (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		abs(a - b),
		intensity
	);
}

// --------------------------------------------------------------------------
// Negation
fixed4 fragNegation (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		fixed4(1,1,1,1) - abs(fixed4(1,1,1,1) - a - b),
		intensity
	);
}

// --------------------------------------------------------------------------
// Exclusion
fixed4 fragExclusion (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		a + b - (2.0 * a * b),
		intensity
	);
}

// --------------------------------------------------------------------------
// Dodge
fixed4 fragDodge (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		(a / fixed4(1,1,1,1) - b),
		intensity
	);
}

// --------------------------------------------------------------------------
// Burn
fixed4 fragBurn (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		fixed4(1,1,1,1) - (fixed4(1,1,1,1) - a) / b,
		intensity
	);
}

// --------------------------------------------------------------------------
// LinearLight

fixed4 add (fixed4 a, fixed4 b) {
	return min(a + b, 1.0);
}

fixed4 sub (fixed4 a, fixed4 b) {
	return max(a + b - 1.0, 0.0);
}

fixed4 fragLinearLight (fixed4 a, fixed4 b, float intensity) {
	return lerp(
		a,
		(b < 0.5) ? sub(a, 2.0 * b) : add(a, 2.0 * (b - 0.5)),
		intensity
	);
}
