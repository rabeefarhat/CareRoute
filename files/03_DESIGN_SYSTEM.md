# 03 — DESIGN SYSTEM: formatting, visuals, animations & handouts

Purpose: every lesson (≈ 2 hours, four per day), diagram, animation and handout in the CareRoute course should look and read like **one course** — calm, clear and beginner-friendly, while always going deeper: the basics first, then what happens under the hood, then the trade-offs and failure modes. Claude follows this file whenever it formats a lesson or builds a visual.

---

## 1. Principles

1. **Readable first.** Short paragraphs, clear headings, generous whitespace, high contrast.
2. **Beginner-friendly, never shallow.** Every concept follows the depth stack (§2.5). Depth comes from precision, not from long paragraphs.
3. **One idea per visual.** A visual explains one flow, one comparison or one structure — never the whole lesson.
4. **Motion explains, never decorates.** Animations show something *moving* through a system (a request, a message, a lock, an event). Failure branches are shown too, not only the happy path.
5. **Consistent meaning of colour.** Blue always means definition, red always means something going wrong, teal always means a trade-off (see §2 and §3).
6. **Accessible.** Keyboard controls, visible focus, captions in text, reduced-motion support, light and dark themes.

---

## 2. Chat formatting (Markdown)

### 2.1 Structure rules
- **Orient, then explain, then go deeper.** Start each concept with a 🧭 one-or-two-sentence orientation, then the why and the basics in plain language, then ⚙️ under the hood, ⚖️ trade-offs and 💣 failure modes. Define every new term the first time it appears.
- `##` for each lesson part (Understand · Build · Test · Practice · Review), `###` for each concept block.
- A progress line at the top of each part, e.g. "Part 3 of 5 · Test ▰▰▰▱▱".
- Paragraphs of **2–4 sentences**. Break up anything longer.
- **Numbered steps** for procedures, **bullets** for lists, **tables** for comparisons.
- End every concept block with a one-line **"In short:"** summary inside a 🟨 takeaway callout.
- Code in fenced blocks with a language tag; the file path goes in a comment on the first line. Build code is complete — never `...` placeholders.
- Leave a blank line before and after every callout, table and code block.

### 2.2 Callout convention (colour-coded blockquotes)

| Marker | Meaning | Use for |
|---|---|---|
| 🧭 **Orientation** | Primary (soft) | One or two sentences: what it is and where it fits — always the first line of a concept |
| 💡 **Why / analogy** | Amber | The problem a concept solves + an everyday comparison |
| 🟦 **Definition** | Blue | Plain-language definition, then the technical one |
| 📘 **The basics** | Blue | How to use it, in numbered steps, with a simple example |
| ⚙️ **Under the hood** | Primary (purple) | One level deeper: what happens at runtime — components, order, state |
| ⚖️ **Trade-offs** | Teal | What you gain, what you pay, when not to use it, the alternative |
| 💣 **Failure mode** | Red | How it breaks: symptom → cause → how you'd notice → fix or prevention |
| 🟩 **Example** | Green | Concrete code or scenario from CareRoute (fictional data only) |
| 🟥 **Common mistake** | Red | A developer error while writing the code; anti-patterns |
| 🟨 **Key takeaway** | Amber | The "In short:" line and must-remember facts |
| 🟪 **Say it out loud** | Violet | A 2–3 sentence explanation in the learner's voice |
| ⬛ **In production** | Slate | Operational reality: scale, monitoring, deployment |
| 🏷️ **Version** | Neutral | `[.NET 10]`, `[C# 14]`, `[EF10]`, `[.NET 9+]` tags |
| 📖 **New term** | Blue | A plain-language definition of a word the first time it appears |
| 🖱️ **In Visual Studio** | Primary | A click path: **Menu > Submenu > Item**, window names, options to pick |
| ✅ **You should see** | Green | The expected result after a step |
| 🆘 **If it doesn't work** | Red | Likely errors and their fixes |
| 📍 **Where we are** | Slate | Project state at the start of the lesson |
| 🏁 **Project checkpoint** | Green | What works now, how to check it, the commit message |
| 🎬 **Visual** | — | Marks where an animation/diagram belongs (right after the concept it explains) |
| 🔍 **Walkthrough** | Primary | Line-by-line (or marker-by-marker) explanation right after a code block |
| 💥 **Break it** | Amber | Reproduce a failure mode on purpose: predict → do it → observe → fix |
| 🧩 **DDD note** | Violet (`--say`) | A short Domain-Driven Design remark *inside* a lesson, only where it explains a design choice ("this is a value object because…", "this is the aggregate boundary, so…"). 3–6 lines max; links the term to Appendix E |
| 🏛️ **Clean Architecture** | Primary | Which layer this code lives in and why (Domain · Application · Infrastructure · Api) — and which way the dependency points |
| 🧪 **Test** | Green | A test in Part 3: the rule or failure mode it protects, then Arrange · Act · Assert |
| ☁️ **Azure** | Trade (teal) | Azure portal click path, pricing tier choice, and the cleanup step (Part H) |

