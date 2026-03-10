FROM python:3.11-slim
LABEL maintainer="ovbslaught@gmail.com"
WORKDIR /app
RUN apt-get update && apt-get install -y git curl ffmpeg build-essential && rm -rf /var/lib/apt/lists/*
COPY requirements.txt .
RUN pip install --no-cache-dir --upgrade pip && pip install --no-cache-dir -r requirements.txt
COPY scripts/ ./scripts/
COPY config/ ./config/
RUN mkdir -p /data/brain-hole /data/admin-queue /data/approved /data/published /data/drive /data/chroma
ENV PYTHONUNBUFFERED=1
ENV PYTHONPATH=/app
CMD ["python", "/app/scripts/mother_brain.py"]
