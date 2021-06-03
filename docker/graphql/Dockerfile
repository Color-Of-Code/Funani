FROM node:alpine
LABEL description="Instant high-performance GraphQL API for your PostgreSQL database https://github.com/graphile/postgraphile"

# Set Node.js app folder
RUN mkdir -p /home/node/app/node_modules && chown -R node:node /home/node/app
WORKDIR /home/node/app

# Copy dependencies
COPY ./src/package.json .
COPY ./src/package-lock.json .

# Copy application files
COPY --chown=node:node ./src .

# Install dependencies
USER node
RUN npm install

EXPOSE 5433
CMD [ "node", "server.js" ]