**🟥 vs 💣:** a common mistake is something *you* do wrong while writing code; a failure mode is how the *running system* breaks — even when the code looks fine.

Example:

```markdown
> 🧭 **Orientation** — Dependency injection is how ASP.NET Core hands your classes the objects they need, instead of each class creating them itself.

> 💡 **Why it matters** — Imagine a kitchen that grows its own vegetables for every dish: slow and hard to change. Dependency injection is like ordering ingredients from a supplier instead.

> ⚙️ **Under the hood** — At startup the container records each registration. For every HTTP request ASP.NET Core creates a scope; the first time a scoped service is requested in that scope it's created and kept there, and when the request ends the scope disposes it. A singleton lives in the root scope for the whole app.

> ⚖️ **Trade-offs** — You gain testability and loose coupling; you pay with indirection (it's less obvious where an object comes from) and lifetime rules you must respect.

> 💣 **Failure mode** — *Symptom:* data from one request shows up in another, or "object disposed" errors. *Cause:* a singleton captured a scoped `DbContext` (captive dependency). *Notice:* scope validation errors at startup in Development, odd bugs under load. *Fix:* inject `IServiceScopeFactory` and create a scope per unit of work.

> 🟨 **Key takeaway** — In short: lifetimes only flow *downwards* — never let a longer-lived service capture a shorter-lived one.
```

### 2.3 Code walkthrough format

Short blocks are explained line by line; long blocks use circled markers in comments.

````markdown
```csharp
// src/CareRoute.Api/Program.cs  (composition root)
builder.Services.AddProblemDetails();                                   // ①
builder.Services.AddExceptionHandler<DomainExceptionHandler>();        // ②
builder.Services.AddOptions<ReferralOptions>()
    .BindConfiguration(ReferralOptions.Section)                         // ③
    .ValidateDataAnnotations()
    .ValidateOnStart();                                                 // ④
```

> 🔍 **Walkthrough**
> ① Registers the ProblemDetails service so every error response uses the standard JSON format. Remove it and unhandled errors return an empty 500.
> ② Adds our `IExceptionHandler`; it maps `DomainException` (a broken business rule) to 409. Handlers run in registration order.
> ③ Binds the `Referrals` section of configuration to the options class. Under the hood, the binder matches property names to configuration keys, case-insensitively, after all configuration sources have been merged.
> ④ Validates at startup instead of on first use — a bad config crashes the app immediately (fail fast) rather than during a user's request.
````

In HTML handouts, render walkthroughs as a two-column layout on wide screens (code left, numbered notes right) and stacked on narrow screens; highlight the matching line when a note is hovered or focused.

### 2.4 Priority markers

| Marker | Level | Treatment |
|---|---|---|
| 🟢 | Core | Taught first and in detail, with the full depth stack |
| 🔵 | Important | The full depth stack, compact: under the hood in a few steps, one or two trade-offs, one or two failure modes |
| 🟣 | Advanced | Short "Going further" section, clearly optional |

### 2.5 The depth stack (every concept, automatically)

