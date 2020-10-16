# -*- coding: utf-8 -*-

import logging
import configparser

LOGGER = logging.getLogger('funani')

def parse_config(args):
    config = configparser.ConfigParser()
    cfg_file = args.config
    config.read(cfg_file)
    LOGGER.debug("Read configuration file '%s'", cfg_file)
    return config
