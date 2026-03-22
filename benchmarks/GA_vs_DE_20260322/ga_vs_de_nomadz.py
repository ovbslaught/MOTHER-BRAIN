"""
GA vs DE Benchmark — NOMADZ Unified Dataset
442 real records from: GitHub (NOMADZ-0, ocean, MOTHER-BRAIN),
                        Asana, Notion, system telemetry
Target: binary classification (completed/active vs pending)
Model: SVM (RBF) — fast eval, meaningful HP landscape
"""

import numpy as np
import pandas as pd
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import matplotlib.gridspec as gridspec
import warnings, time, random, os
from scipy import stats

from sklearn.model_selection import StratifiedKFold, cross_val_score
from sklearn.preprocessing import StandardScaler
from sklearn.pipeline import Pipeline
from sklearn.svm import SVC
from imblearn.over_sampling import SMOTE

from deap import base, creator, tools

warnings.filterwarnings("ignore")

# ─────────────────────────────────────────────
# LOAD DATASET
# ─────────────────────────────────────────────
OUT = "/home/user/workspace/benchmark_outputs"

X = np.load(f"{OUT}/X_features.npy")
y = np.load(f"{OUT}/y_labels.npy")
df = pd.read_csv(f"{OUT}/nomadz_unified_dataset.csv")

print(f"NOMADZ Unified Dataset")
print(f"  Records   : {X.shape[0]}")
print(f"  Features  : {X.shape[1]}")
print(f"  Target    : binary (completed=1 vs pending=0)")
print(f"  Pos rate  : {y.mean():.2%} → imbalanced, using SMOTE")

# SMOTE to handle class imbalance
sm = SMOTE(random_state=42, k_neighbors=3)
X_bal, y_bal = sm.fit_resample(X, y)
print(f"  After SMOTE: {X_bal.shape[0]} records ({y_bal.mean():.2%} pos)")

# ─────────────────────────────────────────────
# CONFIG
# ─────────────────────────────────────────────
N_RUNS    = 30
POP_SIZE  = 10
N_GEN     = 5
BASE_SEED = 42
N_DIMS    = 4

CV = StratifiedKFold(n_splits=3, shuffle=True, random_state=BASE_SEED)
_cache = {}

def decode(x):
    x = np.clip(x, 0, 1)
    C     = 10 ** (x[0] * 6 - 3)
    gamma = 10 ** (x[1] * 6 - 4)
    cw    = "balanced" if x[2] >= 0.5 else None
    shrink = bool(x[3] >= 0.5)
    return C, gamma, cw, shrink

def objective(x):
    key = tuple(np.round(x, 4))
    if key in _cache:
        return _cache[key]
    C, gamma, cw, shrink = decode(x)
    model = Pipeline([
        ("sc",  StandardScaler()),
        ("clf", SVC(kernel="rbf", C=C, gamma=gamma,
                    class_weight=cw, shrinking=shrink,
                    random_state=BASE_SEED))
    ])
    score = cross_val_score(model, X_bal, y_bal, cv=CV, scoring="f1_weighted").mean()
    _cache[key] = score
    return score

# ─────────────────────────────────────────────
# GA
# ─────────────────────────────────────────────
def run_ga(seed):
    _cache.clear()
    rng = random.Random(seed)
    np.random.seed(seed)
    for attr in ("FitnessMax", "Individual"):
        if attr in creator.__dict__: delattr(creator, attr)
    creator.create("FitnessMax", base.Fitness, weights=(1.0,))
    creator.create("Individual", list, fitness=creator.FitnessMax)
    tb = base.Toolbox()
    tb.register("attr_f",     rng.random)
    tb.register("individual", tools.initRepeat, creator.Individual, tb.attr_f, n=N_DIMS)
    tb.register("population", tools.initRepeat, list, tb.individual)
    tb.register("evaluate",   lambda ind: (objective(np.array(ind)),))
    tb.register("mate",       tools.cxBlend, alpha=0.5)
    tb.register("mutate",     tools.mutGaussian, mu=0, sigma=0.2, indpb=0.5)
    tb.register("select",     tools.selTournament, tournsize=3)
    pop = tb.population(n=POP_SIZE)
    hof = tools.HallOfFame(1)
    for ind in pop: ind.fitness.values = tb.evaluate(ind)
    hof.update(pop)
    best_so_far = hof[0].fitness.values[0]
    convergence = [best_so_far]
    for _ in range(N_GEN):
        offspring = list(map(tb.clone, tb.select(pop, len(pop))))
        for c1, c2 in zip(offspring[::2], offspring[1::2]):
            if rng.random() < 0.7:
                tb.mate(c1, c2)
                del c1.fitness.values, c2.fitness.values
        for m in offspring:
            if rng.random() < 0.4:
                tb.mutate(m)
                for i in range(N_DIMS): m[i] = max(0.0, min(1.0, m[i]))
                del m.fitness.values
        for ind in offspring:
            if not ind.fitness.valid: ind.fitness.values = tb.evaluate(ind)
        pop[:] = offspring
        hof.update(pop)
        best_so_far = max(best_so_far, hof[0].fitness.values[0])
        convergence.append(best_so_far)
    C, gamma, cw, shrink = decode(np.array(hof[0]))
    return hof[0].fitness.values[0], convergence, {"C": round(C,4), "gamma": round(gamma,6), "class_weight": str(cw), "shrinking": shrink}

