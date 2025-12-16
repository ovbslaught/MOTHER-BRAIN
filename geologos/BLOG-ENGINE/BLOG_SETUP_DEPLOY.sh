#!/bin/bash
###############################################################################
# GEOLOGOS BLOG: Setup & Deployment Scripts - Production Ready
# Complete automation for Next.js blog with 130-post importer
###############################################################################

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# =============================================================================
# 1. SETUP BLOG PROJECT
# =============================================================================

setup_blog_project() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}1. SETTING UP NEXT.JS BLOG PROJECT${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    # Create Next.js project
    echo -e "${YELLOW}Creating Next.js project...${NC}"
    npx create-next-app@latest geologos-blog \
        --typescript \
        --tailwind \
        --eslint \
        --app \
        --no-git \
        --import-alias "@/*"
    
    cd geologos-blog
    
    # Install dependencies
    echo -e "${YELLOW}Installing dependencies...${NC}"
    npm install \
        gray-matter \
        rss \
        feed \
        nodemailer \
        axios \
        @types/nodemailer \
        --save
    
    echo -e "${GREEN}✓ Blog project created${NC}"
}

# =============================================================================
# 2. CREATE BLOG STRUCTURE
# =============================================================================

create_blog_structure() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}2. CREATING BLOG DIRECTORY STRUCTURE${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    # Create directories
    mkdir -p content/posts
    mkdir -p content/drafts
    mkdir -p public/images/posts
    mkdir -p app/blog
    mkdir -p app/api/newsletter
    mkdir -p app/api/analytics
    mkdir -p lib
    mkdir -p components
    mkdir -p styles
    
    # Create sample post
    cat > content/posts/welcome-to-geologos-blog.md <<'EOF'
---
title: Welcome to GEOLOGOS Blog
excerpt: Exploring universal knowledge synthesis, AI, tools, and innovation across 26 pillars
author: GEOLOGOS
publishedAt: 2025-11-16
featured: true
category: Welcome
tags: [welcome, geologos, knowledge, ai]
pillar: Meta-Documentation
readingTime: 5
---

# Welcome to the GEOLOGOS Blog

Welcome to the official GEOLOGOS Blog, your gateway to:

- **Universal Knowledge Synthesis** — 26 pillars spanning science, humanities, AI, and indigenous knowledge
- **AI & Machine Learning** — From foundations to frontier research
- **Tool Integration** — 203 open-source tools orchestrated
- **Decentralized Infrastructure** — P2P mesh networks, offline-first architecture
- **Accessibility** — Real-time captioning, inclusive design

## What You'll Find Here

Each post explores one of our 26 knowledge pillars in depth:

1. Cosmology & Deep Space
2. Astronomy & Astrophysics
3. Physics
4. Chemistry & Materials Science
... and 22 more, including Indigenous Knowledge, Post-Colonial Theory, Islamic Science, African Science, and Asian Science.

## Connect with Us

- **Knowledge API**: http://localhost:8000/docs
- **Dashboard**: http://localhost:3000
- **GitHub**: [GEOLOGOS Repository]

Stay curious. Learn deeply. Think critically. Build boldly.

*— The GEOLOGOS Team*
EOF

    echo -e "${GREEN}✓ Blog structure created${NC}"
}

# =============================================================================
# 3. IMPORT 130 POSTS FROM JSON
# =============================================================================

