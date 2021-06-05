-- REVOKE ALL ON usr.user FROM editor;
-- REVOKE ALL ON usr.user FROM admin;
-- REVOKE ALL ON usr.user FROM anonymous;

DROP FUNCTION usr.authenticate;
DROP FUNCTION usr.register;

DROP TABLE prv.account;
DROP TABLE usr.user;

DROP TYPE usr.jwt_token;

DROP EXTENSION IF EXISTS "pgcrypto";
