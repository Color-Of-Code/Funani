\connect funani;

/*Create some dummy users*/
INSERT INTO public.user (id, username, email, password_hash) VALUES
('2fc0c398-7d1b-4fcf-8408-7bc838fc2b0c', 'Benjie', 'a@example.com', crypt('123', gen_salt('bf')),
('498241cf-4a35-428a-a877-be2dd9fc81f1', 'Singingwolfboy', 'b@example.com', crypt('123', gen_salt('bf')),
('f80cacd4-3550-43b7-9b5d-c630037732dd', 'Lexius', 'c@example.com', crypt('123', gen_salt('bf'));

/*Create some dummy posts*/
INSERT INTO public.post (author_id, title, body) VALUES
('2fc0c398-7d1b-4fcf-8408-7bc838fc2b0c', 'First post example', 'Lorem ipsum dolor sit amet'),
('498241cf-4a35-428a-a877-be2dd9fc81f1', 'Second post example', 'Consectetur adipiscing elit'),
('f80cacd4-3550-43b7-9b5d-c630037732dd', 'Third post example', 'Aenean blandit felis sodales');

INSERT INTO public.mime_type (mime, is_binary) VALUES
('image/jpeg', TRUE),
('image/tiff', TRUE),
('image/png', TRUE),
('text/plain', FALSE);
