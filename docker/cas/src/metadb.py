# -*- coding: utf-8 -*-

from datetime import datetime
import logging
import os
import subprocess
import traceback
from PIL import Image
from PIL.ExifTags import TAGS, GPSTAGS
from address import shard
logger = logging.getLogger('metadb')
dir_mode = 0o700   # default directory creation mode

# Exif standard: http://www.cipa.jp/std/documents/e/DC-008-2012_E.pdf

# PIL (Pillow)
# /usr/bin/file
# /usr/bin/identify     (image magick)

def _check_add_line(lines, line):
    """Add a unique line to the metadata lines list."""
    if line not in lines:
        lines.append(line)

def _convert_to_float(value):
    """Convert a rational number (tuple or IFDRational) to a float."""
    try:
        if isinstance(value, tuple) and len(value) == 2:
            return float(value[0]) / float(value[1])
        elif hasattr(value, 'numerator') and hasattr(value, 'denominator'):
            return float(value.numerator) / float(value.denominator)
        return float(value)
    except (TypeError, ZeroDivisionError, AttributeError):
        return None

def _convert_to_degrees(value):
    """Convert GPS coordinates from (degrees, minutes, seconds) to decimal degrees."""
    if not isinstance(value, (list, tuple)) or len(value) != 3:
        return None
    try:
        degrees = _convert_to_float(value[0])
        minutes = _convert_to_float(value[1])
        seconds = _convert_to_float(value[2])
        if None in (degrees, minutes, seconds):
            return None
        return round(degrees + (minutes / 60.0) + (seconds / 3600.0), 6)
    except (TypeError, IndexError):
        return None

def _format_tag_value(tag_name, value):
    """Format EXIF tag value based on tag-specific rules."""
    units = {
        'SubjectDistance': 'm',
        'FocalLength': 'mm',
        'FlashEnergy': 'BCPS',
        'ExposureTime': 's',
    }
    if tag_name in units and value is not None:
        value = _convert_to_float(value)
        return f"{value} {units[tag_name]}" if value is not None else None
    return str(value) if value is not None else None

def _handle_gps_info(lines, gps_info):
    """Extract and format GPS information."""
    if not isinstance(gps_info, dict):
        return

    gps_data = {GPSTAGS.get(tag, tag): value for tag, value in gps_info.items()}

    # Latitude
    if 'GPSLatitude' in gps_data and 'GPSLatitudeRef' in gps_data:
        latitude = _convert_to_degrees(gps_data['GPSLatitude'])
        lat_ref = gps_data['GPSLatitudeRef']
        if latitude is not None:
            latitude = -latitude if lat_ref == 'S' else latitude
            _check_add_line(lines, f"exif:GPSLatitude={latitude}{lat_ref}")

    # Longitude
    if 'GPSLongitude' in gps_data and 'GPSLongitudeRef' in gps_data:
        longitude = _convert_to_degrees(gps_data['GPSLongitude'])
        lon_ref = gps_data['GPSLongitudeRef']
        if longitude is not None:
            longitude = -longitude if lon_ref == 'W' else longitude
            _check_add_line(lines, f"exif:GPSLongitude={longitude}{lon_ref}")

    # Altitude
    if 'GPSAltitude' in gps_data:
        altitude = _convert_to_float(gps_data['GPSAltitude'])
        if altitude is not None:
            if 'GPSAltitudeRef' in gps_data and gps_data['GPSAltitudeRef'] != 0:
                altitude = -altitude
            _check_add_line(lines, f"exif:GPSAltitude={altitude}")

def _handle_exif(lines, image, metapath):
    """Process EXIF data from an image and append to metadata lines."""
    try:
        if not hasattr(image, '_getexif'):
            return

        exif_data = image._getexif()
        if not exif_data:
            return

        # Map EXIF tag IDs to names
        exif = {TAGS.get(tag, tag): value for tag, value in exif_data.items()}

        # Tags to process with specific formatting
        tag_formats = [
            'Make', 'Model', 'Orientation', 'DateTimeOriginal', 'DateTimeDigitized',
            'SubjectDistance', 'Flash', 'FlashEnergy', 'FocalLength', 'ExposureTime', 'FNumber'
        ]

        # Process standard EXIF tags
        for tag_name in tag_formats:
            if tag_name in exif:
                formatted_value = _format_tag_value(tag_name, exif[tag_name])
                if formatted_value:
                    _check_add_line(lines, f"exif:{tag_name}={formatted_value}")

        # Process GPSInfo separately
        if 'GPSInfo' in exif:
            _handle_gps_info(lines, exif['GPSInfo'])

    except Exception as error:
        print("Exception: '{}', {}".format(metapath, error))
        traceback.print_exc()
        _check_add_line(lines, "exif:__exception__=could not parse exif")

class MetadataDatabase(object):

    ROOT_PATH = ''

    def __init__(self, root):
        self.ROOT_PATH = os.path.join(root, '.meta')
        os.makedirs(self.ROOT_PATH, dir_mode, True)
        logger.debug("Initialized metadata database at '%s'", self.ROOT_PATH)

    def __str__(self):
        return 'METADB:{}'.format(self.ROOT_PATH)

    def dump(self, hash_value):
        hash_value = hash_value.lower()
        reldirname = shard(hash_value, 2, 2)
        metaabsdirname = os.path.join(self.ROOT_PATH, *reldirname)
        # TODO: detect rights issue in directory structure
        if os.path.isfile(metaabsdirname):
            lines = self._read_meta(metaabsdirname)
            for line in lines:
                print(line)
        else:
            print("File not in Metadata DB")

    def import_file(self, srcfullpath, dstfullpath, reldirname):
        metaabsdirname = os.path.join(self.ROOT_PATH, *reldirname[:-1])
        metafullpath = os.path.join(metaabsdirname, reldirname[-1])
        os.makedirs(metaabsdirname, dir_mode, True)
        self._append_meta(metafullpath, srcfullpath, dstfullpath)
        #print(dstfullpath, " | dup=", is_duplicate, " | ", dir_path, " | ", filename)

    def _read_meta(self, metapath):
        lines = []
        if os.path.isfile(metapath):
            with open(metapath, 'rt', encoding='utf-8') as f:
                lines = f.read().splitlines()
        return sorted(list(set(lines))) # Ensure unique lines

    # Format of the metadata file:
    # items are added if missing
    # src=<source path>
    # size=<filesize>
    def _append_meta(self, metapath, src, dst):
        lines = self._read_meta(metapath)
        org_size = len(lines)
        source_size = os.path.getsize(src)
        destination_size = os.path.getsize(dst)
        if source_size != destination_size:
            raise Exception("File size mismatch! (src:{} / dst:{})".format(src, dst))


        timestamp = os.path.getmtime(src)
        ts_iso = datetime.fromtimestamp(timestamp)
        source_line = "src={}:{}".format(ts_iso.strftime('%Y-%m-%d %H:%M:%S'), src)
        _check_add_line(lines, source_line)

        sizeline = "size={}".format(destination_size)
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
                    image = Image.open(dst)
                    image_width = image.size[0]
                    image_height = image.size[1]
                    _check_add_line(lines, "image-width={}".format(image_width))
                    _check_add_line(lines, "image-height={}".format(image_height))
                    _handle_exif(lines, image, metapath)
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
            # TODO: also upload to the SQL DB!

