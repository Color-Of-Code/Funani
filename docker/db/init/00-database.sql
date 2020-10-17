\connect funani;

CREATE extension IF NOT EXISTS "pgcrypto";

/*Create user table in public schema*/
CREATE TABLE public.user (
    id UUID PRIMARY KEY default gen_random_uuid(),
    username TEXT,
    firstname TEXT,
    lastname TEXT,
    email TEXT NOT NULL UNIQUE check (email ~* '^.+@.+\..+$'),
    password_hash TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE public.user IS
'Funani users.';

/*Create post table in public schema*/
CREATE TABLE public.post (
    id UUID PRIMARY KEY default gen_random_uuid(),
    title TEXT,
    body TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    author_id UUID NOT NULL REFERENCES public.user(id)
);

COMMENT ON TABLE public.post IS
'Funani posts written by a user.';

/*Create MIME type table in public schema*/
CREATE TABLE public.mime_type (
    id SERIAL PRIMARY KEY,
    mime TEXT,
    is_binary BOOLEAN,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE public.mime_type IS
'Known MIME types.';

/*Create files table in public schema*/
CREATE TABLE public.file (
    hash TEXT NOT NULL PRIMARY KEY,
    mime_id INTEGER REFERENCES public.mime_type(id),
    size INTEGER NOT NULL,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE public.file IS
'files.';

/*Create root_paths table in public schema*/
CREATE TABLE public.root_path (
    id SERIAL PRIMARY KEY,
    path TEXT NOT NULL,
    name TEXT,
    description TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE public.root_path IS
'root paths.';

/*Create paths table in public schema*/
CREATE TABLE public.path (
    id SERIAL PRIMARY KEY,
    root_path_id INTEGER REFERENCES public.root_path(id),
    path TEXT NOT NULL,
    mtime INTEGER,
    size INTEGER,
    hash TEXT
);

COMMENT ON TABLE public.path IS
'paths.';


/*Create metadata table in public schema*/
CREATE TABLE public.metadata (
    id SERIAL PRIMARY KEY,
    frames INTEGER,
    width INTEGER,
    height INTEGER,
    date_start TIMESTAMP,
    date_end TIMESTAMP
);

COMMENT ON TABLE public.metadata IS
'image or video metadata.';


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
