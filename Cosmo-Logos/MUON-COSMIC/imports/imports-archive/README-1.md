

---
license: apache-2.0
tags:
- audio
- speech
- audio-to-audio
- speech-language-models
datasets:
- amphion/Emilia-Dataset
- facebook/multilingual_librispeech
- CSTR-Edinburgh/vctk
- google/fleurs
- mozilla-foundation/common_voice_13_0
- mythicinfinity/libritts_r
---

# NeuCodec 🎧

[![NeuCodec Intro](NeuCodec-Thumbnail.jpg)](https://www.youtube.com/watch?v=O7XH1lGZyYY)

*Click the image above to see NeuCodec in action on Youtube!*

*Created by Neuphonic - building faster, smaller, on-device voice AI*

A lightweight neural codec that encodes audio at just 0.8 kbps - perfect for researchers and builders who need something that *just works* for training high quality text-to-speech models.

# Key Features

* 🔊 Low bit-rate compression - a speech codec that compresses and reconstructs audio with near-inaudible reconstruction loss
<br>
* 🎼 Upsamples from 16kHz → 24kHz
<br>
* 🌍 Ready for real-world use - train your own SpeechLMs without needing to build your own codec
<br>
* 🏢 Commercial use permitted - use it in your own tools or products
<br>
* 📊 Released with large pre-encoded datasets - we’ve compressed Emilia-YODAS from 1.7TB to 41GB using NeuCodec, significantly reducing the compute requirements needed for training 
<br>

# Model Details

NeuCodec is a Finite Scalar Quantisation (FSQ) based 0.8kbps audio codec for speech tokenization.
It takes advantage of the following features:

* FSQ quantisation resulting in a single codebook, making it ideal for downstream modeling with Speech Language Models.
* Trained with CC data such that there are no Non-Commercial data restrictions.
* At 50 tokens/sec and 16 bits per token, the overall bit-rate is 0.8kbps.
* The codec takes in 16kHz input and outputs 24kHz using an upsampling decoder.
* The FSQ encoding scheme allows for bit-level error resistance suitable for unreliable and noisy channels.

NeuCodec is largely based on extending the work of [X-Codec2.0](https://huggingface.co/HKUSTAudio/xcodec2).

- **Developed by:** Neuphonic
- **Model type:** Neural Audio Codec
- **License:** apache-2.0
- **Repository:** https://github.com/neuphonic/neucodec
- **Paper:** [arXiv](https://arxiv.org/abs/2509.09550)
- **Pre-encoded Datasets:**
  - [Emilia-YODAS-EN](https://huggingface.co/datasets/neuphonic/emilia-yodas-english-neucodec)
  - *More coming soon!*

# Get Started

Use the code below to get started with the model.

To install from pypi in a dedicated environment, using Python 3.10 or above:

```bash
conda create -n neucodec python=3.10
conda activate neucodec
pip install neucodec
```
Then, to use in python:

```python
import librosa
import torch
import torchaudio
from torchaudio import transforms as T
from neucodec import NeuCodec
 
model = NeuCodec.from_pretrained("neuphonic/neucodec")
model.eval().cuda()   
 
y, sr = torchaudio.load(librosa.ex("libri1"))
if sr != 16_000:
    y = T.Resample(sr, 16_000)(y)[None, ...] # (B, 1, T_16)

with torch.no_grad():
    fsq_codes = model.encode_code(y)
    # fsq_codes = model.encode_code(librosa.ex("libri1")) # or directly pass your filepath!
    print(f"Codes shape: {fsq_codes.shape}")  
    recon = model.decode_code(fsq_codes).cpu() # (B, 1, T_24)

torchaudio.save("reconstructed.wav", recon[0, :, :], 24_000)
```

# Training Details

The model was trained using the following data: 
* Emilia-YODAS
* MLS
* LibriTTS
* Fleurs
* CommonVoice
* HUI
* Additional proprietary set

All publically available data was covered by either the CC-BY-4.0 or CC0 license.