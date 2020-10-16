
import hashlib
import mmap

# https://pypi.python.org/pypi/hashfs/0.4.0
# https://github.com/dgilland/hashfs

# What is the best block size?
# No change in timings if we take a larger block size

# use of mmap for better performance?
# No change in performance compared to normal reading

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
    #return _hash_file_mmap(file_path)
    return _hash_file_default(file_path)

def _hash_file_mmap(file_path):
    hasher = hashlib.sha1()
    with open(file_path, 'rb') as afile:
        buf = mmap.mmap(afile.fileno(), 0, flags=mmap.MAP_PRIVATE, prot=mmap.PROT_READ)
        hasher.update(buf)
    return hasher.hexdigest()

def _hash_file_default(file_path):
    BLOCKSIZE = 65536       # 64KB
    hasher = hashlib.sha1()
    with open(file_path, 'rb') as afile:
        buf = afile.read(BLOCKSIZE)
        while len(buf) > 0:
            hasher.update(buf)
            buf = afile.read(BLOCKSIZE)
    return hasher.hexdigest()

