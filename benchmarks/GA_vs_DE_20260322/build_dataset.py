"""
Consolidate all discovered data sources into a unified ML-ready dataset
for the GA vs DE hyperparameter tuning benchmark.

Sources:
  - GitHub/NOMADZ-0: knowledge_graph.json (361 nodes, 1109 edges) → node feature vectors
  - GitHub/NOMADZ-0: system_monitor.jsonl → system telemetry rows
  - GitHub/NOMADZ-0: characters_sample.json → character attribute vectors
  - GitHub/NOMADZ-0: session_backlog.json → task completion data
  - GitHub/ocean: Component-Status-Files.csv → component status rows
  - Asana: MOTHER-BRAIN/OMEGA-CORE tasks (19 tasks) → project management features
  - Asana: VOLTRON tasks (2 tasks) → additional project rows
  - Notion: databases discovered (Tasks, Notes, Tags, COSMO-LOGOS, Drive Index)
  - Sentry: geologos org (no projects yet)
  - Slack: no messages found (empty workspace)

Strategy:
  Build a 'project entity classification' dataset:
  Each row = one entity (task, character, node, component, etc.)
  Features = numeric/categorical properties
  Target = entity_category (for binary: is_active/incomplete vs complete/ready)
"""

import json
import pandas as pd
import numpy as np
from pathlib import Path

RAW = Path("/home/user/workspace/raw_data")
OUT = Path("/home/user/workspace/benchmark_outputs")
OUT.mkdir(exist_ok=True)

records = []

# ─────────────────────────────────────
# 1. KNOWLEDGE GRAPH NODES (NOMADZ-0)
# ─────────────────────────────────────
with open(RAW / "knowledge_graph.json") as f:
    kg = json.load(f)

type_map = {"document": 0, "named_entity": 1, "technical_term": 2, "tag": 3}
for node in kg["nodes"]:
    nid = node.get("id", 0)
    ntype = node.get("type", "unknown")
    props = node.get("properties", {})
    name = node.get("name", "")

    # Count edges for this node
    edge_count = sum(1 for e in kg["edges"]
                     if e.get("source") == nid or e.get("target") == nid)

    # Name length as proxy for specificity
    name_len = len(name)

    # Is it a URL-sourced document?
    is_url_doc = int("http" in name.lower())

    records.append({
        "source": "knowledge_graph",
        "entity_type": ntype,
        "entity_type_code": type_map.get(ntype, -1),
        "edge_count": edge_count,
        "name_length": name_len,
        "is_url_doc": is_url_doc,
        "file_size": props.get("size", 0),
        "power_level": 0,
        "appearance_count": 0,
        "completed": int(ntype in ("document", "tag")),  # docs/tags = "resolved"
        "created_days_ago": 15,
    })

print(f"Knowledge graph nodes: {len(kg['nodes'])}")

# ─────────────────────────────────────
# 2. SYSTEM MONITOR TELEMETRY
# ─────────────────────────────────────
with open(RAW / "system_monitor.jsonl") as f:
    for line in f:
        row = json.loads(line.strip())
        sys = row.get("system", {})
        svc = row.get("services", {})
        health_map = {"excellent": 1, "good": 0.75, "fair": 0.5, "poor": 0}
        health_score = health_map.get(row.get("overall_health", "fair"), 0.5)
        records.append({
            "source": "system_monitor",
            "entity_type": "telemetry",
            "entity_type_code": 4,
            "edge_count": 0,
            "name_length": 10,
            "is_url_doc": 0,
            "file_size": sys.get("disk_usage", 0),
            "power_level": sys.get("cpu_percent", 0),
            "appearance_count": sys.get("process_count", 0),
            "completed": int(health_score >= 0.75),
            "created_days_ago": 15,
        })

print(f"System monitor rows added")

# ─────────────────────────────────────
# 3. CHARACTERS (NOMADZ-0)
# ─────────────────────────────────────
with open(RAW / "characters_sample.json") as f:
    chars = json.load(f)

