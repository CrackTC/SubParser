#!/bin/sh
cd "$(dirname $0)"

rm -rf ./target
mkdir -p ./target/app

dotnet publish -c Release --runtime alpine-x64
cp ./bin/Release/*/alpine-x64/publish/top.cracktc.SubParser ./target/app/app
cp -r build/* target/

docker buildx build -t cracktc/subparser ./target
