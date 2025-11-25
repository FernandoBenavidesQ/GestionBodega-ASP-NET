# GestionBodega
Repositorio del proyecto de gestión de materiales para una bodega de construcción.  
Este proyecto corresponde al entregable final del examen/práctica de la asignatura.

## Tecnologías utilizadas

- ASP.NET/ ASP.NET Core
- C#  
- Entity Framework / ADO.NET 
- SQL Server como motor de base de datos  
- Visual Studio (versión: …)  
- GitHub como control de versiones

## Objetivo del proyecto

El sistema permite gestionar los ingresos y salidas de materiales en una bodega de construcción, con control de stock, reportes de movimientos y consulta de inventario.  
Funciones principales:

- Registrar materiales (descripción, unidad, stock, tipo, etc.)  
- Seleccionar categorías de materiales  
- Registrar movimientos de entrada y salidas (fecha, cantidad, personal encargado, etc.(Todo esto en tickets con número de cotización))  
- Consultar el stock actualizado por material y categoría  
- Visualizar historial de movimientos  
- Añadir Usuarios al Personal con diferentes cargos.
- Varios.


## Configuración e instalación

1. Clonar el repositorio  
   git clone https://github.com/FernandoBenavidesQ/GestionBodega-ASP-NET.git
   Abrir la solución en Visual Studio.

2. Configurar la cadena de conexión a la base de datos en el archivo appsettings.json (o donde se encuentre). Por ejemplo:
"ConnectionStrings": {
  "DefaultConnection": "Server=(TU_SERVIDOR);Database=GestionBodegaDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}

3. Crear la base de datos:

Ejecutar el archivo BD/Script_GestionBodega.sql en SQL Server Management Studio.
Una vez creada la base de datos y configurada la cadena, compilar y ejecutar el proyecto.
Desde la interfaz, iniciar sesión (si corresponde) o acceder al menú principal.
Probar las funcionalidades: registrar materiales al catalogo, divididas por select en categorías, realizar movimientos, editar stock, solicitar materiales vinculadas a personal mas tickets, realizar devoluciones restantes genera cierre de tickets, etc.

Usuario de prueba

Usuario: fernando.benavides@virginiogomez.cl
Contraseña: Fernandi

Consideraciones para la base de datos

El script SQL incluye la creación del esquema (tablas: Material, Categoria, Movimiento, Personal, etc.) + datos de ejemplo.
Asegúrese de que el usuario de la base de datos tenga permisos para crear tablas, insertar datos y consultar.

Autor

Nombre: Fernando Benavides Quiajda

Carrera / Asignatura: TNS Analista Programador / Construcción de Software

Fecha de entrega: (25/11/2025)
