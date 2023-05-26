#!/usr/bin/sh

if [ $# -eq 0 ]; then
    echo "Please pass a csv file with transactions to enrich as a parameter"
    echo " "
    echo "Usage: "
    echo "       sh enrich.sh <name of csv file with transactions to enrich>"
    exit 1
fi

curl -X POST -k -H 'Content-Type: text/csv' --data-binary @$1 http://localhost:7000/enrich