FROM python:3.9-alpine AS builder

# Pillow dependencies
RUN apk --no-cache add \
    build-base \
    freetype-dev \
    fribidi-dev \
    harfbuzz-dev \
    jpeg-dev \
    lcms2-dev \
    openjpeg-dev \
    tcl-dev \
    tiff-dev \
    tk-dev \
    zlib-dev

COPY requirements.txt .

RUN pip install --user -r requirements.txt

FROM python:3.9-alpine

RUN apk --no-cache add \
    # funani dependencies
    file \
    # Pillow dependencies
    freetype \
    fribidi \
    harfbuzz \
    jpeg \
    lcms2 \
    openjpeg \
    tcl \
    tiff \
    tk \
    zlib

RUN mkdir -p /data

WORKDIR /code

COPY --from=builder /root/.local/lib /root/.local/lib
COPY src .

ENV PATH=/root/.local:$PATH

ENTRYPOINT [ "python", "./funani.py" ]
CMD ["--help"]
