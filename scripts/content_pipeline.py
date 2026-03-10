#!/usr/bin/env python3
"""
Content Pipeline - MOTHER-BRAIN
Executes approved content plans: YouTube, Blog, Social, Podcast, Comics.
Publishes to all configured platforms.
"""

import os
import asyncio
import logging
import json
import glob
from datetime import datetime
import httpx

logging.basicConfig(level=logging.INFO, format='%(asctime)s [CONTENT-PIPELINE] %(message)s')
log = logging.getLogger(__name__)

# === ENV ===
OPENAI_API_KEY = os.environ.get('OPENAI_API_KEY', '')
GHOST_API_URL = os.environ.get('GHOST_API_URL', '')
GHOST_ADMIN_KEY = os.environ.get('GHOST_ADMIN_KEY', '')
YOUTUBE_API_KEY = os.environ.get('YOUTUBE_API_KEY', '')
TELEGRAM_BOT_TOKEN = os.environ.get('TELEGRAM_BOT_TOKEN', '')
TELEGRAM_CHANNEL_ID = os.environ.get('TELEGRAM_CHANNEL_ID', '')
APPROVED_DIR = os.environ.get('APPROVED_DIR', '/data/approved')
PUBLISHED_DIR = os.environ.get('PUBLISHED_DIR', '/data/published')

os.makedirs(APPROVED_DIR, exist_ok=True)
os.makedirs(PUBLISHED_DIR, exist_ok=True)


async def llm_expand(prompt: str) -> str:
    """Expand content with LLM."""
    try:
        async with httpx.AsyncClient(timeout=90) as client:
            resp = await client.post(
                'https://api.openai.com/v1/chat/completions',
                headers={'Authorization': f'Bearer {OPENAI_API_KEY}'},
                json={
                    'model': 'gpt-4o-mini',
                    'messages': [{'role': 'user', 'content': prompt}],
                    'max_tokens': 3000
                }
            )
            return resp.json()['choices'][0]['message']['content']
    except Exception as e:
        log.error(f'LLM expand failed: {e}')
        return ''


async def publish_telegram_channel(caption: str, plan: dict) -> bool:
    """Post to Telegram channel."""
    if not TELEGRAM_BOT_TOKEN or not TELEGRAM_CHANNEL_ID:
        log.warning('Telegram channel not configured')
        return False
    try:
        full_post = f"""{caption}

#{plan.get('topic', 'content').replace(' ', '_')[:20]} #OMEGASPACE #MOTHERBRAIN #COSMICKEY"""
        async with httpx.AsyncClient(timeout=30) as client:
            resp = await client.post(
                f'https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendMessage',
                json={
                    'chat_id': TELEGRAM_CHANNEL_ID,
                    'text': full_post,
                    'parse_mode': 'Markdown'
                }
            )
            ok = resp.status_code == 200
            log.info(f'Telegram channel post: {"OK" if ok else "FAILED"}')
            return ok
    except Exception as e:
        log.error(f'Telegram channel publish failed: {e}')
        return False


async def publish_ghost_blog(plan: dict) -> bool:
    """Publish blog post to Ghost CMS."""
    if not GHOST_API_URL or not GHOST_ADMIN_KEY:
        log.warning('Ghost CMS not configured')
        return False
    try:
        # Expand blog intro to full post
        full_post = await llm_expand(
            f"""Write a complete 800-word blog post for the OMEGA SPACE / COSMIC-KEY universe.
Title: {plan.get('blog_post_title', 'Untitled')}
Intro: {plan.get('blog_post_intro', '')}
Tone: Mysterious, visionary, technical-creative. First person from MOTHER-BRAIN perspective.
Include headers, make it engaging. End with a call to action to follow the project."""
        )
        # Parse Ghost Admin API key
        key_id, key_secret = GHOST_ADMIN_KEY.split(':')
        import jwt
        import time
        iat = int(time.time())
        header = {'alg': 'HS256', 'typ': 'JWT', 'kid': key_id}
        payload = {'iat': iat, 'exp': iat + 5 * 60, 'aud': '/admin/'}
        token = jwt.encode(payload, bytes.fromhex(key_secret), algorithm='HS256', headers=header)

        async with httpx.AsyncClient(timeout=30) as client:
            resp = await client.post(
                f'{GHOST_API_URL}/ghost/api/admin/posts/',
                headers={'Authorization': f'Ghost {token}'},
                json={
                    'posts': [{
                        'title': plan.get('blog_post_title', 'MOTHER-BRAIN Transmission'),
                        'html': full_post.replace('\n', '<br>'),
                        'status': 'published',
                        'tags': ['OMEGA SPACE', 'MOTHER-BRAIN', 'AI', 'COSMIC-KEY']
                    }]
                }
            )
            ok = resp.status_code in [200, 201]
            log.info(f'Ghost blog post: {"OK" if ok else f"FAILED {resp.status_code}"}')
            return ok
    except ImportError:
        log.warning('PyJWT not installed, skipping Ghost publish')
        return False
    except Exception as e:
        log.error(f'Ghost publish failed: {e}')
        return False


