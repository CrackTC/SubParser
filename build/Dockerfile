FROM alpine:latest
RUN apk add --no-cache \
    openssh libunwind \
    nghttp2-libs libidn krb5-libs libuuid lttng-ust zlib \
    libstdc++ libintl \
    icu
COPY app /app
CMD [ "sh", "-c", "cd /app && ./entrypoint.sh" ]
