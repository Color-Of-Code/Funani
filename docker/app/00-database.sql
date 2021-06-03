\connect funani;

/* crypt method to hash our passwords */
CREATE extension IF NOT EXISTS "pgcrypto";

/* roles */
CREATE ROLE anonymous NOINHERIT;
CREATE ROLE admin;
CREATE ROLE editor;

/* Create user table */
CREATE TABLE public.user (
  id UUID PRIMARY KEY default gen_random_uuid(),
  role TEXT,
  firstname TEXT,
  lastname TEXT,
  email TEXT NOT NULL UNIQUE check (email ~* '^.+@.+\..+$'),
  password_hash TEXT,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE public.user IS 'Funani users.';

/* JWT for authentication */
CREATE TYPE public.jwt_token as (
  role text,      --db role of the user
  exp integer,    --expiry date as the unix epoch
  user_id text,   --uuid of the user,
  username text   --username used to sign in, user's email in our case
);

/* authentication function */
create function public.authenticate(email text, password text) returns public.jwt_token as $$
declare account public.user;
begin
select a.* into account
from public.user as a
where a.email = authenticate.email;
if account.password_hash = crypt(password, account.password_hash) then return (
  account.role,
      extract(epoch from now() + interval '7 days'),
      account.id,
      account.email
    )::public.jwt_token;
else return null;
end if;
end;
$$ language plpgsql strict security definer;

/* Create post table */
CREATE TABLE public.post (
  id UUID PRIMARY KEY default gen_random_uuid(),
  title TEXT,
  body TEXT,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  author_id UUID NOT NULL REFERENCES public.user(id)
);
COMMENT ON TABLE public.post IS 'Funani posts written by a user.';

/* Create MIME type table */
CREATE TABLE public.mime_type (
  id SERIAL PRIMARY KEY,
  mime TEXT,
  is_binary BOOLEAN,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE public.mime_type IS 'Known MIME types.';

/* Create files table */
CREATE TABLE public.file (
  hash TEXT NOT NULL PRIMARY KEY,
  mime_id INTEGER REFERENCES public.mime_type(id),
  size INTEGER NOT NULL,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE public.file IS 'files.';

/* Create root_paths table */
CREATE TABLE public.root_path (
  id SERIAL PRIMARY KEY,
  path TEXT NOT NULL,
  name TEXT,
  description TEXT,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE public.root_path IS 'root paths.';

/* Create paths table */
CREATE TABLE public.path (
  id SERIAL PRIMARY KEY,
  root_path_id INTEGER REFERENCES public.root_path(id),
  path TEXT NOT NULL,
  mtime INTEGER,
  size INTEGER,
  hash TEXT
);
COMMENT ON TABLE public.path IS 'paths.';

/* Create metadata table */
CREATE TABLE public.metadata (
  id SERIAL PRIMARY KEY,
  frames INTEGER,
  width INTEGER,
  height INTEGER,
  date_start TIMESTAMP,
  date_end TIMESTAMP
);
COMMENT ON TABLE public.metadata IS 'image or video metadata.';

/* permissions TODO: setup RLS later */
GRANT SELECT ON ALL TABLES IN SCHEMA public TO admin;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO admin;

GRANT SELECT ON "user" TO editor;
GRANT SELECT ON "mime_type" TO editor;
GRANT SELECT ON "file" TO editor;
GRANT SELECT ON "root_path" TO editor;
GRANT SELECT ON "path" TO editor;
GRANT SELECT ON "metadata" TO editor;
GRANT SELECT ON "post" TO editor;

/*
# User Tag TagType TagData1 TagData2

# Hash User TagId

# Hash User Title Description Rating

# TODO:
# for each metadata file in .meta
#   - ensure the mime type is in MimeTypes
#   - find entry in Files table or add it
#   - foreach src, look in Paths table for a Size/Mtime/RootPathId/Path match
*/
