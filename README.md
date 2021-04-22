# NoteApp_UserManagement_Api_NetCore_MongoDb
This microservice was developed with .netcore. 

## Install docker of mongodb and kafka
```console
make generate-all
```

## Run microservice
```console
make run
```

## TODO
- [x] Settings 
- [x] Mongodb 
- [x] Kafka
- [x] Healthcheck
- [x] Api version mechanism
- [ ] Docker file

## HTTP-API
### Create User
```console
curl --location --request POST 'http://localhost:5000/api/users/create' \
--header 'Content-Type: application/json' \
--data-raw '{
    "UserName":"Cengiz",
    "Email":"asd@asd.com",
    "Password":"sdfg"
}'
```

