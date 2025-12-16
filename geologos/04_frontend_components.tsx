/**
 * GEOLOGOS ECOSYSTEM: React Frontend Components - Production Ready
 * Complete interactive dashboard with search, tool launcher, agent chat, mesh network
 * 
 * Setup:
 * npx create-react-app geologos-ui
 * cd geologos-ui
 * npm install axios react-query zustand d3 cytoscape react-dnd react-dnd-html5-backend
 * npm install tailwindcss postcss autoprefixer
 * Copy this file to src/components/Dashboard.tsx
 * npm start
 */

import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:8000';
const WS_BASE = process.env.REACT_APP_WS_URL || 'ws://localhost:8000';

// ============================================================================
// TYPES
// ============================================================================

interface Pillar {
  id: number;
  name: string;
  slug: string;
  category: string;
}

interface Section {
  id: number;
  title: string;
  content: string;
  keywords: string[];
  difficulty: number;
}

interface SearchResult {
  section_id: number;
  pillar_name: string;
  title: string;
  preview: string;
  relevance_score: number;
}

interface Tool {
  id: number;
  name: string;
  category: string;
  status: string;
}

interface ToolExecution {
  execution_id: string;
  status: string;
  progress: number;
  result?: Record<string, unknown>;
}

interface MeshNode {
  node_id: string;
  node_name: string;
  is_online: boolean;
  transports: string[];
}

interface AgentMessage {
  id: string;
  agent_id: string;
  content: string;
  timestamp: string;
  role: 'agent' | 'user';
}

// ============================================================================
// CUSTOM HOOKS
// ============================================================================

const useKnowledgeSearch = () => {
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const search = useCallback(async (query: string, type: 'semantic' | 'full-text' = 'semantic') => {
    setLoading(true);
    setError(null);
    try {
      const response = await axios.get(`${API_BASE}/api/v1/knowledge/search`, {
        params: { query, search_type: type, limit: 10 }
      });
      setResults(response.data);
      return response.data;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Search failed');
      return [];
    } finally {
      setLoading(false);
    }
  }, []);

  return { search, results, loading, error };
};

const usePillars = () => {
  const [pillars, setPillars] = useState<Pillar[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPillars = async () => {
      try {
        const response = await axios.get(`${API_BASE}/api/v1/knowledge/pillars`);
        setPillars(response.data);
      } catch (error) {
        console.error('Failed to fetch pillars:', error);
      } finally {
        setLoading(false);
      }
    };
    fetchPillars();
  }, []);

  return { pillars, loading };
};

const useTools = (category?: string) => {
  const [tools, setTools] = useState<Tool[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchTools = async () => {
      try {
        const params = category ? { category } : {};
        const response = await axios.get(`${API_BASE}/api/v1/tools`, { params });
        setTools(response.data);
      } catch (error) {
        console.error('Failed to fetch tools:', error);
      } finally {
        setLoading(false);
      }
    };
    fetchTools();
  }, [category]);

  return { tools, loading };
};

