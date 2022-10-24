#version 460 core

out vec4 fragColor;

in vec3 color;

void main(void){
    fragColor = vec4(color,0.5);
}