
import logging
import os
import subprocess

from address import shard

logger = logging.getLogger('mediadb')
dmode = 0o700   # default directory creation mode
fmode = 0o400   # default file creation mode

# Only copy file if it doesn't already exist.
def _copy_btrfs(src, dst):
    if not os.path.isfile(dst):
        is_duplicate = False
        # Python 3.5
        #subprocess.run(["cp", "--reflink=always", src, dst], check=True)
        subprocess.check_call(["/bin/cp", "--reflink=always", src, dst])
        subprocess.check_call(["/bin/chmod", "400", dst])
        logger.info("--> Imported '%s'", src)
    else:
        is_duplicate = True

    return (dst, is_duplicate)

def _copy_stdfs(src, dst):
    if not os.path.isfile(dst):
        is_duplicate = False
        subprocess.check_call(["/bin/cp", src, dst])
        subprocess.check_call(["/bin/chmod", "400", dst])
        logger.info("--> Imported '%s'", src)
    else:
        is_duplicate = True

    return (dst, is_duplicate)


class MediaDatabase(object):

    ROOT_PATH = ''

    def __init__(self, root):
        self.ROOT_PATH = os.path.join(root, '.media')
        os.makedirs(self.ROOT_PATH, dmode, True)
        logger.debug("Initialized media database at '%s'", self.ROOT_PATH)

    def __str__(self):
        return 'MEDIADB:{}'.format(self.ROOT_PATH)

    # verification of files integrity
    # /aa/bb/cdefg*
    # at bb level manage a file for each directory called aabb.check
    # contains for each hash, timestamp of last hash check
    def verify_files(self):
        for start_hash in range(0x0000, 0xffff):
            hash_value = '{:04x}'.format(start_hash)
            reldirname = shard(hash_value, 2, 2)
            mediaabsdirname = os.path.join(self.ROOT_PATH, *reldirname)
            if os.path.isdir(mediaabsdirname):
                logger.debug('Verifying files in %s', mediaabsdirname)
            
        logger.error("TODO")

    def import_file(self, srcfullpath, reldirname):
        mediaabsdirname = os.path.join(self.ROOT_PATH, *reldirname[:-1])
        mediafullpath = os.path.join(mediaabsdirname, reldirname[-1])
        os.makedirs(mediaabsdirname, dmode, True)
        #return _copy_stdfs(srcfullpath, mediafullpath)
        return _copy_btrfs(srcfullpath, mediafullpath)

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

