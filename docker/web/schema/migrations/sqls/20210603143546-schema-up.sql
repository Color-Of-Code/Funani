CREATE SCHEMA usr; -- user shared area
CREATE SCHEMA prv; -- private user data (not accessible via postgraphile)
CREATE SCHEMA sys; -- system space for background jobs

ALTER DEFAULT privileges REVOKE EXECUTE ON functions FROM public;
