#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoord;
out vec3 ViewPos;
out vec4 LightSpaceFragPos;
out mat3 TBN;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;
uniform vec3 viewPosition;
uniform mat4 lightSpaceMatrix;

void main()
{
    FragPos = vec3(model * vec4(aPos, 1.0));
    Normal = normalMatrix * aNormal;
    TexCoord = aTexCoord;
    ViewPos = viewPosition;
    
    // Calculate TBN matrix for normal mapping
    vec3 T = normalize(normalMatrix * aTangent);
    vec3 B = normalize(normalMatrix * aBitangent);
    vec3 N = normalize(Normal);
    TBN = mat3(T, B, N);
    
    // For shadow mapping
    LightSpaceFragPos = lightSpaceMatrix * vec4(FragPos, 1.0);
    
    gl_Position = projection * view * model * vec4(aPos, 1.0);
}