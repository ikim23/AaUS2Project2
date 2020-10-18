const _ = require('lodash');
const AWS = require('aws-sdk');

const dynamodb = new AWS.DynamoDB();

const tableName = process.env.TABLE_REPLICA;
const keys = {
  INSERT: ['dynamodb.NewImage', 'PutRequest.Item'],
  MODIFY: ['dynamodb.NewImage', 'PutRequest.Item'],
  REMOVE: ['dynamodb.Keys', 'DeleteRequest.Key'],
};

const mapRecordsToRequestItems = records => _.map(records, (record) => {
  const [key, requestType] = _.get(keys, record.eventName);
  const item = _.get(record, key);
  return _.set({}, requestType, item);
});

module.exports.handler = (event, context, callback) => {
  console.log(JSON.stringify(event));
  const requestItems = mapRecordsToRequestItems(event.Records);
  const params = _.set({}, `RequestItems.${tableName}`, requestItems);
  dynamodb.batchWriteItem(params)
    .promise()
    .then((data) => {
      console.log(JSON.stringify(data));
      callback();
    })
    .catch((error) => {
      console.error(error);
      callback(error);
    });
};