for char in chars:
    abilities = char.get("abilities", [])
    max_power = max((a.get("power_level", 0) for a in abilities), default=0)
    avg_power = np.mean([a.get("power_level", 0) for a in abilities]) if abilities else 0
    meta = char.get("meta", {})
    records.append({
        "source": "characters",
        "entity_type": char.get("archetype", "unknown").lower(),
        "entity_type_code": 5,
        "edge_count": len(char.get("relationships", [])),
        "name_length": len(char.get("name", "")),
        "is_url_doc": 0,
        "file_size": len(char.get("story_arcs", [])),
        "power_level": max_power,
        "appearance_count": meta.get("appearance_count", 0),
        "completed": int(meta.get("codex_unlocked", False)),
        "created_days_ago": 15,
    })

print(f"Characters: {len(chars)}")

# ─────────────────────────────────────
# 4. COMPONENT STATUS (ocean/sol-x-workflows)
# ─────────────────────────────────────
comp_df = pd.read_csv(RAW / "component_status.csv")
for _, row in comp_df.iterrows():
    status_str = str(row.get("Status", ""))
    is_ready = int("Ready" in status_str or "✅" in status_str)
    files_str = str(row.get("Files", ""))
    file_count = len([x for x in files_str.split(",") if x.strip()])
    name = str(row.get("Component", ""))
    records.append({
        "source": "component_status",
        "entity_type": "component",
        "entity_type_code": 6,
        "edge_count": file_count,
        "name_length": len(name),
        "is_url_doc": 0,
        "file_size": 0,
        "power_level": 0,
        "appearance_count": 0,
        "completed": is_ready,
        "created_days_ago": 15,
    })

print(f"Components: {len(comp_df)}")

# ─────────────────────────────────────
# 5. ASANA TASKS — MOTHER-BRAIN
# ─────────────────────────────────────
import datetime

mother_brain_tasks = [
    "Define schedule trigger schema (cron + event-driven)",
    "Build execution routing logic (agent dispatcher)",
    "Implement state aggregation bus",
    "Wire research agent + toolchain",
    "Wire creative production agent",
    "Wire DevOps automation agent",
    "Deploy short-term vector store (pgvector or Weaviate)",
    "Design long-term PostgreSQL schema",
    "Implement semantic gating / relevance filter",
    "Build persistent decision log",
    "Build 3D environment state atomizer (tensor pipeline)",
    "Define observation space",
    "Design reward function",
    "Implement PPO training loop",
    "Build action matrix synthesis layer",
    "Integrate rigid-body physics engine",
    "Build swarm management controller",
    "Implement neural telemetry stream (feeds back to MOTHER-BRAIN)",
    "Build real-time monitoring dashboard",
]

created_dates = [
    "2026-03-12T18:29:11.682Z", "2026-03-12T18:29:12.759Z", "2026-03-12T18:29:13.718Z",
    "2026-03-12T18:29:14.616Z", "2026-03-12T18:29:15.550Z", "2026-03-12T18:29:16.249Z",
    "2026-03-12T18:29:18.109Z", "2026-03-12T18:29:18.978Z", "2026-03-12T18:29:20.000Z",
    "2026-03-12T18:29:20.911Z", "2026-03-12T18:29:22.362Z", "2026-03-12T18:29:23.343Z",
    "2026-03-12T18:29:24.234Z", "2026-03-12T18:29:25.102Z", "2026-03-12T18:29:25.978Z",
    "2026-03-12T18:29:27.464Z", "2026-03-12T18:29:28.592Z", "2026-03-12T18:29:29.355Z",
    "2026-03-12T18:29:30.235Z",
]

