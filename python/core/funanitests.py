# -*- coding: utf-8 -*-

#TODO Write the tests

#Test: import one file
# pre: empty db
# pre: file to import
# run: import
# post: db contains file with proper hash
# post: metadata contains file information

#Test: import a file already present but different path
# pre: db with fileA
# pre: file to import
# run: import
# post: db contains the same file (no changes)
# post: metadata contains a new src line

#Test: verify a db (all ok)
# pre: db with files
# run: verify
# post: all files verified ok

#Test: verify a db (one check fails)
# pre: db with files
# pre: tweak bytes in one of the files
# run: verify
# post: tweaked file should report verification failed

#Test: get meta for an existing file in the DB
# pre: db with files
# run: meta with sha1 of existing file
# post: returns the metadata for that file

#Test: get meta for a not existing file in the DB
# pre: db with files
# run: meta with sha1 of existing file
# post: returns error message


