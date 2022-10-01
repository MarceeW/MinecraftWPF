#version 460 core

out vec4 fragColor;

uniform vec3 frameColor;

void main(void){
    fragColor = vec4(frameColor,0.5);
}