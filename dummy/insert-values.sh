#!/bin/bash

set -e

CONTAINER_NAME="user-postgres"
DATABASE_NAME="financialdb"
POSTGRES_USER="postgres"

echo "==> Seeding database '$DATABASE_NAME' in container '$CONTAINER_NAME'"

docker exec -i "$CONTAINER_NAME" psql -U "$POSTGRES_USER" -d "$DATABASE_NAME" <<'EOF'

-- Users
INSERT INTO "Users" ("Id", "Name", "Email", "CreatedAt")
VALUES
    ('12345678-1111-1111-1111-111111111111', 'Alice Johnson', 'alice@example.com', NOW()),
    ('22222222-2222-2222-2222-222222222222', 'Bob Smith', 'bob@example.com', NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Accounts
INSERT INTO "Accounts" ("Id", "UserId", "Currency", "CreatedAt")
VALUES
    ('aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '12345678-1111-1111-1111-111111111111', 'BRL', NOW()),
    ('bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '22222222-2222-2222-2222-222222222222', 'BRL', NOW())
ON CONFLICT ("Id") DO NOTHING;

EOF


docker exec -it "$CONTAINER_NAME" psql -U "$POSTGRES_USER" -d "$DATABASE_NAME" -c 'SELECT * FROM "Users";'
docker exec -it "$CONTAINER_NAME" psql -U "$POSTGRES_USER" -d "$DATABASE_NAME" -c 'SELECT * FROM "Accounts";'


echo "==> Seed completed."
