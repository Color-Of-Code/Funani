

import configparser
import logging
import os
import sys

from funanidb import FunaniDatabase

logging.basicConfig(format='%(asctime)s.%(msecs)03d - %(name)s - %(levelname)s - %(message)s', datefmt='%Y-%m-%d %H:%M:%S', level=logging.DEBUG)

logger = logging.getLogger('funani')

config = configparser.ConfigParser()
config.read('funani.cfg')
logger.info("Read configuration file")

db = FunaniDatabase(config['database'])

def traverse(directory_path, extensions):
    for root, dirs, files in os.walk(directory_path):
        for name in files:
            if name.lower().endswith(extensions):
                db.import_file(root, name)

extensions = ('.png', '.jpg', '.jpeg')
traverse(sys.argv[1], extensions)


