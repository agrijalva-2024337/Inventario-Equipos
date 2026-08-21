DROP DATABASE InventarioMultiempresa;

CREATE DATABASE InventarioMultiempresa;
GO

USE InventarioMultiempresa;
GO


-------------------- TABLAS -----------------------

-- Pais
CREATE TABLE  Pais(
	id_pais INT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR (100) NOT NULL,
	codigo_iso2 VARCHAR(2) NOT NULL,
	codigo_iso3 VARCHAR(3) NOT NULL,
	codigot_telefonico VARCHAR(5),
	moneda_local VARCHAR(10),
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo'
);
GO

-- Usuario

CREATE TABLE Usuario(
	id_usuario INT IDENTITY(1,1) PRIMARY KEY,
	nombre_completo VARCHAR(150) NOT NULL,
	correo VARCHAR(150) NOT NULL,
	usuario_login VARCHAR(50) NOT NULL,
	password_hash VARCHAR(255) NOT NULL,
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	fecha_creacion DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT UQ_Usuario_Correo UNIQUE (correo),
	CONSTRAINT UQ_Usuario_Login UNIQUE (usuario_login)
);
GO

-- Empresa

CREATE TABLE Empresa(
	id_empresa INT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR(150) NOT NULL,
	nit_codigo VARCHAR(50) NOT NULL,
	direccion VARCHAR(50) NOT NULL,
	telefono VARCHAR(30),
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	fecha_creacion DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT UQ_Empresa_Nit UNIQUE (nit_codigo)
);
GO

-- Relación usuario con empresa

CREATE TABLE Usuario_Empresa(
	id_usuario_empresa INT IDENTITY(1,1) PRIMARY KEY,
	id_usuario INT NOT NULL,
	id_empresa INT NOT NULL,
	rol VARCHAR(50) NOT NULL,
	empresa_predeterminada BIT NOT NULL DEFAULT 0,
	fecha_asignacion DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT FK_UsuarioEmpresa_Usuario FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario),
	CONSTRAINT FK_UsuarioEmpresa_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa),
	CONSTRAINT UQ_UsuarioEmpresa UNIQUE (id_usuario, id_empresa)
);
GO

-- Sede

CREATE TABLE Sede(
	id_sede INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	id_pais INT NOT NULL,
	nombre VARCHAR(100) NOT NULL,
	direccion VARCHAR(100) NOT NULL,
	ciudad VARCHAR(100) NOT NULL,
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	CONSTRAINT FK_Sede_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa),
	CONSTRAINT FK_Sede_Pais FOREIGN KEY (id_pais) REFERENCES Pais(id_pais)
);
GO

-- Ubicación

CREATE TABLE Ubicacion(
	id_ubicacion INT IDENTITY(1,1) PRIMARY KEY,
	id_sede INT NOT NULL,
	nombre VARCHAR(100) NOT NULL,
	descripcion VARCHAR(200),
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	CONSTRAINT FK_Ubicacion_Sede FOREIGN KEY (id_sede) REFERENCES Sede(id_sede)
);
GO

-- Area

CREATE TABLE Area(
	id_area INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	nombre VARCHAR(100) NOT NULL,
	descripcion VARCHAR(200),
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	CONSTRAINT FK_Area_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa)
);
GO

-- Responsable

CREATE TABLE Responsable(
	id_responsable INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	id_area INT NOT NULL,
	id_usuario INT NULL,
	nombre_completo VARCHAR(150) NOT NULL,
	cargo VARCHAR(100) NOT NULL,
	correo VARCHAR(150) NOT NULL,
	telefono VARCHAR(30) NOT NULL,
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	CONSTRAINT FK_Responsable_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa),
	CONSTRAINT FK_Responsable_Usuario FOREIGN KEY (id_Usuario) REFERENCES Usuario(id_usuario)
);
GO

-- Categoria Activo

CREATE TABLE Categoria_Activo(
	id_categoria INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	nombre VARCHAR(100) NOT NULL,
	descripcion VARCHAR(200),
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	CONSTRAINT FK_Categoria_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa)
);
GO

-- Estado Activo

CREATE TABLE Estado_Activo(
	id_estado INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	nombre VARCHAR(50) NOT NULL,
	descripcion VARCHAR(150),
	CONSTRAINT FK_EstadoActivo_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa)
);
GO

-- Tipo Mantenimiento

CREATE TABLE Tipo_Mantenimiento(
id_tipo_mantenimiento INT IDENTITY(1,1) PRIMARY KEY,
id_empresa INT NOT NULL,
nombre VARCHAR(50) NOT NULL,
descripcion VARCHAR(150),
CONSTRAINT FK_TipoMantenimiento_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa)
);
GO

-- Motivo Baja

