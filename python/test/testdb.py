#!/usr/bin/python3
# -*- coding: utf-8 -*-

import sqlite3 as lite
import sys

con = None

def initialize_db():
    try:
        with lite.connect('test.db') as con:
            cur = con.cursor()
            cur.execute('SELECT SQLITE_VERSION()')

            data = cur.fetchone()
            print("SQLite version: %s" % data)

            cur.execute("PRAGMA foreign_keys = ON;");

            # A table with MIME types
            # Id INT, Mime TEXT, Is Binary INT
            cur.execute("CREATE TABLE IF NOT EXISTS MimeTypes(Id INT, Mime TEXT, IsBinary INT)")
            cur.execute("INSERT INTO MimeTypes VALUES(1,'image/jpeg',1)")

            # A table with files
            # Hash TEXT, Size INT, Mime Id INT
            cur.execute("CREATE TABLE IF NOT EXISTS Files(Hash TEXT UNIQUE, Size INT, MimeId INT, FOREIGN KEY(MimeId) REFERENCES MimeTypes(Id))")

            # A table with root paths
            # Id INT, Path TEXT, Name TEXT, Description TEXT
            cur.execute("CREATE TABLE IF NOT EXISTS Roots(Id INT, Path TEXT, Name TEXT, Description TEXT)")

            # A table with paths
            # Id INT, RootPathId INT, Path TEXT, INT mtime, INT Size, Hash TEXT
            cur.execute("CREATE TABLE IF NOT EXISTS Paths(Id INT, Hash TEXT, Mtime INT, Size INT, RootPathId INT, Path TEXT, FOREIGN KEY(RootPathId) REFERENCES Roots(Id))")

            # Image Metadata
            cur.execute("CREATE TABLE IF NOT EXISTS MetaImage(Id INT, Hash TEXT, DateTaken INT, Width INT, Height INT)")

            # Video Metadata
            cur.execute("CREATE TABLE IF NOT EXISTS MetaVideo(Id INT, Hash TEXT, DateStart INT, DateEnd INT, Width INT, Height INT)")

            # User Tag TagType TagData1 TagData2

            # Hash User TagId

            # Hash User Title Description Rating

    except lite.Error as e:
        print("Error %s:" % e.args[0])
        sys.exit(1)

initialize_db()
# TODO:
# for each metadata file in .meta
#   - ensure the mime type is in MimeTypes
#   - find entry in Files table or add it
#   - foreach src, look in Paths table for a Size/Mtime/RootPathId/Path match


