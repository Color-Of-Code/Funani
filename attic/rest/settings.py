# -*- coding: utf-8 -*-

import os

from models.accounts import ACCOUNT_ENDPOINT
from models.files import FILE_ENDPOINT
from models.roots import ROOT_ENDPOINT
from models.paths import PATH_ENDPOINT

MONGO_HOST = os.environ.get('MONGO_HOST', 'localhost')
MONGO_PORT = os.environ.get('MONGO_PORT', 27017)
#MONGO_USERNAME = os.environ.get('MONGO_USERNAME', 'user')
#MONGO_PASSWORD = os.environ.get('MONGO_PASSWORD', 'user')
MONGO_DBNAME = os.environ.get('MONGO_DBNAME', 'funani')

RESOURCE_METHODS = ['GET', 'POST']
ITEM_METHODS = ['GET', 'PATCH']

DATE_FORMAT = "%Y-%m-%dT%H:%M:%S"

CACHE_CONTROL = 'max-age=20'
CACHE_EXPIRES = 20

X_DOMAINS = '*'
X_HEADERS = ['Authorization', 'If-Match', 'Access-Control-Expose-Headers',
             'Content-Type', 'Pragma', 'Cache-Control']
X_EXPOSE_HEADERS = ['Origin', 'X-Requested-With', 'Content-Type', 'Accept']

DOMAIN = {
    'accounts': ACCOUNT_ENDPOINT,
    'files': FILE_ENDPOINT,
    'roots': ROOT_ENDPOINT,
    'paths': PATH_ENDPOINT,
}


# A table with root paths
# Id INT, Path TEXT, Name TEXT, Description TEXT
#cur.execute("CREATE TABLE IF NOT EXISTS Roots(Id INT, Path TEXT, Name TEXT, Description TEXT)")

# A table with paths
# Id INT, RootPathId INT, Path TEXT, INT mtime, INT Size, Hash TEXT
#cur.execute("CREATE TABLE IF NOT EXISTS Paths(Id INT, Hash TEXT, Mtime INT, Size INT, RootPathId INT, Path TEXT, FOREIGN KEY(RootPathId) REFERENCES Roots(Id))")

# Image Metadata
#cur.execute("CREATE TABLE IF NOT EXISTS MetaImage(Id INT, Hash TEXT, DateTaken INT, Width INT, Height INT)")

# Video Metadata
#cur.execute("CREATE TABLE IF NOT EXISTS MetaVideo(Id INT, Hash TEXT, DateStart INT, DateEnd INT, Width INT, Height INT)")

# User Tag TagType TagData1 TagData2

# Hash User TagId

# Hash User Title Description Rating
