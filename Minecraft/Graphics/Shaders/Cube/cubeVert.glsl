#version 330 core

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec3 a_Normal;

out vec3 normal;
out vec3 fragPos;
out vec3 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    normal = mat3(transpose(inverse(model))) * a_Normal;;
    fragPos = vec3(model * vec4(a_Position,1.0));
    texCoord = a_Position;

    gl_Position = vec4(a_Position, 1.0) * model * view * projection;
}