# ─────────────────────────────────────────────
# DE (best1bin)
# ─────────────────────────────────────────────
def run_de(seed):
    _cache.clear()
    rng = np.random.default_rng(seed)
    pop = rng.random((POP_SIZE, N_DIMS))
    fitness = np.array([objective(ind) for ind in pop])
    best_idx = np.argmax(fitness)
    best_so_far = fitness[best_idx]
    convergence = [best_so_far]
    F, CR = 0.8, 0.9
    for gen in range(N_GEN):
        for i in range(POP_SIZE):
            idxs = [j for j in range(POP_SIZE) if j != i]
            a, b = rng.choice(idxs, 2, replace=False)
            mutant = np.clip(pop[best_idx] + F * (pop[a] - pop[b]), 0, 1)
            cross_pts = rng.random(N_DIMS) < CR
            if not cross_pts.any(): cross_pts[rng.integers(N_DIMS)] = True
            trial = np.where(cross_pts, mutant, pop[i])
            tf = objective(trial)
            if tf >= fitness[i]: pop[i] = trial; fitness[i] = tf
        best_idx = np.argmax(fitness)
        best_so_far = max(best_so_far, fitness[best_idx])
        convergence.append(best_so_far)
    C, gamma, cw, shrink = decode(pop[np.argmax(fitness)])
    return fitness[np.argmax(fitness)], convergence, {"C": round(C,4), "gamma": round(gamma,6), "class_weight": str(cw), "shrinking": shrink}

# ─────────────────────────────────────────────
# RUN
# ─────────────────────────────────────────────
EVAL_BUDGET = POP_SIZE * (N_GEN + 1)
print(f"\n{N_RUNS} runs × 2 optimizers | Pop={POP_SIZE} | Gens={N_GEN} | Budget={EVAL_BUDGET}\n")

ga_scores, ga_curves, ga_plist = [], [], []
de_scores, de_curves, de_plist = [], [], []

t0 = time.time()
for run in range(N_RUNS):
    seed = BASE_SEED + run * 17
    gs, gc, gp = run_ga(seed)
    ds, dc, dp = run_de(seed)
    ga_scores.append(gs); ga_curves.append(gc); ga_plist.append(gp)
    de_scores.append(ds); de_curves.append(dc); de_plist.append(dp)
    print(f"  Run {run+1:02d}/30 | GA={gs:.4f}  DE={ds:.4f}", flush=True)

print(f"\nDone in {time.time()-t0:.1f}s")

ga_scores = np.array(ga_scores); de_scores = np.array(de_scores)
ga_curves = np.array(ga_curves); de_curves = np.array(de_curves)

# ─────────────────────────────────────────────
# STATS
# ─────────────────────────────────────────────
def summ(a):
    return {"mean":np.mean(a),"median":np.median(a),"std":np.std(a),
            "min":np.min(a),"max":np.max(a),"q25":np.percentile(a,25),"q75":np.percentile(a,75)}

ga_s = summ(ga_scores); de_s = summ(de_scores)
stat_df = pd.DataFrame([ga_s, de_s], index=["GA","DE"]).round(5)
print("\n=== SUMMARY ==="); print(stat_df.to_string())

w_stat, w_pval = stats.wilcoxon(ga_scores, de_scores)
print(f"\nWilcoxon W={w_stat:.2f}, p={w_pval:.4f}")

def conv_speed(curves):
    sp = []
    for c in curves:
        t = 0.99 * c[-1]
        for i,v in enumerate(c):
            if v >= t: sp.append(i); break
        else: sp.append(len(c)-1)
    return np.array(sp)

ga_cs = conv_speed(ga_curves); de_cs = conv_speed(de_curves)
print(f"Conv speed — GA:{np.median(ga_cs):.1f}  DE:{np.median(de_cs):.1f}")

