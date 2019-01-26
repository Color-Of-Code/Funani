Funani
======

FUNANI is an open source project aiming at solving the management of large image and other media collections in a practical way.

Documentation
-------------

https://color-of-code.de/funani/architecture

Modules
-------

- python/core: the low-level storage interface (usable)
- python/rest: a RESTful API for accessing the data and metadata (**todo**)
- python/web: a Web front end for end users (**todo**)

Dependencies
------------

- all: python 3
- python/core: Pillow
- python/rest: eve, mongodb
- python/web: Flask, Nodejs, Bower

Runtime
-------

The core library has no service dependencies, the data and metadata is stored in files.

Examples of use:

1) Importing using find (allows to use find options to filter the files based on conditions):

``` bash
find /home/data/family/common/Pictures/ -type f -exec
  ./funani.py --loglevel info import "{}" \;
```

2) Importing using internal recursion feature:

``` bash
  ./funani.py --loglevel info import --recursive
      /home/data/family/common/Pictures/
```

3) Importing a single file:

``` bash
  ./funani.py --loglevel info import
      /home/data/family/common/Pictures/picture1.jpg
```

