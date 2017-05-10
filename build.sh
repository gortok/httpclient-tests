SERVICE="httpclientsync"
pushd ./src/
dotnet restore
dotnet publish -c Release
popd
NETWORK="InternalNetwork"
docker build -t "$SERVICE":latest .
ISCREATED=`docker network ls | grep "$NETWORK"`
if [ -z "$ISCREATED" ]; then
  docker network create $NETWORK 
fi
CONTAINER=`docker ps --all | grep "$SERVICE"`
if [ -z "$CONTAINER" ]; then
  docker run -d --net=$NETWORK --name $SERVICE -t $SERVICE:latest
else
  docker rm $SERVICE
  docker run -d --net=$NETWORK --name $SERVICE -t $SERVICE:latest
fi

