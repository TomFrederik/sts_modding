#!/bin/bash
set -e

# Configuration
MOD_NAME="FirstMod"
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

# Create/update manifest if it doesn't exist
MANIFEST="$MOD_DIR/$MOD_NAME.json"
if [ ! -f "$MANIFEST" ]; then
    echo "Creating manifest..."
    cat > "$MANIFEST" << 'EOF'
{
  "id": "FirstMod",
  "name": "FirstMod",
  "author": "TomFrederik",
  "description": "Test mod - BurningBlood heals 12 instead of 6",
  "version": "1.0.0",
  "has_pck": false,
  "has_dll": true,
  "affects_gameplay": true
}
EOF
fi

echo "=== Build complete ==="
echo "DLL: $MOD_DIR/$MOD_NAME.dll"
echo "Manifest: $MANIFEST"
