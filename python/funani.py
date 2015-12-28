#!/usr/bin/python3
# -*- coding: utf-8 -*-

from funaniarg import parse_args
from funanilog import setup_logging
from funanicfg import parse_config
from funanidb import FunaniDatabase

args = parse_args()
setup_logging(args)
config = parse_config(args)

# setup database -------------------------
db = FunaniDatabase(config['database'])

#print(args)
if args.command == 'import':
    if args.recursive:
        for path in args.file:
            db.import_recursive(path, args.reflink)
    else:
        for path in args.file:
            db.import_file(path, args.reflink)

elif args.command == 'meta':
    db.meta_get(args.hash, args.fixdb)

elif args.command == 'check':
    db.check_file(args.file)

elif args.command == 'verify':
    db.verify_files()

