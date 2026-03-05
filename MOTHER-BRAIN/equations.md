{
  "schema": "voltron.math.equations.v1",
  "generated_at": "2026-03-04T23:58:00-05:00",
  "authority": "SOL",
  "system_state": {
    "phi_base": 0.886,
    "phi_projected": 1.100,
    "phiindex_bounded": 1.000,
    "phiband": "CEILINGHARMONIC"
  },
  "equations": [
    {
      "id": "EQ-001-RESONANCE",
      "name": "Universal Resonance Scaling (VOLTRON form)",
      "formula_ascii": "f_sync = k * M^alpha",
      "notes": [
        "Keep f_sync target aligned to the 0.618 resonance motif when used as a control target"
      ],
      "variables": {
        "f_sync": "Global heartbeat frequency (target motif: 0.618 Hz)",
        "k": "Proportionality constant",
        "M": "Active nodes (mass proxy)",
        "alpha": "Scaling exponent (domain-chosen)"
      },
      "sources": [
        "what-was-the-equation-willow-s-WNGHFAm2S5.fBrQZKrCEbA.md"
      ],
      "revisions": [
        {
          "rev": 1,
          "timestamp": "2026-03-04T23:58:00-05:00",
          "change": "Initialized canonical storage record"
        }
      ]
    },
    {
      "id": "EQ-002-HARMONIC-PHI",
      "name": "Phi-index stability bands (log + bound)",
      "formula_ascii": "Phi_bands = [0.618, 0.707, 0.786, 0.886, 1.000]; Phi_projected may exceed 1.0 but Phi_bounded = min(Phi_projected, 1.0)",
      "mapping": {
        "0.618": "Base stability",
        "0.707": "Energy balance motif",
        "0.786": "Level-3 recursion motif",
        "0.886": "Level-4 renewal motif",
        "1.000": "Ceiling bound"
      },
      "sources": [
        "snapshot-space-name-csomologos-17_2klOkRPGjoH_apLA5Jw.md"
      ],
      "revisions": [
        {
          "rev": 1,
          "timestamp": "2026-03-04T23:58:00-05:00",
          "change": "Encoded CEILING_HARMONIC bounding rule explicitly"
        }
      ]
    },
    {
      "id": "EQ-003-WILLOW-ERROR",
      "name": "Logical error suppression (scaling form)",
      "formula_ascii": "P_logical ~ 1 / (d^p)",
      "variables": {
        "P_logical": "Probability of logical error",
        "d": "Code distance (e.g., distance-7 mentioned in corpus)",
        "p": "Scaling factor"
      },
      "sources": [
        "what-was-the-equation-willow-s-WNGHFAm2S5.fBrQZKrCEbA.md"
      ],
      "revisions": [
        {
          "rev": 1,
          "timestamp": "2026-03-04T23:58:00-05:00",
          "change": "Stored generic scaling law without claiming specific numeric halving law beyond corpus"
        }
      ]
    },
    {
      "id": "EQ-004-SPECTRAL-ANCHORS",
      "name": "Frequency anchors (narrative-to-physical constants list)",
      "constants": {
        "H_line_MHz": 1420.40575,
        "OH_line_MHz": 1665.402,
        "Schumann_Hz": 7.83,
        "Foundation_Hz": 60,
        "A4_Hz": 440
      },
      "sources": [
        "this-sounds-like-an-incredibly-O3E_7XRSQMqMAIlBTrwplA.md"
      ],
      "revisions": [
        {
          "rev": 1,
          "timestamp": "2026-03-04T23:58:00-05:00",
          "change": "Canonicalized units into explicit Hz/MHz numeric fields"
        }
      ]
    }
  ]
}