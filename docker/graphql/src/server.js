const http = require("http");
const { postgraphile } = require("postgraphile");

const e = process.env;

http
  .createServer(
    postgraphile(e.DATABASE_URL, "public", {
      enhanceGraphiql: true,
      graphiql: true,
      jwtSecret: e.JWT_SECRET,
      jwtPgTypeIdentifier: e.JWT_PG_TYPE,
      watchPg: true,
    })
  )
  .listen(e.PORT);
