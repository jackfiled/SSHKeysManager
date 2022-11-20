﻿CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "EmailAddress" TEXT NOT NULL,
    "Password" TEXT NOT NULL DEFAULT '',
    "Permission" INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE "Servers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Servers" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Token" TEXT NOT NULL
);

CREATE TABLE "Keys" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Keys" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "Key" TEXT NOT NULL
);

CREATE TABLE "Relations" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Relations" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "ServerId" INTEGER NOT NULL
);
