shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE: hint_screen_texture, filter_linear_mipmap;

// Convert RGB to HSV
vec3 rgb_to_hsv(vec3 rgb) {
    vec3 hsv;
    float cmax = max(max(rgb.r, rgb.g), rgb.b);
    float cmin = min(min(rgb.r, rgb.g), rgb.b);
    float delta = cmax - cmin;

    if (delta == 0.0) {
        hsv.x = 0.0;
    } else if (cmax == rgb.r) {
        hsv.x = mod((rgb.g - rgb.b) / delta, 6.0);
    } else if (cmax == rgb.g) {
        hsv.x = (rgb.b - rgb.r) / delta + 2.0;
    } else {
        hsv.x = (rgb.r - rgb.g) / delta + 4.0;
    }

    hsv.x *= 60.0;
    if (hsv.x < 0.0) {
        hsv.x += 360.0;
    }

    hsv.z = cmax;
    if (cmax == 0.0) {
        hsv.y = 0.0;
    } else {
        hsv.y = delta / cmax;
    }

    return hsv;
}

// Convert HSV to RGB
vec3 hsv_to_rgb(vec3 hsv) {
    vec3 rgb;
    float c = hsv.z * hsv.y;
    float x = c * (1.0 - abs(mod(hsv.x / 60.0, 2.0) - 1.0));
    float m = hsv.z - c;

    if (hsv.x < 60.0) {
        rgb = vec3(c, x, 0.0);
    } else if (hsv.x < 120.0) {
        rgb = vec3(x, c, 0.0);
    } else if (hsv.x < 180.0) {
        rgb = vec3(0.0, c, x);
    } else if (hsv.x < 240.0) {
        rgb = vec3(0.0, x, c);
    } else if (hsv.x < 300.0) {
        rgb = vec3(x, 0.0, c);
    } else {
        rgb = vec3(c, 0.0, x);
    }

    rgb += m;

    return rgb;
}

vec4 colorgrade(vec4 color) {
	
	float brightness = 1.0;
	float contrast = 1.0;
	float saturation = 0.8;
	
	color.rgb = mix(vec3(0.0), color.rgb, brightness);
    color.rgb = mix(vec3(0.5), color.rgb, contrast);
    color.rgb = mix(vec3(dot(vec3(1.0), color.rgb) * 0.33333), color.rgb, saturation);
	return color;
}

vec4 fb(vec4 color) {
	vec3 hsv = rgb_to_hsv(color.rgb);
	if (hsv.z > 0.95) {
		return color;
	} else {
		return vec4(0.0, 0.0, 0.0, 1.0);
	}
}

void fragment() {
	vec4 origcolor = texture(SCREEN_TEXTURE, SCREEN_UV);
	
	vec2 uv = SCREEN_UV;
	
	float blur_strength = 15.0;

    // First pass: horizontal blur
    vec4 sum = vec4(0.0);
    vec2 blur_size = blur_strength * SCREEN_PIXEL_SIZE * vec2(4.0, 3.0);
	
	sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, -16) * SCREEN_PIXEL_SIZE)) * 0.0;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, -12) * SCREEN_PIXEL_SIZE)) * 2.6274169979695223;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, -8) * SCREEN_PIXEL_SIZE)) * 4.738873177971204;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, -4) * SCREEN_PIXEL_SIZE)) * 6.13499449549888;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, 0) * SCREEN_PIXEL_SIZE)) * 6.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, 4) * SCREEN_PIXEL_SIZE)) * 6.13499449549888;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, 8) * SCREEN_PIXEL_SIZE)) * 4.738873177971204;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-16, 12) * SCREEN_PIXEL_SIZE)) * 2.6274169979695223;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, -16) * SCREEN_PIXEL_SIZE)) * 2.6274169979695223;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, -12) * SCREEN_PIXEL_SIZE)) * 5.656854249492383;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, -8) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, -4) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, 0) * SCREEN_PIXEL_SIZE)) * 10.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, 4) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, 8) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-12, 12) * SCREEN_PIXEL_SIZE)) * 5.656854249492383;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, -16) * SCREEN_PIXEL_SIZE)) * 4.738873177971204;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, -12) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, -8) * SCREEN_PIXEL_SIZE)) * 11.313708498984761;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, -4) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, 0) * SCREEN_PIXEL_SIZE)) * 14.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, 4) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, 8) * SCREEN_PIXEL_SIZE)) * 11.313708498984761;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-8, 12) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, -16) * SCREEN_PIXEL_SIZE)) * 6.13499449549888;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, -12) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, -8) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, -4) * SCREEN_PIXEL_SIZE)) * 16.970562748477143;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, 0) * SCREEN_PIXEL_SIZE)) * 18.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, 4) * SCREEN_PIXEL_SIZE)) * 16.970562748477143;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, 8) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(-4, 12) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, -16) * SCREEN_PIXEL_SIZE)) * 6.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, -12) * SCREEN_PIXEL_SIZE)) * 10.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, -8) * SCREEN_PIXEL_SIZE)) * 14.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, -4) * SCREEN_PIXEL_SIZE)) * 18.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, 0) * SCREEN_PIXEL_SIZE)) * 22.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, 4) * SCREEN_PIXEL_SIZE)) * 18.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, 8) * SCREEN_PIXEL_SIZE)) * 14.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(0, 12) * SCREEN_PIXEL_SIZE)) * 10.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, -16) * SCREEN_PIXEL_SIZE)) * 6.13499449549888;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, -12) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, -8) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, -4) * SCREEN_PIXEL_SIZE)) * 16.970562748477143;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, 0) * SCREEN_PIXEL_SIZE)) * 18.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, 4) * SCREEN_PIXEL_SIZE)) * 16.970562748477143;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, 8) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(4, 12) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, -16) * SCREEN_PIXEL_SIZE)) * 4.738873177971204;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, -12) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, -8) * SCREEN_PIXEL_SIZE)) * 11.313708498984761;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, -4) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, 0) * SCREEN_PIXEL_SIZE)) * 14.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, 4) * SCREEN_PIXEL_SIZE)) * 13.683145087970363;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, 8) * SCREEN_PIXEL_SIZE)) * 11.313708498984761;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(8, 12) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, -16) * SCREEN_PIXEL_SIZE)) * 2.6274169979695223;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, -12) * SCREEN_PIXEL_SIZE)) * 5.656854249492383;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, -8) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, -4) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, 0) * SCREEN_PIXEL_SIZE)) * 10.627416997969522;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, 4) * SCREEN_PIXEL_SIZE)) * 9.978306357296004;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, 8) * SCREEN_PIXEL_SIZE)) * 8.205211896113566;
sum += fb(texture(SCREEN_TEXTURE, vec2(uv.x, uv.y) + vec2(12, 12) * SCREEN_PIXEL_SIZE)) * 5.656854249492383;

	vec4 color = colorgrade(origcolor);
	
	vec3 hsvorig = rgb_to_hsv(origcolor.rgb);
    // Output blurred color
	vec4 blured = sum / 656.1142023741581;
	
	vec3 outrgb= vec3(1) - (vec3(1)-color.rgb) * (vec3(1) - blured.rgb);
	
    COLOR = vec4(outrgb, 1.0);
}
