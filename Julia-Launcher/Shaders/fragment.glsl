#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform vec3 viewPosition;
uniform float ambientStrength;
uniform float diffuseStrength;
uniform float specularStrength;
uniform float shininess;
uniform sampler2D texture_diffuse1;

uniform float diffuseThresholds[3];
uniform float diffuseFactors[4];
uniform float specularThreshold;
uniform vec3 rimColor;
uniform float rimPower;
uniform float colorLevels;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPosition - FragPos);
    vec3 viewDir = normalize(viewPosition - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    // Ambient
    vec3 ambient = ambientStrength * lightColor;

    // Diffuse (Toon Shading)
    float diff = max(dot(norm, lightDir), 0.0);
    float diffuseFactor;
    if (diff > diffuseThresholds[0]) diffuseFactor = diffuseFactors[0];
    else if (diff > diffuseThresholds[1]) diffuseFactor = diffuseFactors[1];
    else if (diff > diffuseThresholds[2]) diffuseFactor = diffuseFactors[2];
    else diffuseFactor = diffuseFactors[3];
    vec3 diffuse = diffuseStrength * diffuseFactor * lightColor;

    // Specular (Toon Shading)
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    float specularFactor = (spec > specularThreshold) ? 1.0 : 0.0;
    vec3 specular = specularStrength * specularFactor * lightColor;

    // Rim Lighting
    float rim = pow(1.0 - max(dot(norm, viewDir), 0.0), rimPower);
    vec3 rimLighting = rim * rimColor;

    // Texture
    vec4 texColor = texture(texture_diffuse1, TexCoord);

    // Combine lighting and texture
    vec3 baseColor = (ambient + diffuse) * texColor.rgb;
    vec3 highlights = specular + rimLighting;
    vec3 result = baseColor + highlights;

    // Color Quantization
    if (colorLevels > 0.0) {
        result = floor(result * colorLevels) / colorLevels;
    }

    FragColor = vec4(result, texColor.a);
}