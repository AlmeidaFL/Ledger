#!/bin/bash

set -e

if [ "$#" -lt 4 ]; then
    echo "Error: Right use: $0 REPO_URL IMAGE_NAME SERVICE_NAME COMMIT_SHA INITIAL_PATH_OVERRIDE"
    exit 1
fi

REPO_URL=$1
IMAGE_NAME=$2
SERVICE_NAME=$3
COMMIT_SHA=$4
INITIAL_PATH_OVERRIDE=$5

if [[ -n "$INITIAL_PATH_OVERRIDE" ]]; then
  echo "Using overridden path: $INITIAL_PATH_OVERRIDE"
  DOCKERFILE_PATH="$INITIAL_PATH_OVERRIDE/$SERVICE_NAME/Dockerfile"
else
  echo "Using default path"
  DOCKERFILE_PATH="$SERVICE_NAME/$SERVICE_NAME/Dockerfile"
fi

docker build \
  -t "$REPO_URL/$IMAGE_NAME:$COMMIT_SHA" \
  -f "$DOCKERFILE_PATH" .

docker push $REPO_URL/$IMAGE_NAME:$COMMIT_SHA