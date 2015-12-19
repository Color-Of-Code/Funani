#!/usr/bin/python3

import argparse
import configparser
import logging
import os
import sys

from funanidb import FunaniDatabase

# setting up the command line options parser
parser = argparse.ArgumentParser(description='Funani low level access to the back end data and metadata.')
parser.add_argument('--loglevel', choices=['error', 'warning', 'info', 'debug'], help='Set the log level')
subparsers = parser.add_subparsers(dest='command', help='sub-command help')

# import
parser_import = subparsers.add_parser('import', help='import files or directories')
parser_import.add_argument('--recursive', action='store_true', help='Import also directory contents recursively')
parser_import.add_argument('file', nargs='*', metavar='FILE', help='Files (and/or directories in recursive mode) to import in DB')

# check
parser_check = subparsers.add_parser('check', help='check metadata for an existing file')
parser_check.add_argument('file', metavar='FILE', help='Check the hits for this file in DB')

# verify
parser_verify = subparsers.add_parser('verify', help='verify integrity of the data in DB')

args = parser.parse_args()

# setup logging --------------------------
argloglevel = logging.NOTSET
if args.loglevel == 'error':
    argloglevel = logging.ERROR
elif args.loglevel == 'warning':
    argloglevel = logging.WARNING
elif args.loglevel == 'info':
    argloglevel = logging.INFO
elif args.loglevel == 'debug':
    argloglevel = logging.DEBUG

logging.basicConfig(
    format='%(asctime)s.%(msecs)03d - %(name)s - %(levelname)s - %(message)s',
    datefmt='%Y-%m-%d %H:%M:%S',
    level=argloglevel)

logger = logging.getLogger('funani')

# parse configuration file --------------------------
config = configparser.ConfigParser()
config.read('funani.cfg')
logger.debug("Read configuration file")

# setup database --------------------------
db = FunaniDatabase(config['database'])

#print(args)
if args.command == 'import':
    if args.recursive:
        for path in args.file:
            db.import_recursive(path)
    else:
        for path in args.file:
            db.import_single_file(path)

elif args.command == 'check':
    db.check_file(args.file)

elif args.command == 'verify':
    db.verify_files()

