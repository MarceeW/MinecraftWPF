#version 460 core

in vec2 texCoord;
in vec3 normal;
in float shade;
in float visibility;

uniform sampler2D tex;

out vec4 fragColor;

void main(void){
    vec4 textureColor = texture(tex,texCoord);

    fragColor = vec4(textureColor.xyz * shade,textureColor.w);
    fragColor = mix(vec4(0.73,0.83,1.0, 1.0),fragColor,visibility);
}