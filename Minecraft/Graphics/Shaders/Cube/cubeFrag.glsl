#version 330 core

in vec3 texCoord;
in vec3 fragPos;
in vec3 normal;

uniform samplerCube tex;
uniform vec3 color;
uniform bool hasTexture;

out vec4 fragColor;

void main(void){

	vec3 lightPos = vec3(1.0f,1.0f,1.0f);

    float ambient = 0.5f;

    float diffuse = max(dot(lightPos,normal),0.0f);

    float result = diffuse;

    if(result < 0.5)
        result += ambient;

    if(hasTexture)
        fragColor = texture(tex,texCoord) + result;
    else
        fragColor = vec4(color,1.0) * result;
}