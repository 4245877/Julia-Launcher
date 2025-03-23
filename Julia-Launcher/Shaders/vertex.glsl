#version 330 core
layout (location = 0) in vec3 aPos;         // Позиция вершины
layout (location = 1) in vec3 aNormal;      // Нормаль вершины
layout (location = 2) in vec2 aTexCoords;   // Текстурные координаты

out vec3 FragPos;       // Позиция фрагмента в мировом пространстве
out vec3 Normal;        // Трансформированная нормаль
out vec2 TexCoords;     // Текстурные координаты
out vec3 LightPos[4];   // Позиции источников света в пространстве вида

uniform mat4 model;         // Матрица модели
uniform mat4 view;          // Матрица вида
uniform mat4 projection;    // Матрица проекции
uniform vec3 lightPos[4];   // Позиции источников света (в мировом пространстве)

void main()
{
    // Позиция вершины в мировом пространстве
    FragPos = vec3(model * vec4(aPos, 1.0));
    
    // Трансформация нормали с учетом матрицы модели
    Normal = mat3(transpose(inverse(model))) * aNormal;
    
    // Передача текстурных координат
    TexCoords = aTexCoords;
    
    // Преобразование позиций источников света в пространство вида
    for (int i = 0; i < 4; i++) {
        LightPos[i] = vec3(view * vec4(lightPos[i], 1.0));
    }
    
    // Итоговая позиция вершины в пространстве отсечения
    gl_Position = projection * view * vec4(FragPos, 1.0);
}