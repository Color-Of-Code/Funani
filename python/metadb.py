# -*- coding: utf-8 -*-

from datetime import datetime
import logging
import os
import subprocess
from PIL import Image
import PIL.ExifTags
from address import shard

logger = logging.getLogger('metadb')
dmode = 0o700   # default directory creation mode

# Exif standard: http://www.cipa.jp/std/documents/e/DC-008-2012_E.pdf

# PIL (Pillow)
# /usr/bin/file
# /usr/bin/identify     (image magick)

def _check_add_line(lines, line):
    if line not in lines:
        lines.append(line) 
        #print("..Adding to metadata: ", line)

def _handle_exif(lines, im):
    try:
        exif = im._getexif()
        if exif:
            for tag, value in exif.items():
                if tag not in [37500, 50341, 37510, 282, 283, 40961, 296, 531]:
                    if tag == 271: # Make
                        _check_add_line(lines, "exif:Make={}".format(value))
                    if tag == 272: # Model
                        _check_add_line(lines, "exif:Model={}".format(value))
                    if tag == 274: # Orientation
                        _check_add_line(lines, "exif:Orientation={}".format(value))
                    if tag == 36867: # DateTimeOriginal
                        _check_add_line(lines, "exif:DateTimeOriginal={}".format(value))
                    if tag == 36868: # DateTimeDigitized
                        _check_add_line(lines, "exif:DateTimeDigitized={}".format(value))
                    if tag == 37382: # SubjectDistance
                        _check_add_line(lines, "exif:SubjectDistance={}/{} m".format(*value))
                    if tag == 37385: # Flash
                        _check_add_line(lines, "exif:Flash={}".format(value))
                    if tag == 41483: # FlashEnergy
                        _check_add_line(lines, "exif:FlashEnergy={}/{} BCPS".format(*value))
                    if tag == 37386: # FocalLength
                        _check_add_line(lines, "exif:FocalLength={}/{} mm".format(*value))
                    if tag == 33434: # ExposureTime
                        _check_add_line(lines, "exif:ExposureTime={}/{} s".format(*value))
                    if tag == 33437: # FNumber
                        _check_add_line(lines, "exif:FNumber={}/{}".format(*value))

                    #if tag not in [271, 272, 274, 33434, 33437, 36867, 36868, 37382, 37385, 37386, 41483]:
                    #    decoded = PIL.ExifTags.TAGS.get(tag, tag)
                    #    print('tag=', tag, " decoded=", decoded, '->', value)
    except Exception as error:
        _check_add_line(lines, "exif:__exception__=could not parse exif")

class MetadataDatabase(object):

    ROOT_PATH = ''

    def __init__(self, root):
        self.ROOT_PATH = os.path.join(root, '.meta')
        os.makedirs(self.ROOT_PATH, dmode, True)
        logger.debug("Initialized metadata database at '%s'", self.ROOT_PATH)

    def __str__(self):
        return 'METADB:{}'.format(self.ROOT_PATH)

    def dump(self, hash_value):
        hash_value = hash_value.lower()
        reldirname = shard(hash_value, 2, 2)
        metaabsdirname = os.path.join(self.ROOT_PATH, *reldirname)
        if os.path.isfile(metaabsdirname):
            lines = self._read_meta(metaabsdirname)
            for line in lines:
                print(line)
        else:
            print("File not in Metadata DB")

    def import_file(self, srcfullpath, dstfullpath, reldirname):
        metaabsdirname = os.path.join(self.ROOT_PATH, *reldirname[:-1])
        metafullpath = os.path.join(metaabsdirname, reldirname[-1])
        os.makedirs(metaabsdirname, dmode, True)
        self._append_meta(metafullpath, srcfullpath, dstfullpath)
        #print(dstfullpath, " | dup=", is_duplicate, " | ", dir_path, " | ", filename)

    def _read_meta(self, metapath):
        lines = []
        if os.path.isfile(metapath):
            with open(metapath, 'rt', encoding='utf-8') as f:
                lines = f.read().splitlines()
        return lines

    
    # Format of the metadata file:
    # items are added if missing
    # src=<source path>
    # size=<filesize>
    def _append_meta(self, metapath, src, dst):
        lines = self._read_meta(metapath)
        org_size = len(lines)
        srcsize = os.path.getsize(src)
        dstsize = os.path.getsize(dst)
        if srcsize != dstsize:
            raise Exception("File size mismatch!")


        ts = os.path.getmtime(src)
        ts_iso = datetime.fromtimestamp(ts)
        srcline = "src={}:{}".format(ts_iso.strftime('%Y-%m-%d %H:%M:%S'), src)
        _check_add_line(lines, srcline)

        sizeline = "size={}".format(dstsize)
        _check_add_line(lines, sizeline)

        mime = subprocess.check_output(["/usr/bin/file", "-bi", dst])
        mime = mime.decode("utf-8").strip()
        mimeline = "mime={}".format(mime)
        _check_add_line(lines, mimeline)

        if mime.startswith("image/"):
            if mime.startswith("image/x-xcf"):
                pass
            elif mime.startswith("image/x-canon-cr2"):
                pass
            else:
                try:
                    im = Image.open(dst)
                    w = im.size[0]
                    h = im.size[1]
                    _check_add_line(lines, "image-width={}".format(w))
                    _check_add_line(lines, "image-height={}".format(h))
                    _handle_exif(lines, im)
                except Exception as error:
                    logger.error("Could not use PIL to get metadata for file mime='%s' file='%s' (%s)", mime, metapath, error)

        if len(lines) < org_size:
            raise Exception("Size can never be less than before, there is something going very wrong")

        if len(lines) > org_size:
            with open(metapath, mode='wt', encoding='utf-8') as f:
                content = '\n'.join(sorted(lines))
                #print(content)
                f.write(content)
            logger.info("Updated metadata for '%s'", src)

