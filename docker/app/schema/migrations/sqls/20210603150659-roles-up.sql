/* roles */
CREATE ROLE anonymous NOINHERIT;
CREATE ROLE admin;
CREATE ROLE editor;

GRANT usage ON SCHEMA usr TO admin, editor;
