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

    // Combine lighting with texture
    vec3 result = (ambient + diffuse + specular) * texColor.rgb;
    FragColor = vec4(result, texColor.a);
}