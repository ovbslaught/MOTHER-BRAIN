#!/usr/bin/env python3
"""
GEOLOGOS Dataset Factory - Silver Layer Build
Normalize, chunk, and deduplicate blog/PDF/session data into Parquet.
"""

import json
import pathlib
import hashlib
from typing import List, Dict, Any
import logging
from datetime import datetime

try:
    import pandas as pd
    import pyarrow.parquet as pq
except ImportError:
    raise ImportError("Please install: pip install pandas pyarrow")

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class SilverBuilder:
    """Build normalized silver layer from bronze."""

    def __init__(self, bronze_root: str, silver_root: str, chunk_size: int = 512, overlap: int = 128):
        self.bronze_root = pathlib.Path(bronze_root)
        self.silver_root = pathlib.Path(silver_root)
        self.silver_root.mkdir(parents=True, exist_ok=True)
        self.chunk_size = chunk_size  # tokens (approximate)
        self.overlap = overlap
        self.dedup_threshold = 0.95

    def _read_markdown(self, filepath: pathlib.Path) -> str:
        """Read markdown file."""
        with open(filepath, "r", encoding="utf-8") as f:
            return f.read()

    def _read_pdf_text(self, filepath: pathlib.Path) -> str:
        """Read extracted PDF text (assumes .txt beside .pdf)."""
        txt_path = filepath.with_suffix(".txt")
        if txt_path.exists():
            with open(txt_path, "r", encoding="utf-8") as f:
                return f.read()
        return ""

    def _read_jsonl(self, filepath: pathlib.Path) -> List[Dict[str, Any]]:
        """Read JSONL session file."""
        records = []
        with open(filepath, "r", encoding="utf-8") as f:
            for line in f:
                if line.strip():
                    records.append(json.loads(line))
        return records

    def _chunk_text(self, text: str, metadata: Dict[str, Any]) -> List[Dict[str, Any]]:
        """Split text into chunks with overlap."""
        words = text.split()
        chunks = []
        for i in range(0, len(words), self.chunk_size - self.overlap):
            chunk_words = words[i : i + self.chunk_size]
            chunk_text = " ".join(chunk_words)
            chunk_id = len(chunks)
            chunk_hash = hashlib.sha256(chunk_text.encode()).hexdigest()
            chunks.append({
                "chunk_id": chunk_id,
                "text": chunk_text,
                "metadata": {**metadata, "chunk_index": chunk_id},
                "chunk_hash": chunk_hash,
            })
        return chunks

    def build_source(self, source_type: str) -> List[Dict[str, Any]]:
        """Build silver records from bronze source."""
        source_dir = self.bronze_root / source_type
        if not source_dir.exists():
            logger.warning(f"Source dir not found: {source_dir}")
            return []

        all_chunks = []

        if source_type == "blogs":
            for md_file in sorted(source_dir.glob("*.md")):
                text = self._read_markdown(md_file)
                metadata = {
                    "source_type": "blog",
                    "filename": md_file.name,
                    "title": md_file.stem,
                }
                chunks = self._chunk_text(text, metadata)
                all_chunks.extend(chunks)
                logger.info(f"  {md_file.name}: {len(chunks)} chunks")

        elif source_type == "pdfs":
            for pdf_file in sorted(source_dir.glob("*.pdf")):
                text = self._read_pdf_text(pdf_file)
                if not text:
                    logger.warning(f"  {pdf_file.name}: no text extracted")
                    continue
                metadata = {
                    "source_type": "pdf",
                    "filename": pdf_file.name,
                    "title": pdf_file.stem,
                }
                chunks = self._chunk_text(text, metadata)
                all_chunks.extend(chunks)
                logger.info(f"  {pdf_file.name}: {len(chunks)} chunks")

        elif source_type == "sessions":
            for jsonl_file in sorted(source_dir.glob("*.jsonl")):
                records = self._read_jsonl(jsonl_file)
                for rec in records:
                    text = rec.get("content", rec.get("text", ""))
                    if not text:
                        continue
                    metadata = {
                        "source_type": "session",
                        "filename": jsonl_file.name,
                        "session_id": rec.get("session_id", "unknown"),
                        "timestamp": rec.get("timestamp", ""),
                    }
                    chunks = self._chunk_text(text, metadata)
                    all_chunks.extend(chunks)
                logger.info(f"  {jsonl_file.name}: {len(chunks)} chunks")

        return all_chunks

    def build_all(self) -> Dict[str, Any]:
        """Build silver parquets for all sources."""
        stats = {
            "blogs": self.build_source("blogs"),
            "pdfs": self.build_source("pdfs"),
            "sessions": self.build_source("sessions"),
        }

        timestamp = datetime.utcnow().strftime("%Y%m%d")
        for source_type, chunks in stats.items():
            if not chunks:
                logger.warning(f"No chunks for {source_type}")
                continue

            df = pd.DataFrame(chunks)
            output_path = self.silver_root / f"{source_type}_v1.0_{timestamp}.parquet"
            df.to_parquet(output_path, index=False)
            logger.info(f"✓ {output_path} ({len(df)} rows)")

        return stats


if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser(description="Build silver layer from bronze")
    parser.add_argument("--bronze-root", default="data/bronze")
    parser.add_argument("--silver-root", default="data/silver")
    parser.add_argument("--chunk-size", type=int, default=512)
    parser.add_argument("--overlap", type=int, default=128)

    args = parser.parse_args()

    builder = SilverBuilder(
        args.bronze_root,
        args.silver_root,
        chunk_size=args.chunk_size,
        overlap=args.overlap,
    )
    builder.build_all()
    logger.info("✓ Silver build complete")
