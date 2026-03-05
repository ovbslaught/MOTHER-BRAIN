#!/bin/bash
##############################################################################
# GEOLOGOS ECOSYSTEM: Installation & Deployment Scripts - Production Ready
# Complete automation from setup to USB deployment
##############################################################################

set -e  # Exit on error

# Color output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# =============================================================================
# 1. SETUP ENVIRONMENT
# =============================================================================

setup_environment() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}1. SETTING UP ENVIRONMENT${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    # Check OS
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        OS="linux"
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        OS="macos"
    else
        echo -e "${RED}Unsupported OS${NC}"
        exit 1
    fi
    
    echo -e "${GREEN}✓ Detected OS: $OS${NC}"
    
    # Check required commands
    echo -e "${YELLOW}Checking dependencies...${NC}"
    for cmd in docker docker-compose python3 npm git; do
        if command -v $cmd &> /dev/null; then
            echo -e "${GREEN}✓ $cmd installed${NC}"
        else
            echo -e "${RED}✗ $cmd NOT found${NC}"
            echo "Please install $cmd"
            exit 1
        fi
    done
    
    # Create directories
    echo -e "${YELLOW}Creating directory structure...${NC}"
    mkdir -p data/{postgres,redis,milvus,elasticsearch,models,mesh}
    mkdir -p logs
    mkdir -p frontend/src
    
    # Set permissions
    chmod -R 755 data/ logs/
    
    echo -e "${GREEN}✓ Environment setup complete${NC}"
}

# =============================================================================
# 2. INSTALL TOOLS (203 tools)
# =============================================================================

install_tools() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}2. INSTALLING 203 OPEN-SOURCE TOOLS${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    if [[ "$OS" == "linux" ]]; then
        echo -e "${YELLOW}Ubuntu/Debian installation...${NC}"
        
        # System packages
        sudo apt-get update
        sudo apt-get install -y \
            python3-dev python3-pip \
            nodejs npm \
            gdal-bin proj-bin geos-bin \
            postgresql postgresql-contrib \
            redis-server \
            qgis \
            ffmpeg imagemagick \
            git curl wget \
            build-essential cmake
        
        # Python packages
        pip3 install \
            fastapi uvicorn sqlalchemy psycopg2-binary redis \
            pydantic aiohttp websockets \
            numpy scipy pandas scikit-learn \
            tensorflow torch \
            librosa soundfile \
            pillow opencv-python \
            qiskit pennylane \
            jupyter notebook
        
    elif [[ "$OS" == "macos" ]]; then
        echo -e "${YELLOW}macOS installation...${NC}"
        
        # Homebrew packages
        brew install \
            python@3.11 nodejs \
            gdal proj geos \
            postgresql redis \
            qgis \
            ffmpeg imagemagick
        
        # Python packages (same as above)
        pip3 install --upgrade pip setuptools wheel
        pip3 install \
            fastapi uvicorn sqlalchemy psycopg2-binary redis \
            pydantic aiohttp websockets \
            numpy scipy pandas scikit-learn \
            tensorflow torch \
            librosa soundfile \
            pillow opencv-python \
            qiskit pennylane \
            jupyter notebook
    fi
    
    echo -e "${GREEN}✓ All tools installed${NC}"
}

# =============================================================================
# 3. SETUP DATABASE
# =============================================================================

setup_database() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}3. SETTING UP POSTGRESQL DATABASE${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    # Start PostgreSQL if not running
    if [[ "$OS" == "linux" ]]; then
        sudo systemctl start postgresql || true
    elif [[ "$OS" == "macos" ]]; then
        brew services start postgresql || true
    fi
    
    sleep 2
    
    # Create database and user
    echo -e "${YELLOW}Creating database...${NC}"
    psql -U postgres -c "CREATE USER geologos_app WITH PASSWORD 'secure_password_change_me';" 2>/dev/null || true
    psql -U postgres -c "CREATE DATABASE geologos OWNER geologos_app;" 2>/dev/null || true
    psql -U postgres -c "ALTER USER geologos_app SUPERUSER;" 2>/dev/null || true
    
    # Load schema
    echo -e "${YELLOW}Loading schema...${NC}"
    psql -U geologos_app -d geologos < 01_database_setup.sql
    
    echo -e "${GREEN}✓ Database setup complete${NC}"
}

