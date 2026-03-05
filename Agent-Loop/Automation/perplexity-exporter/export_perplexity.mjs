import fs from "fs";
import path from "path";
import { chromium } from "playwright";

const OUT_DIR = process.env.OUT_DIR || "exports";
const COOKIES_JSON = process.env.COOKIES_JSON || "cookies/perplexity.json";
const THREAD_URLS_FILE = process.env.THREAD_URLS_FILE || "threads.txt";

function mustRead(p) {
  if (!fs.existsSync(p)) throw new Error(`Missing file: ${p}`);
  return fs.readFileSync(p, "utf8");
}

function safeName(s) {
  return s.replace(/[^w-]+/g, "_").slice(0, 120);
}

fs.mkdirSync(OUT_DIR, { recursive: true });

const threadUrls = mustRead(THREAD_URLS_FILE)
  .split("
").map(x => x.trim())
  .filter(x => x && !x.startsWith("#"));

const cookies = JSON.parse(mustRead(COOKIES_JSON));

const browser = await chromium.launch({ headless: true });
const context = await browser.newContext();
await context.addCookies(cookies);

const page = await context.newPage();
for (const url of threadUrls) {
  await page.goto(url, { waitUntil: "domcontentloaded", timeout: 120_000 });
  await page.waitForTimeout(3000);

  // Generic export strategy:
  // 1) Try to find an "Export" button/menu
  // 2) If not found, fallback to saving the page HTML + a text extraction
  // (We’ll refine selectors after you give me one thread URL + DOM snapshot.)

  const title = safeName((await page.title()) || "perplexity_thread");
  const stamp = new Date().toISOString().replace(/[:.]/g, "-");
  const base = `${stamp}__${title}`;

  const html = await page.content();
  fs.writeFileSync(path.join(OUT_DIR, `${base}.html`), html, "utf8");

  const text = await page.evaluate(() => document.body.innerText || "");
  fs.writeFileSync(path.join(OUT_DIR, `${base}.txt`), text, "utf8");

  // Screenshot for debugging
  await page.screenshot({ path: path.join(OUT_DIR, `${base}.png`), fullPage: true });
}

await browser.close();
