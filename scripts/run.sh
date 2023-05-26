#!/usr/bin/sh

docker build -t data-enricher ..
docker run -it --rm -p 7000:80 --name data-enricher data-enricher