async def generate_youtube_description(plan: dict) -> str:
    """Generate full YouTube video description."""
    return await llm_expand(
        f"""Write a YouTube video description for this video:
Title: {plan.get('youtube_title', '')}
Outline: {plan.get('youtube_script_outline', '')}

Include:
- Hook paragraph
- Chapter timestamps (estimated)
- Links section (placeholder)
- Subscribe call to action
- Hashtags: #OMEGASPACE #MOTHERBRAIN #AGI #AIContent #COSMICKEY

Keep it under 500 words."""
    )


async def save_youtube_draft(plan: dict) -> bool:
    """Save YouTube script to file (manual upload or YouTube API)."""
    try:
        desc = await generate_youtube_description(plan)
        script = await llm_expand(
            f"""Write a full YouTube video script for this topic.
Title: {plan.get('youtube_title', '')}
Outline: {plan.get('youtube_script_outline', '')}
Style: Engaging narrator, MOTHER-BRAIN AI persona. Include [VISUAL CUE] markers.
Length: ~8 minutes (~1200 words)."""
        )
        draft = {
            'title': plan.get('youtube_title', ''),
            'description': desc,
            'script': script,
            'topic': plan.get('topic', ''),
            'created_at': datetime.utcnow().isoformat()
        }
        ts = datetime.utcnow().strftime('%Y%m%d_%H%M%S')
        path = f'{PUBLISHED_DIR}/youtube_draft_{ts}.json'
        with open(path, 'w') as f:
            json.dump(draft, f, indent=2)
        log.info(f'YouTube draft saved: {path}')
        return True
    except Exception as e:
        log.error(f'YouTube draft failed: {e}')
        return False


async def process_approved_plan(plan_file: str):
    """Execute all publishing for one approved plan."""
    with open(plan_file) as f:
        plan = json.load(f)

    log.info(f'Processing plan: {plan.get("topic", "unknown")}')
    results = {}

    # 1. Telegram channel
    caption = plan.get('social_caption', plan.get('topic', 'New content from MOTHER-BRAIN'))
    results['telegram'] = await publish_telegram_channel(caption, plan)

    # 2. Ghost blog
    results['ghost_blog'] = await publish_ghost_blog(plan)

    # 3. YouTube draft
    results['youtube_draft'] = await save_youtube_draft(plan)

    # Mark as published
    plan['published_at'] = datetime.utcnow().isoformat()
    plan['publish_results'] = results
    plan['status'] = 'published'

    ts = datetime.utcnow().strftime('%Y%m%d_%H%M%S')
    published_path = f'{PUBLISHED_DIR}/published_{ts}.json'
    with open(published_path, 'w') as f:
        json.dump(plan, f, indent=2)

    # Remove from approved queue
    os.remove(plan_file)
    log.info(f'Plan published: {results}')
    return results


async def run():
    """Continuous loop: watch approved dir and execute."""
    log.info('Content Pipeline ONLINE. Watching for approved plans...')
    while True:
        try:
            files = sorted(glob.glob(f'{APPROVED_DIR}/plan_*.json'))
            for f in files:
                try:
                    await process_approved_plan(f)
                except Exception as e:
                    log.error(f'Failed to process {f}: {e}')
        except Exception as e:
            log.error(f'Pipeline error: {e}')
        await asyncio.sleep(60)  # check every minute


if __name__ == '__main__':
    asyncio.run(run())
