FROM mcr.microsoft.com/dotnet/runtime-deps:9.0.3

# create group & user
RUN addgroup --gid 1000 --system app && adduser --uid 1000 -system app --gid 1000

# create work dir and set permissions as WORKDIR sets permissions as root
RUN mkdir /app && chown -R app:app /app
WORKDIR /app

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="niscode-organization-sync"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

# switch to created user
USER app

ENTRYPOINT ["./init.sh"]
