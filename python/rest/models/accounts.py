# -*- coding: utf-8 -*-

ACCOUNT_SCHEMA = {
    'firstname': {
        'type': 'string'
        },
    'lastname': {
        'type': 'string'
        },
    'username': {
        'type': 'string',
        'required': True,
        'unique': True,
        },
    'password': {
        'type': 'string',
        'required': True,
        },
    'roles': {
        'type': 'list',
        'allowed': ['user', 'admin'],
        'required': True,
        },
}

ACCOUNT_ENDPOINT = {
    'item_title': 'account',
    
    'additional_lookup': {
        'url': r'regex("[\w]+")',
        'field': 'username',
    },

    # We also disable endpoint caching as we don't want client apps to
    # cache account data.
    'cache_control': '',
    'cache_expires': 0,

    'allowed_roles': ['admin'],

    'schema': ACCOUNT_SCHEMA,
}
