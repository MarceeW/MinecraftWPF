#version 460 core

layout(location = 0) in vec3 a_Position;

uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    gl_Position = vec4(a_Position,1.0) * view * projection;
}