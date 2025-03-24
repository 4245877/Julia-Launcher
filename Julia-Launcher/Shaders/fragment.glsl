// fragment.glsl
#version 330 core
in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

out vec4 FragColor;

uniform vec3 lightPos;              // Позиция источника света
uniform vec3 lightColor;            // Цвет света
uniform vec3 viewPos;               // Позиция камеры
uniform float ambientStrength;      // Сила фонового освещения
uniform float specularStrength;     // Сила зеркального освещения

uniform sampler2D texture_diffuse1; // Текстура диффузного отражения
uniform sampler2D texture_specular1;// Текстура зеркального отражения

void main()
{
    // Нормализация векторов
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    vec3 viewDir = normalize(viewPos - FragPos);

    // Фоновое освещение (Ambient)
    vec3 ambient = ambientStrength * lightColor;

    // Рассеянное освещение (Diffuse)
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    // Зеркальное освещение (Specular)
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0); // Жесткость блика по умолчанию
    vec3 specular = specularStrength * spec * lightColor;

    // Применение текстур
    vec3 texDiffuse = texture(texture_diffuse1, TexCoords).rgb;
    vec3 texSpecular = texture(texture_specular1, TexCoords).rgb;

    // Итоговый цвет
    vec3 result = (ambient + diffuse) * texDiffuse + specular * texSpecular;
    FragColor = vec4(result, 1.0);
}