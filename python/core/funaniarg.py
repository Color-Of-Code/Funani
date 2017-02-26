# -*- coding: utf-8 -*-

import argparse

#TODO: implement export function
# TODO: meta datacheck

def parse_args():
    # setting up the command line options parser
    parser = argparse.ArgumentParser(
        description='Funani low level access to the back end data and metadata.'
        )
    parser.add_argument(
        '--loglevel',
        choices=['error', 'warning', 'info', 'debug'],
        help='Set the log level'
        )
    parser.add_argument(
        '--config',
        default='funani.cfg',
        help='specify the database configuration file'
        )
    subparsers = parser.add_subparsers(
        dest='command',
        help='sub-command help'
        )

    # import
    parser_import = subparsers.add_parser(
        'import',
        help='import files or directories'
        )
    parser_import.add_argument(
        '--reflink',
        action='store_true',
        help='Use speed optimized reflink copies for Btrfs'
        )
    parser_import.add_argument(
        '--recursive',
        action='store_true',
        help='Import also directory contents recursively'
        )
    parser_import.add_argument(
        'file',
        nargs='*',
        metavar='FILE',
        help='Files (and/or directories in recursive mode) to import in DB'
        )

    # export
    parser_export = subparsers.add_parser(
        'export',
        help='export files or directories'
        )
    parser_export.add_argument(
        '--reflink',
        action='store_true',
        help='Use speed optimized reflink copies for Btrfs'
        )
    parser_export.add_argument(
        '--recursive',
        action='store_true',
        help='Export directory contents recursively'
        )
    parser_export.add_argument(
        'destination',
        metavar='DIR',
        help='Destination directory, where the exported files will be copied into'
        )
    parser_export.add_argument(
        'file',
        nargs='*',
        metavar='FILE',
        help='Files (and/or directories in recursive mode) to export from DB'
        )

    # meta
    parser_meta = subparsers.add_parser(
        'meta',
        help='metadata operations'
        )
    parser_meta.add_argument(
        '--fixdb',
        action='store_true',
        help='Check contents of the database and eventually fix it'
        )
    parser_meta.add_argument(
        'hash',
        nargs='*',
        metavar='SHA1',
        help='Get the metadata for specified hash(es)'
        )

    # check
    parser_check = subparsers.add_parser(
        'check',
        help='check metadata for an existing file'
        )
    parser_check.add_argument(
        'file',
        metavar='FILE',
        help='Check the hits for this file in DB'
        )

    # verify
    parser_verify = subparsers.add_parser(
        'verify',
        help='verify integrity of the data in DB'
        )
    parser_verify.add_argument(
        '--force',
        action='store_true',
        help='Force verification even for recently verified files (slow)'
        )

    return parser.parse_args()

