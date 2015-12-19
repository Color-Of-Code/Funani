
import logging
import os
from metadb import MetadataDatabase
from mediadb import MediaDatabase
from address import hash_file, shard

logger = logging.getLogger('funanidb')

#images
extensions_images = ('.jpg', '.jpeg', '.png', '.tif', '.tiff', '.pnm', '.bmp', '.xcf', '.gif')
extensions_videos = ('.mp4', '.mov', '.avi', '.mpg', '.3gp')
extensions_all = extensions_images + extensions_videos
extensions = extensions_videos

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

    def verify_files(self):
        self.media_db.verify_files()

    def check_file(self, file_path):
        srcfullpath = os.path.abspath(file_path)
        srcfullpath = os.path.realpath(srcfullpath)

        hash_value = hash_file(srcfullpath)
        self.metadata_db.dump(hash_value)

    def _traverse(self, directory_path):
        logger.info("recursing through '%s'", directory_path)
        for root, dirs, files in os.walk(directory_path):
            for name in files:
                if name.lower().endswith(extensions):
                    self.import_file(root, name)
                else:
                    if not name.lower().endswith(extensions_all):
                        if name.lower() != 'thumbs.db':
                            logger.warning("skipping '%s'", os.path.join(root, name))

    def import_file(self, dir_path, filename):
        srcfullpath = os.path.join(dir_path, filename)
        self.import_single_file(srcfullpath)

    def import_recursive(self, src):
        srcfullpath = os.path.abspath(src)
        srcfullpath = os.path.realpath(srcfullpath)
        if os.path.isfile(src):
            self.import_single_file(src)
        else:
            self._traverse(src)

    def import_single_file(self, src):
        srcfullpath = os.path.abspath(src)
        srcfullpath = os.path.realpath(srcfullpath)
        
        # compute the hash and relative path to the file
        hash_value = hash_file(srcfullpath)
        reldirname = shard(hash_value, 2, 2)

        (dstfullpath, is_duplicate) = self.media_db.import_file(srcfullpath, reldirname)
        self.metadata_db.import_file(srcfullpath, dstfullpath, reldirname)

