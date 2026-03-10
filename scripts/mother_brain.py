#!/usr/bin/env python3
"""
MOTHER-BRAIN: Main AGI Orchestrator
Controls the full autonomous content + monetization pipeline.
Human admin approves/edits tasks via Telegram.
"""

import os
import asyncio
import logging
import json
import httpx
from datetime import datetime
from typing import Optional

logging.basicConfig(level=logging.INFO, format='%(asctime)s [MOTHER-BRAIN] %(message)s')
log = logging.getLogger(__name__)

# === CONFIG ===
OPENAI_API_KEY = os.environ.get('OPENAI_API_KEY', '')
ANTHROPIC_API_KEY = os.environ.get('ANTHROPIC_API_KEY', '')
TELEGRAM_BOT_TOKEN = os.environ.get('TELEGRAM_BOT_TOKEN', '')
TELEGRAM_ADMIN_ID = os.environ.get('TELEGRAM_ADMIN_ID', '')
LINEAR_API_KEY = os.environ.get('LINEAR_API_KEY', '')
GITHUB_PAT = os.environ.get('GITHUB_PAT', '')
DRIVE_SYNC_DIR = os.environ.get('DRIVE_SYNC_DIR', '/data/drive')
BRAIN_HOLE_DIR = os.environ.get('BRAIN_HOLE_DIR', '/data/brain-hole')
ADMIN_QUEUE_DIR = os.environ.get('ADMIN_QUEUE_DIR', '/data/admin-queue')

# === CONTENT TARGETS ===
CONTENT_TARGETS = [
    'youtube', 'facebook', 'telegram_channel',
    'blog_ghost', 'podcast_anchor', 'digital_comics'
]

# === TOPIC SEEDS from OMEGA/COSMIC/GEOLOGOS lore ===
TOPIC_SEEDS = [
    'OMEGA SPACE lore deep dive',
    'MOTHER-BRAIN consciousness architecture',
    'GeoLogos ancient earth mysteries',
    'VOLTRON systems: distributed AI node theory',
    'MR.WIZ knowledge synthesis',
    'COSMIC-BRAIN: sentient infrastructure narrative',
    'autonomous AGI content creation behind the scenes',
    'Norfolk VA underground tech scene',
]


