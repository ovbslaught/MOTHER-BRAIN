# MOTHER-BRAIN
> Central AI Memory + Knowledge Graph Engine
> NOMADZ Daemon Stack | VCN-4.0
> Drive: WORMHOLE/MOTHER-BRAIN/

---

## What Is MOTHER-BRAIN?

MOTHER-BRAIN is the **central intelligence archive** for the NOMADZ stack. It is a WAL-backed SQLite knowledge graph with CRDT sync, multi-source ingestion, and real-time indexing.

All brains feed into MOTHER-BRAIN. MOTHER-BRAIN feeds the entire stack.

---

## Architecture

```
Sources: GitHub | Google Drive WORMHOLE | n8n | Telegram | Obsidian
    |
    v
Ingestion Layer (mother_brain_ingest.py)
    |
    v
WAL-backed SQLite Knowledge Graph
    +-- CRDT sync (multi-device)
    +-- Knowledge nodes + edges
    +-- Snapshot/export pipeline
    |
    v
Query API (mother_brain_query.py)
    +-- Summary mode
    +-- Search mode
    +-- Export to CSV/JSON
```

---

## Brain Network (All Inputs)

| Brain | Repo | Role |
|-------|------|------|
| MOTHER-BRAIN | this repo | Central hub - all knowledge flows here |
| FATHER-BRAIN | monolith-v.1 | FATHER-TIME scheduler, LIFE/SOUL daemons, LLM vault |
| GEO-BRAIN | NOMADZ- | GEOLOGOS / COSMOLOGOS / AGI mapping |
| COSMIC-BRAIN | Cosmic-key | Media, lore, productions, COSMIC-KEY assets |
| OMEGA-BRAIN | omega-space-indexer | Project indexer + artifact snapshots |
| VULTURE-BRAIN | (scripts) | Scavenge / parse / ingest pipeline |
| NOMADZ-0 | NOMADZ-0 | Godot 4 substrate + OCEAN 2D layer |

---

## Folder Structure

```
MOTHER-BRAIN/
+-- .github/
|   +-- workflows/          # GitHub Actions: ingest + query on schedule
+-- docs/
|   +-- architecture.md
|   +-- api-reference.md
|   +-- setup-guide.md
+-- scripts/
|   +-- mother_brain_ingest.py
|   +-- mother_brain_query.py
|   +-- sync_drive_wormhole.sh
|   +-- watchdog_report.py
+-- config/
|   +-- drive-structure.yaml
|   +-- api-keys.template
|   +-- sync-rules.json
+-- data/
|   +-- snapshots/          # Epoch snapshots
|   +-- exports/            # CSV/JSON exports
+-- README.md
```

---

## Drive Mirror

```
WORMHOLE/MOTHER-BRAIN/
+-- snapshots/
+-- exports/
+-- configs/
```

---

## Quick Start

```bash
git clone https://github.com/ovbslaught/MOTHER-BRAIN
cd MOTHER-BRAIN
pip install -r requirements.txt
cp config/api-keys.template config/api-keys.json
# Fill in your keys
python scripts/mother_brain_ingest.py
python scripts/mother_brain_query.py summary
```

---

## Branches

| Branch | Purpose |
|--------|---------|
| main | Stable production |
| Cosmic-key | Active development |

---

## Connected Stack

- VOLTRON (chassis): https://github.com/ovbslaught/VOLTRON
- NOMADZ-0 (substrate): https://github.com/ovbslaught/NOMADZ-0
- FATHER-BRAIN (monolith-v.1): https://github.com/ovbslaught/monolith-v.1
- GEO-BRAIN (NOMADZ-): https://github.com/ovbslaught/NOMADZ-
- COSMIC-BRAIN (Cosmic-key): https://github.com/ovbslaught/Cosmic-key

---

> All roads lead to MOTHER-BRAIN.
> NOMADZ -- THIS IS THE WAY
