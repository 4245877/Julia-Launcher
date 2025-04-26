#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;
in vec4 LightSpaceFragPos;

uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform vec3 viewPosition;
uniform float ambientStrength;
uniform float diffuseStrength;
uniform float specularStrength;
uniform float shininess;
uniform sampler2D texture_diffuse1;
uniform sampler2D shadowMap;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPosition - FragPos);
    vec3 viewDir = normalize(viewPosition - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    // Ambient (low to emphasize shadows)
    vec3 ambient = ambientStrength * lightColor;

    // Diffuse with quantization for toon effect
    float diff = max(dot(norm, lightDir), 0.0);
    float diffuseFactor;
    if (diff > 0.7) {
        diffuseFactor = 1.0;    // Fully lit
    } else if (diff > 0.3) {
        diffuseFactor = 0.6;    // Mid-tone
    } else {
        diffuseFactor = 0.2;    // Shadow
    }
    vec3 diffuse = diffuseStrength * diffuseFactor * lightColor;

    // Specular with hard cutoff
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    vec3 specular = vec3(0.0);
    if (spec > 0.5) {
        specular = specularStrength * lightColor;
    }

    // Texture
    vec4 texColor = texture(texture_diffuse1, TexCoord);

    // Shadow calculation with PCF
    float shadow = 0.0;
    vec4 lightSpacePos = LightSpaceFragPos / LightSpaceFragPos.w;
    vec3 projCoords = lightSpacePos.xyz * 0.5 + 0.5;
    if (projCoords.z <= 1.0 && projCoords.x >= 0.0 && projCoords.x <= 1.0 && projCoords.y >= 0.0 && projCoords.y <= 1.0) {
        vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
        for(int x = -1; x <= 1; ++x) {
            for(int y = -1; y <= 1; ++y) {
                vec2 offset = vec2(x, y) * texelSize;
                float closestDepth = texture(shadowMap, projCoords.xy + offset).r;
                shadow += projCoords.z > closestDepth ? 1.0 : 0.0;
            }
        }
        shadow /= 9.0;
    } else if (projCoords.z > 1.0) {
        shadow = 1.0; // Beyond light's far plane, assume in shadow
    }

    // Combine lighting with texture and shadow
    vec3 directLight = diffuse + specular;
    vec3 lighting = ambient + directLight * (1.0 - shadow);
    vec3 result = lighting * texColor.rgb;
    FragColor = vec4(result, texColor.a);
}