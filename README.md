# Mesa de Partes Digital

Prototipo de consola en **C#** para registrar y consultar expedientes de una mesa de partes, desarrollado como proyecto del curso **Fundamentos de Programación (CIIN1205P)** de la Universidad Privada del Norte.

> **Estado:** versión 0.1 (avance de la Evaluación Parcial). Las funciones pendientes están en la sección [Hoja de ruta](#hoja-de-ruta).

## Contexto

La Ley N.° 31170 obliga a las entidades del Estado a implementar mesas de partes digitales. El desafío plantea una oficina municipal que lleva su registro en papel y necesita un prototipo propio que registre, valide, busque, ordene y guarde expedientes. Este repositorio contiene ese prototipo.

## Funciones

| Función | Estado |
|---|---|
| Registrar expedientes (DNI, nombre, tipo de trámite y folios) con código correlativo | Implementado |
| Listar todos los expedientes | Implementado |
| Buscar expedientes por DNI | Implementado |
| Validar los datos ingresados | Pendiente |
| Código con formato fijo `EXP-AAAA-NNNNNN` | Pendiente |
| Guardar y cargar expedientes en un archivo de texto | Pendiente |
| Buscar por código de expediente | Pendiente |
| Ordenar expedientes por código (inserción) | Pendiente |

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior (o Visual Studio 2022).

## Cómo ejecutarlo

```bash
git clone https://github.com/<usuario>/mesa-de-partes-digital.git
cd mesa-de-partes-digital
dotnet run --project src/MesaDePartes
```

También puedes abrir `src/MesaDePartes/MesaDePartes.csproj` con Visual Studio y presionar F5.

## Estructura del repositorio

```
mesa-de-partes-digital/
├── src/
│   └── MesaDePartes/
│       ├── MesaDePartes.csproj
│       ├── Program.cs                  # menú y funciones de registro, listado y búsqueda
│       └── Models/
│           └── RegistroDocumento.cs    # modelo del expediente
├── docs/
│   ├── Informe_EP_Mesa_de_Partes_Digital.docx
│   ├── GUIA_GIT.md                     # flujo de trabajo del equipo
│   └── evidencias/                     # capturas de las pruebas
├── .gitignore
└── README.md
```

## Pruebas

Casos ejecutados con la versión 0.1 (capturas en [`docs/evidencias`](docs/evidencias)):

| ID | Caso | Resultado |
|---|---|---|
| T-01 | Registro de un expediente válido | Correcto |
| T-02 | Listado general | Correcto |
| T-03 | Búsqueda por DNI con un resultado | Correcto |
| T-04 | Búsqueda por DNI con dos resultados | Correcto |

Los casos T-05 a T-10 (validaciones, persistencia, búsqueda por código y ordenamiento) están descritos en el informe y se ejecutarán con la versión final.

## Hoja de ruta

1. `feat/validaciones`: validar DNI, nombre, trámite y folios, y volver a pedir el dato si es inválido.
2. `feat/codigo-expediente`: generar `EXP-AAAA-NNNNNN` calculando el siguiente número desde los registros.
3. `feat/persistencia-archivo`: guardar y cargar los expedientes desde un archivo de texto.
4. `feat/buscar-por-codigo`: nueva opción de búsqueda.
5. `feat/ordenar-por-codigo`: ordenamiento por inserción sobre un arreglo.
6. `docs/pruebas-finales`: ejecutar T-05 a T-10 y actualizar el informe.

## Equipo

| Integrante | Código | GitHub |
|---|---|---|
| Diego Alonso Villagas Huamani | N00546269 | @diegovhuamani |
| Valentino Ettore Flores Carpio | N00551932 | @vaal67 |
| Diogo David Pérez Piña | N00547056 | @diogo2020pp-spec |
| Sebastián Paul Ramírez Carbonell | N00540389 | @ |

**Docente:** César Edinson Aguirre Rodríguez