1. 🧭 Orientation → 2. 💡 Why → 3. 🟦 Definition → 4. 📘 The basics → 5. ⚙️ Under the hood → 6. ⚖️ Trade-offs → 7. 💣 Failure modes → then 🟩 example, 🟥 mistake, 🟪 say it out loud, 🟨 in short.

At the end of Part 1, a **⚖️/💣 summary table** lists every concept of the lesson with its main trade-off, its main failure mode, and how you'd notice it.

---

## 3. Palette (design tokens)

| Token | Light | Dark | Role |
|---|---|---|---|
| `--bg` | `#F6F7FB` | `#0E121A` | Page background |
| `--surface` | `#FFFFFF` | `#161C27` | Cards, stage |
| `--surface-2` | `#EEF1F7` | `#1E2533` | Nodes, subtle fills |
| `--text` | `#1B2230` | `#E6EAF2` | Body text |
| `--muted` | `#5A6478` | `#9AA4B6` | Secondary text |
| `--border` | `#DCE1EA` | `#2A3242` | Borders, dividers |
| `--primary` | `#512BD4` | `#9D85F2` | .NET purple: active state, moving token, primary buttons, ⚙️ under the hood |
| `--primary-soft` | `#EDE8FC` | `#2A2342` | Visited state, focus ring, 🧭 orientation background |
| `--def` / `--def-bg` | `#1D4ED8` / `#E8F0FE` | `#7FAEFF` / `#16233D` | 🟦 Definition, 📘 basics |
| `--ex` / `--ex-bg` | `#047857` / `#E6F6EF` | `#4FD1A1` / `#11291F` | 🟩 Example, success |
| `--err` / `--err-bg` | `#B91C1C` / `#FDECEC` | `#F58A8A` / `#2E1618` | 🟥 Mistake, 💣 failure mode, failure state |
| `--key` / `--key-bg` | `#B45309` / `#FEF4E2` | `#F4B860` / `#2E2312` | 🟨 Takeaway, 💡 why, warning |
| `--trade` / `--trade-bg` | `#0E7490` / `#E3F4F8` | `#5CCFE6` / `#0F262C` | ⚖️ Trade-offs |
| `--say` / `--say-bg` | `#6D28D9` / `#F1EAFE` | `#B79BFF` / `#231A38` | 🟪 Say it out loud |
| `--prod` / `--prod-bg` | `#334155` / `#EEF1F5` | `#A9B5C7` / `#1C2230` | ⬛ Production |
| `--code-bg` / `--code-text` | `#0F172A` / `#E2E8F0` | same | Code blocks |

In animations: **primary** = where the action is now, **primary-soft** = already visited, **err** = failure, **ex** = success/acknowledged, **key** = waiting/retrying.

---

## 4. Typography, spacing & shape

- **Font:** Inter (Google Fonts) → fallback `"Segoe UI", system-ui, -apple-system, sans-serif`. **Code:** JetBrains Mono → fallback `"Cascadia Code", Consolas, monospace`.
- **Sizes:** body 17px / line-height 1.65; small 14px; h1 32px; h2 24px; h3 19px. Prose max width **72ch**.
- **Spacing:** 8px scale (8 / 16 / 24 / 32 / 48). Card padding 20–24px; gaps 16–24px.
- **Shape:** cards radius **16px**, buttons 10px, chips fully rounded; 1px borders; soft shadow; callout cards have a **4px left accent bar** in their semantic colour.

---

## 5. Components

- **Callout card** — label chip (emoji + uppercase label) + body; semantic colour via `--c` / `--c-bg`.
- **Concept card** — title, 2–3 lines, optional mini-diagram; used in grids in handouts.
- **Under-the-hood card** — ⚙️ label and a numbered 3–6 step sequence of what happens at runtime; purple accent bar.
- **Trade-off table** — columns: option · you gain · you pay · choose it when; teal header.
- **Failure-mode card** — 💣 label and four rows: symptom · cause · how you'd notice · fix/prevention; red accent bar.
- **Comparison table** — wrapped in a horizontally scrollable container.
- **Code block** — dark surface, mono font, horizontal scroll, never wraps code.
- **Q&A reveal** — `<details>`/`<summary>`: the question is visible, the model answer is revealed on click.
- **Step player** — SVG stage + caption + controls (§6).
- **Cheat-sheet box** — compact bullet list in a bordered card, print-friendly.