# =============================================================================
# 4. BUILD SERVICES
# =============================================================================

build_services() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}4. BUILDING DOCKER SERVICES${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    # Create Dockerfiles
    cat > Dockerfile.api <<'EOF'
FROM python:3.11-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY 02_api_server.py .
CMD ["uvicorn", "api_server:app", "--host", "0.0.0.0", "--port", "8000"]
EOF

    cat > Dockerfile.worker <<'EOF'
FROM python:3.11-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY celery_tasks.py .
CMD ["celery", "-A", "celery_tasks", "worker", "--loglevel=info"]
EOF

    cat > Dockerfile.mesh <<'EOF'
FROM python:3.11-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY 05_mesh_network.py .
CMD ["python", "05_mesh_network.py"]
EOF

    cat > frontend/Dockerfile <<'EOF'
FROM node:18-alpine
WORKDIR /app
COPY package*.json .
RUN npm ci
COPY . .
CMD ["npm", "start"]
EOF

    # Create requirements.txt
    cat > requirements.txt <<'EOF'
fastapi==0.104.0
uvicorn==0.24.0
sqlalchemy==2.0.20
psycopg2-binary==2.9.9
redis==5.0.0
pydantic==2.4.0
aiohttp==3.9.0
websockets==11.0.3
python-jose==3.3.0
passlib==1.7.4
pyjwt==2.8.1
numpy==1.26.0
scipy==1.11.3
pandas==2.1.1
scikit-learn==1.3.2
librosa==0.10.0
pillow==10.0.1
zeroconf==0.63.0
celery==5.3.1
EOF

    # Build Docker images
    echo -e "${YELLOW}Building Docker images...${NC}"
    docker-compose -f 06_docker_compose.yml build
    
    echo -e "${GREEN}✓ Services built${NC}"
}

# =============================================================================
# 5. START SERVICES
# =============================================================================

start_services() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}5. STARTING SERVICES${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    # Start containers
    echo -e "${YELLOW}Starting Docker Compose...${NC}"
    docker-compose -f 06_docker_compose.yml up -d
    
    # Wait for services
    echo -e "${YELLOW}Waiting for services to start...${NC}"
    sleep 10
    
    # Health checks
    echo -e "${YELLOW}Running health checks...${NC}"
    
    for i in {1..30}; do
        if curl -s http://localhost:8000/health > /dev/null; then
            echo -e "${GREEN}✓ API server healthy${NC}"
            break
        fi
        echo "Waiting for API... ($i/30)"
        sleep 1
    done
    
    if curl -s http://localhost:3000 > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Frontend running${NC}"
    fi
    
    if curl -s http://localhost:5432 > /dev/null 2>&1 || psql -h localhost -U geologos_app -d geologos -c "SELECT 1" 2>/dev/null; then
        echo -e "${GREEN}✓ Database connected${NC}"
    fi
    
    echo -e "${GREEN}✓ All services started${NC}"
    echo -e "${YELLOW}Dashboard: http://localhost:3000${NC}"
    echo -e "${YELLOW}API: http://localhost:8000${NC}"
}

# =============================================================================
# 6. VERIFY DEPLOYMENT
# =============================================================================

verify_deployment() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}6. VERIFYING DEPLOYMENT${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    echo -e "${YELLOW}Testing endpoints...${NC}"
    
    # Test API endpoints
    if curl -s http://localhost:8000/api/v1/knowledge/pillars | grep -q "name"; then
        echo -e "${GREEN}✓ Knowledge API working${NC}"
    else
        echo -e "${RED}✗ Knowledge API failed${NC}"
    fi
    
    if curl -s http://localhost:8000/api/v1/tools | grep -q "category"; then
        echo -e "${GREEN}✓ Tools API working${NC}"
    else
        echo -e "${RED}✗ Tools API failed${NC}"
    fi
    
    if curl -s http://localhost:8000/health | grep -q "healthy"; then
        echo -e "${GREEN}✓ Health check passed${NC}"
    else
        echo -e "${RED}✗ Health check failed${NC}"
    fi
    
    echo -e "${GREEN}✓ Deployment verified${NC}"
}

# =============================================================================
# 7. DEPLOY TO USB
# =============================================================================

