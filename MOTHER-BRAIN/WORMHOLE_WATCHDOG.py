#!/usr/bin/env python3
import hashlib,json,logging,os,subprocess,sys,time
from datetime import datetime,timezone
from pathlib import Path
if os.path.exists("/data/data/com.termux"):
    BASE_LOCAL="/storage/emulated/0/WORMHOLE"
else:
    BASE_LOCAL=os.path.expanduser("~/WORMHOLE")
PILLARS=["NOMADZ-0","FATHER-LIFE","MOTHER-BRAIN","MEGA-BRAIN","OMEGA-BRAIN","VULTURE-BRAIN","BRAIN-HOLE","BRAIN-FOOD","CANON","RAG_STORE","agents"]
C={"base_local":BASE_LOCAL,"base_remote":"gdrive:WORMHOLE","sync_targets":PILLARS,"manifest_path":os.path.join(BASE_LOCAL,"MOTHER-BRAIN/00_SYSTEM/wormhole_manifest.json"),"log_path":os.path.join(BASE_LOCAL,"MOTHER-BRAIN/logs/watchdog_heartbeat.jsonl"),"scan_interval_seconds":300,"backoff_base_seconds":2,"backoff_max_seconds":64,"max_retry_attempts":6,"ntfy_topic":"cosmic-key-omega-sol","watched_extensions":[".md",".json",".txt",".py",".gd"],"skip_dirs":[".git",".obsidian","__pycache__",".trash"],"rclone_flags":["--retries","3","--timeout","30s","--transfers","2","--stats","0"]}
Path(C["log_path"]).parent.mkdir(parents=True,exist_ok=True)
logging.basicConfig(level=logging.INFO,format="[%(asctime)s] %(levelname)s %(message)s",handlers=[logging.StreamHandler(sys.stdout)])
log=logging.getLogger("WATCHDOG")
def hb(ev,d):
    try:
        row={"ts":datetime.now(timezone.utc).isoformat(),"event":ev};row.update(d)
        open(C["log_path"],"a").write(json.dumps(row)+"
")
    except:pass
def push(title,msg,pri="default"):
    t=C.get("ntfy_topic","")
    if not t:return
    try:
        import urllib.request
        urllib.request.urlopen(urllib.request.Request("https://ntfy.sh/"+t,msg.encode(),{"Title":title,"Priority":pri,"Tags":"wormhole"},method="POST"),timeout=5)
    except:pass
def sha256f(p):
    h=hashlib.sha256()
    try:
        with open(p,"rb") as f:
            for c in iter(lambda:f.read(65536),b""):h.update(c)
        return "sha256:"+h.hexdigest()
    except:return "sha256:ERROR"
def load_manifest():
    p=Path(C["manifest_path"])
    try:return json.loads(p.read_text()) if p.exists() else {}
    except:return {}
def save_manifest(m):
    p=Path(C["manifest_path"]);p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(m,indent=2))
def scan_files(root):
    res={}
    for dp,dns,fns in os.walk(root):
        dns[:]=[d for d in dns if d not in set(C["skip_dirs"])]
        for fn in fns:
            if Path(fn).suffix.lower() not in set(C["watched_extensions"]):continue
            fp=Path(dp)/fn
            try:res[str(fp.relative_to(root))]={"hash":sha256f(fp),"mtime":fp.stat().st_mtime,"full_path":str(fp)}
            except:pass
    return res
def detect_drift(scan,manifest):
    out=[]
    for rp,info in scan.items():
        prev=manifest.get(rp)
        if not prev:out.append({"rel_path":rp,"reason":"NEW","hash":info["hash"],"mtime":info["mtime"],"full_path":info["full_path"]});continue
        if prev.get("hash")!=info["hash"]:out.append({"rel_path":rp,"reason":"CHANGED","hash":info["hash"],"mtime":info["mtime"],"full_path":info["full_path"]})
    return out
