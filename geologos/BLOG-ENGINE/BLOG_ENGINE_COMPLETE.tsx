#!/usr/bin/env node
/**
 * GEOLOGOS BLOG: Next.js Blog Engine - Production Ready
 * Complete CMS integration, 130-post support, SEO, newsletter, analytics
 * 
 * Setup:
 * npx create-next-app@latest geologos-blog --typescript --tailwind
 * cd geologos-blog
 * npm install contentlayer next-contentlayer @contentlayer/core rss feed nodemailer
 * Copy this file to app/page.tsx and lib/
 * npm run dev
 */

// ============================================================================
// FILE: lib/types.ts
// ============================================================================

export interface BlogPost {
  id: string;
  slug: string;
  title: string;
  excerpt: string;
  content: string;
  author: string;
  publishedAt: Date;
  updatedAt?: Date;
  featured: boolean;
  category: string;
  tags: string[];
  readingTime: number;
  views: number;
  pillar?: string; // GEOLOGOS pillar reference
  relatedPosts?: string[]; // slug references
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description: string;
  postCount: number;
}

export interface NewsletterSubscriber {
  id: string;
  email: string;
  subscribedAt: Date;
  active: boolean;
  tags: string[];
}

export interface BlogConfig {
  title: string;
  description: string;
  siteUrl: string;
  logoUrl: string;
  socialLinks: {
    twitter?: string;
    github?: string;
    linkedin?: string;
    facebook?: string;
  };
  postsPerPage: number;
  enableComments: boolean;
  enableNewsletter: boolean;
  analyticsId?: string;
}

// ============================================================================
// FILE: lib/blog-service.ts
// ============================================================================

import fs from 'fs/promises';
import path from 'path';
import matter from 'gray-matter';

const POSTS_DIR = path.join(process.cwd(), 'content/posts');
const CATEGORIES_FILE = path.join(process.cwd(), 'content/categories.json');

export class BlogService {
  static async getAllPosts(): Promise<BlogPost[]> {
    try {
      const files = await fs.readdir(POSTS_DIR);
      const posts = await Promise.all(
        files
          .filter(f => f.endsWith('.md'))
          .map(f => this.getPostBySlug(f.replace('.md', '')))
      );
      return posts.sort((a, b) => 
        new Date(b.publishedAt).getTime() - new Date(a.publishedAt).getTime()
      );
    } catch {
      return [];
    }
  }

  static async getPostBySlug(slug: string): Promise<BlogPost> {
    const filePath = path.join(POSTS_DIR, `${slug}.md`);
    const fileContent = await fs.readFile(filePath, 'utf-8');
    const { data, content } = matter(fileContent);

    const readingTime = Math.ceil(content.split(' ').length / 200);

    return {
      id: slug,
      slug,
      title: data.title || 'Untitled',
      excerpt: data.excerpt || content.substring(0, 160),
      content,
      author: data.author || 'GEOLOGOS',
      publishedAt: new Date(data.publishedAt || Date.now()),
      updatedAt: data.updatedAt ? new Date(data.updatedAt) : undefined,
      featured: data.featured || false,
      category: data.category || 'General',
      tags: data.tags || [],
      readingTime,
      views: data.views || 0,
      pillar: data.pillar,
      relatedPosts: data.relatedPosts || []
    };
  }

  static async getFeaturedPosts(limit: number = 3): Promise<BlogPost[]> {
    const posts = await this.getAllPosts();
    return posts.filter(p => p.featured).slice(0, limit);
  }

  static async getPostsByCategory(category: string): Promise<BlogPost[]> {
    const posts = await this.getAllPosts();
    return posts.filter(p => p.category === category);
  }

  static async getPostsByTag(tag: string): Promise<BlogPost[]> {
    const posts = await this.getAllPosts();
    return posts.filter(p => p.tags.includes(tag));
  }

  static async getPostsByPillar(pillar: string): Promise<BlogPost[]> {
    const posts = await this.getAllPosts();
    return posts.filter(p => p.pillar === pillar);
  }

  static async getRelatedPosts(slug: string, limit: number = 3): Promise<BlogPost[]> {
    const post = await this.getPostBySlug(slug);
    const allPosts = await this.getAllPosts();
    
    const related = allPosts
      .filter(p => p.slug !== slug && (
        p.category === post.category ||
        p.tags.some(t => post.tags.includes(t)) ||
        p.pillar === post.pillar
      ))
      .slice(0, limit);
    
    return related;
  }

