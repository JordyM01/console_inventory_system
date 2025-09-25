# Sistema de Gestión de Inventario en Consola

Este proyecto es una aplicación de consola robusta desarrollada en C# y .NET para la gestión de un inventario de ferretería. Implementa una Interfaz de Usuario de Texto (TUI) avanzada, utilizando principios de Programación Orientada a Objetos para crear una experiencia modular y mantenible.

## Características

- **Gestión Completa de Productos (CRUD)**: Añadir, visualizar, buscar, actualizar y eliminar productos del inventario.

- **Interfaz de Usuario de Texto (TUI)**: Una experiencia de usuario fluida y sin parpadeos gracias a un motor de renderizado optimizado (double buffering).

- **Componentes Reutilizables**: La UI está construida con componentes modulares como Tablas, Formularios y Botones, facilitando la escalabilidad.

- **Persistencia de Datos**: El inventario se guarda en un archivo `inventory.json`, lo que permite una fácil lectura y manipulación externa.

- **Navegación Intuitiva**: Soporte completo para navegación con teclado, incluyendo flechas, Tab, Enter y Escape.

## Diseño de las Vistas (Arte ASCII)

### Vista Principal (Menú Principal)

```
+-----------------------------------------------------------------------+
|                                                                       |
|           Sistema de gestion de inventario en consola                |
|                                                                       |
+-----------------------------------------------------------------------+


       >> Agregar producto    [ A ]         Eliminar producto  [ D ]


          Mostrar productos   [ S ]         Acerca del software[ I ]


          Actualizar producto[ U ]         Salir              [ESC]

```

### Vista de Lista de Productos (Mostrar Productos)

```
+-----------------------------------------------------------------------+
| / Mostrar productos                                                   |
|                                                                       |
| Inicio                      +--------------------------------------+  |
|                             |                                      |  |
| Agregar producto            +--------------------------------------+  |
|                                                                       |
| > Mostrar productos         +--------------------------------------+  |
|                             | Producto            | SKU     | Precio|  |
| Actualizar producto         +--------------------------------------+  |
|                             | Martillo de Uña 16oz| HM-001  |¢450,50|  |
| Eliminar producto           | Destornillador Phil | DE-PH2  |¢180,00|  |
|                             | Cinta Metrica 5m    | CM-5M   |¢275,75|  |
| Acerca del software         | Llave Ajustable 8"  | LL-AJ8  |¢520,00|  |
|                             | Tubo PVC Sanitario  | PL-PVC12|¢195,25|  |
| Salir                       | Bombillo LED 9W     | EL-BLED9|¢110,00|  |
|                             +--------------------------------------+  |
+-----------------------------------------------------------------------+
```

### Vista de Formulario (Agregar Producto)

```
+-----------------------------------------------------------------------+
| / Agregar producto                                                    |
|                                                                       |
| Inicio                      +--- Nuevo Producto -------------------+  |
|                             |                                       |  |
| > Agregar producto          |    +----------------------+           |  |
|                             | Id | 7EB4B54A             |           |  |
| Mostrar productos           |    +----------------------+           |  |
|                             |    +---------------------------------+ |  |
| Actualizar producto         | SKU| ████████████████████████████████| |  |
|                             |    +---------------------------------+ |  |
| Eliminar producto           |    +---------------------------------+ |  |
|                             |Prod|                                 | |  |
| Acerca del software         |    +---------------------------------+ |  |
|                             |         [+] |    0   | [-]             |  |
| Salir                       |Cantidad     +--------+                 |  |
|                             |    +---------------------------------+ |  |
|                             |Cat |                                 | |  |
|                             |    +---------------------------------+ |  |
|                             |         [+] |    0   | [-]             |  |
|                             |Cant Minima  +--------+                 |  |
|                             |    +---------------------------------+ |  |
|                             |Desc|                                 | |  |
|                             |    +---------------------------------+ |  |
|                             |         [+] |  ¢0,00 | [-]             |  |
|                             |Precio       +--------+                 |  |
|                             |                                       |  |
|                             |                                       |  |
|                             | [ Cancelar / ESC ]  [ Guardar / Enter]|  |
+-----------------------------------------------------------------------+
```

### Vista de Eliminación de Productos

```
+-----------------------------------------------------------------------+
| / Eliminar producto                                                   |
|                                                                       |
| Inicio                      +--------------------------------------+  |
|                             |                                      |  |
| Agregar producto            +--------------------------------------+  |
|                                                                       |
| Mostrar productos           +--------------------------------------+  |
|                             | Producto        |SKU   |Precio |Acción|  |
| Actualizar producto         +--------------------------------------+  |
|                             |Martillo de Uña  |HM-001|¢450,50| [ X ]|  |
| > Eliminar producto         |Destornillador   |DE-PH2|¢180,00| [ X ]|  |
|                             |Cinta Metrica 5m |CM-5M |¢275,75| [ X ]|  |
| Acerca del software         |Llave Ajustable  |LL-AJ8|¢520,00| [ X ]|  |
|                             |Tubo PVC Sanitar |PL-PVC|¢195,25| [ X ]|  |
| Salir                       |Bombillo LED 9W  |EL-BLE|¢110,00| [ X ]|  |
|                             +--------------------------------------+  |
+-----------------------------------------------------------------------+
```

## Arquitectura

El sistema sigue una arquitectura limpia con una clara separación de responsabilidades en tres capas principales:

- **Capa de Interfaz de Usuario (UI)**: Responsable de todo lo que el usuario ve. No contiene lógica de negocio y está compuesta por Vistas y Componentes reutilizables.

- **Capa de Lógica de Negocio**: Contiene las reglas y operaciones del dominio (InventoryManager). Es agnóstica a la UI y a los datos.

- **Capa de Datos**: Gestiona la lectura y escritura de la información en el archivo `inventory.json`.

## Requisitos

- .NET 6.0 SDK o superior.

## Cómo Empezar

1. Clona el repositorio:

   ```bash
   git clone <URL_DEL_REPOSITORIO>
   ```

2. Navega al directorio del proyecto:

   ```bash
   cd console_inventory_system
   ```

3. Ejecuta la aplicación:

   ```bash
   dotnet run
   ```

La primera vez que se ejecute, se creará un archivo `inventory.json` vacío.
