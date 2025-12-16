import { Server, Tool, TextContent } from '@modelcontextprotocol/sdk';  // Adjusted import based on SDK
import fs from 'fs';
import path from 'path';
import grayMatter from 'gray-matter';
import glob from 'glob';

const app = new Server('obsidian-vault');

const VAULT_PATH = process.env.OBSIDIAN_VAULT_PATH || './vault';
const GODOT_PATH = process.env.GODOT_PROJECT_PATH || './godot_project';

app.listTools = async () => [
  new Tool('read_note', 'Read note content and metadata', { type: 'object', properties: { path: { type: 'string' } } }),
  new Tool('create_note', 'Create new note', { type: 'object', properties: { folder: { type: 'string' }, title: { type: 'string' }, metadata: { type: 'object' }, content: { type: 'string' } } }),
  new Tool('search_notes', 'Search by criteria', { type: 'object', properties: { query: { type: 'string' }, type: { type: 'string' }, limit: { type: 'number' } } }),
  new Tool('update_note', 'Update existing note', { type: 'object', properties: { path: { type: 'string' }, metadata: { type: 'object' }, content: { type: 'string' } } }),
  new Tool('export_to_godot', 'Export entities to Godot', { type: 'object', properties: { export_type: { type: 'string' } } }),
  new Tool('get_entity_graph', 'Generate relationship graph', { type: 'object', properties: { center_entity: { type: 'string' }, depth: { type: 'number' } } }),
];

app.callTool = async (name, args) => {
  switch (name) {
    case 'read_note':
      const notePath = path.join(VAULT_PATH, args.path);
      const note = grayMatter(fs.readFileSync(notePath, 'utf8'));
      return [new TextContent('text', JSON.stringify({ metadata: note.data, content: note.content }))];
    case 'create_note':
      const folderPath = path.join(VAULT_PATH, args.folder);
      fs.mkdirSync(folderPath, { recursive: true });
      const fullPath = path.join(folderPath, `${args.title}.md`);
      const frontmatter = '---\n' + JSON.stringify(args.metadata) + '\n---\n' + args.content;
      fs.writeFileSync(fullPath, frontmatter);
      return [new TextContent('text', 'Note created')];
    case 'search_notes':
      const files = glob.sync(`${VAULT_PATH}/**/*.md`);
      const results = files.filter(f => f.includes(args.query || '') && (args.type ? grayMatter(fs.readFileSync(f)).data.type === args.type : true)).slice(0, args.limit || 10);
      return [new TextContent('text', JSON.stringify(results.map(f => grayMatter(fs.readFileSync(f)).data)))];
    case 'update_note':
      const updatePath = path.join(VAULT_PATH, args.path);
      const existing = grayMatter(fs.readFileSync(updatePath, 'utf8'));
      const newMetadata = { ...existing.data, ...args.metadata };
      const newContent = '---\n' + JSON.stringify(newMetadata) + '\n---\n' + (args.content || existing.content);
      fs.writeFileSync(updatePath, newContent);
      return [new TextContent('text', 'Note updated')];
    case 'export_to_godot':
      const exportType = args.export_type || 'all';
      const dataDir = path.join(GODOT_PATH, 'data');
      fs.mkdirSync(dataDir, { recursive: true });
      // Query and export (simplified; fetch all types)
      const entities = await app.callTool('search_notes', { query: '', type: exportType === 'all' ? '*' : exportType, limit: 1000 });
      fs.writeFileSync(path.join(dataDir, 'universe.json'), JSON.stringify(entities));
      // Split into separate files
      ['characters', 'locations', 'quests', 'items', 'factions'].forEach(t => {
        const filtered = entities.filter(e => e.type === t.slice(0, -1));
        fs.writeFileSync(path.join(dataDir, `${t}.json`), JSON.stringify(filtered));
      });
      return [new TextContent('text', 'Exported to Godot')];
    case 'get_entity_graph':
      // Implement graph generation (stub)
      return [new TextContent('text', JSON.stringify({ graph: 'stub graph' }))];
    default:
      return [new TextContent('text', 'Unknown tool')];
  }
};

// Run the server (assuming SDK has run method)
app.run({ port: 5001 });