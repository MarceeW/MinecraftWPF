#version 460 core

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec3 a_Normal;
layout(location = 2) in vec2 a_TexCoord;
layout(location = 3) in float a_Shade;

out vec3 pos;
out vec3 normal;
out vec2 texCoord;
out float shade;
out float visibility;

uniform mat4 view;
uniform mat4 projection;
uniform double renderDistance;

float density;
float gradient = 16.0;

void main(void)
{
    density = 0.08 / float(renderDistance);

    pos = a_Position;
    normal = a_Normal;
    texCoord = a_TexCoord;
    shade = a_Shade;

    vec4 posRelativeToCam = vec4(a_Position, 1.0) * view;

    gl_Position = posRelativeToCam * projection;

    float distance = length(posRelativeToCam.xyz);
    visibility = clamp(exp(-pow((distance*density),gradient)),0.0,1.0);
}