#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

out vec4 FragColor;

uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;
uniform float ambientStrength;
uniform float specularStrength;
uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;

void main()
{
    // Ambient (фоновое освещение)
    vec3 ambient = ambientStrength * lightColor;

    // Diffuse (рассеянное освещение)
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    // Specular (зеркальное освещение)
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;

    // Текстуры
    vec3 texDiffuse = texture(texture_diffuse1, TexCoords).rgb;
    vec3 texSpecular = texture(texture_specular1, TexCoords).rgb;

    // Итоговый цвет
    vec3 result = (ambient + diffuse) * texDiffuse + specular * texSpecular;
    FragColor = vec4(result, 1.0);
}