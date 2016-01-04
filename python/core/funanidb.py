# -*- coding: utf-8 -*-

import logging
import os
from metadb import MetadataDatabase
from mediadb import MediaDatabase
from address import hash_file, shard

logger = logging.getLogger('funanidb')

extensions_images = ('.jpg', '.jpeg', '.png', '.tif', '.tiff', '.pnm', '.cr2', '.bmp', '.xcf', '.gif')
extensions_videos = ('.mp4', '.mov', '.avi', '.mpg', '.3gp')
extensions_all = extensions_images + extensions_videos

# extensions to use below
extensions = extensions_all

class FunaniDatabase(object):

    ROOT_PATH = ''
    metadata_db = None
    media_db = None
    
    def __init__(self, section):
        self.ROOT_PATH = section['path']
        self.metadata_db = MetadataDatabase(self.ROOT_PATH)
        self.media_db = MediaDatabase(self.ROOT_PATH)
        logger.debug("Initialized database at '%s'", self.ROOT_PATH)

    def __str__(self):
        return 'FUNANIDB:{}'.format(self.ROOT_PATH)

    def verify_files(self, force):
        self.media_db.verify_files(force)

    def meta_get(self, hash_values, fixdb):
        for hash_value in hash_values:
            self.metadata_db.dump(hash_value)

    def check_file(self, file_path):
        srcfullpath = os.path.abspath(file_path)
        srcfullpath = os.path.realpath(srcfullpath)

        #TODO: Build a DB table with "root paths" (repetitive Media base paths)
        #      root paths shall not be substrings of each other
        #TODO: Build a DB table with "root path id" | modtime | "relative path" | hash

        #TODO: Try to find the modtime:/filepath in the DB -> if yes return that metadata

        # otherwise fall back to this default behaviour
        hash_value = hash_file(srcfullpath)
        print("hash:", hash_value)
        self.metadata_db.dump(hash_value)

    def _traverse(self, directory_path, reflink):
        logger.info("recursing through '%s'", directory_path)
        for root, dirs, files in os.walk(directory_path):
            for name in files:
                if name.lower().endswith(extensions):
                    srcfullpath = os.path.join(root, name)
                    self.import_file(srcfullpath, reflink)
                else:
                    if not name.lower().endswith(extensions_all):
                        if name.lower() != 'thumbs.db': # avoid noise in output
                            logger.warning("skipping '%s'", os.path.join(root, name))

    def import_recursive(self, src, reflink):
        srcfullpath = os.path.abspath(src)
        srcfullpath = os.path.realpath(src)
        if os.path.isfile(srcfullpath):
            self.import_file(srcfullpath, reflink)
        else:
            self._traverse(srcfullpath, reflink)

    def import_file(self, src, reflink):
        srcfullpath = os.path.abspath(src)
        srcfullpath = os.path.realpath(srcfullpath)
        
        # compute the hash and relative path to the file
        hash_value = hash_file(srcfullpath)
        reldirname = shard(hash_value, 2, 2)

        (dstfullpath, is_duplicate) = self.media_db.import_file(srcfullpath, reldirname, reflink)
        self.metadata_db.import_file(srcfullpath, dstfullpath, reldirname)