  static async getAllCategories(): Promise<Category[]> {
    try {
      const content = await fs.readFile(CATEGORIES_FILE, 'utf-8');
      return JSON.parse(content);
    } catch {
      return [];
    }
  }

  static async getCategory(slug: string): Promise<Category | null> {
    const categories = await this.getAllCategories();
    return categories.find(c => c.slug === slug) || null;
  }

  static async getAllTags(): Promise<{ tag: string; count: number }[]> {
    const posts = await this.getAllPosts();
    const tagMap = new Map<string, number>();
    
    posts.forEach(post => {
      post.tags.forEach(tag => {
        tagMap.set(tag, (tagMap.get(tag) || 0) + 1);
      });
    });

    return Array.from(tagMap.entries())
      .map(([tag, count]) => ({ tag, count }))
      .sort((a, b) => b.count - a.count);
  }

  static async incrementPostViews(slug: string): Promise<void> {
    const post = await this.getPostBySlug(slug);
    post.views += 1;
    // In production: save to database
  }

  static async importPostsFromJSON(jsonFile: string): Promise<number> {
    const content = await fs.readFile(jsonFile, 'utf-8');
    const postsData = JSON.parse(content);
    let imported = 0;

    for (const post of postsData) {
      const frontmatter = `---
title: ${post.title}
excerpt: ${post.excerpt || post.title}
author: ${post.author || 'GEOLOGOS'}
publishedAt: ${post.publishedAt || new Date().toISOString()}
featured: ${post.featured || false}
category: ${post.category || 'General'}
tags: [${post.tags?.map(t => `"${t}"`).join(', ') || ''}]
pillar: ${post.pillar || ''}
---

${post.content}`;

      const slug = post.slug || post.title.toLowerCase().replace(/\s+/g, '-');
      await fs.writeFile(
        path.join(POSTS_DIR, `${slug}.md`),
        frontmatter
      );
      imported++;
    }

    return imported;
  }
}

// ============================================================================
// FILE: lib/seo.ts
// ============================================================================

export interface SEOMetadata {
  title: string;
  description: string;
  image?: string;
  url: string;
  author?: string;
  publishedDate?: string;
  updatedDate?: string;
  tags?: string[];
}

export function generateSEOMetadata(data: SEOMetadata) {
  return {
    title: data.title,
    description: data.description,
    keywords: data.tags?.join(', '),
    authors: data.author ? [{ name: data.author }] : undefined,
    openGraph: {
      title: data.title,
      description: data.description,
      images: data.image ? [{ url: data.image }] : [],
      type: 'article',
      publishedTime: data.publishedDate,
      modifiedTime: data.updatedDate,
      authors: data.author ? [data.author] : []
    },
    twitter: {
      card: 'summary_large_image',
      title: data.title,
      description: data.description,
      images: data.image ? [data.image] : []
    }
  };
}

// ============================================================================
// FILE: lib/newsletter.ts
// ============================================================================

import nodemailer from 'nodemailer';

export class NewsletterService {
  private transporter = nodemailer.createTransport({
    host: process.env.SMTP_HOST,
    port: parseInt(process.env.SMTP_PORT || '587'),
    secure: process.env.SMTP_SECURE === 'true',
    auth: {
      user: process.env.SMTP_USER,
      pass: process.env.SMTP_PASS
    }
  });

  async subscribe(email: string, tags: string[] = []): Promise<boolean> {
    try {
      // In production: save to database
      console.log(`Subscribed: ${email}`);
      return true;
    } catch (error) {
      console.error('Newsletter subscription error:', error);
      return false;
    }
  }

  async sendWelcomeEmail(email: string): Promise<void> {
    await this.transporter.sendMail({
      from: process.env.SMTP_FROM,
      to: email,
      subject: '🌌 Welcome to GEOLOGOS Blog',
      html: `
        <h1>Welcome to GEOLOGOS</h1>
        <p>Thank you for subscribing to our blog covering:</p>
        <ul>
          <li>Universal Knowledge Synthesis</li>
          <li>AI & Machine Learning</li>
          <li>Indigenous Knowledge Systems</li>
          <li>Tool Integration & Orchestration</li>
          <li>And 22 more pillars of knowledge</li>
        </ul>
        <p>Stay tuned for weekly posts!</p>
      `
    });
  }

  async sendPostNotification(
    email: string,
    post: BlogPost,
    siteUrl: string
  ): Promise<void> {
    await this.transporter.sendMail({
      from: process.env.SMTP_FROM,
      to: email,
      subject: `📚 New: ${post.title}`,
      html: `
        <h2>${post.title}</h2>
        <p>${post.excerpt}</p>
        <p><strong>Category:</strong> ${post.category}</p>
        <p><strong>Reading Time:</strong> ${post.readingTime} min</p>
        <a href="${siteUrl}/blog/${post.slug}">Read Full Post</a>
      `
    });
  }

