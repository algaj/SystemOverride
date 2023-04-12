shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;


uniform float hit_effect_factor;


void fragment() {
    // Calculate the distance from the current fragment to the center of the screen
    float dist = distance(SCREEN_UV, vec2(0.5, 0.5));

    // Calculate the vignette intensity based on the distance
    float vignette = smoothstep(1.0, 0.0, dist);
	
	COLOR = vec4(texture(screen_texture, SCREEN_UV).rgb * vignette, 1.0);
}
