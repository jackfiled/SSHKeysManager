CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "EmailAddress" TEXT NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221111021111_InitialCreate', '7.0.0');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Users" ADD "Password" TEXT NOT NULL DEFAULT '';

ALTER TABLE "Users" ADD "Permission" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221112152354_AddUserPasswordAndPermission', '7.0.0');

COMMIT;

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Servers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Servers" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Token" TEXT NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221113061313_AddServer', '7.0.0');

COMMIT;

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Keys" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Keys" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "Key" TEXT NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221113071219_AddSSHKeys', '7.0.0');

COMMIT;

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Relations" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Relations" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "ServerId" INTEGER NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221113081038_AddUserServerRelation', '7.0.0');

COMMIT;