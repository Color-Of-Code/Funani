# CAS

The content addressable storage is realized with python by using sha1 as a hashing algorithm.

## Build

```bash
docker build -f Containerfile -t funani-cas .
```

## Update

This updates the contents of the `poetry.lock` file to the latest versions possible as specified inside the `pyproject.toml` file

```bash
docker run -it --rm --entrypoint /bin/sh -v ./poetry.lock:/home/poetry.lock -v ./pyproject.toml:/home/pyproject.toml funani-cas

cd /home
poetry update
```

## Test

## Run

Inside the provided `funani.cfg` file, `/data` is the storage path. Mount a volume on that path to provide the container wih
the storage of your choice.

```bash
docker run -it --rm \
    -v /home/Funani/database:/data \
    -v /home:/home \
    funani-cas \
    check '/home/data/family/Pictures/some-picture.jpg'
```

With a proper alias like `funani` on the command, things get simple to use

```bash
alias funani="docker run -it --rm -v /home/Funani/database:/data -v /home:/home funani-cas"
```

Provided the alias is active the upper command boils down to:

```bash
funani check '/home/data/family/Pictures/some-picture.jpg'
```