now = datetime.datetime(2026, 3, 22, tzinfo=datetime.timezone.utc)
for task_name, created_at in zip(mother_brain_tasks, created_dates):
    created_dt = datetime.datetime.fromisoformat(created_at.replace("Z", "+00:00"))
    days_ago = (now - created_dt).days
    # Classify complexity by name length + keyword presence
    is_infra = int(any(k in task_name.lower() for k in ["deploy", "build", "implement", "integrate"]))
    records.append({
        "source": "asana_mother_brain",
        "entity_type": "task",
        "entity_type_code": 7,
        "edge_count": 0,
        "name_length": len(task_name),
        "is_url_doc": 0,
        "file_size": 0,
        "power_level": is_infra * 7,
        "appearance_count": 1,
        "completed": 0,  # all uncompleted
        "created_days_ago": days_ago,
    })

# VOLTRON tasks
voltron_tasks = [
    ("NOMADZ-0", "sync and push to github", "2026-02-05T01:15:43.509Z"),
    ("VOLTRON CASCADE: BACKUP-ASANA-PULSE", "BACKUP: rclone sdcardGEOLOGOS→Drive", "2026-02-11T22:41:39.946Z"),
]
for tname, tnotes, tcreated in voltron_tasks:
    created_dt = datetime.datetime.fromisoformat(tcreated.replace("Z", "+00:00"))
    days_ago = (now - created_dt).days
    records.append({
        "source": "asana_voltron",
        "entity_type": "task",
        "entity_type_code": 7,
        "edge_count": 0,
        "name_length": len(tname),
        "is_url_doc": 0,
        "file_size": 0,
        "power_level": 5,
        "appearance_count": 1,
        "completed": 0,
        "created_days_ago": days_ago,
    })

print(f"Asana tasks: {len(mother_brain_tasks) + len(voltron_tasks)}")

# ─────────────────────────────────────
# 6. SESSION BACKLOG TASKS (NOMADZ-0)
# ─────────────────────────────────────
with open(RAW / "session_backlog.json") as f:
    backlog = json.load(f)

completed_tasks = backlog.get("completed_tasks", [])
next_steps = backlog.get("next_steps", [])

for task in completed_tasks:
    records.append({
        "source": "session_backlog_completed",
        "entity_type": "task",
        "entity_type_code": 7,
        "edge_count": 0,
        "name_length": len(task),
        "is_url_doc": 0,
        "file_size": 0,
        "power_level": 6,
        "appearance_count": 1,
        "completed": 1,
        "created_days_ago": 15,
    })

for task in next_steps:
    records.append({
        "source": "session_backlog_pending",
        "entity_type": "task",
        "entity_type_code": 7,
        "edge_count": 0,
        "name_length": len(task),
        "is_url_doc": 0,
        "file_size": 0,
        "power_level": 5,
        "appearance_count": 1,
        "completed": 0,
        "created_days_ago": 15,
    })

print(f"Session backlog: {len(completed_tasks)} completed + {len(next_steps)} pending")

# ─────────────────────────────────────
# 7. NOTION DATABASES (metadata)
# ─────────────────────────────────────
notion_items = [
    ("MOTHER-BRAIN Pulse Log", "page", 1, "2026-03-20"),
    ("VII. PROJECT ECOSYSTEM ROADMAPS", "page", 0, "2026-03-22"),
    ("NOMADZ-0 STORY BIBLE", "page", 0, "2026-03-20"),
    ("MOTHER-BRAIN Drive Index", "database", 1, "2026-03-07"),
    ("COSMO-LOGOS", "database", 1, "2026-03-07"),
    ("Tasks", "database", 1, "2025-10-30"),
    ("Tools & Knowledge Pillars", "database", 1, "2026-03-07"),
    ("Notes", "database", 1, "2026-03-07"),
    ("Tags", "database", 1, "2026-03-07"),
    ("Tag Rank", "database", 1, "2026-03-07"),
    ("Cosmic Key Release Checklist", "page", 0, "2026-02-12"),
    ("Master Archive Specification", "page", 1, "2026-03-20"),
    ("VOLTRON", "page", 0, "2026-03-07"),
    ("Projects & Tasks", "page", 0, "2026-02-12"),
    ("LINKS-TOOLS-BRAINS", "page", 0, "2026-03-07"),
    ("/codeOMADZ-bootstrap", "page", 0, "2026-03-07"),
    ("NOMADZ", "page", 0, "2025-12-01"),
    ("Login-PASS", "page", 0, "2026-03-07"),
    ("Book Recommendation", "page", 0, "2026-03-07"),
    ("Google Keep", "page", 0, "2026-03-07"),
    ("KEYS-TOKENS", "page", 0, "2026-03-07"),
    ("Url libraries and Gitinjests", "page", 0, "2026-03-07"),
    ("To do", "page", 0, "2026-03-07"),
]

