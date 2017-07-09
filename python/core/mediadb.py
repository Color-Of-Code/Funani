# -*- coding: utf-8 -*-

from datetime import datetime
from random import shuffle
import json
import logging
import os
import subprocess
import sys

from address import shard, hash_file

LOGGER = logging.getLogger('mediadb')
DMODE = 0o700   # default directory creation mode
FMODE = 0o400   # default file creation mode

# Only copy file if it doesn't already exist.
def _copy_btrfs(src, dst):
    if not os.path.isfile(dst):
        is_duplicate = False
        # Python 3.5
        #subprocess.run(["cp", "--reflink=always", src, dst], check=True)
        subprocess.check_call(["/bin/cp", "--reflink=always", src, dst])
        subprocess.check_call(["/bin/chmod", "400", dst])
        LOGGER.info("--> Imported '%s'", src)
    else:
        is_duplicate = True

    return (dst, is_duplicate)

def _copy_stdfs(src, dst):
    if not os.path.isfile(dst):
        is_duplicate = False
        subprocess.check_call(["/bin/cp", src, dst])
        subprocess.check_call(["/bin/chmod", "400", dst])
        LOGGER.info("--> Imported '%s'", src)
    else:
        is_duplicate = True

    return (dst, is_duplicate)


class MediaDatabase(object):

    ROOT_PATH = ''
    AUTO_REFLINK_ROOT = ''

    def __init__(self, root, reflink_root):
        self.ROOT_PATH = os.path.join(root, '.media')
        self.AUTO_REFLINK_ROOT = reflink_root
        os.makedirs(self.ROOT_PATH, DMODE, True)
        LOGGER.debug("Initialized media database at '%s'", self.ROOT_PATH)

    def __str__(self):
        return 'MEDIADB:{}'.format(self.ROOT_PATH)

    def _flush_verification_status(self, jsonfilepath, results):
        with open(jsonfilepath, 'w') as jsonfile:
            json.dump(results, jsonfile)

    # Determine if we have to recheck the file
    def _get_recheck_flag(self, force, results, name):
        if force:
            # in this case, always do so
            return True
        # assume we have to by default
        recheck = True
        if name in results:
            lastcheck = results[name]
            if lastcheck:
                lastts = lastcheck['checked']
                delta = datetime.now() - datetime.strptime(lastts, "%Y-%m-%dT%H:%M:%S.%f")
                #TODO: make these 60 configurable (via config file verify-limit option)
                if delta.days < 60:
                    recheck = False
                    LOGGER.debug('... skipping because was done already %i days ago',
                                 delta.days)
        return recheck

    # verification of files integrity
    # contains for each hash, timestamp of last hash check
    def _verify_files_in_dir(self, reldirname, mediaabsdirname, force):
        results = {}
        jsonfilepath = os.path.join(mediaabsdirname, 'verify.json')
        if os.path.isfile(jsonfilepath):
            LOGGER.debug('Reading %s', jsonfilepath)
            with open(jsonfilepath, 'r') as jsonfile:
                results = json.load(jsonfile)
                #print(results)
        changed = False
        # go through all files in directory and check hash
        for root, dirs, files in os.walk(mediaabsdirname):
            del dirs[:] # we do not want to recurse
            for name in files:
                if not name.lower().endswith('.json'):
                    recheck = self._get_recheck_flag(force, results, name)
                    if recheck:
                        file_to_verify = os.path.join(root, name)
                        LOGGER.debug("Verifying '%s'", file_to_verify)
                        actual_hash_value = hash_file(file_to_verify)
                        expected_hash_value = '{}{}{}'.format(reldirname[0], reldirname[1], name)
                        status = 'OK' if expected_hash_value == actual_hash_value else 'FAILED'
                        timestamp = datetime.now().isoformat()
                        changed = True
                        results[name] = {}
                        results[name]['status'] = status
                        results[name]['checked'] = timestamp
                        if status != 'OK':
                            LOGGER.error("Mismatching hash for file %s", file_to_verify)
                        else:
                            LOGGER.info("OK - %s", actual_hash_value)

        for name in sorted(results.keys()):
            if results[name]['status'] != 'OK':
                LOGGER.error("Mismatching hash for file %s%s", reldirname, name)

        if changed:
            self._flush_verification_status(jsonfilepath, results)

    def verify_files(self, force):
        # randomize the order of processing so that for lengthy
        # operations all files have a change to be checked if process has
        # to be aborted and is restarted later
        parentdirs = list(range(0x0000, 0xffff))
        shuffle(parentdirs)
        index = 0
        for start_hash in parentdirs:
            sys.stdout.write("%d%% (%d) \r" % (index*100>>16, index))
            sys.stdout.flush()
            index = index + 1
            hash_value = '{:04x}'.format(start_hash)
            reldirname = shard(hash_value, 2, 2)
            mediaabsdirname = os.path.join(self.ROOT_PATH, *reldirname)
            if os.path.isdir(mediaabsdirname):
                LOGGER.debug('Verifying files in %s', mediaabsdirname)
                self._verify_files_in_dir(reldirname, mediaabsdirname, force)

    def import_file(self, srcfullpath, reldirname, reflink):
        mediaabsdirname = os.path.join(self.ROOT_PATH, *reldirname[:-1])
        mediafullpath = os.path.join(mediaabsdirname, reldirname[-1])
        os.makedirs(mediaabsdirname, DMODE, True)
        # automatically use reflink if the root path is the same as specified
        if self.AUTO_REFLINK_ROOT and srcfullpath.startswith(self.AUTO_REFLINK_ROOT):
            reflink = True
        if reflink:
            return _copy_btrfs(srcfullpath, mediafullpath)
        else:
            return _copy_stdfs(srcfullpath, mediafullpath)

#TODO: find out when the optimization with btrfs can be used

#def find_mount_point(path):
#    path = os.path.abspath(path)
#    while not os.path.ismount(path):
#        path = os.path.dirname(path)
#    return path

#path = sys.argv[1]
#path = os.path.realpath(path)
#print("real path: ", path)
#mount_point = find_mount_point(path)
#print("mount point: ", mount_point)
#print("lstat: ", os.lstat(path))
#print("lstat /home/data/test.jpg: ", os.lstat("/home/data/test.jpg"))
#print(os.path.ismount(path))
#(drive, tail) = os.path.splitdrive(path)
#print((drive, tail))

