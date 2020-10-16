# -*- coding: utf-8 -*-

ROOT_SCHEMA = {
    'path': {
        'type': 'string',
        'required': True,
        'unique': True,
        },
    'name': {
        'type': 'string'
        },
    'description': {
        'type': 'string'
        },
}

ROOT_ENDPOINT = {
    'item_title': 'root',
    'schema': ROOT_SCHEMA,
    'resource_methods': ['GET', 'POST', 'DELETE'],
}

