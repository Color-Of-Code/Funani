
import logging
import os
import subprocess

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
        logger.debug("Imported '%s' into '%s'", src, dst)
    else:
        is_duplicate = True

    return (dst, is_duplicate)

def _copy_stdfs(src, dst):
    if not os.path.isfile(dst):
        is_duplicate = False
        with tmpfile(stream, self.fmode) as fname:
            os.makepath(os.path.dirname(filepath))
            shutil.copy(fname, filepath)
    else:
        is_duplicate = True

    return (filepath, is_duplicate)


class MediaDatabase(object):

    ROOT_PATH = ''

    def __init__(self, root):
        self.ROOT_PATH = os.path.join(root, '.media')
        os.makedirs(self.ROOT_PATH, dmode, True)
        logger.debug("Initialized media database at '%s'", self.ROOT_PATH)

    def __str__(self):
        return 'MEDIADB:{}'.format(self.ROOT_PATH)

    def import_file(self, srcfullpath, reldirname):
        mediaabsdirname = os.path.join(self.ROOT_PATH, *reldirname[:-1])
        mediafullpath = os.path.join(mediaabsdirname, reldirname[-1])
        os.makedirs(mediaabsdirname, dmode, True)
        return _copy_btrfs(srcfullpath, mediafullpath)
