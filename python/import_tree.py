
import hashlib
import os
import subprocess
import sys

#for arg in sys.argv:
#    print(arg)

from os.path import join, getsize

# https://pypi.python.org/pypi/hashfs/0.4.0
# https://github.com/dgilland/hashfs

DATABASE_ROOT = "/home/jaap/Dokumente/Projekte/Funani/database"
MEDIA_ROOT = join(DATABASE_ROOT, ".media")
META_ROOT = join(DATABASE_ROOT, ".meta")
dmode = 0o700
fmode = 0o400

print(MEDIA_ROOT)
os.makedirs(MEDIA_ROOT, dmode, True)
print(META_ROOT)
os.makedirs(META_ROOT, dmode, True)

def compact(items):
    """Return only truthy elements of `items`."""
    return [item for item in items if item]

def shard(digest, depth, width):
    # This creates a list of `depth` number of tokens with width
    # `width` from the first part of the id plus the remainder.
    return compact([digest[i * width:width * (i + 1)]
                    for i in range(depth)] +
                   [digest[depth * width:]])

def hash_file(file_path):
    BLOCKSIZE = 65536
    hasher = hashlib.sha1()
    with open(file_path, 'rb') as afile:
        buf = afile.read(BLOCKSIZE)
        while len(buf) > 0:
            hasher.update(buf)
            buf = afile.read(BLOCKSIZE)
    return hasher.hexdigest()

# Only copy file if it doesn't already exist.
def _copy_btrfs(src, dst):
    if not os.path.isfile(dst):
        is_duplicate = False
        # Python 3.5
        #subprocess.run(["cp", "--reflink=always", src, dst], check=True)
        subprocess.check_call(["/bin/cp", "--reflink=always", src, dst])
        subprocess.check_call(["/bin/chmod", "400", dst])
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

def _check_add_line(lines, line):
    if line not in lines:
        lines.append(line) 
        print("..Adding to metadata: ", line)

# Format of the metadata file:
# items are added if missing
# src=<source path>
# size=<filesize>
def _append_meta(metapath, src, dst):
    lines = []
    if os.path.isfile(metapath):
        with open(metapath, 'rt', encoding='utf-8') as f:
            lines = f.read().splitlines()
    org_size = len(lines)
    srcsize = getsize(src)
    dstsize = getsize(dst)
    if srcsize != dstsize:
        raise Exception("File size mismatch!")

    srcline = "src={}".format(src)
    _check_add_line(lines, srcline)

    sizeline = "size={}".format(dstsize)
    _check_add_line(lines, sizeline)

    mime = subprocess.check_output(["/usr/bin/file", "-bi", dst])
    mime = mime.decode("utf-8").strip()
    mimeline = "mime={}".format(mime)
    _check_add_line(lines, mimeline)

    if len(lines) < org_size:
        raise Exception("Size can never be less than before, there is something going very wrong")

    if len(lines) > org_size:
        with open(metapath, mode='wt', encoding='utf-8') as f:
            print("--- Metapath: ", metapath)
            content = '\n'.join(sorted(lines))
            print(content)
            f.write(content)

def process_image_file(dir_path, filename):
    srcfullpath = join(dir_path, filename)
    hash_value = hash_file(srcfullpath)
    reldirname = shard(hash_value, 2, 2)
    mediaabsdirname = join(MEDIA_ROOT, *reldirname[:-1])
    mediafullpath = join(mediaabsdirname, reldirname[-1])
    os.makedirs(mediaabsdirname, dmode, True)

    metaabsdirname = join(META_ROOT, *reldirname[:-1])
    metafullpath = join(metaabsdirname, reldirname[-1])
    os.makedirs(metaabsdirname, dmode, True)

    (dstfullpath, is_duplicate) = _copy_btrfs(srcfullpath, mediafullpath)
    _append_meta(metafullpath, srcfullpath, dstfullpath)
    #print(dstfullpath, " | dup=", is_duplicate, " | ", dir_path, " | ", filename)

def traverse(directory_path):
    for root, dirs, files in os.walk(directory_path):
        for name in files:
            if name.lower().endswith(('.png', '.jpg', '.jpeg')):
                process_image_file(root, name)


traverse(sys.argv[1])


