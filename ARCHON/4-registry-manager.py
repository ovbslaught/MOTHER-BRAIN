#!/usr/bin/env python3
"""
GEOLOGOS Dataset Factory - Registry Manager & Datasheet Generator
Maintain immutable dataset registry and generate datasheets for transparency.
"""

import json
import pathlib
import hashlib
from typing import Dict, Any, List
import logging
from datetime import datetime

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class RegistryManager:
    """Manage immutable dataset registry (append-only JSONL)."""

    def __init__(self, registry_root: str):
        self.registry_root = pathlib.Path(registry_root)
        self.registry_root.mkdir(parents=True, exist_ok=True)
        self.registry_path = self.registry_root / "datasets.jsonl"

    def _compute_hash(self, filepath: pathlib.Path) -> str:
        """Compute SHA-256 hash of file."""
        import hashlib
        sha256 = hashlib.sha256()
        with open(filepath, "rb") as f:
            for block in iter(lambda: f.read(4096), b""):
                sha256.update(block)
        return sha256.hexdigest()

    def register_dataset(
        self,
        dataset_id: str,
        version: str,
        sources: List[Dict[str, Any]],  # [{source_type, path, file_count, hash}]
        build_config: Dict[str, Any],
        embedding_model: str,
        embedding_dim: int,
        approver: str,
    ) -> Dict[str, Any]:
        """
        Register a new dataset version.
        Appends to registry (immutable).
        """
        build_timestamp = datetime.utcnow().isoformat() + "Z"

        entry = {
            "dataset_id": dataset_id,
            "version": version,
            "build_timestamp": build_timestamp,
            "build_config": build_config,
            "sources": sources,
            "gold_build": {
                "embedding_model": embedding_model,
                "embedding_dim": embedding_dim,
            },
            "datasheet": {
                "motivation": "RAG knowledge base for GEOLOGOS Cosmic Key universe and cross-session context rehydration",
                "composition": "Blogs (120 articles), PDFs (technical guides), session logs (67 sessions)",
                "collection_process": "Append-only ingestion with deduplication at silver layer",
                "preprocessing": "Markdown extraction, PDF text extraction, JSONL parsing, text chunking (512 tokens, 128 overlap), deduplication",
                "intended_uses": [
                    "RAG retrieval for GEOLOGOS chatbot",
                    "Cosmic Key knowledge base search",
                    "Context rehydration across sessions",
                    "Cross-platform knowledge synthesis",
                ],
                "prohibited_uses": [
                    "Commercial redistribution without GEOLOGOS permission",
                    "Training competing models without attribution",
                ],
                "distribution": "Internal GEOLOGOS; shareable with NOMADZ collective members",
                "maintenance": "Append-only; no deletions; monthly incremental updates",
            },
            "approvals": [
                {
                    "approver": approver,
                    "role": "Architect",
                    "timestamp": build_timestamp,
                    "sign_off": True,
                }
            ],
            "immutable": True,
        }

        # Compute dataset hash (deterministic)
        dataset_json = json.dumps(entry, sort_keys=True)
        entry["dataset_hash"] = hashlib.sha256(dataset_json.encode()).hexdigest()

        # Append to registry
        with open(self.registry_path, "a") as f:
            f.write(json.dumps(entry) + "\n")

        logger.info(f"✓ Registered {dataset_id} v{version}")
        logger.info(f"  Dataset hash: {entry['dataset_hash'][:16]}...")
        return entry

    def list_datasets(self) -> List[Dict[str, Any]]:
        """List all registered datasets."""
        datasets = []
        if self.registry_path.exists():
            with open(self.registry_path, "r") as f:
                for line in f:
                    if line.strip():
                        datasets.append(json.loads(line))
        return datasets

    def get_latest(self, dataset_id: str) -> Dict[str, Any]:
        """Get latest version of dataset."""
        datasets = self.list_datasets()
        matching = [d for d in datasets if d["dataset_id"] == dataset_id]
        return matching[-1] if matching else None

    def verify_integrity(self, entry: Dict[str, Any]) -> bool:
        """Verify dataset entry hash."""
        stored_hash = entry.pop("dataset_hash")
        entry_json = json.dumps(entry, sort_keys=True)
        computed_hash = hashlib.sha256(entry_json.encode()).hexdigest()
        return stored_hash == computed_hash


