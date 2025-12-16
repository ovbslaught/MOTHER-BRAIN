-- =============================================================================
-- GEOLOGOS ECOSYSTEM: DATABASE SETUP & SCHEMA
-- PostgreSQL 14+
-- =============================================================================

-- Enable extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";  -- for full-text search
CREATE EXTENSION IF NOT EXISTS "pgvector";  -- for vector embeddings

-- =============================================================================
-- 1. CORE KNOWLEDGE TABLES
-- =============================================================================

CREATE TABLE IF NOT EXISTS pillars (
    id SERIAL PRIMARY KEY,
    number INT UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) UNIQUE NOT NULL,
    description TEXT,
    estimated_words INT,
    section_count INT,
    category VARCHAR(50),  -- 'science', 'humanities', 'applied', 'ai', 'non_western'
    status VARCHAR(20) DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS sections (
    id SERIAL PRIMARY KEY,
    pillar_id INT NOT NULL REFERENCES pillars(id) ON DELETE CASCADE,
    section_number INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    content TEXT NOT NULL,
    summary TEXT,
    keywords TEXT[],
    word_count INT,
    embedding vector(1536),  -- OpenAI embedding dimension
    embedding_model VARCHAR(50) DEFAULT 'text-embedding-3-small',
    tsvector_col tsvector,  -- for full-text search
    difficulty_level INT DEFAULT 3,  -- 1-5, where 5 is frontier research
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS cross_references (
    id SERIAL PRIMARY KEY,
    source_section_id INT NOT NULL REFERENCES sections(id) ON DELETE CASCADE,
    target_section_id INT NOT NULL REFERENCES sections(id) ON DELETE CASCADE,
    relationship_type VARCHAR(50) NOT NULL,  -- 'related', 'extends', 'contradicts', 'prerequisite'
    strength FLOAT DEFAULT 0.5,  -- 0.0-1.0 relevance
    explanation TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS search_history (
    id SERIAL PRIMARY KEY,
    query TEXT NOT NULL,
    search_type VARCHAR(20),  -- 'full-text' or 'semantic'
    results_count INT,
    execution_time_ms INT,
    user_id VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for knowledge
CREATE INDEX idx_sections_pillar ON sections(pillar_id);
CREATE INDEX idx_sections_embedding ON sections USING ivfflat(embedding vector_cosine_ops);
CREATE INDEX idx_sections_tsvector ON sections USING GIN(tsvector_col);
CREATE INDEX idx_cross_refs_source ON cross_references(source_section_id);
CREATE INDEX idx_cross_refs_target ON cross_references(target_section_id);

-- =============================================================================
-- 2. TOOL REGISTRY TABLES
-- =============================================================================

CREATE TABLE IF NOT EXISTS tools (
    id SERIAL PRIMARY KEY,
    tool_id VARCHAR(100) UNIQUE NOT NULL,  -- 'qgis-001'
    name VARCHAR(255) NOT NULL,
    category VARCHAR(50) NOT NULL,
    description TEXT,
    repository_url VARCHAR(500),
    license VARCHAR(50),
    version VARCHAR(50),
    tags TEXT[],
    status VARCHAR(20) DEFAULT 'ready',  -- 'ready', 'deprecated', 'broken'
    last_tested TIMESTAMP,
    execution_profile JSONB,  -- {resources: {cpu, ram, gpu}, timeout, supports_batch}
    installation_commands JSONB,  -- {ubuntu: '', macos: '', windows: '', docker: ''}
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tool_dependencies (
    id SERIAL PRIMARY KEY,
    tool_id INT NOT NULL REFERENCES tools(id) ON DELETE CASCADE,
    dependency_name VARCHAR(255) NOT NULL,
    version_spec VARCHAR(50),  -- '>=3.0', '==2.1.0', etc
    is_optional BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tool_inputs (
    id SERIAL PRIMARY KEY,
    tool_id INT NOT NULL REFERENCES tools(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    type VARCHAR(50),  -- 'file', 'text', 'number', 'json'
    format VARCHAR(255),  -- 'shp,geojson', 'png,jpg', etc
    required BOOLEAN DEFAULT TRUE,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tool_outputs (
    id SERIAL PRIMARY KEY,
    tool_id INT NOT NULL REFERENCES tools(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    type VARCHAR(50),
    format VARCHAR(255),
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tool_executions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tool_id INT NOT NULL REFERENCES tools(id),
    status VARCHAR(20),  -- 'pending', 'running', 'success', 'failed', 'timeout'
    input_params JSONB,
    output_data JSONB,
    error_log TEXT,
    start_time TIMESTAMP,
    end_time TIMESTAMP,
    duration_ms INT,
    resources_used JSONB,  -- {cpu_cores, ram_gb, gpu_memory}
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for tools
CREATE INDEX idx_tools_category ON tools(category);
CREATE INDEX idx_tools_status ON tools(status);
CREATE INDEX idx_executions_tool ON tool_executions(tool_id);
CREATE INDEX idx_executions_status ON tool_executions(status);
CREATE INDEX idx_executions_created ON tool_executions(created_at DESC);

-- =============================================================================
-- 3. AGENT/LLM TABLES
-- =============================================================================

CREATE TABLE IF NOT EXISTS agents (
    id SERIAL PRIMARY KEY,
    agent_id VARCHAR(100) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    role VARCHAR(100),  -- 'researcher', 'coordinator', 'executor', 'critic'
    personality_prompt TEXT,
    model_id VARCHAR(100),  -- 'llama2-7b', 'mistral-7b'
    context_window INT,
    temperature FLOAT DEFAULT 0.7,
    system_prompt TEXT,
    capabilities TEXT[],
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS conversations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    title VARCHAR(255),
    agent_ids INT[],
    topic VARCHAR(255),
    messages JSONB,  -- [{role, agent_id, content, timestamp, embedding}]
    context JSONB,  -- {geologos_sections, available_tools, ...}
    status VARCHAR(20) DEFAULT 'active',  -- 'active', 'archived', 'deleted'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS agent_knowledge (
    id SERIAL PRIMARY KEY,
    agent_id INT NOT NULL REFERENCES agents(id),
    section_id INT NOT NULL REFERENCES sections(id),
    relevance_score FLOAT,  -- 0-1, how relevant this section is to agent's role
    last_accessed TIMESTAMP,
    access_count INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================================================================
-- 4. MESH NETWORK & SYNC TABLES
-- =============================================================================

CREATE TABLE IF NOT EXISTS mesh_nodes (
    id SERIAL PRIMARY KEY,
    node_id VARCHAR(255) UNIQUE NOT NULL,
    node_name VARCHAR(255),
    public_key VARCHAR(500),
    transports TEXT[],  -- ['wifi', 'lora', 'bluetooth']
    capabilities TEXT[],  -- ['compute', 'storage', 'execution']
    last_seen TIMESTAMP,
    is_online BOOLEAN DEFAULT FALSE,
    metadata JSONB,  -- {location, resources, version}
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS sync_state (
    id SERIAL PRIMARY KEY,
    entity_type VARCHAR(50),  -- 'section', 'tool', 'conversation', 'execution'
    entity_id INT,
    node_id VARCHAR(255) REFERENCES mesh_nodes(node_id),
    vector_clock JSONB,  -- CRDT causality tracking
    last_modified_node VARCHAR(255),
    tombstone BOOLEAN DEFAULT FALSE,  -- for deletion tracking
    timestamp_ms BIGINT,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS mesh_messages (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    from_node VARCHAR(255) REFERENCES mesh_nodes(node_id),
    to_node VARCHAR(255) REFERENCES mesh_nodes(node_id),
    message_type VARCHAR(50),  -- 'sync', 'heartbeat', 'discovery', 'data'
    payload JSONB,
    status VARCHAR(20),  -- 'pending', 'sent', 'delivered', 'failed'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for mesh
CREATE INDEX idx_nodes_online ON mesh_nodes(is_online);
CREATE INDEX idx_nodes_last_seen ON mesh_nodes(last_seen DESC);
CREATE INDEX idx_sync_entity ON sync_state(entity_type, entity_id);
CREATE INDEX idx_messages_from ON mesh_messages(from_node);
CREATE INDEX idx_messages_status ON mesh_messages(status);

-- =============================================================================
-- 5. ACCESSIBILITY & USER PREFERENCES
-- =============================================================================

CREATE TABLE IF NOT EXISTS user_preferences (
    user_id VARCHAR(255) PRIMARY KEY,
    accessibility_settings JSONB,  -- {captions: true, high_contrast: true, font_size: 16}
    theme VARCHAR(50),  -- 'light', 'dark', 'high-contrast'
    language VARCHAR(10) DEFAULT 'en',
    notifications_enabled BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS captions (
    id SERIAL PRIMARY KEY,
    session_id VARCHAR(255),
    audio_chunk_id VARCHAR(255),
    transcript TEXT NOT NULL,
    confidence FLOAT,
    timestamp_ms INT,
    speaker_id VARCHAR(100),
    language VARCHAR(10),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS audit_log (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(255),
    action VARCHAR(100),
    entity_type VARCHAR(50),
    entity_id INT,
    old_value JSONB,
    new_value JSONB,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================================================================
-- 6. TRIGGERS & FUNCTIONS
-- =============================================================================

-- Update timestamp trigger
CREATE OR REPLACE FUNCTION update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Auto-generate tsvector for full-text search
CREATE OR REPLACE FUNCTION generate_tsvector_section()
RETURNS TRIGGER AS $$
BEGIN
    NEW.tsvector_col = to_tsvector('english', NEW.title || ' ' || COALESCE(NEW.content, ''));
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply triggers
CREATE TRIGGER trg_sections_timestamp BEFORE UPDATE ON sections FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER trg_sections_tsvector BEFORE INSERT OR UPDATE ON sections FOR EACH ROW EXECUTE FUNCTION generate_tsvector_section();
CREATE TRIGGER trg_pillars_timestamp BEFORE UPDATE ON pillars FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER trg_tools_timestamp BEFORE UPDATE ON tools FOR EACH ROW EXECUTE FUNCTION update_timestamp();
CREATE TRIGGER trg_nodes_timestamp BEFORE UPDATE ON mesh_nodes FOR EACH ROW EXECUTE FUNCTION update_timestamp();

-- =============================================================================
-- 7. VIEWS FOR COMMON QUERIES
-- =============================================================================

CREATE OR REPLACE VIEW v_pillar_statistics AS
SELECT 
    p.id,
    p.name,
    COUNT(s.id) as section_count,
    SUM(s.word_count) as total_words,
    COUNT(DISTINCT cr.target_section_id) as cross_references
FROM pillars p
LEFT JOIN sections s ON p.id = s.pillar_id
LEFT JOIN cross_references cr ON s.id = cr.source_section_id
GROUP BY p.id, p.name;

CREATE OR REPLACE VIEW v_tool_health AS
SELECT 
    t.id,
    t.name,
    t.status,
    COUNT(te.id) as execution_count,
    SUM(CASE WHEN te.status = 'success' THEN 1 ELSE 0 END) as successful_executions,
    ROUND(100.0 * SUM(CASE WHEN te.status = 'success' THEN 1 ELSE 0 END) / NULLIF(COUNT(te.id), 0), 2) as success_rate,
    AVG(te.duration_ms) as avg_duration_ms
FROM tools t
LEFT JOIN tool_executions te ON t.id = te.tool_id
GROUP BY t.id, t.name, t.status;

CREATE OR REPLACE VIEW v_network_topology AS
SELECT 
    mn.node_id,
    mn.node_name,
    mn.is_online,
    COUNT(DISTINCT mm.from_node) as incoming_connections,
    COUNT(DISTINCT mm.to_node) as outgoing_connections,
    MAX(mn.last_seen) as last_seen
FROM mesh_nodes mn
LEFT JOIN mesh_messages mm ON mn.node_id = mm.from_node OR mn.node_id = mm.to_node
GROUP BY mn.node_id, mn.node_name, mn.is_online;

-- =============================================================================
-- 8. MATERIALIZED VIEW FOR PERFORMANCE
-- =============================================================================

CREATE MATERIALIZED VIEW IF NOT EXISTS mv_section_search_index AS
SELECT 
    s.id,
    s.pillar_id,
    p.name as pillar_name,
    s.title,
    LEFT(s.content, 500) as preview,
    s.keywords,
    s.difficulty_level,
    s.embedding,
    s.tsvector_col
FROM sections s
JOIN pillars p ON s.pillar_id = p.id;

CREATE INDEX idx_mv_section_search_embedding ON mv_section_search_index USING ivfflat(embedding vector_cosine_ops);

-- =============================================================================
-- INITIALIZATION DATA
-- =============================================================================

-- Insert pillars (26 total)
INSERT INTO pillars (number, name, slug, description, category) VALUES
(1, 'Cosmology & Deep Space', 'cosmology', 'Galaxy catalogs, dark matter/energy, CMB', 'science'),
(2, 'Astronomy & Astrophysics', 'astronomy', 'Stars, exoplanets, stellar evolution', 'science'),
(3, 'Physics', 'physics', 'Classical, Quantum, Relativity', 'science'),
(4, 'Chemistry & Materials Science', 'chemistry', 'Atomic structure, bonding, materials', 'science'),
(5, 'Earth Systems & Atmosphere', 'earth-systems', 'Atmospheric physics, climate, weather', 'science'),
(6, 'Oceanography & Marine Science', 'oceanography', 'Ocean circulation, marine biology', 'science'),
(7, 'Biology', 'biology', 'Molecular to organismal, genetics, evolution', 'science'),
(8, 'Medicine & Human Physiology', 'medicine', 'Organ systems, pathophysiology, pharmacology', 'science'),
(9, 'Sound & Audio Systems', 'sound-audio', 'Acoustics, signal processing, synthesis', 'science'),
(10, 'Mathematics Foundations', 'mathematics', 'Linear algebra, calculus, probability', 'science'),
(11, 'Algorithms & Computational Frameworks', 'algorithms', 'ML, cryptography, distributed systems', 'science'),
(12, 'Music Theory & Composition', 'music', 'Harmony, form, orchestration', 'humanities'),
(13, 'Visual Arts & Digital Design', 'visual-arts', 'Composition, color, typography, animation', 'humanities'),
(14, 'History, Culture & Society', 'history', 'World epochs, religions, social structures', 'humanities'),
(15, 'Philosophy, Ethics & Knowledge Systems', 'philosophy', 'Epistemology, metaphysics, ethics', 'humanities'),
(16, 'Human Domains', 'human-domains', 'Architecture, sustainability, homesteading', 'applied'),
(17, 'Plugin Ecosystems & Free Software', 'plugins', 'VST, DAW, synthesis, open-source', 'applied'),
(18, 'Meta-Documentation & Archival', 'meta-documentation', 'Version control, reproducibility, preservation', 'applied'),
(19, 'Artificial Intelligence & Machine Learning', 'ai-ml', 'Transformers, LLMs, neural networks, safety', 'ai'),
(20, 'Indigenous Knowledge Systems', 'indigenous-knowledge', 'Epistemologies, TEK, traditional medicine', 'non_western'),
(21, 'Post-Colonial Theory & Global South', 'post-colonial', 'Decolonial thinking, development, justice', 'non_western'),
(22, 'Islamic Science & Mathematics', 'islamic-science', 'Golden Age, algebra, astronomy, optics', 'non_western'),
(23, 'African Science & Technology', 'african-science', 'Mathematics, metallurgy, architecture', 'non_western'),
(24, 'Asian Science & Technology', 'asian-science', 'Chinese, Indian, Japanese, SE Asian', 'non_western'),
(25, 'Prompts, Queries & AI Interaction', 'prompts-queries', 'Prompt engineering, query strategies', 'ai'),
(26, 'Computational Tools & Software', 'tools', '203 open-source tools integrated', 'applied');

-- =============================================================================
-- PERMISSIONS
-- =============================================================================

-- Create application role
DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'geologos_app') THEN
    CREATE ROLE geologos_app WITH LOGIN PASSWORD 'secure_password_change_me';
  END IF;
END
$$;

-- Grant permissions
GRANT CONNECT ON DATABASE postgres TO geologos_app;
GRANT USAGE ON SCHEMA public TO geologos_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO geologos_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO geologos_app;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO geologos_app;

-- =============================================================================
-- MIGRATION COMPLETE
-- =============================================================================
COMMIT;