---

## 6. Step-by-step animations (🎬)

**Structure:** an SVG stage with labelled nodes, a moving **token** (the request/message/event/value), a **caption** under the stage that explains the current step in one or two sentences, and controls.

**Behaviour:**
1. Controls: **◀ Prev · ▶ Play/⏸ Pause · Next ▶ · ⟲ Reset** and a counter "Step 3 / 9". Keyboard: ← / → to step, Space to play/pause.
2. Each step = `{ node, x, y, caption, state }` where `state` is `active`, `error`, `success` or `waiting`.
3. The current node gets the **active** style (primary stroke), earlier nodes get **visited**, failures turn **red**, completions turn **green**.
4. The token moves with a 600 ms ease transition; auto-play advances every ~1.6 s and stops at the last step.
5. `prefers-reduced-motion`: no movement transitions — the token jumps and highlights still change.
6. The caption lives in an `aria-live="polite"` element so screen readers announce each step.
7. 6–12 steps per animation. Show the happy path first, then **at least one failure branch** (e.g., the exception handler catching on the way back, a duplicate message being ignored, a conflict returning 409).

**Typical animations in this course:** request through the middleware pipeline; inside a controller request (model binding → validation → filters → action → result); `await` and the thread pool; Clean Architecture rings with the dependency arrow pointing inward; two bookings racing for one slot (with and without an aggregate boundary); domain events dispatched before vs after saving; MediatR pipeline behaviors; command vs query paths; B-tree seek vs scan; deadlock cycle; change-tracker states; JWT validation; a BOLA attack; message routing through exchanges; the no-loss chain; outbox → relay → inbox; trace waterfall; a process manager with a missed deadline; a correlation ID flowing through logs of three services; liveness vs readiness probes; an anti-corruption layer translating a foreign model; cache stampede; cascading failure vs circuit breaker; App Service slot swap; a Durable Functions orchestration replaying with a durable timer; a Service Bus message reaching the dead-letter sub-queue; an APIM policy pipeline (inbound → backend → outbound).

---

## 7. Lesson handout (end of every standard lesson)

A single self-contained HTML page with the theme above, in this order:
1. **Header** — lesson ID, title, goal, project step, the bounded context(s) and Clean Architecture layer(s) touched.
2. **Key concepts** — grid of concept cards (🧭 + 🟦).
3. **Under the hood** — one ⚙️ card per key concept.
4. **Trade-offs** — one ⚖️ table per key decision.
5. **Failure modes** — 💣 cards (symptom · cause · notice · fix).
6. **Examples** — 2–4 short code/scenario cards from CareRoute (🟩).
6b. **Tests written today** — one line per test: name · rule it protects (🧪).
6c. **DDD notes** — the 🧩 notes of the lesson, if any.
7. **Common mistakes** — red cards (🟥).
8. **In production** — slate cards (⬛), when relevant.
9. **Explain-it Q&A** — every question with its model answer and follow-ups in click-to-reveal blocks (🟪).
10. **Cheat sheet** — the lesson's key lines (🟨).
11. **Glossary** — every new term, plus the ubiquitous-language terms added (📖).
12. **Project checkpoint** — what CareRoute can do after this lesson (🏁).
13. **Footer** — "Next: Lxx — title".

It must work in light and dark mode, print cleanly (controls hidden, white background), and scroll wide content inside its own container.

---

## 8. Which tool to use

