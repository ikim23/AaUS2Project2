# AWS Lambda - DynamoDB Trigger

This small app demonstrates how can be AWS Lambda used to create cross-region replication. Data changes from one DynamoDB table are reflected to another using [DynamoDB Streams](http://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Streams.Lambda.Tutorial.html).

## Requirements:

- docker
- docker-compose

## Usage:

Clone repository:
```
git clone https://github.com/ikim23/aws-lambda-dynamodb-trigger.git
```
Create `.env` file:
```
mv .env.template .env
```
Set environment variable values in `.env` file:

|Variable|Description|
|-|-|
|AWS_ACCESS_KEY_ID|Access key ID for serverless framework|
|AWS_SECRET_ACCESS_KEY|Access key for serverless framework|
|STAGE|name of deployment stage (e.g dev)|
|REGION|deployment region (e.g us-west-1)|

Install NPM modules:
```
docker-compose run install
```
Deploy app to AWS:
```
docker-compose run deploy
```
Open AWS Console > DynamoDB:

insert/modify/delete data from `aws-lambda-dynamodb-trigger-user-<STAGE>` table. All actions will be replicated in `aws-lambda-dynamodb-trigger-user-replica-<STAGE>` table.
