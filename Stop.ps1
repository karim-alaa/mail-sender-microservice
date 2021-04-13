docker container stop producer >$null 2>&1
docker container rm producer >$null 2>&1

docker container stop consumer >$null 2>&1
docker container rm consumer >$null 2>&1

docker container stop database >$null 2>&1
docker container rm database >$null 2>&1

docker container stop rabbit-1 >$null 2>&1
docker container rm rabbit-1 >$null 2>&1

docker container stop rabbit-2 >$null 2>&1
docker container rm rabbit-2 >$null 2>&1

docker container stop rabbit-3 >$null 2>&1
docker container rm rabbit-3 >$null 2>&1