
import logging
import os
from metadb import MetadataDatabase
from mediadb import MediaDatabase
from address import hash_file, shard

logger = logging.getLogger('funanidb')

class FunaniDatabase(object):

    ROOT_PATH = ''
    metadata_db = None
    media_db = None
    
    def __init__(self, section):
        self.ROOT_PATH = section['path']
        self.metadata_db = MetadataDatabase(self.ROOT_PATH)
        self.media_db = MediaDatabase(self.ROOT_PATH)
        logger.info("Initialized database at '%s'", self.ROOT_PATH)

    def __str__(self):
        return 'FUNANIDB:{}'.format(self.ROOT_PATH)

    def check_file(self, file_path):
        srcfullpath = os.path.abspath(file_path)
        hash_value = hash_file(srcfullpath)
        self.metadata_db.dump(hash_value)

    def import_file(self, dir_path, filename):
        srcfullpath = join(dir_path, filename)
        
        # compute the hash and relative path to the file
        hash_value = hash_file(srcfullpath)
        reldirname = shard(hash_value, 2, 2)

        (dstfullpath, is_duplicate) = media_db.import_file(srcfullpath, reldirname)
        self.metadata_db.import_file(srcfullpath, reldirname)
