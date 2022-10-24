#version 460 core

in vec2 texCoord;
in vec3 normal;
in float shade;
in float visibility;

uniform sampler2D tex;

out vec4 fragColor;

void main(void){
    vec4 textureColor = texture(tex,texCoord);

    if(textureColor.w == 0.0)
        discard;

    fragColor = vec4(textureColor.xyz * shade,textureColor.w) * visibility;
}