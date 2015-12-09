Funani
======

FUNANI is an open source project aiming at solving the management of large image and other media collections in a practical way.

Wiki
----

http://www.color-of-code.de/wiki/product:funani:start

Dependencies
------------

Build:

 The Build dependencies are solved using NuGet (integrated in both SharpDevelop & Visual Studio)
 * Catel: The software is based on the MVVM Catel framework.
 * MongoDB: The C# driver and the fluent extensions

Runtime:

The MongoDB has to be downloaded and unpacked somewhere on your system. MongoDB can be downloaded here:

- http://www.mongodb.org/downloads


Notes
-----

JSON field deletion find, unset

- { field: { $exists: true } }
- { $unset: { field: 1 } }