import re
for title, etype, has_data, date_str in notion_items:
    try:
        dt = datetime.datetime.strptime(date_str, "%Y-%m-%d").replace(tzinfo=datetime.timezone.utc)
        days_ago = (now - dt).days
    except:
        days_ago = 30
    is_db = int(etype == "database")
    records.append({
        "source": "notion",
        "entity_type": "notion_" + etype,
        "entity_type_code": 8 + is_db,
        "edge_count": 0,
        "name_length": len(title),
        "is_url_doc": 0,
        "file_size": 0,
        "power_level": has_data * 8,
        "appearance_count": 0,
        "completed": has_data,
        "created_days_ago": days_ago,
    })

print(f"Notion items: {len(notion_items)}")

# ─────────────────────────────────────
# BUILD DATAFRAME
# ─────────────────────────────────────
df = pd.DataFrame(records)
print(f"\nTotal records: {len(df)}")
print(f"Target balance: {df['completed'].value_counts().to_dict()}")
print(f"Sources: {df['source'].value_counts().to_dict()}")

# Feature columns for ML (all numeric)
feature_cols = [
    "entity_type_code",
    "edge_count",
    "name_length",
    "is_url_doc",
    "file_size",
    "power_level",
    "appearance_count",
    "created_days_ago",
]

X = df[feature_cols].fillna(0).values.astype(float)
y = df["completed"].values.astype(int)

# Normalize features
from sklearn.preprocessing import RobustScaler
scaler = RobustScaler()
X_scaled = scaler.fit_transform(X)

print(f"\nFeature matrix: {X.shape}")
print(f"Positive rate: {y.mean():.2%}")
print(f"Feature columns: {feature_cols}")

# Save full dataset
df.to_csv(OUT / "nomadz_unified_dataset.csv", index=False)
print(f"\nSaved: {OUT}/nomadz_unified_dataset.csv")

# Save ML-ready arrays
np.save(OUT / "X_features.npy", X_scaled)
np.save(OUT / "y_labels.npy", y)

# Dataset profile
profile = pd.DataFrame({
    "Attribute": [
        "Dataset Name", "Sources", "Total Records", "Features",
        "Target", "Positive Rate",
        "Source Breakdown",
        "Feature Columns",
    ],
    "Value": [
        "NOMADZ Unified Dataset",
        "GitHub (NOMADZ-0, ocean, MOTHER-BRAIN), Asana, Notion, system telemetry",
        str(len(df)),
        str(len(feature_cols)),
        "Binary: completed/active (1) vs pending/unresolved (0)",
        f"{y.mean():.2%}",
        str(df['source'].value_counts().to_dict()),
        str(feature_cols),
    ]
})
profile.to_csv(OUT / "dataset_profile.csv", index=False)
print("Saved dataset profile.")

# Summary stats per source
source_stats = df.groupby("source")["completed"].agg(["count", "mean"]).rename(
    columns={"count": "n_records", "mean": "positive_rate"})
source_stats["positive_rate"] = source_stats["positive_rate"].round(3)
print("\nPer-source summary:")
print(source_stats.to_string())
source_stats.to_csv(OUT / "source_summary.csv")
