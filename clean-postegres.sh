#!/bin/bash

CONTAINER="postgres"
USER="postgres"

echo "Cleaning all db data"
echo "===userdb==="

docker exec -i $CONTAINER psql -U $USER userdb -c \
'TRUNCATE TABLE public."Users", public."Accounts", public."OutboxMessages", public."ProcessedEvents" RESTART IDENTITY CASCADE;'

echo "===financialservice==="

docker exec -i $CONTAINER psql -U $USER financialservice -c \
'TRUNCATE TABLE public."Users", public."Accounts", public."Transactions", public."JournalEntries", public."OutboxMessages", public."AccountLocks" RESTART IDENTITY CASCADE;'

echo "===simpleauth==="

docker exec -i $CONTAINER psql -U $USER simpleauth -c \
'TRUNCATE TABLE public."Users", public."RefreshTokens", public."LoginAttempts" RESTART IDENTITY CASCADE;'

echo "Use financial service migrations to create default user"

cd FinancialService/FinancialService/
dotnet ef database update

echo "Done."
