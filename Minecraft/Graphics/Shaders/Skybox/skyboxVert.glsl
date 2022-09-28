#version 330 core

layout(location = 0) in vec3 a_Position;

out vec3 texCoord;

uniform vec3 model;
uniform mat4 projection;
uniform mat4 view;

void main(void)
{
    texCoord = a_Position;
    vec4 pos = vec4(a_Position + model, 1.0) * view * projection;
    gl_Position = pos.xyww;
}