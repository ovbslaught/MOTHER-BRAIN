#!/usr/bin/env python3
"""
Telegram Admin Bot for MOTHER-BRAIN
Handles /approve /edit /reject commands for content plans.
Also provides system status and manual triggers.
"""

import os
import asyncio
import logging
import json
import glob
from datetime import datetime
import httpx

logging.basicConfig(level=logging.INFO, format='%(asctime)s [TELEGRAM-ADMIN] %(message)s')
log = logging.getLogger(__name__)

BOT_TOKEN = os.environ.get('TELEGRAM_BOT_TOKEN', '')
ADMIN_ID = int(os.environ.get('TELEGRAM_ADMIN_ID', '0'))
ADMIN_QUEUE_DIR = os.environ.get('ADMIN_QUEUE_DIR', '/data/admin-queue')
APPROVED_DIR = os.environ.get('APPROVED_DIR', '/data/approved')
BASE_URL = f'https://api.telegram.org/bot{BOT_TOKEN}'

os.makedirs(ADMIN_QUEUE_DIR, exist_ok=True)
os.makedirs(APPROVED_DIR, exist_ok=True)


async def api_call(method: str, data: dict) -> dict:
    async with httpx.AsyncClient(timeout=30) as client:
        resp = await client.post(f'{BASE_URL}/{method}', json=data)
        return resp.json()


async def send_message(chat_id: int, text: str, parse_mode: str = 'Markdown') -> dict:
    return await api_call('sendMessage', {
        'chat_id': chat_id, 'text': text, 'parse_mode': parse_mode
    })


async def handle_command(message: dict):
    chat_id = message['chat']['id']
    user_id = message['from']['id']
    text = message.get('text', '')

    # Security: only admin can control
    if user_id != ADMIN_ID:
        await send_message(chat_id, 'Unauthorized. This is MOTHER-BRAIN admin interface.')
        return

    if text == '/start' or text == '/help':
        help_text = """*MOTHER-BRAIN Admin Interface* \U0001f9e0

/status - System status
/queue - List pending plans
/approve_TIMESTAMP - Approve a content plan
/reject_TIMESTAMP - Reject a content plan  
/trigger - Force new content cycle
/drain - Show all approved content ready to publish
/help - This message"""
        await send_message(chat_id, help_text)

    elif text == '/status':
        pending = len(glob.glob(f'{ADMIN_QUEUE_DIR}/plan_*.json'))
        approved = len(glob.glob(f'{APPROVED_DIR}/plan_*.json'))
        status = f"""*MOTHER-BRAIN Status* \u2705

Pending approval: {pending} plans
Approved & ready: {approved} plans
Time: {datetime.utcnow().strftime('%Y-%m-%d %H:%M UTC')}

All systems nominal."""
        await send_message(chat_id, status)

    elif text == '/queue':
        files = sorted(glob.glob(f'{ADMIN_QUEUE_DIR}/plan_*.json'))
        if not files:
            await send_message(chat_id, 'No pending plans in queue.')
            return
        msg = f'*Pending Plans ({len(files)}):*\n\n'
        for f in files[-5:]:  # show last 5
            ts = os.path.basename(f).replace('plan_', '').replace('.json', '')
            try:
                with open(f) as fh:
                    plan = json.load(fh)
                topic = plan.get('topic', 'Unknown')
                msg += f'\u2022 `{ts}` - {topic}\n'
                msg += f'  /approve_{ts} | /reject_{ts}\n\n'
            except Exception:
                msg += f'\u2022 `{ts}` (unreadable)\n'
        await send_message(chat_id, msg)

    elif text.startswith('/approve_'):
        ts = text.replace('/approve_', '').strip()
        src = f'{ADMIN_QUEUE_DIR}/plan_{ts}.json'
        dst = f'{APPROVED_DIR}/plan_{ts}.json'
        if os.path.exists(src):
            os.rename(src, dst)
            # Mark approved
            with open(dst) as f:
                plan = json.load(f)
            plan['approved_at'] = datetime.utcnow().isoformat()
            plan['status'] = 'approved'
            with open(dst, 'w') as f:
                json.dump(plan, f, indent=2)
            await send_message(chat_id, f'\u2705 Plan `{ts}` APPROVED\nContent pipeline will execute next cycle.')
            log.info(f'Plan approved: {ts}')
        else:
            await send_message(chat_id, f'\u274c Plan `{ts}` not found in queue.')

    elif text.startswith('/reject_'):
        ts = text.replace('/reject_', '').strip()
        src = f'{ADMIN_QUEUE_DIR}/plan_{ts}.json'
        if os.path.exists(src):
            os.remove(src)
            await send_message(chat_id, f'\U0001f5d1 Plan `{ts}` REJECTED and discarded.')
            log.info(f'Plan rejected: {ts}')
        else:
            await send_message(chat_id, f'\u274c Plan `{ts}` not found.')

    elif text.startswith('/edit_'):
        ts = text.split(' ')[0].replace('/edit_', '').strip()
        note = text.replace(f'/edit_{ts}', '').strip()
        src = f'{ADMIN_QUEUE_DIR}/plan_{ts}.json'
        if os.path.exists(src):
            with open(src) as f:
                plan = json.load(f)
            plan['admin_note'] = note
            plan['edited'] = True
            with open(src, 'w') as f:
                json.dump(plan, f, indent=2)
            await send_message(chat_id, f'\u270f Plan `{ts}` updated with note: {note}')
        else:
            await send_message(chat_id, f'\u274c Plan `{ts}` not found.')

    elif text == '/drain':
        files = sorted(glob.glob(f'{APPROVED_DIR}/plan_*.json'))
        if not files:
            await send_message(chat_id, 'No approved content ready.')
            return
        msg = f'*Approved Content Ready ({len(files)}):*\n\n'
        for f in files[-5:]:
            ts = os.path.basename(f).replace('plan_', '').replace('.json', '')
            try:
                with open(f) as fh:
                    plan = json.load(fh)
                msg += f'\u2022 `{ts}` - {plan.get("topic", "?")}\n'
                msg += f'  YouTube: {plan.get("youtube_title", "?")[:50]}\n'
                msg += f'  Blog: {plan.get("blog_post_title", "?")[:50]}\n\n'
            except Exception:
                pass
        await send_message(chat_id, msg)

    elif text == '/trigger':
        trigger_file = f'{ADMIN_QUEUE_DIR}/.trigger_{datetime.utcnow().strftime("%Y%m%d_%H%M%S")}'
        open(trigger_file, 'w').close()
        await send_message(chat_id, '\u26a1 Manual trigger sent. New content cycle will start shortly.')

    else:
        await send_message(chat_id, f'Unknown command: `{text}`\nUse /help for available commands.')


async def poll_updates(offset: int = 0) -> tuple:
    result = await api_call('getUpdates', {'offset': offset, 'timeout': 30, 'allowed_updates': ['message']})
    updates = result.get('result', [])
    for update in updates:
        offset = update['update_id'] + 1
        if 'message' in update:
            try:
                await handle_command(update['message'])
            except Exception as e:
                log.error(f'Handler error: {e}')
    return offset, len(updates)


async def run():
    log.info('Telegram admin bot starting...')
    offset = 0
    while True:
        try:
            offset, count = await poll_updates(offset)
            if count == 0:
                await asyncio.sleep(1)
        except Exception as e:
            log.error(f'Poll error: {e}')
            await asyncio.sleep(5)


if __name__ == '__main__':
    asyncio.run(run())
