# -*- coding: utf-8 -*-

import logging

def setup_logging(args):
    argloglevel = logging.INFO  # default
    if args.loglevel == 'error':
        argloglevel = logging.ERROR
    elif args.loglevel == 'warning':
        argloglevel = logging.WARNING
    elif args.loglevel == 'info':
        argloglevel = logging.INFO
    elif args.loglevel == 'debug':
        argloglevel = logging.DEBUG

    logging.basicConfig(
        format='%(asctime)s.%(msecs)03d - %(name)s - %(levelname)s - %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S',
        level=argloglevel)

