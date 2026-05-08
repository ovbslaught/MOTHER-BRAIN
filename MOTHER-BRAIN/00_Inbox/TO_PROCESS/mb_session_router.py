#!/usr/bin/env python3
import re, json, sys
from pathlib import Path
from datetime import datetime

ROOT = Path("/storage/emulated/0/Wormhole")
CODE_DIR = ROOT / "MOTHER-BRAIN" / "05Code"
OBS_INBOX = ROOT / "MOTHER-BRAIN" / "01KnowledgeGraph" / "00Inbox"
AGENTS_DIR = ROOT / "agents"
OMEGA_JSON_DIR = ROOT / "OMEGA-BRAIN" / "json"
LOGS_DIR = ROOT / "logs"
CANON_ROOT = ROOT / "CANON"
CANON_WAL = CANON_ROOT / "wal" / "events-2026-03.jsonl"

for p in (CODE_DIR, OBS_INBOX, AGENTS_DIR, OMEGA_JSON_DIR, LOGS_DIR, CANON_WAL.parent):
    p.mkdir(parents=True, exist_ok=True)


def push_to_notion(summary: str, meta: dict):
    return


def append_canon_event(event: dict):
    event.setdefault("ts", datetime.utcnow().isoformat() + "Z")
    line = json.dumps(event, sort_keys=True)
    with CANON_WAL.open("a", encoding="utf-8") as f:
        f.write(line + "\
")


def split_by_json_separator(text: str):
    parts, current = [], []
    for line in text.splitlines():
        if line.strip() == "---JSON---":
            if current:
                parts.append("\
".join(current).strip())
                current = []
        else:
            current.append(line)
    if current:
        parts.append("\
".join(current).strip())
    return parts


def extract_blocks(text: str):
    code_blocks = []
    code_pat = re.compile(r"```(\\w+)?\
(.*?)```", re.DOTALL)
    remaining = text
    for m in code_pat.finditer(text):
        lang = (m.group(1) or "").strip().lower()
        body = m.group(2)
        code_blocks.append((lang, body))
    remaining = code_pat.sub("", remaining)
    return code_blocks, remaining.strip()


def parse_json_docs(segments):
    docs = []
    for seg in segments:
        s = seg.strip()
        if s.startswith("{") and s.endswith("}"):
            try:
                obj = json.loads(s)
                docs.append(obj)
            except Exception:
                continue
    return docs


def classify_json_obj(obj):
    if isinstance(obj, dict) and "agents" in obj and "defaults" in obj:
        return "registry"
    if isinstance(obj, dict) and obj.get("id") and obj.get("persona"):
        return "agent_override"
    if isinstance(obj, dict) and "files" in obj and "total_file_count" in obj:
        return "files_snapshot"
    return "generic"


def save_code_blocks(code_blocks):
    ts = datetime.utcnow().strftime("%Y%m%dT%H%M%SZ")
    saved = []
    ext_map = {"python": ".py", "py": ".py", "bash": ".sh", "sh": ".sh",
               "gd": ".gd", "gdscript": ".gd"}
    for idx, (lang, body) in enumerate(code_blocks, start=1):
        ext = ext_map.get(lang, ".txt")
        path = CODE_DIR / f"session_{ts}_{idx}{ext}"
        path.write_text(body.strip() + "\
", encoding="utf-8")
        saved.append(str(path))
    return saved


def save_json_docs(json_docs):
    ts = datetime.utcnow().strftime("%Y%m%dT%H%M%SZ")
    refs = []
    for idx, obj in enumerate(json_docs, start=1):
        kind = classify_json_obj(obj)
        if kind == "registry":
            path = ROOT / "agents.registry.json"
        elif kind == "agent_override":
            agent_id = obj.get("id", f"agent_{idx}")
            path = AGENTS_DIR / f"{agent_id}.json"
        elif kind == "files_snapshot":
            path = OMEGA_JSON_DIR / f"files_snapshot_{ts}_{idx}.json"
        else:
            path = OMEGA_JSON_DIR / f"generic_{ts}_{idx}.json"
        path.write_text(json.dumps(obj, indent=2), encoding="utf-8")
        refs.append((kind, str(path)))
    return refs


def save_transcript(plain_text, code_refs, json_refs):
    ts = datetime.utcnow().strftime("%Y%m%dT%H%M%SZ")
    md_path = OBS_INBOX / f"session_{ts}.md"
    lines = [
        "---",
        f"created_utc: {datetime.utcnow().isoformat()}Z",
        "type: session_transcript",
        "---",
        "",
        "# Session transcript",
        "",
    ]
    if plain_text:
        lines.append("## Chat")
        lines.append("")
        lines.append(plain_text)
        lines.append("")
    if code_refs:
        lines.append("## Extracted code files")
        for p in code_refs:
            lines.append(f"- `{p}`")
        lines.append("")
    if json_refs:
        lines.append("## Extracted JSON artifacts")
        for kind, p in json_refs:
            lines.append(f"- `{kind}` → `{p}`")
        lines.append("")
    md_path.write_text("\
".join(lines), encoding="utf-8")
    return md_path


def main():
    if len(sys.argv) < 2:
        print("usage: mb_session_router.py <session_text_file>", file=sys.stderr)
        sys.exit(1)

    src = Path(sys.argv[1])
    raw = src.read_text(encoding="utf-8")

    segments = split_by_json_separator(raw)
    if not segments:
        segments = [raw]

    code_blocks, plain_text = extract_blocks(segments[0])
    json_docs = parse_json_docs(segments)

    code_refs = save_code_blocks(code_blocks)
    json_refs = save_json_docs(json_docs)
    md_path = save_transcript(plain_text, code_refs, json_refs)

    summary = f"Ingested session → {md_path.name}, code={len(code_refs)}, json={len(json_refs)}"

    append_canon_event({
        "type": "session.ingest",
        "source_file": str(src),
        "obsidian_note": str(md_path),
        "code_files": code_refs,
        "json_artifacts": json_refs,
        "summary": summary
    })

    push_to_notion(summary, {"obsidian_note": str(md_path)})

    log_path = LOGS_DIR / "session_ingest.log"
    with log_path.open("a", encoding="utf-8") as lf:
        lf.write(summary + "\
")

    print(summary)


if __name__ == "__main__":
    main()
