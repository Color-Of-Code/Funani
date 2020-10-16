Funani
======

FUNANI is an open source project aiming at solving the management of large image and other media collections in a practical way.

Documentation
-------------

https://color-of-code.de/funani/architecture

Modules
-------

- docker/cas: the low-level storage interface (usable)
- docker/db: postgresql database for storage
- docker/graphql: graphql server
- docker/web: web server: TODO

Runtime
-------

The core library has no service dependencies, the data and metadata is stored in files.

Build the CAS container:

```bash
cd docker/cas
docker build . -t cas
```

Running the container:

- the first volume mount is for the file storage itself (is read/write)
- the home is mount onto /mnt in order to be able to import files (mounted files are read only)

```bash
docker run --rm -it -v /home/data/storage:/data -v /home:/mnt:ro cas:latest --help
```

Examples of use (seen from within the container, TODO: wrap in a script to operate from outside):

1) Importing using find (allows to use find options to filter the files based on conditions):

```bash
find /home/data/family/common/Pictures/ -type f -exec
  ./funani.py --loglevel info import "{}" \;
```

2) Importing using internal recursion feature:

```bash
  ./funani.py --loglevel info import --recursive
      /home/data/family/common/Pictures/
```

3) Importing a single file:

```bash
  ./funani.py --loglevel info import
      /home/data/family/common/Pictures/picture1.jpg
```

TODO: fix this documentation

Command to import pictures mounted onto `/mnt`

```bash
docker run --rm -it -v /home/data/storage:/data -v /home:/mnt:ro cas:latest import --recursive /mnt/jdehaan/Pictures
```
