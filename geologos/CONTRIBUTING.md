# CONTRIBUTING.md - GEOLOGOS Catalog Bible

Welcome to the GEOLOGOS Universal Knowledge Catalog Bible community! We're building humanity's knowledge commons together.

## How to Contribute

### 1. Types of Contributions Welcome

**Content Expansion**
- Add new sections to existing pillars
- Contribute supplementary modules (see README for priority list)
- Improve existing content with citations, examples, case studies
- Translate sections into other languages

**Quality Improvements**
- Fix inaccuracies, typos, broken links
- Improve clarity and accessibility
- Add visual elements (diagrams, infographics descriptions)
- Suggest restructuring for better flow

**Technical Enhancements**
- Improve JSON schema
- Add API documentation
- Create interactive visualizations
- Build tools/converters

**Community Building**
- Suggest editorial improvements
- Facilitate discussions
- Help with translations
- Support new contributors

### 2. Getting Started

**Step 1: Fork the Repository**
```bash
git clone https://github.com/[user]/catalog-bible.git
cd catalog-bible
git checkout -b feature/your-contribution-name
```

**Step 2: Review Relevant Documentation**
- Read the section you're contributing to
- Check CHANGELOG.md for recent updates
- Review existing pull requests for context

**Step 3: Make Your Changes**
- Follow the content structure for consistency
- Use markdown formatting (see STYLE_GUIDE.md)
- Add citations with [id] format
- Update cross-references if applicable

**Step 4: Test & Validate**
```bash
python validate_catalog.py  # Checks markdown, JSON, links
npm run lint  # Code formatting
```

**Step 5: Submit Pull Request**
- Clear title and description
- Reference related issues
- Explain your changes and why
- Include any new citations/sources

### 3. Content Standards

**Depth Levels**
- **Level 1:** Conceptual foundations (new contributors start here)
- **Level 2–5:** Requires domain expertise or collaboration with expert reviewer

**Citation Requirements**
- All factual claims must be cited [id]
- Use consistent format: [1][2][3] for multiple sources
- Update SOURCES.md with new references
- Prefer peer-reviewed, open-access, and primary sources

**Accessibility**
- Use clear, jargon-minimal language where possible
- Define technical terms on first use
- Provide examples and metaphors
- WCAG 2.1 AA compliance for visual content

**Quality Checklist**
- [ ] Content accurate and current
- [ ] Properly cited
- [ ] Cross-references updated
- [ ] Accessible language
- [ ] Markdown formatting correct
- [ ] No broken links
- [ ] Integrated with existing sections

### 4. Code of Conduct

We maintain a welcoming, inclusive community. All participants agree to:

**Respect & Inclusion**
- Treat all contributors with respect
- Value diverse perspectives, backgrounds, expertise
- Use inclusive language
- Welcome newcomers and provide mentoring

**Intellectual Integrity**
- Credit sources and prior work
- Acknowledge contributors
- Be honest about limitations and uncertainties
- Engage in good-faith discussion

**No Harassment**
- Zero tolerance for harassment, discrimination, slurs
- Constructive criticism on ideas, not personal attacks
- Respect for boundaries
- Reporting mechanism for violations (see Safety section)

**Professionalism**
- Collaborative spirit
- Assume good intentions
- Constructive conflict resolution
- Focus on improving shared knowledge

### 5. Process

**For Small Changes** (typos, minor edits)
1. Submit PR directly
2. One maintainer review
3. Merge if approved

**For Content Additions** (new sections, supplementary modules)
1. Open GitHub Issue first (describe proposed content)
2. Discuss with maintainers (get feedback, avoid duplicates)
3. Fork and create branch
4. Submit PR with detailed explanation
5. Two maintainer reviews + community feedback
6. Address review comments
7. Merge when consensus reached

**For Major Changes** (restructuring, new pillars, significant policy)
1. Open GitHub Discussion for community input
2. Request RFC (Request for Comments)
3. Collect feedback (min. 2 weeks)
4. Present analysis to maintainers
5. Community vote if needed
6. Implementation phase

### 6. Review Process

**Reviewers Will Check**
- Accuracy of information
- Proper citations
- Alignment with existing content
- Completeness and depth
- Clarity and accessibility
- Technical correctness
- Integration with other pillars

**Expected Timeline**
- Small changes: 3–7 days
- Medium additions: 7–14 days
- Major changes: 14–30 days

**Feedback Approach**
- Constructive, specific suggestions
- Ask clarifying questions
- Acknowledge good work
- Collaborative problem-solving

### 7. Attribution & Recognition

**Every Contributor Gets:**
- GitHub author credit (commit history)
- Listed in CONTRIBUTORS.md (all versions)
- Mentioned in quarterly CHANGELOG
- Optional byline in contributed sections

**Recognition Tiers**
- 1–5 contributions: Contributor
- 6–20 contributions: Active Contributor
- 20+ contributions: Core Team consideration

### 8. Tools & Resources

**Writing**
- Markdown guide: STYLE_GUIDE.md
- Citation format: CITATIONS.md
- Templates: /templates/ folder

**Validation**
- `validate_catalog.py` — Check structure, links, completeness
- `check_citations.py` — Verify all claims cited
- `format_check.py` — Markdown consistency

**Collaboration**
- GitHub Issues for discussion
- GitHub Discussions for proposals
- Slack/Discord (link in README)

### 9. Safety & Reporting

**Reporting Violations**
- Email: conduct@geologos-catalog.org
- Anonymous form: [link]
- Response within 48 hours

**Conflicts of Interest**
- Disclose if personally/financially invested
- Recuse from reviews if needed
- Transparency builds trust

### 10. Frequently Asked Questions

**Q: I'm new to open source. Can I contribute?**
A: Absolutely! Start with small fixes (typos, clarity improvements) to get familiar with the process. Mentors available to guide you.

**Q: How do I know if my idea is in scope?**
A: Read the mission statement and open an issue first. Maintainers will provide feedback quickly.

**Q: Can I translate content?**
A: Yes! Start with README and one high-priority section. Coordinate via GitHub Discussions to avoid duplicate work.

**Q: What if I disagree with existing content?**
A: Open an issue or discussion. Provide sources supporting your perspective. Constructive debate strengthens knowledge.

**Q: How long until my PR is merged?**
A: Depends on complexity (see timeline above). Feel free to comment if delayed; we may need more info.

**Q: Can I get paid for contributions?**
A: Currently, contributions are volunteer. We're exploring grant funding for dedicated roles in v1.2+.

### 11. Decision-Making & Governance

**For routine decisions:** Maintainers decide based on contribution guidelines
**For significant changes:** Community vote (all active contributors)
**For conflicts:** Structured mediation process (details in GOVERNANCE.md)

**Current Maintainers:**
- [List]

**Becoming a Maintainer:**
- 20+ quality contributions
- Demonstrated commitment to mission
- Community nomination + vote

---

## 🎉 Thank You!

Your contributions make GEOLOGOS stronger, more accurate, more inclusive. We appreciate every improvement, no matter how small.

**Let's build humanity's knowledge commons together.**

---

*Last Updated: November 16, 2025*
*Version: 1.0*