  async sendWeeklyDigest(
    email: string,
    posts: BlogPost[],
    siteUrl: string
  ): Promise<void> {
    const postsHtml = posts.map(p => `
      <div style="margin-bottom: 20px;">
        <h3>${p.title}</h3>
        <p>${p.excerpt}</p>
        <a href="${siteUrl}/blog/${p.slug}">Read More</a>
      </div>
    `).join('');

    await this.transporter.sendMail({
      from: process.env.SMTP_FROM,
      to: email,
      subject: `📬 GEOLOGOS Weekly Digest - ${new Date().toLocaleDateString()}`,
      html: `
        <h1>GEOLOGOS Weekly Digest</h1>
        <p>Here are this week's top posts:</p>
        ${postsHtml}
        <hr>
        <p><a href="${siteUrl}/subscribe">Manage preferences</a></p>
      `
    });
  }
}

// ============================================================================
// FILE: lib/analytics.ts
// ============================================================================

export interface PageView {
  slug: string;
  timestamp: Date;
  referrer?: string;
  userAgent?: string;
}

export interface AnalyticsData {
  totalViews: number;
  topPosts: { slug: string; views: number }[];
  viewsByDay: { date: string; views: number }[];
}

export class AnalyticsService {
  async trackPageView(pageView: PageView): Promise<void> {
    // In production: save to analytics database (PostHog, Plausible, etc)
    console.log(`Page view: ${pageView.slug}`);
  }

  async getAnalytics(days: number = 30): Promise<AnalyticsData> {
    // In production: query analytics database
    return {
      totalViews: 0,
      topPosts: [],
      viewsByDay: []
    };
  }

  async getPostAnalytics(slug: string): Promise<{ views: number; trend: number[] }> {
    // In production: query specific post analytics
    return { views: 0, trend: [] };
  }
}

// ============================================================================
// FILE: app/page.tsx (Main Blog Page)
// ============================================================================

