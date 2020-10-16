# -*- coding: utf-8 -*-

PATH_SCHEMA = {
    'sha1': {
        'type': 'string',
        'required': True,
        },
    'root': {
        'type': 'string', # should be "foreign key" in roots
        'required': True,
        },
    'path': {
        'type': 'string',
        'required': True,
        },
    'mtime': {
        'type': 'datetime',
        'required': True,
        },
    'size': {
        'type': 'integer',
        'required': True,
        },
}

PATH_ENDPOINT = {
    'item_title': 'path',
    'schema': PATH_SCHEMA,
    'resource_methods': ['GET', 'POST', 'DELETE'],
}

