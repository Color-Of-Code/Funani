/* Create post table */
CREATE TABLE usr.post (
  id UUID PRIMARY KEY default gen_random_uuid(),
  title TEXT,
  body TEXT,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  author_id UUID NOT NULL REFERENCES usr.user(id)
);
COMMENT ON TABLE usr.post IS 'Funani posts written by a user.';

/* Create MIME type table */
CREATE TABLE usr.mime_type (
  id SERIAL PRIMARY KEY,
  mime TEXT,
  is_binary BOOLEAN,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE usr.mime_type IS 'Known MIME types.';

/* Create files table */
CREATE TABLE usr.file (
  hash TEXT NOT NULL PRIMARY KEY,
  mime_id INTEGER REFERENCES usr.mime_type(id),
  size INTEGER NOT NULL,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE usr.file IS 'files.';

/* Create root_paths table */
CREATE TABLE usr.root_path (
  id SERIAL PRIMARY KEY,
  path TEXT NOT NULL,
  name TEXT,
  description TEXT,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE usr.root_path IS 'root paths.';

/* Create paths table */
CREATE TABLE usr.path (
  id SERIAL PRIMARY KEY,
  root_path_id INTEGER REFERENCES usr.root_path(id),
  path TEXT NOT NULL,
  mtime INTEGER,
  size INTEGER,
  hash TEXT
);
COMMENT ON TABLE usr.path IS 'paths.';

/* Create metadata table */
CREATE TABLE usr.metadata (
  id SERIAL PRIMARY KEY,
  frames INTEGER,
  width INTEGER,
  height INTEGER,
  date_start TIMESTAMP,
  date_end TIMESTAMP
);
COMMENT ON TABLE usr.metadata IS 'image or video metadata.';

/* permissions TODO: setup RLS later */
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