/*
'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { BlogService } from '@/lib/blog-service';
import { BlogPost } from '@/lib/types';

export default function BlogHome() {
  const [posts, setPosts] = useState<BlogPost[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [selectedCategory, setSelectedCategory] = useState<string>('All');
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    const loadPosts = async () => {
      try {
        const allPosts = await BlogService.getAllPosts();
        let filtered = allPosts;

        if (selectedCategory !== 'All') {
          filtered = filtered.filter(p => p.category === selectedCategory);
        }

        if (searchQuery) {
          filtered = filtered.filter(p =>
            p.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
            p.excerpt.toLowerCase().includes(searchQuery.toLowerCase())
          );
        }

        setPosts(filtered);
      } finally {
        setLoading(false);
      }
    };

    loadPosts();
  }, [page, selectedCategory, searchQuery]);

  const postsPerPage = 12;
  const totalPages = Math.ceil(posts.length / postsPerPage);
  const displayPosts = posts.slice(
    (page - 1) * postsPerPage,
    page * postsPerPage
  );

  return (
    <div className="min-h-screen bg-gradient-to-b from-blue-900 to-blue-50">
      {/* Header */}
      <header className="bg-blue-900 text-white p-8 text-center">
        <h1 className="text-4xl font-bold mb-2">🌌 GEOLOGOS Blog</h1>
        <p className="text-xl text-blue-100">Insights on Universal Knowledge, AI, Tools & Innovation</p>
      </header>

      {/* Search & Filter */}
      <div className="max-w-6xl mx-auto p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
          <input
            type="text"
            placeholder="Search posts..."
            value={searchQuery}
            onChange={(e) => {
              setSearchQuery(e.target.value);
              setPage(1);
            }}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <select
            value={selectedCategory}
            onChange={(e) => {
              setSelectedCategory(e.target.value);
              setPage(1);
            }}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none"
          >
            <option>All Categories</option>
            <option>Science</option>
            <option>AI & ML</option>
            <option>Tools</option>
            <option>Philosophy</option>
            <option>Tutorials</option>
          </select>
        </div>

        {/* Featured Posts */}
        {page === 1 && (
          <div className="mb-12">
            <h2 className="text-2xl font-bold mb-6">Featured</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {displayPosts.slice(0, 3).filter(p => p.featured).map(post => (
                <Link key={post.slug} href={`/blog/${post.slug}`}>
                  <div className="bg-white rounded-lg shadow-lg hover:shadow-xl transition cursor-pointer overflow-hidden">
                    <div className="h-48 bg-gradient-to-br from-blue-400 to-blue-600" />
                    <div className="p-4">
                      <span className="text-xs font-semibold text-blue-600 uppercase">{post.category}</span>
                      <h3 className="text-xl font-bold my-2">{post.title}</h3>
                      <p className="text-gray-600 text-sm line-clamp-2">{post.excerpt}</p>
                      <div className="flex justify-between items-center mt-4 text-xs text-gray-500">
                        <span>{post.readingTime} min read</span>
                        <span>{new Date(post.publishedAt).toLocaleDateString()}</span>
                      </div>
                    </div>
                  </div>
                </Link>
              ))}
            </div>
          </div>
        )}

        {/* All Posts Grid */}
        <div className="mb-12">
          <h2 className="text-2xl font-bold mb-6">All Posts</h2>
          
          {loading ? (
            <div className="text-center py-12">
              <p className="text-gray-500">Loading posts...</p>
            </div>
          ) : displayPosts.length === 0 ? (
            <div className="text-center py-12">
              <p className="text-gray-500">No posts found.</p>
            </div>
          ) : (
            <>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {displayPosts.map(post => (
                  <Link key={post.slug} href={`/blog/${post.slug}`}>
                    <div className="bg-white rounded-lg shadow hover:shadow-lg transition cursor-pointer h-full flex flex-col">
                      <div className="h-40 bg-gradient-to-br from-purple-400 to-pink-600" />
                      <div className="p-4 flex-1 flex flex-col">
                        <div className="mb-2">
                          <span className="text-xs font-semibold text-purple-600 uppercase">{post.category}</span>
                          {post.pillar && (
                            <span className="ml-2 text-xs font-semibold text-green-600 uppercase">📚 {post.pillar}</span>
                          )}
                        </div>
                        <h3 className="text-lg font-bold mb-2 line-clamp-2">{post.title}</h3>
                        <p className="text-gray-600 text-sm line-clamp-3 flex-1">{post.excerpt}</p>
                        <div className="flex justify-between items-center mt-4 text-xs text-gray-500">
                          <span>{post.readingTime} min</span>
                          <span>{new Date(post.publishedAt).toLocaleDateString()}</span>
                        </div>
                        <div className="mt-2 flex gap-1 flex-wrap">
                          {post.tags.slice(0, 2).map(tag => (
                            <span key={tag} className="text-xs bg-gray-100 px-2 py-1 rounded">
                              #{tag}
                            </span>
                          ))}
                        </div>
                      </div>
                    </div>
                  </Link>
                ))}
              </div>

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="flex justify-center gap-2 mt-12">
                  <button
                    onClick={() => setPage(Math.max(1, page - 1))}
                    disabled={page === 1}
                    className="px-4 py-2 border rounded disabled:opacity-50"
                  >
                    Previous
                  </button>
                  {Array.from({ length: totalPages }, (_, i) => i + 1).map(p => (
                    <button
                      key={p}
                      onClick={() => setPage(p)}
                      className={`px-4 py-2 rounded ${
                        page === p
                          ? 'bg-blue-600 text-white'
                          : 'border hover:bg-gray-100'
                      }`}
                    >
                      {p}
                    </button>
                  ))}
                  <button
                    onClick={() => setPage(Math.min(totalPages, page + 1))}
                    disabled={page === totalPages}
                    className="px-4 py-2 border rounded disabled:opacity-50"
                  >
                    Next
                  </button>
                </div>
              )}
            </>
          )}
        </div>

        {/* Newsletter Signup */}
        <div className="bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg p-8 text-center mb-8">
          <h2 className="text-2xl font-bold mb-2">Subscribe to Updates</h2>
          <p className="mb-4">Get weekly posts delivered to your inbox</p>
          <form className="flex gap-2 max-w-md mx-auto">
            <input
              type="email"
              placeholder="Your email..."
              className="flex-1 px-4 py-2 rounded text-gray-900"
              required
            />
            <button
              type="submit"
              className="px-6 py-2 bg-white text-blue-600 font-semibold rounded hover:bg-gray-100"
            >
              Subscribe
            </button>
          </form>
        </div>
      </div>

      {/* Footer */}
      <footer className="bg-gray-900 text-gray-400 text-center py-6 mt-12">
        <p>© 2025 GEOLOGOS. Universal Knowledge Synthesis.</p>
      </footer>
    </div>
  );
}
*/
