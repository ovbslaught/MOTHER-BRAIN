# Godot Cross-Platform CI/CD Workflow
## Complete GitHub Actions Setup Guide

This document provides a step-by-step guide to set up automated cross-platform builds for your Godot project using GitHub Actions.

---

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Repository Setup](#repository-setup)
3. [CI/CD Workflow Configuration](#cicd-workflow-configuration)
4. [Platform-Specific Build Jobs](#platform-specific-build-jobs)
5. [Artifact Management](#artifact-management)
6. [Deployment Options](#deployment-options)

---

## Prerequisites

### Required Files
- **project.godot** - Your Godot project file
- **export_presets.cfg** - Export configuration for all platforms
- **.github/workflows/** - Directory for CI/CD workflows

### Godot Editor Setup
1. Open your project in Godot Engine
2. Navigate to **Project > Export**
3. Add export presets for each target platform:
   - Windows Desktop
   - Linux/X11
   - Mac OSX
   - HTML5 (Web)
   - Android (optional)
   - iOS (optional)
4. Configure settings for each preset
5. Ensure `export_presets.cfg` is saved in project root

---

## Repository Setup

### 1. Create GitHub Repository
```bash
# Initialize git repository
cd your-godot-project
git init
git add .
git commit -m "Initial commit"

# Add remote and push
git remote add origin https://github.com/yourusername/your-game.git
git branch -M main
git push -u origin main
```

### 2. Configure .gitignore
Create or update `.gitignore`:
```
# Godot-specific ignores
.import/
export.cfg
export_presets.cfg.bak
*.translation
*.import

# Build outputs
builds/
.godot/

# OS-specific
.DS_Store
Thumbs.db
```

**Important:** Do NOT ignore `export_presets.cfg` - it's required for CI/CD!

---

## CI/CD Workflow Configuration

### Complete GitHub Actions Workflow

Create `.github/workflows/godot-ci.yml`:

```yaml
name: "Godot Cross-Platform Export"

on:
  push:
    branches: [ "main", "develop" ]
  pull_request:
    branches: [ "main" ]
  workflow_dispatch:  # Allow manual triggers

env:
  GODOT_VERSION: 4.3
  EXPORT_NAME: MyGame
  PROJECT_PATH: .
  
jobs:
  # Build for Windows
  export-windows:
    name: Windows Export
    runs-on: ubuntu-latest
    container:
      image: barichello/godot-ci:4.3
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          lfs: true
      
      - name: Setup
        run: |
          mkdir -v -p ~/.local/share/godot/export_templates/
          mv /root/.local/share/godot/export_templates/${GODOT_VERSION}.stable ~/.local/share/godot/export_templates/${GODOT_VERSION}.stable
      
      - name: Windows Build
        run: |
          mkdir -v -p build/windows
          godot --headless --verbose --export-release "Windows Desktop" build/windows/$EXPORT_NAME.exe
      
      - name: Upload Artifact
        uses: actions/upload-artifact@v4
        with:
          name: windows-build
          path: build/windows

  # Build for Linux
  export-linux:
    name: Linux Export
    runs-on: ubuntu-latest
    container:
      image: barichello/godot-ci:4.3
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          lfs: true
      
      - name: Setup
        run: |
          mkdir -v -p ~/.local/share/godot/export_templates/
          mv /root/.local/share/godot/export_templates/${GODOT_VERSION}.stable ~/.local/share/godot/export_templates/${GODOT_VERSION}.stable
      
      - name: Linux Build
        run: |
          mkdir -v -p build/linux
          godot --headless --verbose --export-release "Linux/X11" build/linux/$EXPORT_NAME.x86_64
      
      - name: Upload Artifact
        uses: actions/upload-artifact@v4
        with:
          name: linux-build
          path: build/linux

  # Build for macOS
  export-macos:
    name: macOS Export
    runs-on: ubuntu-latest
    container:
      image: barichello/godot-ci:4.3
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          lfs: true
      
      - name: Setup
        run: |
          mkdir -v -p ~/.local/share/godot/export_templates/
          mv /root/.local/share/godot/export_templates/${GODOT_VERSION}.stable ~/.local/share/godot/export_templates/${GODOT_VERSION}.stable
      
      - name: macOS Build
        run: |
          mkdir -v -p build/macos
          godot --headless --verbose --export-release "Mac OSX" build/macos/$EXPORT_NAME.zip
      
      - name: Upload Artifact
        uses: actions/upload-artifact@v4
        with:
          name: macos-build
          path: build/macos

  # Build for Web
  export-web:
    name: Web Export
    runs-on: ubuntu-latest
    container:
      image: barichello/godot-ci:4.3
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          lfs: true
      
      - name: Setup
        run: |
          mkdir -v -p ~/.local/share/godot/export_templates/
          mv /root/.local/share/godot/export_templates/${GODOT_VERSION}.stable ~/.local/share/godot/export_templates/${GODOT_VERSION}.stable
      
      - name: Web Build
        run: |
          mkdir -v -p build/web
          godot --headless --verbose --export-release "HTML5" build/web/index.html
      
      - name: Upload Artifact
        uses: actions/upload-artifact@v4
        with:
          name: web-build
          path: build/web
      
      # Optional: Auto-deploy to GitHub Pages
      - name: Deploy to GitHub Pages
        uses: peaceiris/actions-gh-pages@v4
        if: github.ref == 'refs/heads/main'
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./build/web

  # Build for Android
  export-android:
    name: Android Export
    runs-on: ubuntu-latest
    container:
      image: barichello/godot-ci:4.3
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          lfs: true
      
      - name: Setup Android SDK
        run: |
          mkdir -v -p ~/.local/share/godot/export_templates/
          mv /root/.local/share/godot/export_templates/${GODOT_VERSION}.stable ~/.local/share/godot/export_templates/${GODOT_VERSION}.stable
      
      - name: Android Build
        run: |
          mkdir -v -p build/android
          godot --headless --verbose --export-release "Android" build/android/$EXPORT_NAME.apk
      
      - name: Upload Artifact
        uses: actions/upload-artifact@v4
        with:
          name: android-build
          path: build/android
```

---

## Platform-Specific Build Jobs

### Debug vs Release Builds

For **debug builds**, modify the export command:
```yaml
# Debug build (includes console, debugging symbols)
godot --headless --verbose --export-debug "Windows Desktop" build/windows/$EXPORT_NAME-debug.exe

# Release build (optimized, no debug features)
godot --headless --verbose --export-release "Windows Desktop" build/windows/$EXPORT_NAME.exe
```

### Conditional Platform Builds

Add workflow inputs to selectively build platforms:
```yaml
on:
  workflow_dispatch:
    inputs:
      platform:
        description: 'Platform to build'
        required: true
        default: 'all'
        type: choice
        options:
          - all
          - windows
          - linux
          - macos
          - web
          - android

jobs:
  export-windows:
    if: github.event.inputs.platform == 'windows' || github.event.inputs.platform == 'all'
    # ... rest of job
```

---

## Artifact Management

### Downloading Build Artifacts
After workflow completion, download artifacts:
1. Go to **Actions** tab in GitHub repository
2. Click on the completed workflow run
3. Scroll to **Artifacts** section
4. Download platform-specific builds

### Automated Artifact Retention
Configure artifact retention in workflow:
```yaml
- name: Upload Artifact
  uses: actions/upload-artifact@v4
  with:
    name: windows-build
    path: build/windows
    retention-days: 30  # Keep artifacts for 30 days
```

---

## Deployment Options

### Option 1: Deploy to Itch.io
Add itch.io deployment job:
```yaml
deploy-itch:
  name: Deploy to Itch.io
  runs-on: ubuntu-latest
  needs: [export-windows, export-linux, export-macos, export-web]
  if: github.ref == 'refs/heads/main'
  steps:
    - name: Download all artifacts
      uses: actions/download-artifact@v4
    
    - name: Install Butler
      run: |
        curl -L -o butler.zip https://broth.itch.ovh/butler/linux-amd64/LATEST/archive/default
        unzip butler.zip
        chmod +x butler
        ./butler -V
    
    - name: Upload to Itch.io
      env:
        BUTLER_API_KEY: ${{ secrets.BUTLER_API_KEY }}
      run: |
        ./butler push windows-build yourusername/yourgame:windows
        ./butler push linux-build yourusername/yourgame:linux
        ./butler push macos-build yourusername/yourgame:macos
        ./butler push web-build yourusername/yourgame:web
```

### Option 2: Deploy to Steam
Use Steamworks SDK with Steam CMD for automated deployment.

### Option 3: GitHub Releases
Create automatic releases:
```yaml
release:
  name: Create Release
  runs-on: ubuntu-latest
  needs: [export-windows, export-linux, export-macos]
  if: startsWith(github.ref, 'refs/tags/v')
  steps:
    - name: Download artifacts
      uses: actions/download-artifact@v4
    
    - name: Create Release
      uses: softprops/action-gh-release@v1
      with:
        files: |
          windows-build/*
          linux-build/*
          macos-build/*
        draft: false
        prerelease: false
```

---

## Testing the Workflow

### 1. Push to Repository
```bash
git add .github/workflows/godot-ci.yml
git commit -m "Add CI/CD workflow"
git push origin main
```

### 2. Monitor Workflow
- Navigate to **Actions** tab
- Watch real-time build logs
- Check for errors or warnings

### 3. Download and Test Builds
- Download artifacts from completed workflow
- Test each platform build locally
- Verify functionality and performance

---

## Troubleshooting

### Common Issues

**Issue:** Export templates not found
```
Solution: Ensure GODOT_VERSION matches your export templates version
```

**Issue:** Export preset not found
```
Solution: Check export_presets.cfg contains correct preset names
```

**Issue:** Build fails on specific platform
```
Solution: Review platform-specific requirements and settings
```

### Debug Tips
- Enable verbose logging: `--verbose`
- Use `--headless` for CI environments
- Check export logs in GitHub Actions
- Test exports locally before pushing

---

## Best Practices

1. **Version Control**
   - Commit `export_presets.cfg`
   - Use semantic versioning for releases
   - Tag releases: `v1.0.0`, `v1.1.0`, etc.

2. **Security**
   - Store sensitive keys in GitHub Secrets
   - Never commit API keys or passwords
   - Use environment variables for credentials

3. **Performance**
   - Cache dependencies when possible
   - Run builds in parallel
   - Use matrix builds for multiple versions

4. **Quality Assurance**
   - Run automated tests before builds
   - Validate export presets in CI
   - Test builds on actual target devices

---

## Next Steps

1. ✓ Set up export presets in Godot
2. ✓ Create GitHub Actions workflow
3. ✓ Configure platform-specific builds
4. ✓ Test workflow execution
5. ✓ Set up automated deployment
6. ✓ Document deployment process
7. ✓ Create release checklist

**Ready to Deploy!**