import_posts_from_json() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}3. IMPORTING 130+ POSTS${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    if [ ! -f "$1" ]; then
        echo -e "${YELLOW}Creating sample posts JSON (130 posts structure)...${NC}"
        
        # Create sample posts JSON
        python3 << 'PYTHON'
import json
import datetime

posts = []

# Science posts (50)
science_categories = [
    ("Cosmology", "Exploring the universe at the largest scales"),
    ("Astronomy", "Stars, galaxies, and exoplanets"),
    ("Physics", "Classical mechanics to quantum fields"),
    ("Chemistry", "Atomic structure and molecular bonding"),
    ("Biology", "Life from molecules to ecosystems")
]

# For each science category, create 10 posts
for category, desc in science_categories:
    for i in range(1, 11):
        posts.append({
            "title": f"{category} Post {i}: Deep Dive into {category}",
            "slug": f"{category.lower()}-post-{i}",
            "excerpt": f"Exploring {desc}",
            "content": f"# {category} Post {i}\n\n{desc}\n\nThis is a sample post.",
            "author": "GEOLOGOS",
            "publishedAt": (datetime.datetime.now() - datetime.timedelta(days=50-i)).isoformat(),
            "category": category,
            "tags": [category.lower(), "science", "exploration"],
            "featured": i <= 3,
            "pillar": category
        })

# AI & ML posts (20)
for i in range(1, 21):
    posts.append({
        "title": f"AI & ML Post {i}: Foundations to Frontiers",
        "slug": f"ai-ml-post-{i}",
        "excerpt": "Understanding artificial intelligence and machine learning",
        "content": f"# AI & ML Post {i}\n\nExploring the landscape of AI and ML.",
        "author": "GEOLOGOS",
        "publishedAt": (datetime.datetime.now() - datetime.timedelta(days=70-i)).isoformat(),
        "category": "AI & ML",
        "tags": ["ai", "ml", "neural-networks", "deep-learning"],
        "featured": i <= 2,
        "pillar": "Artificial Intelligence & Machine Learning"
    })

# Indigenous Knowledge posts (15)
for i in range(1, 16):
    posts.append({
        "title": f"Indigenous Knowledge Post {i}: Ancestral Wisdom",
        "slug": f"indigenous-post-{i}",
        "excerpt": "Honoring traditional ecological knowledge and epistemologies",
        "content": f"# Indigenous Knowledge Post {i}\n\nTradition meets innovation.",
        "author": "GEOLOGOS",
        "publishedAt": (datetime.datetime.now() - datetime.timedelta(days=85-i)).isoformat(),
        "category": "Indigenous Knowledge",
        "tags": ["indigenous", "ecology", "knowledge", "sustainability"],
        "featured": i == 1,
        "pillar": "Indigenous Knowledge Systems"
    })

# Tools & Integration posts (20)
for i in range(1, 21):
    posts.append({
        "title": f"Tools Post {i}: 203 Open-Source Tools",
        "slug": f"tools-post-{i}",
        "excerpt": "Integrating and orchestrating 203 open-source tools",
        "content": f"# Tools Post {i}\n\nPractical guide to tool integration.",
        "author": "GEOLOGOS",
        "publishedAt": (datetime.datetime.now() - datetime.timedelta(days=100-i)).isoformat(),
        "category": "Tools",
        "tags": ["tools", "integration", "open-source", "orchestration"],
        "featured": False,
        "pillar": "Computational Tools & Software"
    })

# Philosophy & Theory posts (15)
for i in range(1, 16):
    posts.append({
        "title": f"Philosophy Post {i}: Epistemologies & Ethics",
        "slug": f"philosophy-post-{i}",
        "excerpt": "Exploring knowledge systems and ethical frameworks",
        "content": f"# Philosophy Post {i}\n\nThought-provoking exploration.",
        "author": "GEOLOGOS",
        "publishedAt": (datetime.datetime.now() - datetime.timedelta(days=110-i)).isoformat(),
        "category": "Philosophy",
        "tags": ["philosophy", "ethics", "epistemology", "metaphysics"],
        "featured": False,
        "pillar": "Philosophy, Ethics & Knowledge Systems"
    })

# Tutorials & How-Tos (10)
for i in range(1, 11):
    posts.append({
        "title": f"Tutorial {i}: Getting Started with GEOLOGOS",
        "slug": f"tutorial-{i}",
        "excerpt": "Step-by-step guides to using GEOLOGOS ecosystem",
        "content": f"# Tutorial {i}\n\nPractical step-by-step guide.",
        "author": "GEOLOGOS",
        "publishedAt": (datetime.datetime.now() - datetime.timedelta(days=120-i)).isoformat(),
        "category": "Tutorials",
        "tags": ["tutorial", "guide", "how-to", "getting-started"],
        "featured": False,
        "pillar": "Meta-Documentation & Archival"
    })

# Write to file
with open('posts.json', 'w') as f:
    json.dump(posts, f, indent=2)

print(f"Created {len(posts)} sample posts in posts.json")
PYTHON

        echo -e "${GREEN}✓ Sample posts created in posts.json${NC}"
    fi
    
    # Import posts
    echo -e "${YELLOW}Importing posts from JSON...${NC}"
    python3 << PYTHON
import json
import os
from pathlib import Path

with open('posts.json', 'r') as f:
    posts = json.load(f)

posts_dir = Path('content/posts')
posts_dir.mkdir(parents=True, exist_ok=True)

for post in posts:
    frontmatter = f"""---
title: {post['title']}
excerpt: {post.get('excerpt', post['title'])}
author: {post.get('author', 'GEOLOGOS')}
publishedAt: {post.get('publishedAt', '')}
featured: {str(post.get('featured', False)).lower()}
category: {post.get('category', 'General')}
tags: [{', '.join(f'"{tag}"' for tag in post.get('tags', []))}]
pillar: {post.get('pillar', '')}
---

{post.get('content', post.get('excerpt', 'Post content'))}
"""
    
    slug = post.get('slug', post['title'].lower().replace(' ', '-'))
    post_file = posts_dir / f'{slug}.md'
    post_file.write_text(frontmatter)

print(f"Imported {len(posts)} posts to content/posts/")
PYTHON

    echo -e "${GREEN}✓ Posts imported: $(ls content/posts/*.md | wc -l) posts${NC}"
}

# =============================================================================
# 4. SETUP ENVIRONMENT
# =============================================================================

