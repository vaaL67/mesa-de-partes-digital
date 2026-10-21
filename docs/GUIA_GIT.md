# Guía de trabajo con Git y GitHub

Flujo sencillo para un equipo de cuatro personas. El objetivo es que el historial de commits muestre el trabajo real y progresivo de cada integrante.

## 1. Configuración (cada integrante, una sola vez)

```bash
git config --global user.name "Nombre Apellido"
git config --global user.email "correo-de-su-cuenta-github@ejemplo.com"
```

El correo debe ser el de **su propia cuenta de GitHub**, para que los commits aparezcan a su nombre.

## 2. Subir el proyecto (lo hace un solo integrante)

1. En GitHub: **New repository** → nombre `mesa-de-partes-digital` → **sin** README, sin .gitignore, sin licencia → Create.
2. En su PC, dentro de la carpeta del proyecto:

```bash
git init -b main
git add .
git commit -m "chore: importar la versión 0.1 del prototipo"
git remote add origin https://github.com/<usuario>/mesa-de-partes-digital.git
git push -u origin main
git tag v0.1.0
git push origin v0.1.0
```

3. En GitHub: **Settings → Collaborators** → agregar a los otros tres integrantes y al docente (si lo pide).

## 3. Cada integrante clona el repositorio

```bash
git clone https://github.com/<usuario>/mesa-de-partes-digital.git
cd mesa-de-partes-digital
```

## 4. Flujo para cada tarea nueva

```bash
git checkout main
git pull
git checkout -b feat/validaciones        # una rama por tarea

# ... programar, probar con dotnet run ...

git add src/
git commit -m "feat: validar DNI de 8 dígitos"
git push -u origin feat/validaciones
```

Luego, en GitHub, abrir un **Pull Request** hacia `main`, pedir que otro integrante lo revise y hacer **Merge**. Después, cada quien actualiza su copia con `git checkout main && git pull`.

Reglas del equipo:

- Nadie sube cambios directamente a `main`: todo entra por Pull Request.
- Commits pequeños y frecuentes: un commit por cambio con sentido (una función, una corrección).
- Antes de cada commit, comprobar que el programa compila y corre.

## 5. Mensajes de commit

Formato: `tipo: descripción corta en infinitivo`.

| Tipo | Cuándo usarlo | Ejemplo |
|---|---|---|
| `feat` | Función nueva | `feat: agregar búsqueda por código` |
| `fix` | Corrección de un error | `fix: rellenar con ceros el correlativo` |
| `docs` | Documentación | `docs: agregar capturas de las pruebas` |
| `test` | Casos de prueba | `test: documentar caso T-05` |
| `refactor` | Reorganizar sin cambiar el comportamiento | `refactor: mover validaciones a una clase` |
| `chore` | Tareas de mantenimiento | `chore: actualizar .gitignore` |

## 6. Reparto sugerido de las tareas pendientes

Ajústenlo entre ustedes. Cada tarea corresponde a una rama y a un requerimiento del informe.

| Rama | Requerimiento | Qué hacer | Responsable |
|---|---|---|---|
| `feat/validaciones` | RF-02 | Clase `Validador` con `DniValido`, `NombreValido`, `TramiteValido` y `FoliosValidos`; repetir la pregunta si el dato es inválido | [integrante] |
| `feat/codigo-expediente` | RF-03 | `GenerarCodigo` con formato `EXP-AAAA-NNNNNN`, año actual y siguiente número calculado desde la lista | [integrante] |
| `feat/persistencia-archivo` | RF-06 | Clase `ArchivoExpedientes` con `Guardar` y `Cargar` (un expediente por línea, separador `;`) | [integrante] |
| `feat/buscar-por-codigo` | RF-04 | Nueva opción de menú y función de búsqueda por código | [integrante] |
| `feat/ordenar-por-codigo` | RF-05 | Ordenamiento por inserción sobre `RegistroDocumento[]` y opción de menú | [integrante] |
| `docs/pruebas-finales` | T-05 a T-10 | Ejecutar los casos, tomar capturas en `docs/evidencias` y actualizar el informe | [integrante] |

## 7. Evidencia para el informe y la sustentación

- Captura de la pestaña **Commits** de GitHub, con fechas y autores.
- Captura del grafo de ramas: **Insights → Network**, o en la terminal:

```bash
git log --oneline --graph --decorate --all
```

- Enlace al repositorio en la portada del informe.

## 8. Recomendaciones

- No cambien las fechas de los commits ni reescriban el historial con `git rebase` o `git push --force` sobre `main`: el historial debe mostrar lo que de verdad se hizo y cuándo.
- Si el docente revisa el repositorio, el reparto de commits entre los cuatro integrantes también cuenta como evidencia de trabajo en equipo.
- Si el repositorio es privado, agreguen al docente como colaborador o cámbienlo a público antes de entregar.
