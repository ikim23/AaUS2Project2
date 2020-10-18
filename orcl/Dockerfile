# http://www.gilfillan.space/2016/04/24/install-oracle-instant-client-1604/
FROM node:8.7

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY . .

RUN apt-get update && apt-get install -y gcc libaio1 alien

RUN alien -i packages/oracle-instantclient12.2-basic-12.2.0.1.0-1.x86_64.rpm
RUN alien -i packages/oracle-instantclient12.2-devel-12.2.0.1.0-1.x86_64.rpm

RUN npm install

CMD [ "npm", "start" ]
