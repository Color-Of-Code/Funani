Funani
======

FUNANI is an open source project aiming at solving the management of large image and other media collections in a practical way.

Wiki
----

http://color-of-code.de/wiki/index.php?title=Main_Page


MongoDB
-------

Added the C# driver as a git submodule

- git submodule add  -- "https://github.com/mongodb/mongo-csharp-driver.git"  "src/mongo-csharp-driver"

The MongoDB has to be downloaded and unpacked somewhere on your system. MongoDB can be downloaded here:

- http://www.mongodb.org/downloads


Notes
-----

JSON field deletion find, unset

- { field: { $exists: true } }
- { $unset: { field: 1 } }

