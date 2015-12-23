
import hashlib

# https://pypi.python.org/pypi/hashfs/0.4.0
# https://github.com/dgilland/hashfs

# TODO: What is the best block size

# TODO: use of mmap for better performance?
# http://stackoverflow.com/questions/32748231/preferred-block-size-when-reading-writing-big-binary-files
# https://docs.python.org/3.4/library/mmap.html

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
    BLOCKSIZE = 65536       # 64KB
    hasher = hashlib.sha1()
    with open(file_path, 'rb') as afile:
        buf = afile.read(BLOCKSIZE)
        while len(buf) > 0:
            hasher.update(buf)
            buf = afile.read(BLOCKSIZE)
    return hasher.hexdigest()

