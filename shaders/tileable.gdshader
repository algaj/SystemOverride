shader_type canvas_item;

uniform float texture_scale;

uniform vec4 tint_color: source_color;

void fragment() {
	COLOR = texture(TEXTURE, UV / texture_scale) * tint_color;
}
