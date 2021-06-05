/* crypt method to hash our passwords */
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

/* JWT for authentication */
CREATE TYPE usr.jwt_token as (
  role text,      --db role of the user
  exp integer,    --expiry date as the unix epoch
  user_id text,   --uuid of the user,
  username text   --username used to sign in, user's email in our case
);

/* Create user table */
CREATE TABLE usr.user (
  id UUID PRIMARY KEY default gen_random_uuid(),
  role TEXT,
  first_name TEXT,
  last_name TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
 );

COMMENT ON TABLE usr.user IS 'Funani users.';

COMMENT ON COLUMN usr.user.id IS '@omit create,update
User id';
COMMENT ON COLUMN usr.user.role IS '@omit create,update
User role';
COMMENT ON COLUMN usr.user.created_at IS '@omit create,update
Creation date';

COMMENT ON COLUMN usr.user.first_name IS 'First name';
COMMENT ON COLUMN usr.user.last_name IS 'Family name';

-- the sensitive account information

CREATE TABLE prv.account (
  id UUID PRIMARY KEY REFERENCES usr.user (id) ON DELETE CASCADE,
  email TEXT NOT NULL UNIQUE CHECK (email ~* '^.+@.+\..+$'),
  password_hash TEXT NOT NULL
);

CREATE INDEX user_account_email_idx ON prv.account (email);

-- registration function
CREATE FUNCTION usr.register(first_name text, last_name text, email text, password text)
  RETURNS usr.user
  AS $$
DECLARE
  person usr.user;
BEGIN
  INSERT INTO usr.user (first_name, last_name)
    VALUES (first_name, last_name)
  RETURNING
    * INTO person;
  INSERT INTO prv.account (id, email, password_hash)
    VALUES (person.id, email, crypt(password, gen_salt('bf')));
  RETURN person;
END;
$$
LANGUAGE plpgsql
STRICT
SECURITY DEFINER;

-- authentication function
CREATE FUNCTION usr.authenticate(email text, password text)
  RETURNS usr.jwt_token
  AS $$
DECLARE
  found prv.account;
BEGIN
  SELECT
    * INTO found
    FROM
      prv.account
    WHERE account.email = authenticate.email;
    IF found.password_hash = crypt(authenticate.password, found.password_hash) THEN
      RETURN (
        'editor', -- TODO: use role set
        extract(epoch from now() + interval '7 days'),
        found.id,
        found.email
        )::usr.jwt_token;
    ELSE
      RETURN NULL;
    END IF;
END;
$$
LANGUAGE plpgsql
STRICT
SECURITY DEFINER;

-- permissions

GRANT ALL privileges ON TABLE usr.user TO editor;
GRANT USAGE ON SCHEMA usr TO anonymous;
GRANT EXECUTE ON FUNCTION usr.register TO anonymous;
GRANT EXECUTE ON FUNCTION usr.authenticate TO anonymous,editor,admin;
