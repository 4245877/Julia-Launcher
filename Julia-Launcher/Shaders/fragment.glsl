#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

out vec4 FragColor;

uniform vec3 lightPos;              // Позиция источника света
uniform vec3 lightColor;           // Цвет света
uniform vec3 viewPos;              // Позиция камеры
uniform float ambientStrength;     // Сила фонового освещения
uniform float specularStrength;    // Сила зеркального освещения
uniform float shininess;           // Показатель степени зеркального блика
uniform float rimStrength;         // Сила подсветки краев
uniform float rimPower;            // Резкость подсветки краев
uniform vec3 rimColor;             // Цвет подсветки краев
uniform sampler2D texture_diffuse1; // Текстура диффузного отражения
uniform sampler2D texture_specular1;// Текстура зеркального отражения
uniform samplerCube environmentMap;// Кубическая карта окружающей среды

void main()
{
    // Нормализация векторов
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    vec3 viewDir = normalize(viewPos - FragPos);

    // Фоновое освещение (Ambient) с использованием environment map
    vec3 ambient = ambientStrength * texture(environmentMap, norm).rgb;

    // Рассеянное освещение (Diffuse)
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    // Зеркальное освещение (Specular)
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    vec3 specular = specularStrength * spec * lightColor;

    // Подсветка краев (Rim Lighting)
    float rim = 1.0 - max(dot(viewDir, norm), 0.0);
    rim = pow(rim, rimPower);
    vec3 rimLight = rimStrength * rim * rimColor;

    // Применение текстур
    vec3 texDiffuse = texture(texture_diffuse1, TexCoords).rgb;
    vec3 texSpecular = texture(texture_specular1, TexCoords).rgb;

    // Итоговый цвет
    vec3 result = (ambient + diffuse) * texDiffuse + specular * texSpecular + rimLight;
    FragColor = vec4(result, 1.0);
}