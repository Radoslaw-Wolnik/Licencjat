FROM minio/minio:latest AS minio-base

# Grab the mc (Client) binary
FROM minio/mc:latest AS mc-base

# Final image
FROM minio-base

# Copy mc into the MinIO image
COPY --from=mc-base /usr/bin/mc /usr/bin/mc

# Copy our entrypoint script
COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

# Use our entrypoint (will run mc commands, then hand off to MinIO)
ENTRYPOINT [ "/usr/local/bin/entrypoint.sh" ]
CMD ["server", "/data", "--console-address", ":9001"]