def rclone_copy(local,rel,pillar):
    r=subprocess.run(["rclone","copyto",local,C["base_remote"]+"/"+pillar+"/"+rel]+C["rclone_flags"],capture_output=True,text=True,timeout=120)
    return r.returncode==0
def sync_backoff(fi):
    rp=fi["rel_path"];fp=fi["full_path"];pillar=fi.get("pillar","");b=C["backoff_base_seconds"]
    for attempt in range(1,C["max_retry_attempts"]+1):
        log.info("Sync "+str(attempt)+"/"+str(C["max_retry_attempts"])+": "+rp)
        if rclone_copy(fp,rp,pillar):hb("SYNCED",{"file":rp,"attempt":attempt});return "SYNCED"
        if attempt<C["max_retry_attempts"]:time.sleep(min(b,C["backoff_max_seconds"]));b*=2
    hb("SYNC_FAILED",{"file":rp});push("WORMHOLE FAIL - "+pillar,"Failed: "+rp,pri="high");return "SYNC_FAILED"
def full_bisync():
    r=subprocess.run(["rclone","bisync",C["base_local"],C["base_remote"],"--resilient","--conflict-resolve","newer"]+C["rclone_flags"],capture_output=True,text=True,timeout=300)
    ok=r.returncode==0;hb("BISYNC_OK" if ok else "BISYNC_WARN",{"rc":r.returncode})
    if ok:push("WORMHOLE SYNCED","Full bisync OK.")
    return ok
def main():
    log.info("="*50);log.info("WORMHOLE_WATCHDOG v1.0.2  base="+C["base_local"]);log.info("Pillars: "+str(C["sync_targets"]));log.info("="*50)
    hb("START",{"base":C["base_local"]})
    base=Path(C["base_local"])
    if not base.exists():log.error("base_local missing: "+str(base));sys.exit(1)
    manifest=load_manifest();cycle=0
    while True:
        cycle+=1;log.info("=== Cycle #"+str(cycle)+" ===")
        if cycle==1 or cycle%12==0:full_bisync()
        total=0;drifted_total=0
        for pillar in C["sync_targets"]:
            pp=base/pillar
            if not pp.exists():log.warning("  skip: "+pillar);continue
            scan=scan_files(pp)
            for v in scan.values():v["pillar"]=pillar
            pm={k.replace(pillar+"///",""):v for k,v in manifest.items() if k.startswith(pillar+"///")}
            drifted=detect_drift(scan,pm);total+=len(scan);drifted_total+=len(drifted)
            if not drifted:log.info("  OK "+pillar+" ("+str(len(scan))+" files)");continue
            log.info("  DRIFT "+pillar+": "+str(len(drifted))+" files");push("DRIFT - "+pillar,str(len(drifted))+" files need sync")
            for fi in drifted:
                status=sync_backoff(fi);mk=pillar+"///"+fi["rel_path"]
                if status=="SYNCED":manifest[mk]={"hash":fi["hash"],"mtime":fi["mtime"],"last_sync_ts":time.time(),"sync_status":"SYNCED"}
                else:
                    e=manifest.get(mk,{});e["sync_status"]="SYNC_FAILED";e["last_fail_ts"]=time.time();manifest[mk]=e
                save_manifest(manifest)
        hb("CYCLE_DONE",{"cycle":cycle,"files":total,"drifted":drifted_total})
        log.info("Cycle #"+str(cycle)+" done. "+str(total)+" files, "+str(drifted_total)+" synced. Sleep "+str(C["scan_interval_seconds"])+"s")
        time.sleep(C["scan_interval_seconds"])
if __name__=="__main__":
    try:main()
    except KeyboardInterrupt:log.info("Stopped.");hb("STOP",{"reason":"KeyboardInterrupt"})
    except Exception as e:log.exception(e);hb("CRASH",{"error":str(e)});push("WATCHDOG CRASHED",str(e),pri="urgent");sys.exit(1)