# ─────────────────────────────────────────────
# SAVE CSVs
# ─────────────────────────────────────────────
pd.DataFrame({
    "run": list(range(1,N_RUNS+1)),
    "seed": [BASE_SEED+r*17 for r in range(N_RUNS)],
    "ga_score": ga_scores, "de_score": de_scores,
    "ga_conv_gen": ga_cs, "de_conv_gen": de_cs,
}).to_csv(f"{OUT}/per_run_scores.csv", index=False)

stat_df.to_csv(f"{OUT}/summary_stats.csv")

gens = np.arange(ga_curves.shape[1])
pd.DataFrame({
    "generation": gens,
    "ga_mean": ga_curves.mean(0), "ga_std": ga_curves.std(0), "ga_best": ga_curves.max(0),
    "de_mean": de_curves.mean(0), "de_std": de_curves.std(0), "de_best": de_curves.max(0),
}).to_csv(f"{OUT}/convergence_curves.csv", index=False)

ga_bp = ga_plist[np.argmax(ga_scores)]
de_bp = de_plist[np.argmax(de_scores)]
pd.DataFrame([ga_bp, de_bp], index=["GA_best","DE_best"]).to_csv(f"{OUT}/best_hyperparams.csv")

# ─────────────────────────────────────────────
# CHARTS
# ─────────────────────────────────────────────
PA = {"GA":"#4E79A7","DE":"#F28E2B"}
fig = plt.figure(figsize=(18,13), facecolor="#0f1117")
fig.suptitle(
    "GA vs Differential Evolution — Hyperparameter Tuning Benchmark\n"
    "Dataset: NOMADZ Unified (442 records | GitHub · Asana · Notion · Telemetry) | SVM (RBF) | 30 Runs × 60 Evals",
    color="white", fontsize=12.5, y=0.98
)
gsl = gridspec.GridSpec(2, 3, figure=fig, hspace=0.46, wspace=0.38)

# 1. Convergence
ax1 = fig.add_subplot(gsl[0,:2])
ax1.set_facecolor("#1a1d27")
for nm, curves, col in [("GA",ga_curves,PA["GA"]),("DE",de_curves,PA["DE"])]:
    m = curves.mean(0); s = curves.std(0); g = np.arange(len(m))
    ax1.plot(g, m, color=col, lw=2.5, label=f"{nm} mean")
    ax1.fill_between(g, m-s, m+s, color=col, alpha=0.18)
    ax1.plot(g, curves.max(0), color=col, lw=1.2, ls="--", alpha=0.6, label=f"{nm} best run")
ax1.set_title("Convergence Curves — Mean ± Std (30 Runs)", color="white", fontsize=11)
ax1.set_xlabel("Generation", color="white"); ax1.set_ylabel("Weighted F1 (3-Fold CV)", color="white")
ax1.tick_params(colors="white"); ax1.spines[:].set_color("#444")
ax1.legend(facecolor="#1a1d27", labelcolor="white", fontsize=9)

# 2. Box
ax2 = fig.add_subplot(gsl[0,2])
ax2.set_facecolor("#1a1d27")
bp = ax2.boxplot([ga_scores,de_scores], labels=["GA","DE"], patch_artist=True,
    medianprops=dict(color="white",lw=2), whiskerprops=dict(color="white"),
    capprops=dict(color="white"), flierprops=dict(markerfacecolor="white",marker="o",markersize=3,alpha=0.5))
for patch,col in zip(bp["boxes"],[PA["GA"],PA["DE"]]):
    patch.set_facecolor(col); patch.set_alpha(0.85)
ytop = max(ga_scores.max(),de_scores.max())+0.001
ax2.plot([1,2],[ytop,ytop],color="white",lw=1)
ax2.text(1.5,ytop+0.0003,f"p={w_pval:.4f}"+(" *" if w_pval<0.05 else " ns"),
         ha="center",color="white",fontsize=9)
ax2.set_title("Final Score Distribution", color="white", fontsize=11)
ax2.set_ylabel("Weighted F1", color="white")
ax2.tick_params(colors="white"); ax2.spines[:].set_color("#444")

# 3. Violin
ax3 = fig.add_subplot(gsl[1,0])
ax3.set_facecolor("#1a1d27")
vp = ax3.violinplot([ga_scores,de_scores], positions=[1,2], showmedians=True, showextrema=True)
for body,col in zip(vp["bodies"],[PA["GA"],PA["DE"]]):
    body.set_facecolor(col); body.set_alpha(0.75)