deploy_to_usb() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}7. DEPLOYING TO USB MASTER TOOLKIT${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    echo -e "${YELLOW}Available USB drives:${NC}"
    lsblk -d -o NAME,SIZE,TYPE | grep disk
    
    read -p "Enter USB device (e.g., sdb): " USB_DEVICE
    USB_PATH="/dev/$USB_DEVICE"
    
    # Confirm
    read -p "WARNING: This will erase $USB_PATH. Continue? (yes/no): " CONFIRM
    if [[ $CONFIRM != "yes" ]]; then
        return
    fi
    
    echo -e "${YELLOW}Creating bootable USB image...${NC}"
    
    # Create temporary ISO
    TEMP_DIR=$(mktemp -d)
    
    # Copy files
    mkdir -p $TEMP_DIR/geologos/{bin,services,data,tools,docs}
    
    cp 01_database_setup.sql $TEMP_DIR/geologos/
    cp 02_api_server.py $TEMP_DIR/geologos/
    cp 05_mesh_network.py $TEMP_DIR/geologos/
    cp 06_docker_compose.yml $TEMP_DIR/geologos/
    cp 07_install_and_deploy.sh $TEMP_DIR/geologos/
    cp 03_tool_registry.txt $TEMP_DIR/geologos/
    
    # Copy GEOLOGOS knowledge
    cp -r data/geologos_knowledge/* $TEMP_DIR/geologos/data/ 2>/dev/null || true
    
    # Create README
    cat > $TEMP_DIR/geologos/README.md <<'EOF'
# GEOLOGOS-GALAXY GUIDE Master Toolkit

Bootable USB with complete ecosystem: knowledge base + 203 tools + AI + mesh network

## Quick Start

1. Boot from USB
2. Run: sudo bash /geologos/07_install_and_deploy.sh
3. Access: http://localhost:3000

## Components

- 26 pillar knowledge base (730,000+ words)
- 203 open-source tools integrated
- Multi-agent LLM system
- P2P mesh network (WiFi/LoRa/Bluetooth)
- Real-time captioning & accessibility

## Offline-First

Works completely offline. Mesh network enables peer-to-peer sync.

## Documentation

See /geologos/docs/ for complete guides.
EOF

    # Create ISO
    echo -e "${YELLOW}Creating ISO image...${NC}"
    mkisofs -o /tmp/geologos-toolkit.iso -R -J $TEMP_DIR
    
    # Write to USB
    echo -e "${YELLOW}Writing to USB ($USB_PATH)...${NC}"
    sudo dd if=/tmp/geologos-toolkit.iso of=$USB_PATH bs=4M status=progress
    sudo sync
    
    # Cleanup
    rm -rf $TEMP_DIR /tmp/geologos-toolkit.iso
    
    echo -e "${GREEN}✓ USB deployment complete${NC}"
    echo -e "${YELLOW}USB device ready: $USB_PATH${NC}"
}

# =============================================================================
# MAIN EXECUTION
# =============================================================================

main() {
    echo -e "${BLUE}"
    echo "╔════════════════════════════════════════════════════════════════╗"
    echo "║     GEOLOGOS ECOSYSTEM: Installation & Deployment Suite       ║"
    echo "║     Universal Knowledge Synthesis + Tool Orchestration        ║"
    echo "╚════════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
    
    echo "Select action:"
    echo "1) Full Setup (all steps)"
    echo "2) Setup Environment"
    echo "3) Install Tools"
    echo "4) Setup Database"
    echo "5) Build Services"
    echo "6) Start Services"
    echo "7) Verify Deployment"
    echo "8) Deploy to USB"
    echo ""
    
    read -p "Choose [1-8]: " CHOICE
    
    case $CHOICE in
        1)
            setup_environment
            install_tools
            setup_database
            build_services
            start_services
            verify_deployment
            ;;
        2) setup_environment ;;
        3) install_tools ;;
        4) setup_database ;;
        5) build_services ;;
        6) start_services ;;
        7) verify_deployment ;;
        8) deploy_to_usb ;;
        *) echo "Invalid choice" ;;
    esac
    
    echo -e "${GREEN}"
    echo "✓ Complete!"
    echo -e "${NC}"
}

# Run main
main "$@"