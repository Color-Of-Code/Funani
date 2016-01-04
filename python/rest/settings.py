# -*- coding: utf-8 -*-

RESOURCE_METHODS = ['GET','POST','DELETE']

ITEM_METHODS = ['GET','PATCH','DELETE']

X_DOMAINS = '*'
X_HEADERS = ['Authorization','If-Match','Access-Control-Expose-Headers','Content-Type','Pragma','Cache-Control']
X_EXPOSE_HEADERS = ['Origin', 'X-Requested-With', 'Content-Type', 'Accept']

DOMAIN = {
    'user': {
        'schema': {
            'firstname': {
                'type': 'string'
            },
            'lastname': {
                'type': 'string'
            },
            'username': {
                'type': 'string',
                'unique': True
            },
            'password': {
                'type': 'string'
            }
        }
    },
    'file': {
        'MONGO_QUERY_BLACKLIST' : ['$where'],
        'schema': {
            'sha1':{
                'type': 'string'
                },
            'mimetype':{
                'type': 'string'
                },
            'size': {
                'type': 'integer'
                }
            },
        'resource_methods': ['GET', 'POST','DELETE'],
        },
    'root': {
        'MONGO_QUERY_BLACKLIST' : ['$where'],
        'schema': {
            'path':{
                'type': 'string'
                },
            'name':{
                'type': 'string'
                },
            'description': {
                'type': 'string'
                }
            },
        'resource_methods': ['GET', 'POST','DELETE'],
        }
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
