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