1. **Inline visual tool available** (a visualizer that renders SVG/HTML widgets in the chat) → use it for 🎬 animations and diagrams. If that tool has its own styling rules, follow them for technical constraints and map this palette's *meanings* onto them as closely as allowed.
2. **Otherwise** → a self-contained HTML artifact using §9 and §10.
3. **No visual tools at all** → a Mermaid or ASCII diagram plus a numbered "frame-by-frame" list describing what the animation would show.
4. Handouts are always HTML artifacts when artifacts are available; follow the host's artifact rules (viewport, allowed script/font hosts, file size).
5. Keep the teaching prose in the chat, **outside** the visual. The visual carries the structure; the text carries the explanation.

---

## 9. CSS starter (shared by every visual and handout)

```css
:root{
  --bg:#F6F7FB;--surface:#FFFFFF;--surface-2:#EEF1F7;--text:#1B2230;--muted:#5A6478;--border:#DCE1EA;
  --primary:#512BD4;--primary-soft:#EDE8FC;
  --def:#1D4ED8;--def-bg:#E8F0FE;--ex:#047857;--ex-bg:#E6F6EF;--err:#B91C1C;--err-bg:#FDECEC;
  --key:#B45309;--key-bg:#FEF4E2;--trade:#0E7490;--trade-bg:#E3F4F8;--say:#6D28D9;--say-bg:#F1EAFE;--prod:#334155;--prod-bg:#EEF1F5;
  --code-bg:#0F172A;--code-text:#E2E8F0;
  --radius:16px;--radius-sm:10px;--shadow:0 1px 2px rgba(16,24,40,.06),0 4px 16px rgba(16,24,40,.06);
  --font:"Inter","Segoe UI",system-ui,-apple-system,sans-serif;
  --mono:"JetBrains Mono","Cascadia Code",Consolas,monospace;
}
@media (prefers-color-scheme: dark){:root:not([data-theme="light"]){
  --bg:#0E121A;--surface:#161C27;--surface-2:#1E2533;--text:#E6EAF2;--muted:#9AA4B6;--border:#2A3242;
  --primary:#9D85F2;--primary-soft:#2A2342;
  --def:#7FAEFF;--def-bg:#16233D;--ex:#4FD1A1;--ex-bg:#11291F;--err:#F58A8A;--err-bg:#2E1618;
  --key:#F4B860;--key-bg:#2E2312;--trade:#5CCFE6;--trade-bg:#0F262C;--say:#B79BFF;--say-bg:#231A38;--prod:#A9B5C7;--prod-bg:#1C2230;
  --shadow:none;}}
:root[data-theme="dark"]{
  --bg:#0E121A;--surface:#161C27;--surface-2:#1E2533;--text:#E6EAF2;--muted:#9AA4B6;--border:#2A3242;
  --primary:#9D85F2;--primary-soft:#2A2342;
  --def:#7FAEFF;--def-bg:#16233D;--ex:#4FD1A1;--ex-bg:#11291F;--err:#F58A8A;--err-bg:#2E1618;
  --key:#F4B860;--key-bg:#2E2312;--trade:#5CCFE6;--trade-bg:#0F262C;--say:#B79BFF;--say-bg:#231A38;--prod:#A9B5C7;--prod-bg:#1C2230;
  --shadow:none;}

body{margin:0;background:var(--bg);color:var(--text);font:17px/1.65 var(--font);}
.wrap{max-width:960px;margin:0 auto;padding:32px 20px 64px;}
.prose{max-width:72ch;}
h1{font-size:2rem;line-height:1.2;margin:0 0 8px;} h2{font-size:1.5rem;margin:40px 0 12px;} h3{font-size:1.19rem;margin:28px 0 8px;}
.grid{display:grid;gap:16px;grid-template-columns:repeat(auto-fit,minmax(260px,1fr));}
.card{background:var(--surface);border:1px solid var(--border);border-radius:var(--radius);padding:20px 22px;box-shadow:var(--shadow);}
.callout{border-left:4px solid var(--c);background:var(--c-bg);border-radius:var(--radius);padding:16px 20px;margin:16px 0;}
.callout .label{display:inline-flex;gap:6px;align-items:center;font-size:.78rem;font-weight:700;letter-spacing:.05em;text-transform:uppercase;color:var(--c);margin-bottom:6px;}
.callout.definition{--c:var(--def);--c-bg:var(--def-bg);} .callout.example{--c:var(--ex);--c-bg:var(--ex-bg);}
.callout.mistake{--c:var(--err);--c-bg:var(--err-bg);}   .callout.takeaway{--c:var(--key);--c-bg:var(--key-bg);}
.callout.say{--c:var(--say);--c-bg:var(--say-bg);}       .callout.production{--c:var(--prod);--c-bg:var(--prod-bg);}
.callout.orient{--c:var(--primary);--c-bg:var(--primary-soft);} .callout.hood{--c:var(--primary);--c-bg:var(--primary-soft);}
.callout.basics{--c:var(--def);--c-bg:var(--def-bg);}   .callout.trade{--c:var(--trade);--c-bg:var(--trade-bg);}
.callout.failure{--c:var(--err);--c-bg:var(--err-bg);}
.failure-card dl{display:grid;grid-template-columns:max-content 1fr;gap:6px 14px;margin:0;} .failure-card dt{font-weight:700;color:var(--err);}
.trade-table th{background:var(--trade-bg);color:var(--trade);}
code,pre{font-family:var(--mono);font-size:.9em;}
pre{background:var(--code-bg);color:var(--code-text);padding:16px 18px;border-radius:var(--radius-sm);overflow-x:auto;}
.table-wrap{overflow-x:auto;} table{border-collapse:collapse;width:100%;} th,td{padding:10px 12px;border-bottom:1px solid var(--border);text-align:left;}
details.qa{background:var(--surface);border:1px solid var(--border);border-radius:var(--radius);padding:14px 18px;margin:10px 0;}
details.qa summary{cursor:pointer;font-weight:600;} details.qa[open] summary{color:var(--say);margin-bottom:8px;}
.btn{font:600 .9rem var(--font);border:1px solid var(--border);background:var(--surface);color:var(--text);border-radius:var(--radius-sm);padding:8px 14px;cursor:pointer;}
.btn.primary{background:var(--primary);color:#fff;border-color:transparent;}
.btn:focus-visible,.stage:focus-visible{outline:3px solid var(--primary);outline-offset:2px;}

/* Step player */
.stage{background:var(--surface);border:1px solid var(--border);border-radius:var(--radius);padding:20px;box-shadow:var(--shadow);}
.stage svg{width:100%;height:auto;display:block;}
.node rect{fill:var(--surface-2);stroke:var(--border);stroke-width:1.5;transition:fill .4s ease,stroke .4s ease;}
.node text{fill:var(--text);font:600 14px var(--font);}
.node.visited rect{fill:var(--primary-soft);}
.node.active rect{fill:var(--primary-soft);stroke:var(--primary);stroke-width:3;}
.node.error rect{fill:var(--err-bg);stroke:var(--err);stroke-width:3;}
.node.success rect{fill:var(--ex-bg);stroke:var(--ex);stroke-width:3;}
.node.waiting rect{fill:var(--key-bg);stroke:var(--key);stroke-width:3;}
.token{fill:var(--primary);transition:transform .6s cubic-bezier(.4,0,.2,1);}
.caption{min-height:3.3em;margin:14px 0 10px;}
.controls{display:flex;flex-wrap:wrap;gap:8px;align-items:center;}
.counter{margin-left:auto;color:var(--muted);font-size:.9rem;}
@media (prefers-reduced-motion: reduce){*{transition:none!important;animation:none!important;}}
@media print{.controls{display:none;} body{background:#fff;color:#000;} .card,.stage{box-shadow:none;}}
```

