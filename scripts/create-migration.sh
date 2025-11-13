#!/bin/bash

# Bash script for creating EF Core migrations
MIGRATION_NAME=$1
PROJECT_PATH=${2:-"src/ElectroShop.Persistence"}
CONTEXT_PATH="ElectroShop.Persistence.Contexts.ElectroShopDbContext"
STARTUP_PROJECT="src/ElectroShop.WebApi"

if [ -z "$MIGRATION_NAME" ]; then
    echo "Usage: ./create-migration.sh <MigrationName> [ProjectPath]"
    exit 1
fi

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_ROOT="$( cd "$SCRIPT_DIR/.." && pwd )"

cd "$PROJECT_ROOT"

echo "Creating migration: $MIGRATION_NAME" 
echo "Context project: $PROJECT_PATH"
echo "Startup project: $STARTUP_PROJECT"

dotnet ef migrations add "$MIGRATION_NAME" \
    --project "$PROJECT_PATH" \
    --startup-project "$STARTUP_PROJECT" \
    --context "$CONTEXT_PATH"

if [ $? -eq 0 ]; then
    echo "Migration '$MIGRATION_NAME' created successfully!"
else
    echo "Failed to create migration!"
    exit 1
fi

