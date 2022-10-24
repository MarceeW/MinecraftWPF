#version 460 core

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec3 a_Normal;
layout(location = 2) in vec2 a_TexCoord;
layout(location = 3) in float a_Shade;

out vec3 normal;
out vec2 texCoord;

uniform mat4 view;
uniform mat4 model;
uniform mat4 projection;

void main(void)
{
    normal = a_Normal;
    texCoord = a_TexCoord;

    gl_Position = vec4(a_Position, 1.0) * model * view * projection;
}