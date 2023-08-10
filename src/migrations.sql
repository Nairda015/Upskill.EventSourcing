--create table if not exists with name Categories and columns Guid Id, string Name, Guid? ParentId
CREATE DATABASE "Upskill"
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

CREATE TABLE IF NOT EXISTS public."Categories"
(
    "Id" UUID NOT NULL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "ParentId" UUID NULL
)