shader_type canvas_item;

uniform sampler2D u_color_shift_map;

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

void fragment() {
	
	vec4 main_tex = texture(TEXTURE, UV * 3.0);
	vec4 color_shift_map = texture(u_color_shift_map, UV);
	
	vec3 hsv = rgb_to_hsv(main_tex.rgb);
	
	hsv.x += color_shift_map.x * 360.0f;
	hsv.y = 0.5;
	hsv.z = 0.3;
	
	COLOR = vec4(hsv_to_rgb(hsv), main_tex.a);
}