setup_environment() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}4. SETTING UP ENVIRONMENT VARIABLES${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    cat > .env.local <<'EOF'
# Blog Configuration
NEXT_PUBLIC_SITE_URL=http://localhost:3000
NEXT_PUBLIC_SITE_NAME=GEOLOGOS Blog
NEXT_PUBLIC_SITE_DESCRIPTION=Universal knowledge synthesis, AI, tools, and innovation

# API Configuration
NEXT_PUBLIC_API_URL=http://localhost:8000
NEXT_PUBLIC_WS_URL=ws://localhost:8000

# Newsletter (optional)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_SECURE=false
SMTP_USER=your-email@gmail.com
SMTP_PASS=your-app-password
SMTP_FROM=noreply@geologos.com

# Analytics (optional)
NEXT_PUBLIC_GA_ID=your-google-analytics-id

# Comments (optional - Disqus)
NEXT_PUBLIC_DISQUS_SHORTNAME=your-disqus-shortname

# Social Media
NEXT_PUBLIC_TWITTER=@geologos
NEXT_PUBLIC_GITHUB=geologos
NEXT_PUBLIC_LINKEDIN=geologos
EOF

    echo -e "${GREEN}✓ Environment configured${NC}"
    echo -e "${YELLOW}⚠️  Update .env.local with your actual credentials${NC}"
}

# =============================================================================
# 5. BUILD & RUN
# =============================================================================

build_and_run() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}5. BUILDING & RUNNING BLOG${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    echo -e "${YELLOW}Running development server...${NC}"
    npm run dev &
    
    sleep 2
    echo -e "${GREEN}✓ Blog running at http://localhost:3000${NC}"
}

# =============================================================================
# 6. GENERATE RSS FEED
# =============================================================================

generate_rss_feed() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}6. GENERATING RSS FEED${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    python3 << 'PYTHON'
import json
import os
from datetime import datetime
from pathlib import Path

# Create RSS feed
rss_content = '''<?xml version="1.0" encoding="UTF-8"?>
<rss version="2.0">
  <channel>
    <title>GEOLOGOS Blog</title>
    <link>http://localhost:3000</link>
    <description>Universal knowledge synthesis, AI, tools, and innovation</description>
    <language>en-us</language>
'''

# Read posts
posts_dir = Path('content/posts')
for post_file in sorted(posts_dir.glob('*.md'), reverse=True):
    content = post_file.read_text()
    
    # Parse frontmatter
    parts = content.split('---')
    if len(parts) >= 3:
        lines = parts[1].strip().split('\n')
        post_data = {}
        for line in lines:
            if ':' in line:
                key, value = line.split(':', 1)
                post_data[key.strip()] = value.strip()
        
        slug = post_file.stem
        rss_content += f'''
    <item>
      <title>{post_data.get('title', 'Untitled')}</title>
      <link>http://localhost:3000/blog/{slug}</link>
      <description>{post_data.get('excerpt', '')}</description>
      <pubDate>{post_data.get('publishedAt', '')}</pubDate>
      <category>{post_data.get('category', 'General')}</category>
    </item>
'''

rss_content += '''
  </channel>
</rss>
'''

# Write RSS feed
Path('public/rss.xml').write_text(rss_content)
print("✓ RSS feed generated: public/rss.xml")
PYTHON

    echo -e "${GREEN}✓ RSS feed generated${NC}"
}

# =============================================================================
# 7. DEPLOY TO VERCEL
# =============================================================================

deploy_to_vercel() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}7. DEPLOYING TO VERCEL${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    echo -e "${YELLOW}Installing Vercel CLI...${NC}"
    npm install -g vercel
    
    echo -e "${YELLOW}Deploying to Vercel...${NC}"
    vercel --prod
    
    echo -e "${GREEN}✓ Blog deployed!${NC}"
}

# =============================================================================
# MAIN
# =============================================================================

main() {
    echo -e "${BLUE}"
    echo "╔════════════════════════════════════════════════════════════════╗"
    echo "║           GEOLOGOS BLOG: Setup & Deployment Suite            ║"
    echo "║        Complete Next.js Blog with 130+ Posts Support         ║"
    echo "╚════════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
    
    echo "Select action:"
    echo "1) Full Setup (all steps)"
    echo "2) Setup Project"
    echo "3) Create Structure"
    echo "4) Import Posts"
    echo "5) Setup Environment"
    echo "6) Build & Run"
    echo "7) Generate RSS Feed"
    echo "8) Deploy to Vercel"
    echo ""
    
    read -p "Choose [1-8]: " CHOICE
    
    case $CHOICE in
        1)
            setup_blog_project
            create_blog_structure
            import_posts_from_json
            setup_environment
            generate_rss_feed
            echo -e "\n${GREEN}✓ Full setup complete!${NC}"
            echo -e "${YELLOW}Next: run 'npm run dev' to start the blog${NC}"
            ;;
        2) setup_blog_project ;;
        3) create_blog_structure ;;
        4) import_posts_from_json ;;
        5) setup_environment ;;
        6) build_and_run ;;
        7) generate_rss_feed ;;
        8) deploy_to_vercel ;;
        *) echo "Invalid choice" ;;
    esac
}

main "$@"