for part in ["cmedians","cbars","cmaxes","cmins"]: vp[part].set_color("white")
ax3.set_xticks([1,2]); ax3.set_xticklabels(["GA","DE"])
ax3.set_title("Score Distribution — Violin", color="white", fontsize=11)
ax3.set_ylabel("Weighted F1", color="white")
ax3.tick_params(colors="white"); ax3.spines[:].set_color("#444")

# 4. Convergence speed
ax4 = fig.add_subplot(gsl[1,1])
ax4.set_facecolor("#1a1d27")
bins = np.arange(-0.5, N_GEN+2)
ax4.hist(ga_cs, bins=bins, color=PA["GA"], alpha=0.75, label="GA", edgecolor="white", lw=0.4)
ax4.hist(de_cs, bins=bins, color=PA["DE"], alpha=0.75, label="DE", edgecolor="white", lw=0.4)
ax4.axvline(np.median(ga_cs), color=PA["GA"], lw=2.2, ls="--", label=f"GA med={np.median(ga_cs):.0f}")
ax4.axvline(np.median(de_cs), color=PA["DE"], lw=2.2, ls="--", label=f"DE med={np.median(de_cs):.0f}")
ax4.set_title("Convergence Speed\n(Gen to 99% of best)", color="white", fontsize=11)
ax4.set_xlabel("Generation", color="white"); ax4.set_ylabel("Runs", color="white")
ax4.tick_params(colors="white"); ax4.spines[:].set_color("#444")
ax4.legend(facecolor="#1a1d27", labelcolor="white", fontsize=8)

# 5. Stats table + source breakdown
ax5 = fig.add_subplot(gsl[1,2])
ax5.set_facecolor("#1a1d27"); ax5.axis("off")
tdata = [
    ["Metric","GA","DE"],
    ["Mean F1",  f"{ga_s['mean']:.4f}",   f"{de_s['mean']:.4f}"],
    ["Median",   f"{ga_s['median']:.4f}", f"{de_s['median']:.4f}"],
    ["Std Dev",  f"{ga_s['std']:.5f}",    f"{de_s['std']:.5f}"],
    ["Best",     f"{ga_s['max']:.4f}",    f"{de_s['max']:.4f}"],
    ["Worst",    f"{ga_s['min']:.4f}",    f"{de_s['min']:.4f}"],
    ["Q25–Q75",  f"{ga_s['q25']:.4f}–{ga_s['q75']:.4f}",
                 f"{de_s['q25']:.4f}–{de_s['q75']:.4f}"],
    ["Conv Gen", f"{np.median(ga_cs):.0f}", f"{np.median(de_cs):.0f}"],
    ["p-value",  f"{w_pval:.4f}", "Wilcoxon"],
    ["Dataset",  "NOMADZ", "442 rows"],
    ["Sources",  "GitHub", "Asana+Notion"],
]
cc = [["#3a3d50"]*3] + [["#2a2d3a"]*3]*(len(tdata)-1)
tbl = ax5.table(cellText=tdata, cellLoc="center", loc="center", cellColours=cc)
tbl.auto_set_font_size(False); tbl.set_fontsize(8.5); tbl.scale(1.1, 1.45)
for (r,c), cell in tbl.get_celld().items():
    cell.set_text_props(color="white"); cell.set_edgecolor("#555")
ax5.set_title("Statistical Summary", color="white", fontsize=11, pad=12)

plt.savefig(f"{OUT}/ga_vs_de_benchmark.png", dpi=150, bbox_inches="tight", facecolor="#0f1117")
print("Chart saved.")

# ─────────────────────────────────────────────
# RECOMMENDATION
# ─────────────────────────────────────────────
w_mean  = "GA" if ga_s["mean"]  > de_s["mean"]  else "DE"
w_best  = "GA" if ga_s["max"]   > de_s["max"]   else "DE"
w_speed = "GA" if np.median(ga_cs) <= np.median(de_cs) else "DE"
w_var   = "GA" if ga_s["std"]   < de_s["std"]   else "DE"
sig_str = "statistically significant" if w_pval < 0.05 else "not statistically significant"
ga_wins = sum([w_mean=="GA", w_best=="GA", w_speed=="GA", w_var=="GA"])
de_wins = 4 - ga_wins
overall = "GA" if ga_wins >= 3 else ("DE" if de_wins >= 3 else "MIXED")

src_counts = df['source'].value_counts().to_dict()