---

## 10. Step-player starter (HTML + JS)

```html
<div class="stage" id="pipeline" tabindex="0" aria-label="Animation: a request through the middleware pipeline">
  <svg viewBox="0 0 760 170" role="img" aria-labelledby="pipeline-title">
    <title id="pipeline-title">Request through the ASP.NET Core middleware pipeline</title>
    <g class="node" id="exception"><rect x="20"  y="56" width="150" height="56" rx="12"/><text x="95"  y="89" text-anchor="middle">ExceptionHandler</text></g>
    <g class="node" id="auth">     <rect x="210" y="56" width="150" height="56" rx="12"/><text x="285" y="89" text-anchor="middle">Authentication</text></g>
    <g class="node" id="authz">    <rect x="400" y="56" width="150" height="56" rx="12"/><text x="475" y="89" text-anchor="middle">Authorization</text></g>
    <g class="node" id="endpoint"> <rect x="590" y="56" width="150" height="56" rx="12"/><text x="665" y="89" text-anchor="middle">Controller</text></g>
    <circle class="token" r="9" cx="0" cy="0"/>
  </svg>
  <p class="caption" aria-live="polite"></p>
  <div class="controls">
    <button class="btn" data-act="prev">◀ Prev</button>
    <button class="btn primary" data-act="play">▶ Play</button>
    <button class="btn" data-act="next">Next ▶</button>
    <button class="btn" data-act="reset">⟲ Reset</button>
    <span class="counter"></span>
  </div>
</div>

<script>
function stepPlayer(root, steps, render) {
  let i = 0, timer = null;
  const reduce = matchMedia('(prefers-reduced-motion: reduce)').matches;
  const caption = root.querySelector('.caption');
  const counter = root.querySelector('.counter');
  const playBtn = root.querySelector('[data-act="play"]');
  const show = n => {
    i = Math.max(0, Math.min(steps.length - 1, n));
    render(steps[i], i, reduce);
    caption.textContent = steps[i].caption;
    counter.textContent = `Step ${i + 1} / ${steps.length}`;
  };
  const stop = () => { clearInterval(timer); timer = null; playBtn.textContent = '▶ Play'; };
  const actions = {
    next:  () => { stop(); show(i + 1); },
    prev:  () => { stop(); show(i - 1); },
    reset: () => { stop(); show(0); },
    play:  () => {
      if (timer) return stop();
      if (i === steps.length - 1) show(0);
      playBtn.textContent = '⏸ Pause';
      timer = setInterval(() => (i < steps.length - 1 ? show(i + 1) : stop()), 1600);
    }
  };
  root.addEventListener('click', e => {
    const act = e.target.closest('button[data-act]')?.dataset.act;
    if (act) actions[act]();
  });
  root.addEventListener('keydown', e => {
    if (e.key === 'ArrowRight') actions.next();
    if (e.key === 'ArrowLeft')  actions.prev();
    if (e.key === ' ') { e.preventDefault(); actions.play(); }
  });
  show(0);
}

// Example: steps for the middleware animation (token moves in, then back out)
const root = document.getElementById('pipeline');
const steps = [
  { node: 'exception', x: 95,  caption: 'The request enters the pipeline. ExceptionHandler wraps everything after it.' },
  { node: 'auth',      x: 285, caption: 'Authentication reads the JWT and builds the ClaimsPrincipal.' },
  { node: 'authz',     x: 475, caption: 'Authorization evaluates the endpoint policy.' },
  { node: 'endpoint',  x: 665, caption: 'The controller action runs and throws a DomainException.', state: 'error' },
  { node: 'authz',     x: 475, caption: 'The exception bubbles back out through the pipeline…' },
  { node: 'exception', x: 95,  caption: 'ExceptionHandler catches it and writes a 409 ProblemDetails response.', state: 'success' }
];
const nodes = [...root.querySelectorAll('.node')];
const token = root.querySelector('.token');
stepPlayer(root, steps, (s, i) => {
  nodes.forEach(n => {
    const current = n.id === s.node;
    n.classList.toggle('active',  current && !s.state);
    n.classList.toggle('error',   current && s.state === 'error');
    n.classList.toggle('success', current && s.state === 'success');
    n.classList.toggle('waiting', current && s.state === 'waiting');
    n.classList.toggle('visited', !current && steps.slice(0, i).some(p => p.node === n.id));
  });
  token.style.transform = `translate(${s.x}px, ${s.y ?? 36}px)`;  // token rides just above the nodes
});
</script>
```
