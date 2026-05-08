import os, platform, shutil

def check():
    print("--- MOTHER-BRAIN HEALTH CHECK ---")
    usage = shutil.disk_usage("/")
    print(f"Storage: {usage.free // (2**30)}GB free of {usage.total // (2**30)}GB")
    print(f"Platform: {platform.machine()} | Termux Environment")
    print(f"Wake Lock: {'Active' if os.path.exists('/dev/wakelock') else 'Unknown'}")

if __name__ == "__main__":
    check()
