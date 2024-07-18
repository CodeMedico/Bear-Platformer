shader_type canvas_item;
uniform bool _fatLaser = false;
void fragment() {
	if (_fatLaser){
		vec2 muv = UV;
	float mask;
	mask = muv.y*(1.0-muv.y);
	mask = smoothstep(0,0.25,mask);
	mask = pow(mask,3);
	COLOR.a = COLOR.a * mask;
	if (mask > 0.999){
		COLOR.g = 1.0;
		COLOR.r = 1.0;
		COLOR.b = 1.0;
	}
	}
	COLOR.r *= 2.0;
}

void vertex(){
	
}
