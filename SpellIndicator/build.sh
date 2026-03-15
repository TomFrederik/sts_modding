#!/bin/bash
set -e

# Configuration
MOD_NAME="SpellIndicator"
MODS_DIR="${MODS:-$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods}"
MOD_DIR="$MODS_DIR/$MOD_NAME"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "=== Building $MOD_NAME ==="

# Build C# project
echo "Building C# project..."
cd "$SCRIPT_DIR"
dotnet build -c Release

# Find the built DLL
DLL_PATH="$SCRIPT_DIR/.godot/mono/temp/bin/Release/$MOD_NAME.dll"
if [ ! -f "$DLL_PATH" ]; then
    echo "Error: DLL not found at $DLL_PATH"
    exit 1
fi

# Create mod directory if needed
mkdir -p "$MOD_DIR"

# Copy DLL
echo "Copying DLL to $MOD_DIR..."
cp "$DLL_PATH" "$MOD_DIR/"

# Copy manifest
echo "Copying manifest..."
cp "$SCRIPT_DIR/$MOD_NAME.json" "$MOD_DIR/"

echo "=== Build complete ==="
echo "DLL: $MOD_DIR/$MOD_NAME.dll"
echo "Manifest: $MOD_DIR/$MOD_NAME.json"
