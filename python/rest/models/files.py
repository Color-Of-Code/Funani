# -*- coding: utf-8 -*-

FILE_SCHEMA = {
    'sha1': {
        'type': 'string',
        'required': True,
        'unique': True,
        },
    'mimetype': {
        'type': 'string',
        'required': True,
        },
    'size': {
        'type': 'integer',
        'required': True,
        },
}

FILE_ENDPOINT = {
    'item_title': 'file',
    'schema': FILE_SCHEMA,
    'resource_methods': ['GET', 'POST', 'DELETE'],
}
