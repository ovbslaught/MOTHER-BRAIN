#!/usr/bin/env python3
"""
GEOLOGOS Dataset Factory - RAG Retrieval Interface
Query the vector store to retrieve relevant context.
"""

import json
import pathlib
from typing import List, Dict, Any
import logging

try:
    import numpy as np
    import faiss
except ImportError:
    raise ImportError("Please install: pip install numpy faiss-cpu")

try:
    from sentence_transformers import SentenceTransformer
except ImportError:
    raise ImportError("Please install: pip install sentence-transformers")

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class RAGRetriever:
    """Retrieve context from vector store for RAG."""

    def __init__(self, gold_root: str, embedding_model: str = "nomic-embed-text-v1.5"):
        self.gold_root = pathlib.Path(gold_root)
        self.vector_store_dir = self.gold_root / "vector_store"

        # Load config
        config_path = self.gold_root / "retrieval_config.json"
        with open(config_path, "r") as f:
            self.config = json.load(f)

        # Load FAISS index
        index_path = self.vector_store_dir / "faiss_index.bin"
        self.index = faiss.read_index(str(index_path))
        logger.info(f"✓ Loaded FAISS index: {self.index.ntotal} vectors")

        # Load chunk lookup
        lookup_path = self.vector_store_dir / "chunk_lookup.jsonl"
        self.chunk_lookup = {}
        with open(lookup_path, "r") as f:
            for line in f:
                record = json.loads(line)
                self.chunk_lookup[record["faiss_id"]] = record

        # Load embedding model
        logger.info(f"Loading embedding model: {embedding_model}")
        self.model = SentenceTransformer(embedding_model)

    def retrieve(self, query: str, top_k: int = None) -> List[Dict[str, Any]]:
        """
        Retrieve top-k most relevant chunks for query.
        """
        if top_k is None:
            top_k = self.config.get("retrieval_top_k", 5)

        # Embed query
        query_embedding = self.model.encode(query).astype(np.float32).reshape(1, -1)

        # Search FAISS
        distances, indices = self.index.search(query_embedding, top_k)

        # Retrieve chunks
        results = []
        for i, (dist, idx) in enumerate(zip(distances[0], indices[0])):
            chunk_record = self.chunk_lookup[int(idx)]
            results.append({
                "rank": i + 1,
                "distance": float(dist),  # L2 distance (lower = better)
                "chunk_id": chunk_record["chunk_id"],
                "text": chunk_record["text"],
                "metadata": chunk_record["metadata"],
            })

        return results

    def format_context(self, results: List[Dict[str, Any]]) -> str:
        """Format retrieved chunks as context string."""
        context_parts = []
        for result in results:
            source = result["metadata"].get("source_type", "unknown").upper()
            title = result["metadata"].get("title", "untitled")
            context_parts.append(f"[{source}: {title}]\n{result['text']}")
        return "\n\n---\n\n".join(context_parts)


if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser(description="Query RAG vector store")
    parser.add_argument("query", help="Search query")
    parser.add_argument("--gold-root", default="data/gold")
    parser.add_argument("--top-k", type=int, default=5)

    args = parser.parse_args()

    retriever = RAGRetriever(args.gold_root)
    results = retriever.retrieve(args.query, top_k=args.top_k)

    logger.info(f"\nResults for: '{args.query}'")
    logger.info("=" * 60)
    for result in results:
        logger.info(f"\nRank {result['rank']} (distance: {result['distance']:.4f})")
        logger.info(f"  {result['metadata'].get('source_type', 'unknown')}: {result['metadata'].get('title', 'untitled')}")
        logger.info(f"  {result['text'][:200]}...")

    # Also output formatted context
    context = retriever.format_context(results)
    print("\n\n=== FORMATTED CONTEXT FOR RAG ===\n")
    print(context)