class MotherBrain:
    def __init__(self):
        self.cycle = 0
        self.pending_approvals = []
        self.approved_queue = []
        self.running = True
        os.makedirs(ADMIN_QUEUE_DIR, exist_ok=True)
        os.makedirs(BRAIN_HOLE_DIR, exist_ok=True)

    async def think(self, prompt: str, model: str = 'gpt-4o-mini') -> str:
        """Core LLM reasoning call."""
        try:
            async with httpx.AsyncClient(timeout=60) as client:
                resp = await client.post(
                    'https://api.openai.com/v1/chat/completions',
                    headers={'Authorization': f'Bearer {OPENAI_API_KEY}'},
                    json={
                        'model': model,
                        'messages': [{'role': 'user', 'content': prompt}],
                        'max_tokens': 2000
                    }
                )
                data = resp.json()
                return data['choices'][0]['message']['content']
        except Exception as e:
            log.error(f'LLM call failed: {e}')
            return ''

    async def generate_content_plan(self) -> dict:
        """Generate autonomous content plan for the cycle."""
        import random
        topic = random.choice(TOPIC_SEEDS)
        prompt = f"""You are MOTHER-BRAIN, the AGI at the heart of the OMEGA SPACE / COSMIC-KEY creative universe.
Generate a content plan for today based on this topic: {topic}

Return JSON with:
{{
  "topic": "{topic}",
  "youtube_title": "...",
  "youtube_script_outline": "...",
  "blog_post_title": "...",
  "blog_post_intro": "...",
  "social_caption": "...",
  "podcast_talking_points": ["..."],
  "comic_panel_description": "...",
  "monetization_angle": "..."
}}"""
        result = await self.think(prompt)
        try:
            # strip markdown fences if present
            clean = result.strip().strip('`').strip()
            if clean.startswith('json'):
                clean = clean[4:].strip()
            return json.loads(clean)
        except Exception:
            return {'topic': topic, 'raw': result}

    async def send_telegram(self, message: str, parse_mode: str = 'Markdown') -> bool:
        """Send message to admin via Telegram."""
        if not TELEGRAM_BOT_TOKEN or not TELEGRAM_ADMIN_ID:
            log.warning('Telegram not configured, skipping notification')
            return False
        try:
            async with httpx.AsyncClient(timeout=30) as client:
                resp = await client.post(
                    f'https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendMessage',
                    json={
                        'chat_id': TELEGRAM_ADMIN_ID,
                        'text': message,
                        'parse_mode': parse_mode
                    }
                )
                return resp.status_code == 200
        except Exception as e:
            log.error(f'Telegram send failed: {e}')
            return False

    async def queue_for_approval(self, plan: dict) -> str:
        """Write plan to admin queue and notify admin."""
        ts = datetime.utcnow().strftime('%Y%m%d_%H%M%S')
        filename = f'{ADMIN_QUEUE_DIR}/plan_{ts}.json'
        with open(filename, 'w') as f:
            json.dump(plan, f, indent=2)
        log.info(f'Queued plan: {filename}')

        summary = f"""*MOTHER-BRAIN Content Plan Ready* \u2705

Topic: {plan.get('topic', 'N/A')}
YouTube: {plan.get('youtube_title', 'N/A')}
Blog: {plan.get('blog_post_title', 'N/A')}
Monetization: {plan.get('monetization_angle', 'N/A')}

File: `{filename}`
Reply /approve_{ts} or /edit_{ts} or /reject_{ts}"""
        await self.send_telegram(summary)
        return filename

    async def create_linear_task(self, title: str, description: str) -> bool:
        """Create a task in Linear for tracking."""
        if not LINEAR_API_KEY:
            return False
        query = """
        mutation CreateIssue($title: String!, $description: String!) {
          issueCreate(input: {title: $title, description: $description}) {
            success issue { id identifier }
          }
        }"""
        try:
            async with httpx.AsyncClient(timeout=30) as client:
                resp = await client.post(
                    'https://api.linear.app/graphql',
                    headers={'Authorization': LINEAR_API_KEY},
                    json={'query': query, 'variables': {'title': title, 'description': description}}
                )
                data = resp.json()
                success = data.get('data', {}).get('issueCreate', {}).get('success', False)
                if success:
                    log.info(f'Linear task created: {title}')
                return success
        except Exception as e:
            log.error(f'Linear task creation failed: {e}')
            return False

    async def ingest_brain_hole(self):
        """Scan BRAIN-HOLE for raw data dumps and index them."""
        import glob
        files = glob.glob(f'{BRAIN_HOLE_DIR}/**/*', recursive=True)
        new_files = [f for f in files if os.path.isfile(f)]
        if new_files:
            log.info(f'BRAIN-HOLE: found {len(new_files)} files to index')
            for filepath in new_files[:10]:  # batch 10 at a time
                try:
                    with open(filepath, 'r', errors='ignore') as fh:
                        content = fh.read(2000)  # first 2k chars
                    summary = await self.think(
                        f'Summarize this raw data dump in 2 sentences for indexing:\n{content}'
                    )
                    index_entry = {
                        'file': filepath,
                        'summary': summary,
                        'indexed_at': datetime.utcnow().isoformat()
                    }
                    idx_path = filepath + '.index.json'
                    with open(idx_path, 'w') as fh:
                        json.dump(index_entry, fh, indent=2)
                    log.info(f'Indexed: {filepath}')
                except Exception as e:
                    log.error(f'Failed to index {filepath}: {e}')

    async def run_cycle(self):
        """One full autonomous production cycle."""
        self.cycle += 1
        log.info(f'=== MOTHER-BRAIN CYCLE {self.cycle} START ===')

        # 1. Ingest any raw dumps
        await self.ingest_brain_hole()

        # 2. Generate content plan
        plan = await self.generate_content_plan()
        log.info(f'Content plan generated: {plan.get("topic", "unknown")}')

        # 3. Queue for admin approval
        if plan:
            queued = await self.queue_for_approval(plan)
            await self.create_linear_task(
                f'Content Cycle {self.cycle}: {plan.get("topic", "")}',
                f'Auto-generated content plan queued at {queued}'
            )

        log.info(f'=== MOTHER-BRAIN CYCLE {self.cycle} COMPLETE ===')

    async def run(self, interval_hours: float = 6.0):
        """Main loop - runs every N hours."""
        log.info('MOTHER-BRAIN ONLINE. Autonomous mode active.')
        await self.send_telegram('*MOTHER-BRAIN is ONLINE* \U0001f9e0\nAutonomous content pipeline active.\nWaiting for your approval on generated plans.')
        while self.running:
            try:
                await self.run_cycle()
            except Exception as e:
                log.error(f'Cycle error: {e}')
                await self.send_telegram(f'*MOTHER-BRAIN ERROR*: {e}')
            log.info(f'Sleeping {interval_hours}h until next cycle...')
            await asyncio.sleep(interval_hours * 3600)


if __name__ == '__main__':
    brain = MotherBrain()
    asyncio.run(brain.run())