rec = f"""
GA vs Differential Evolution — Benchmark Recommendation
=========================================================
Dataset   : NOMADZ Unified — real data from your stack
  Sources : GitHub (knowledge_graph={src_counts.get('knowledge_graph',0)}, session_backlog_completed={src_counts.get('session_backlog_completed',0)},
             session_backlog_pending={src_counts.get('session_backlog_pending',0)}, component_status={src_counts.get('component_status',0)}, characters={src_counts.get('characters',0)})
             Asana (mother_brain={src_counts.get('asana_mother_brain',0)}, voltron={src_counts.get('asana_voltron',0)})
             Notion ({src_counts.get('notion',0)} pages/databases)
             System telemetry ({src_counts.get('system_monitor',0)} records)
  Total   : 442 records | 8 features | SMOTE-balanced
Model     : SVM (RBF kernel), Weighted F1, 3-Fold CV
Budget    : {EVAL_BUDGET} evals/run × {N_RUNS} independent runs

RESULTS
-------
                  GA           DE
Mean F1        : {ga_s['mean']:.4f}       {de_s['mean']:.4f}
Median         : {ga_s['median']:.4f}       {de_s['median']:.4f}
Std deviation  : {ga_s['std']:.5f}      {de_s['std']:.5f}
Best achieved  : {ga_s['max']:.4f}       {de_s['max']:.4f}
Worst achieved : {ga_s['min']:.4f}       {de_s['min']:.4f}
Conv speed     : {np.median(ga_cs):.0f} gen          {np.median(de_cs):.0f} gen

Wilcoxon signed-rank: W={w_stat:.2f}, p={w_pval:.4f} ({sig_str})

DIMENSION WINNERS  (GA wins: {ga_wins}/4 | DE wins: {de_wins}/4)
------------------
Best mean F1        : {w_mean}
Best single run     : {w_best}
Fastest convergence : {w_speed}
Lower variance      : {w_var}

RECOMMENDATION
--------------
"""

if overall == "DE":
    rec += f"""
► USE DE (Differential Evolution) for the next experiment.

DE wins {de_wins}/4 dimensions on your real NOMADZ data. On an imbalanced classification
landscape with mixed entity types (graph nodes, tasks, characters, telemetry), DE's
best1bin mutation navigates the log(C)/log(γ) space more reliably than GA crossover.

The performance difference is {sig_str} (p={w_pval:.4f}).
{"This is a robust, seed-independent result." if w_pval < 0.05 else "The gap is directional but slim — both are viable choices."}
"""
elif overall == "GA":
    rec += f"""
► USE GA for the next experiment.

GA wins {ga_wins}/4 dimensions on your NOMADZ dataset. The blend crossover operator
explores categorical boundaries (class_weight, shrinking) efficiently on this mixed
entity classification task.

The performance difference is {sig_str} (p={w_pval:.4f}).
"""
else:
    rec += f"""
► MIXED RESULT on this dataset. Priority guide:
  • Best final F1 score    → {w_best}
  • Fastest convergence    → {w_speed}
  • Most stable (low var)  → {w_var}
"""

rec += f"""
NEXT EXPERIMENT SETUP
---------------------
Dataset     : Grow NOMADZ Unified — add Google Sheets rows, Colab notebook
              training logs, Sentry events (once projects are wired), and
              WordPress analytics when MCP is enabled.
Model       : Graduate to XGBoost or LightGBM for the next phase (more HP dims).
Optimizer   : {w_best} with these warm-start params:
  GA best:  {ga_bp}
  DE best:  {de_bp}

SCALE-UP GUIDANCE
-----------------
GA  : popsize 20–30, elitist HOF(3), cxSimulatedBinaryBounded for continuous dims.
DE  : rand1bin for exploration phase; jDE self-adaptive for longer runs (>20 gens).
Both: Hybrid warm-start — initialize DE population from GA's top-5 individuals.
     At >10 HP dims (e.g., XGBoost), DE outperforms GA significantly at small pop.

DATA GAPS TO FILL
-----------------
• Google Sheets: No spreadsheets found — create one to log training runs.
• Colab: No .ipynb files in Drive — notebooks likely local/Termux only.
• Sentry: geologos org exists but no projects. Wire MOTHER-BRAIN or NOMADZ-0.
• Slack: Workspace connected but empty. Route daemon logs here.
• HuggingFace: Account 0VSol13 active — publish a dataset to the Hub.
• Linear: Not connected (DISCONNECTED) — reconnect for issue tracking data.
"""

print(rec)
with open(f"{OUT}/recommendation.txt", "w") as f:
    f.write(rec)

print(f"\n✓ All outputs saved to {OUT}/")
