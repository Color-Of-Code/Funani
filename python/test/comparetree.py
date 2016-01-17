#!/usr/bin/python3

import argparse

def parse_args():
    # setting up the command line options parser
    parser = argparse.ArgumentParser(
        description='Compare <src> to <dst> recursiverly file by file (sha1sum) and delete exact matches from <src>.'
        )
    parser.add_argument(
        '--loglevel',
        choices=['error', 'warning', 'info', 'debug'],
        help='Set the log level'
        )
    parser.add_argument(
        '-n',
	dest='dryrun',
        action='store_true',
        help='Just print out actions, do not execute them, dry-run'
        )
    parser.add_argument(
        '--src',
        required=True,
	dest='src',
        help='source path'
        )
    parser.add_argument(
        '--dst',
        required=True,
	dest='dst',
        help='destination path'
        )
    return parser.parse_args()


import hashlib
import os

args = parse_args()
dir1 = os.path.abspath(args.src)
dir2 = os.path.abspath(args.dst)

print("Source:", dir1)
print("Base:", dir2)

def hash_file(file_path):
    return _hash_file_default(file_path)

def _hash_file_default(file_path):
    BLOCKSIZE = 65536       # 64KB
    hasher = hashlib.sha1()
    with open(file_path, 'rb') as afile:
        buf = afile.read(BLOCKSIZE)
        while len(buf) > 0:
            hasher.update(buf)
            buf = afile.read(BLOCKSIZE)
    return hasher.hexdigest()

def check_file(root, name):
    relp = os.path.relpath(root, dir1)
    srcfile = os.path.join(root, name)
    dstfile = os.path.join(dir2, relp, name)
    if os.path.isfile(srcfile) and os.path.isfile(dstfile):
        srch=hash_file(srcfile)
        dsth=hash_file(dstfile)
        if srch != dsth:
            print(relp, name)
            print("  src=", srch, "size=", os.path.getsize(srcfile))
            print("  dst=", dsth, "size=", os.path.getsize(dstfile))
        else:
            if not args.dryrun:
                print(relp, name, "equal, deleting from src")
                os.remove(srcfile)
            else:
                print(relp, name, "equal, WOULD delete src")
    else:
        print("Destination file not found: ", dstfile)

def _traverse(directory_path):
    for root, dirs, files in os.walk(directory_path):
        for name in files:
            check_file(root, name)


_traverse(dir1)
