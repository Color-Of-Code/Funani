# -*- coding: utf-8 -*-

# use the REST API to upload the metadata from the individual files
# inside the Funani core database into the database backend of the
# REST API.

import requests
import argparse
import json
import os

USERNAME = 'admin'
PASSWORD = 'funani'
ROOT = 'http://localhost:5000/'

URIBASE = 'http://admin:funani@localhost:5000/'


def _read_meta(metapath):
    lines = []
    if os.path.isfile(metapath):
        with open(metapath, 'rt', encoding='utf-8') as f:
            lines = f.read().splitlines()
    return lines

def import_file_data(lines):
    sha1 = None
    mimetype = None
    size = None
    for line in lines:
        if line.startswith('sha1='):
            sha1 = line[5:]
        if line.startswith('size='):
            size = int(line[5:])
        if line.startswith('mime='):
            mimetype = line[5:]
            if ';' in mimetype:
                mimetype = mimetype.split(';')[0]
    #print(sha1, mimetype, size)
    r = requests.get(URIBASE + 'files?where={"sha1": "'+sha1+'"}')
    r = r.json()
    r = r['_items']
    if not r:
        #print("no item found, put")
        payload = {
                'sha1': sha1,
                'mimetype': mimetype,
                'size': size,
            }
        r = requests.post(URIBASE + 'files', json = payload)
        r = r.json()

    return r;
    
def import_paths_data(lines):
    sha1 = None
    size = None
    paths = []
    for line in lines:
        if line.startswith('sha1='):
            sha1 = line[5:]
        if line.startswith('size='):
            size = int(line[5:])
        if line.startswith('src='):
            src = line[4:]
            mtime = src[:19]
            mtime = mtime.replace(' ', 'T')
            path = src[20:]
            paths.append((mtime, path))
            # 'root':   'path':    'mtime':
    for p in paths:
        # size mtime path
        r = requests.get(URIBASE + 'paths?where={'+
            '"size": ' + str(size) + ', '+
            '"path": "' + p[1] + '", '+
            '"mtime": "' + p[0] + '" }')
        print(r.url)
        r = r.json()
        r = r['_items']
        if not r:
            print("no item found, put")
            payload = {
                    'sha1': sha1,
                    'size': size,
                    'root': 'TODO',
                    'mtime': p[0],
                    'path': p[1],
                }
            r = requests.post(URIBASE + 'paths', json = payload)
            r = r.json()
    print(paths)

def import_metadata_file(metapath):
    sha1 = ''.join(metapath.split(os.sep)[-3:])
    lines = _read_meta(metapath)
    lines.insert(0, 'sha1={}'.format(sha1))
    print(lines)

    filerecord = import_file_data(lines)
    print(filerecord)
    pathdata = import_paths_data(lines)
    print(pathdata)

def parse_args():
    # setting up the command line options parser
    parser = argparse.ArgumentParser(
        description='Funani REST level access to the back end data and metadata.'
        )
    subparsers = parser.add_subparsers(
        dest='command',
        help='sub-command help'
        )

    # import
    parser_import = subparsers.add_parser(
        'import',
        help='import metadata file(s)'
        )
    parser_import.add_argument(
        'file',
        nargs='*',
        metavar='FILE',
        help='Metadata File to import in DB'
        )
    
    return parser.parse_args()

args = parse_args()

if args.command == 'import':
    for path in args.file:
        import_metadata_file(path)



