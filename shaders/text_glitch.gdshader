shader_type canvas_item;

uniform float time: hint_range(0, 10, 0.01) = 0.0;
uniform float distortion = 2.0;

void vertex() {
	VERTEX.y = VERTEX.y + sin(UV.x * TIME) * distortion;
}

void fragment() {
    COLOR = texture(TEXTURE, UV) * vec4(0.1, 1.0, 0.1, 1.0);
}