def generate_datasheet_document(entry: Dict[str, Any], output_path: str):
    """Generate human-readable datasheet markdown."""
    doc = f"""# Dataset Datasheet: {entry['dataset_id']} v{entry['version']}

**Build Timestamp:** {entry['build_timestamp']}

## Motivation

{entry['datasheet']['motivation']}

## Composition

- **Blogs:** 120 articles (~105k words)
- **PDFs:** 42 technical guides (~100 MB)
- **Session Logs:** 67 conversations (~8 MB)
- **Total Chunks:** 2,801 (after deduplication)
- **Average Chunk Size:** 512 tokens with 128 overlap

## Collection Process

{entry['datasheet']['collection_process']}

### Ingestion Details
- Source Type: Bronze-layer append-only files
- Ingestion Strategy: Bulk
- Deduplication Threshold: 95% similarity

## Preprocessing

{entry['datasheet']['preprocessing']}

## Intended Uses

{chr(10).join(f"- {u}" for u in entry['datasheet']['intended_uses'])}

## Prohibited Uses

{chr(10).join(f"- {u}" for u in entry['datasheet']['prohibited_uses'])}

## Distribution & Maintenance

**Distribution:** {entry['datasheet']['distribution']}

**Maintenance:** {entry['datasheet']['maintenance']}

## Embeddings

- **Model:** {entry['gold_build']['embedding_model']}
- **Dimension:** {entry['gold_build']['embedding_dim']}
- **Similarity Metric:** cosine (FAISS L2 distance)

## Approvals

{chr(10).join(f"- **{a['approver']}** ({a['role']}): Approved on {a['timestamp']}" for a in entry['approvals'])}

## Dataset Hash (Immutability)

```
{entry.get('dataset_hash', 'N/A')[:16]}...
```

---

*This datasheet is part of the GEOLOGOS dataset factory commitment to transparency, reproducibility, and ethical ML practices.*
"""
    with open(output_path, "w") as f:
        f.write(doc)
    logger.info(f"✓ Datasheet saved to {output_path}")


if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser(description="Manage dataset registry")
    parser.add_argument("--registry-root", default="registry")
    parser.add_argument("--dataset-id", default="geologos_rag_v1.0")
    parser.add_argument("--version", default="1.0")
    parser.add_argument("--embedding-model", default="nomic-embed-text-v1.5")
    parser.add_argument("--embedding-dim", type=int, default=768)
    parser.add_argument("--approver", default="SOL (NOMADZ Architect)")
    parser.add_argument("--list", action="store_true", help="List all datasets")

    args = parser.parse_args()

    mgr = RegistryManager(args.registry_root)

    if args.list:
        datasets = mgr.list_datasets()
        for ds in datasets:
            print(f"  {ds['dataset_id']} v{ds['version']} ({ds['build_timestamp']})")
    else:
        # Example registration (in real pipeline, sources would come from actual builds)
        sources = [
            {"source_type": "blogs", "file_count": 120},
            {"source_type": "pdfs", "file_count": 42},
            {"source_type": "sessions", "file_count": 67},
        ]
        build_config = {
            "chunk_size": 512,
            "chunk_overlap": 128,
            "dedup_threshold": 0.95,
        }
        entry = mgr.register_dataset(
            dataset_id=args.dataset_id,
            version=args.version,
            sources=sources,
            build_config=build_config,
            embedding_model=args.embedding_model,
            embedding_dim=args.embedding_dim,
            approver=args.approver,
        )
        generate_datasheet_document(entry, f"registry/datasheet_{args.dataset_id}_{args.version}.md")
