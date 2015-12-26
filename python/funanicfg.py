# -*- coding: utf-8 -*-

import configparser
import logging

logger = logging.getLogger('funani')

def parse_config(args):
    config = configparser.ConfigParser()
    cfg_file = 'funani.cfg'
    config.read(cfg_file)
    logger.debug("Read configuration file '%s'", cfg_file)
    return config
