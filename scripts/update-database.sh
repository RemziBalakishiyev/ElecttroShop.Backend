#!/bin/bash

# Bash script for updating database
PROJECT_PATH=${1:-"src/ElectroShop.Persistence"}
STARTUP_PROJECT=${2:-"src/ElectroShop.WebApi"}
CONTEXT_PATH="ElectroShop.Persistence.Contexts.ElectroShopDbContext"

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_ROOT="$( cd "$SCRIPT_DIR/.." && pwd )"

cd "$PROJECT_ROOT"

echo "Updating database..."

dotnet ef database update \
    --project "$PROJECT_PATH" \
    --startup-project "$STARTUP_PROJECT" \
    --context "$CONTEXT_PATH"

if [ $? -eq 0 ]; then
    echo "Database updated successfully!"
else
    echo "Failed to update database!"
    exit 1
fi

