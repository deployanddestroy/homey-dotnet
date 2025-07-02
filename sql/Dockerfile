FROM postgres:16
WORKDIR /sql
COPY *.sql /sql/
ENTRYPOINT [ "sh", "-c", "export PGPASSWORD=$POSTGRES_PASSWORD && for f in /sql/*.sql; do psql -h $POSTGRES_HOST -U $POSTGRES_USER -d $POSTGRES_DB -f $f; done" ]