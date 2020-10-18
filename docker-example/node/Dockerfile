FROM node:8.7

# create project directory
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

# copy project files fo WORKDIR
COPY . .

# install dependencies
RUN npm i --silent

EXPOSE 3000

CMD ["node", "./app.js"]
