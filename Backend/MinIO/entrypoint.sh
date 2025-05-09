#!/usr/bin/env sh
set -e

# configure mc to talk to our local server
mc alias set local http://127.0.0.1:9000 \
   "${MINIO_ROOT_USER}" "${MINIO_ROOT_PASSWORD}" --api s3v4

# make bucket if missing, ignore if it already exists
mc mb --ignore-existing local/"${MINIO_BUCKET}"

# open it up
mc anonymous set download local/"${MINIO_BUCKET}"

# now hand off to the original MinIO entrypoint
exec minio "$@"
