CREATE TABLE IF NOT EXISTS public."Categories"
(
    "Id" UUID NOT NULL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "ParentId" UUID NULL
);


INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174000', 'Cars', null);
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174001', 'Planes', null);
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174002', 'BMW', '123e4567-e89b-12d3-a456-426614174000');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174003', 'Mercedes', '123e4567-e89b-12d3-a456-426614174000');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174004', 'Boeing', '123e4567-e89b-12d3-a456-426614174001');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174005', 'Airbus', '123e4567-e89b-12d3-a456-426614174001');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174006', 'X3', '123e4567-e89b-12d3-a456-426614174002');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174007', 'X5', '123e4567-e89b-12d3-a456-426614174002');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174008', 'C-Class', '123e4567-e89b-12d3-a456-426614174003');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174009', 'E-Class', '123e4567-e89b-12d3-a456-426614174003');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174010', '747', '123e4567-e89b-12d3-a456-426614174004');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174011', '787', '123e4567-e89b-12d3-a456-426614174004');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174012', 'A320', '123e4567-e89b-12d3-a456-426614174005');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174013', 'A380', '123e4567-e89b-12d3-a456-426614174005');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174014', 'Motorcycles', '123e4567-e89b-12d3-a456-426614174006');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174015', 'Scooters', '123e4567-e89b-12d3-a456-426614174006');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174016', 'Choppers', '123e4567-e89b-12d3-a456-426614174006');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174017', 'Dirt Bikes', '123e4567-e89b-12d3-a456-426614174006');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174018', 'Yachts', '123e4567-e89b-12d3-a456-426614174007');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174019', 'Sailboats', '123e4567-e89b-12d3-a456-426614174007');
INSERT INTO public."Categories" ("Id", "Name", "ParentId") VALUES ('123e4567-e89b-12d3-a456-426614174020', 'Fishing Boats', '123e4567-e89b-12d3-a456-426614174007');
