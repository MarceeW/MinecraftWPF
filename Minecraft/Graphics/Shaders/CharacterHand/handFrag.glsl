#version 460 core

in vec2 texCoord;
in vec3 normal;

uniform sampler2D tex;

out vec4 fragColor;

uniform vec3 viewDir;

void main(void){

    vec4 textureColor = texture(tex,texCoord);

    if(textureColor.w == 0.0)
        discard;

    float ambientStrength = 0.5;
    
    vec3 lightPos = vec3(1.0);

    vec3 lightDir = normalize(lightPos - viewDir);

    float diff = max(dot(normal,lightDir),0.0);

    vec3 result = (vec3(1.0) * ambientStrength + diff) * textureColor.xyz;

    fragColor = vec4(result,textureColor.w);
}