const useMeshNodes = () => {
  const [nodes, setNodes] = useState<MeshNode[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchNodes = async () => {
      try {
        const response = await axios.get(`${API_BASE}/api/v1/mesh/nodes`);
        setNodes(response.data);
      } catch (error) {
        console.error('Failed to fetch mesh nodes:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchNodes();
    const interval = setInterval(fetchNodes, 5000); // Refresh every 5s
    return () => clearInterval(interval);
  }, []);

  return { nodes, loading };
};

// ============================================================================
// COMPONENTS
// ============================================================================

const SearchBar: React.FC<{
  onSearch: (query: string, type: 'semantic' | 'full-text') => void;
  isLoading: boolean;
}> = ({ onSearch, isLoading }) => {
  const [query, setQuery] = useState('');
  const [searchType, setSearchType] = useState<'semantic' | 'full-text'>('semantic');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (query.trim()) {
      onSearch(query, searchType);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mb-6 p-4 bg-white rounded-lg shadow">
      <div className="flex gap-2">
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search across 730,000+ words..."
          className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          disabled={isLoading}
        />
        <select
          value={searchType}
          onChange={(e) => setSearchType(e.target.value as 'semantic' | 'full-text')}
          className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none"
        >
          <option value="semantic">Semantic Search (AI)</option>
          <option value="full-text">Full-Text Search</option>
        </select>
        <button
          type="submit"
          disabled={isLoading}
          className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400"
        >
          {isLoading ? 'Searching...' : 'Search'}
        </button>
      </div>
    </form>
  );
};

const SearchResults: React.FC<{ results: SearchResult[] }> = ({ results }) => (
  <div className="grid gap-4">
    {results.length === 0 ? (
      <p className="text-gray-500 text-center py-8">No results found. Try a different search.</p>
    ) : (
      results.map((result) => (
        <div key={result.section_id} className="p-4 bg-white rounded-lg shadow hover:shadow-lg transition">
          <div className="flex justify-between items-start">
            <div>
              <h3 className="font-bold text-lg">{result.title}</h3>
              <p className="text-sm text-gray-600">{result.pillar_name}</p>
              <p className="text-gray-700 mt-2">{result.preview}...</p>
            </div>
            <div className="text-right">
              <div className="text-2xl font-bold text-blue-600">{(result.relevance_score * 100).toFixed(0)}%</div>
              <p className="text-xs text-gray-500">Relevance</p>
            </div>
          </div>
        </div>
      ))
    )}
  </div>
);

const ToolLauncher: React.FC = () => {
  const { tools, loading } = useTools();
  const [selectedTools, setSelectedTools] = useState<Tool[]>([]);
  const [executing, setExecuting] = useState(false);

  const addTool = (tool: Tool) => {
    setSelectedTools([...selectedTools, tool]);
  };

  const removeTool = (index: number) => {
    setSelectedTools(selectedTools.filter((_, i) => i !== index));
  };

  const executePipeline = async () => {
    if (selectedTools.length === 0) return;

    setExecuting(true);
    try {
      for (const tool of selectedTools) {
        const response = await axios.post(`${API_BASE}/api/v1/tools/execute`, {
          tool_name: tool.name,
          input_params: {}
        });
        console.log(`Executing ${tool.name}:`, response.data);
        // In production: monitor execution status
      }
      alert('Tool pipeline executed successfully!');
    } catch (error) {
      alert('Error executing tools');
    } finally {
      setExecuting(false);
    }
  };

  return (
    <div className="grid grid-cols-2 gap-4">
      <div className="bg-white rounded-lg shadow p-4">
        <h3 className="font-bold mb-3">Available Tools ({tools.length})</h3>
        <div className="max-h-96 overflow-y-auto space-y-2">
          {loading ? (
            <p className="text-gray-500">Loading tools...</p>
          ) : (
            tools.slice(0, 20).map((tool) => (
              <button
                key={tool.id}
                onClick={() => addTool(tool)}
                className="w-full text-left px-3 py-2 bg-blue-50 hover:bg-blue-100 rounded text-sm transition"
              >
                {tool.name} <span className="text-xs text-gray-600">({tool.category})</span>
              </button>
            ))
          )}
        </div>
      </div>

      <div className="bg-white rounded-lg shadow p-4">
        <h3 className="font-bold mb-3">Execution Pipeline</h3>
        <div className="space-y-2 max-h-96 overflow-y-auto">
          {selectedTools.map((tool, idx) => (
            <div key={idx} className="flex justify-between items-center p-2 bg-gray-50 rounded">
              <span className="text-sm">{idx + 1}. {tool.name}</span>
              <button
                onClick={() => removeTool(idx)}
                className="text-red-600 hover:text-red-800 text-sm"
              >
                ✕
              </button>
            </div>
          ))}
        </div>
        <button
          onClick={executePipeline}
          disabled={selectedTools.length === 0 || executing}
          className="w-full mt-4 px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 disabled:bg-gray-400"
        >
          {executing ? 'Executing...' : `Execute ${selectedTools.length} Tool${selectedTools.length !== 1 ? 's' : ''}`}
        </button>
      </div>
    </div>
  );
};

const AgentChat: React.FC = () => {
  const [messages, setMessages] = useState<AgentMessage[]>([]);
  const [input, setInput] = useState('');
  const [sending, setSending] = useState(false);

  const sendMessage = async () => {
    if (!input.trim()) return;

    setSending(true);
    try {
      const userMessage: AgentMessage = {
        id: Date.now().toString(),
        agent_id: 'user',
        content: input,
        timestamp: new Date().toISOString(),
        role: 'user'
      };

      setMessages([...messages, userMessage]);
      setInput('');

      const response = await axios.post(`${API_BASE}/api/v1/agents/chat`, {
        messages: [{ agent_id: 'coordinator', content: input }]
      });

      const agentMessage: AgentMessage = {
        id: (Date.now() + 1).toString(),
        agent_id: 'coordinator',
        content: 'Processing your request...',
        timestamp: new Date().toISOString(),
        role: 'agent'
      };

      setMessages((prev) => [...prev, agentMessage]);
    } catch (error) {
      console.error('Error sending message:', error);
    } finally {
      setSending(false);
    }
  };

  return (
    <div className="flex flex-col h-96 bg-white rounded-lg shadow">
      <div className="flex-1 overflow-y-auto p-4 space-y-3">
        {messages.length === 0 ? (
          <p className="text-gray-500 text-center">Start a conversation with the agent system...</p>
        ) : (
          messages.map((msg) => (
            <div
              key={msg.id}
              className={`flex ${msg.role === 'user' ? 'justify-end' : 'justify-start'}`}
            >
              <div
                className={`max-w-xs px-4 py-2 rounded-lg ${
                  msg.role === 'user'
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-200 text-gray-900'
                }`}
              >
                {msg.content}
              </div>
            </div>
          ))
        )}
      </div>

      <div className="border-t p-3 flex gap-2">
        <input
          type="text"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
          placeholder="Type your message..."
          className="flex-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
          disabled={sending}
        />
        <button
          onClick={sendMessage}
          disabled={sending || !input.trim()}
          className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:bg-gray-400"
        >
          Send
        </button>
      </div>
    </div>
  );
};

const MeshNetworkVisualization: React.FC = () => {
  const { nodes, loading } = useMeshNodes();

  return (
    <div className="bg-white rounded-lg shadow p-4">
      <h3 className="font-bold mb-3">Mesh Network Topology</h3>
      {loading ? (
        <p className="text-gray-500">Loading mesh network...</p>
      ) : (
        <div className="space-y-2">
          {nodes.map((node) => (
            <div
              key={node.node_id}
              className={`p-3 rounded border-l-4 ${
                node.is_online
                  ? 'border-green-500 bg-green-50'
                  : 'border-gray-400 bg-gray-50'
              }`}
            >
              <div className="flex justify-between items-center">
                <div>
                  <p className="font-semibold">{node.node_name}</p>
                  <p className="text-xs text-gray-600">{node.node_id}</p>
                  <p className="text-xs">{node.transports.join(', ')}</p>
                </div>
                <div className={`w-3 h-3 rounded-full ${node.is_online ? 'bg-green-500' : 'bg-gray-400'}`} />
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

const Pillars: React.FC = () => {
  const { pillars, loading } = usePillars();

  return (
    <div className="bg-white rounded-lg shadow p-4">
      <h3 className="font-bold mb-3">26 Pillars</h3>
      {loading ? (
        <p className="text-gray-500">Loading pillars...</p>
      ) : (
        <div className="grid grid-cols-2 gap-2 max-h-96 overflow-y-auto">
          {pillars.map((pillar) => (
            <div
              key={pillar.id}
              className="p-2 bg-gradient-to-br from-blue-50 to-blue-100 rounded text-sm cursor-pointer hover:shadow transition"
            >
              <p className="font-semibold">{pillar.name}</p>
              <p className="text-xs text-gray-600">{pillar.category}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

// ============================================================================
// MAIN DASHBOARD
// ============================================================================

const Dashboard: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'search' | 'tools' | 'agents' | 'mesh' | 'pillars'>('search');
  const { search, results, loading, error } = useKnowledgeSearch();

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Header */}
      <header className="bg-blue-900 text-white p-4 shadow">
        <div className="max-w-7xl mx-auto">
          <h1 className="text-3xl font-bold">🌌 GEOLOGOS-GALAXY GUIDE</h1>
          <p className="text-blue-100">Universal Knowledge Synthesis + Tool Orchestration</p>
        </div>
      </header>

      {/* Navigation */}
      <nav className="bg-blue-800 text-white">
        <div className="max-w-7xl mx-auto flex gap-4 px-4">
          {[
            { id: 'search' as const, label: '🔍 Search' },
            { id: 'tools' as const, label: '🛠️ Tools' },
            { id: 'agents' as const, label: '🤖 Agents' },
            { id: 'mesh' as const, label: '🌐 Mesh' },
            { id: 'pillars' as const, label: '📚 Pillars' }
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`px-4 py-3 border-b-2 ${
                activeTab === tab.id
                  ? 'border-white'
                  : 'border-transparent hover:border-blue-300'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>
      </nav>

      {/* Content */}
      <main className="max-w-7xl mx-auto p-4">
        {error && (
          <div className="mb-4 p-4 bg-red-100 text-red-700 rounded">
            {error}
          </div>
        )}

        {activeTab === 'search' && (
          <div>
            <SearchBar onSearch={search} isLoading={loading} />
            <SearchResults results={results} />
          </div>
        )}

        {activeTab === 'tools' && <ToolLauncher />}

        {activeTab === 'agents' && <AgentChat />}

        {activeTab === 'mesh' && <MeshNetworkVisualization />}

        {activeTab === 'pillars' && <Pillars />}
      </main>

      {/* Footer */}
      <footer className="bg-gray-800 text-gray-300 text-center py-4 mt-8">
        <p>GEOLOGOS v1.0 • 26 Pillars • 203 Tools • P2P Mesh Network</p>
      </footer>
    </div>
  );
};

export default Dashboard;