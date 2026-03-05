#!/usr/bin/env python3
"""
GEOLOGOS Dataset Factory - Bronze Layer Ingestion
Append-only ingestion with SHA-256 hashing and MANIFEST generation.
"""

import os
import json
import hashlib
import pathlib
from datetime import datetime
from typing import List, Dict, Any
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class BronzeIngester:
    """Immutable bronze layer ingestion."""

    def __init__(self, bronze_root: str):
        self.bronze_root = pathlib.Path(bronze_root)
        self.bronze_root.mkdir(parents=True, exist_ok=True)
        self.ingestion_log_path = self.bronze_root / "INGESTION_LOG.jsonl"

    def _hash_file(self, filepath: pathlib.Path) -> str:
        """Compute SHA-256 hash of file."""
        sha256 = hashlib.sha256()
        with open(filepath, "rb") as f:
            for block in iter(lambda: f.read(4096), b""):
                sha256.update(block)
        return sha256.hexdigest()

    def ingest_source(
        self,
        source_type: str,  # "blogs", "pdfs", "sessions"
        source_dir: str,
        file_pattern: str = "*",
    ) -> Dict[str, Any]:
        """
        Ingest all files from source_dir into bronze/{source_type}/.
        Returns MANIFEST.
        """
        source_path = pathlib.Path(source_dir)
        target_dir = self.bronze_root / source_type
        target_dir.mkdir(parents=True, exist_ok=True)

        ingested_files = []
        total_bytes = 0
        ingestion_timestamp = datetime.utcnow().isoformat() + "Z"

        for file in sorted(source_path.glob(file_pattern)):
            if not file.is_file():
                continue

            file_hash = self._hash_file(file)
            file_size = file.stat().st_size
            target_file = target_dir / file.name

            # Copy to bronze (append semantics: always store)
            import shutil
            shutil.copy2(file, target_file)

            record = {
                "filename": file.name,
                "hash_sha256": file_hash,
                "byte_size": file_size,
                "ingestion_time": ingestion_timestamp,
                "source_path": str(file),
            }
            ingested_files.append(record)
            total_bytes += file_size

            logger.info(f"Ingested {file.name} → {target_file} (hash: {file_hash[:8]}...)")

            # Append to INGESTION_LOG
            log_entry = {
                "timestamp": ingestion_timestamp,
                "source_type": source_type,
                "filename": file.name,
                "hash_sha256": file_hash,
                "byte_size": file_size,
            }
            with open(self.ingestion_log_path, "a") as f:
                f.write(json.dumps(log_entry) + "\n")

        manifest = {
            "source_type": source_type,
            "ingestion_timestamp": ingestion_timestamp,
            "total_files": len(ingested_files),
            "total_bytes": total_bytes,
            "ingestion_strategy": "bulk",
            "files": ingested_files,
        }

        manifest_path = target_dir / "MANIFEST.json"
        with open(manifest_path, "w") as f:
            json.dump(manifest, f, indent=2)

        logger.info(f"✓ {source_type} manifest saved to {manifest_path}")
        return manifest


if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser(description="Ingest source data into bronze layer")
    parser.add_argument("--bronze-root", default="data/bronze", help="Bronze root directory")
    parser.add_argument("--blogs-dir", help="Path to blogs markdown directory")
    parser.add_argument("--pdfs-dir", help="Path to PDFs directory")
    parser.add_argument("--sessions-dir", help="Path to session JSONL directory")

    args = parser.parse_args()

    ingester = BronzeIngester(args.bronze_root)

    if args.blogs_dir:
        logger.info("Ingesting blogs...")
        ingester.ingest_source("blogs", args.blogs_dir, "*.md")

    if args.pdfs_dir:
        logger.info("Ingesting PDFs...")
        ingester.ingest_source("pdfs", args.pdfs_dir, "*.pdf")

    if args.sessions_dir:
        logger.info("Ingesting sessions...")
        ingester.ingest_source("sessions", args.sessions_dir, "*.jsonl")

    logger.info("✓ Bronze ingestion complete")
