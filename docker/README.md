# Docker

## Containers

* db: the database server running postgresql
* graphql: the graphql server running postgraphile
* cas: the CAS command line tool for handling the file storage
* thumbnail: container capable of generating thumbnails based on https://github.com/h2non/imaginary

## Setup

Followed instructions:

* https://www.graphile.org/postgraphile/running-postgraphile-as-a-library-in-docker/
* https://www.graphile.org/postgraphile/running-postgraphile-in-docker/

More details here:

* https://www.graphile.org/postgraphile/usage-library/

## Access

| Container | Location |
| --------- | -------- |
| GraphQL API Documentation	| http://localhost:5433/graphiql |
| GraphQL API | http://localhost:5433/graphql |
| PostgreSQL Database | host: localhost, port: 5432 |