CREATE TABLE Motivo_Baja(
	id_motivo_baja INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	nombre VARCHAR(50) NOT NULL,
	descripcion VARCHAR(150),
	CONSTRAINT FK_MotivoBaja_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa)
);
GO

-- Proveedor

CREATE TABLE Proveedor(
	id_proveedor INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	nombre VARCHAR(150) NOT NULL,
	nit VARCHAR(50),
	contacto VARCHAR(100),
	telefono VARCHAR(30) NOT NULL,
	correo VARCHAR(150),
	estado VARCHAR(20) NOT NULL DEFAULT 'Activo',
	CONSTRAINT FK_Proveedor_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa)
);
GO

-- Activo

CREATE TABLE Activo(
	id_activo INT IDENTITY(1,1) PRIMARY KEY,
	id_empresa INT NOT NULL,
	codigo_interno VARCHAR(50) NOT NULL,
	nombre VARCHAR(150) NOT NULL,
	descripcion VARCHAR(300) NOT NULL,
	id_categoria INT NOT NULL,
	marca VARCHAR(100) NOT NULL,
	modelo VARCHAR(100) NOT NULL,
	numero_serie VARCHAR(100) NOT NULL,
	fecha_compra DATE,
	costo_adquisicion DECIMAL(12,2)	NOT NULL,
	moneda VARCHAR(10),
	id_proveedor INT NULL,
	numero_factura VARCHAR(50),
	fecha_vencimiento_garantia DATE,
	id_sede INT NOT NULL,
	id_ubicacion INT NOT NULL,
	id_area INT NULL,
	id_responsable INT NULL,
	id_estado INT NOT NULL,
	observaciones VARCHAR(500),
	CONSTRAINT FK_Activo_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa),
	CONSTRAINT FK_Activo_Categoria FOREIGN KEY (id_categoria) REFERENCES Categoria_Activo(id_categoria),
	CONSTRAINT FK_Activo_Proveedor FOREIGN KEY (id_proveedor) REFERENCES Proveedor(id_proveedor),
	CONSTRAINT FK_Activo_Sede FOREIGN KEY (id_sede) REFERENCES Sede(id_sede),
	CONSTRAINT FK_Activo_Ubicación FOREIGN KEY (id_ubicacion) REFERENCES Ubicacion(id_Ubicacion),
	CONSTRAINT FK_Activo_Area FOREIGN KEY (id_area) REFERENCES Area(id_Area),
	CONSTRAINT FK_Activo_Responsable FOREIGN KEY (id_responsable) REFERENCES Responsable(id_responsable),
	CONSTRAINT FK_Activo_Estado FOREIGN KEY (id_estado) REFERENCES Estado_Activo(id_estado),
	CONSTRAINT UQ_Activo_CodigoInterno UNIQUE (id_empresa, codigo_interno)
 );
 GO

 -- Asignación
 
 CREATE TABLE Asignacion(
	id_asignacion INT IDENTITY(1,1) PRIMARY KEY,
	id_activo INT NOT NULL,
	id_responsable INT NOT NULL,
	id_ubicacion_uso INT NOT NULL,
	fecha_asignacion DATETIME NOT NULL DEFAULT GETDATE(),
	entregado_por VARCHAR(150) NOT NULL,
	recibido_por VARCHAR(150) NOT NULL,
	fecha_devolucion DATETIME NULL,
	activa BIT NOT NULL DEFAULT 1,
	observaciones VARCHAR(300),
	CONSTRAINT FK_Asignacion_Activo FOREIGN KEY (id_activo) REFERENCES Activo(id_activo),
	CONSTRAINT FK_Asignacion_Responsable FOREIGN KEY (id_responsable) REFERENCES Responsable(id_responsable),
    CONSTRAINT FK_Asignacion_Ubicacion   FOREIGN KEY (id_ubicacion_uso) REFERENCES Ubicacion(id_ubicacion)
 );
 GO

 CREATE TABLE Traslado(
	id_traslado INT IDENTITY(1,1) PRIMARY KEY,
	id_activo INT NOT NULL,
	id_ubicacion_origen INT NOT NULL,
	id_ubicacion_destino INT NOT NULL,
	fecha_traslado DATETIME NOT NULL DEFAULT GETDATE(),
	motivo VARCHAR(200),
	responsable_traslado VARCHAR(150),
	CONSTRAINT FK_Traslado_Activo FOREIGN KEY (id_activo) REFERENCES Activo(id_activo),
	CONSTRAINT FK_Traslado_Origen   FOREIGN KEY (id_ubicacion_origen)  REFERENCES Ubicacion(id_ubicacion),
    CONSTRAINT FK_Traslado_Destino  FOREIGN KEY (id_ubicacion_destino) REFERENCES Ubicacion(id_ubicacion)
 );
 GO



 -- Mantenimiento

