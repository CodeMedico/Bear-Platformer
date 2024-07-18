shader_type canvas_item;
uniform sampler2D _noise : repeat_enable;
uniform sampler2D _texture : repeat_disable;
uniform float distortion_strenght : hint_range(-1,1) = 0.2;
uniform vec2 scroll = vec2(0.05,0.05);
void fragment() {
	vec4 noise_coll = texture(_noise, UV+scroll*TIME);
	vec4 screen_coll = texture(_texture, UV + distortion_strenght * noise_coll.rr );
	COLOR.rgba = screen_coll.rgba;
	COLOR.rgb *= vec3(1,1,2.5);
}