CREATE TABLE Mantenimiento(
	id_mantenimiento INT IDENTITY(1,1) PRIMARY KEY,
	id_activo INT NOT NULL,
	id_tipo_mantenimiento INT NOT NULL,
	id_proveedor INT NULL,
	fecha_programada DATE NOT NULL,
	fecha_realizado DATE NNULL,
	responsable VARCHAR(150),
	descripcion_problema VARCHAR(300),
	trabajo_realizado VARCHAR(150),
	costo DECIMAL(12,2) NOT NULL,
	numero_factura VARCHAR(50),
	estado_mantenimiento VARCHAR(30),
	CONSTRAINT FK_Mantenimiento_Activo   FOREIGN KEY (id_activo)             REFERENCES Activo(id_activo),
    CONSTRAINT FK_Mantenimiento_Tipo     FOREIGN KEY (id_tipo_mantenimiento) REFERENCES Tipo_Mantenimiento(id_tipo_mantenimiento), 
    CONSTRAINT FK_Mantenimiento_Proveedor FOREIGN KEY (id_proveedor)         REFERENCES Proveedor(id_proveedor)
);
GO

-- Baja

CREATE TABLE Baja (
    id_baja INT IDENTITY(1,1) PRIMARY KEY,
    id_activo INT NOT NULL,
    id_motivo_baja INT NOT NULL,
    fecha_baja DATE NOT NULL DEFAULT GETDATE(),
    documento_referencia VARCHAR(100),
    autorizado_por VARCHAR(150),
    observaciones VARCHAR(300),
    CONSTRAINT FK_Baja_Activo FOREIGN KEY (id_activo) REFERENCES Activo(id_activo),
    CONSTRAINT FK_Baja_MotivoBaja FOREIGN KEY (id_motivo_baja) REFERENCES Motivo_Baja(id_motivo_baja)
);
GO

-- Invetario Fisico
CREATE TABLE Inventario_Fisico (
    id_inventario INT IDENTITY(1,1) PRIMARY KEY,
    id_empresa INT NOT NULL,
    id_sede INT NULL,
    id_ubicacion INT NULL,
    fecha_inicio DATETIME NOT NULL DEFAULT GETDATE(),
    fecha_cierre DATETIME NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Abierto',
    id_usuario_responsable INT NOT NULL,
    observaciones VARCHAR(300),
    CONSTRAINT FK_InventarioFisico_Empresa   FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa),
    CONSTRAINT FK_InventarioFisico_Sede      FOREIGN KEY (id_sede) REFERENCES Sede(id_sede),
    CONSTRAINT FK_InventarioFisico_Ubicacion FOREIGN KEY (id_ubicacion) REFERENCES Ubicacion(id_ubicacion),
    CONSTRAINT FK_InventarioFisico_Usuario   FOREIGN KEY (id_usuario_responsable) REFERENCES Usuario(id_usuario)
);
GO

--Detalle Inventario

CREATE TABLE Detalle_Inventario (
    id_detalle INT IDENTITY(1,1) PRIMARY KEY,
    id_inventario INT NOT NULL,
    id_activo INT NOT NULL,
    encontrado BIT NOT NULL DEFAULT 0,
    id_ubicacion_encontrada INT NULL,
    estado_fisico_observado VARCHAR(100),
    observaciones VARCHAR(300),
    fecha_verificacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DetalleInventario_Inventario FOREIGN KEY (id_inventario) REFERENCES Inventario_Fisico(id_inventario),
    CONSTRAINT FK_DetalleInventario_Activo FOREIGN KEY (id_activo) REFERENCES Activo(id_activo),
    CONSTRAINT FK_DetalleInventario_Ubicacion  FOREIGN KEY (id_ubicacion_encontrada) REFERENCES Ubicacion(id_ubicacion)
);
GO


-- Historial Cambios

CREATE TABLE Historial_Cambios (
    id_historial INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    id_empresa INT NOT NULL,
    fecha_hora DATETIME NOT NULL DEFAULT GETDATE(),
    tipo_operacion VARCHAR(30) NOT NULL,
    entidad_afectada VARCHAR(100) NOT NULL,
    id_registro_afectado INT NOT NULL,
    informacion_anterior VARCHAR(MAX),
    informacion_nueva VARCHAR(MAX),
    CONSTRAINT FK_HistorialCambios_Usuario FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario),
    CONSTRAINT FK_HistorialCambios_Empresa FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa)
);
GO



CREATE INDEX IX_Activo_Empresa ON Activo(id_empresa);
CREATE INDEX IX_Activo_Estado ON Activo(id_estado);
CREATE INDEX IX_Asignacion_Activo ON Asignacion(id_activo);
CREATE INDEX IX_Traslado_Activo ON Traslado(id_activo);
CREATE INDEX IX_Mantenimiento_Activo ON Mantenimiento(id_activo);
CREATE INDEX IX_HistorialCambios_Empresa ON Historial_Cambios(id_empresa);

