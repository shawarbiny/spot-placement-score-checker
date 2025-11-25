# Screenshot Capture Checklist

Use this checklist to track your progress when capturing screenshots for the user guide.

## Pre-Capture Preparation

- [ ] Read the complete `SCREENSHOT_GUIDE.md` document
- [ ] Set up a clean browser environment (incognito/private mode)
- [ ] Prepare test data with generic names:
  - Demo User / Sample User for user name
  - Production Subscription / Development Subscription for subscription names
  - Remove/blur any real Azure subscription IDs
- [ ] Set browser window to consistent size (1920x1080 recommended)
- [ ] Ensure zoom level is 100%
- [ ] Close unnecessary browser tabs and bookmarks

## Screenshot Capture Status

### Authentication & Dashboard
- [ ] **01-signin.png** - Sign-in page
  - Email field visible but empty or with placeholder
  - No real email addresses visible
  - Size: ~1200x800px

- [ ] **02-dashboard.png** - Main dashboard
  - All form sections visible
  - User name blurred or generic
  - Subscription names hidden/generic
  - Size: ~1200x1400px

### Form Inputs
- [ ] **03-subscription-dropdown.png** - Subscription dropdown
  - Dropdown expanded showing options
  - Generic subscription names only
  - No real subscription IDs visible
  - Size: ~1200x900px

- [ ] **04-availability-options.png** - Availability options
  - Checkbox for availability zones (checked)
  - Desired count field with sample value (e.g., 5)
  - Cropped to show just this section
  - Size: ~800x400px

- [ ] **05-sku-selection.png** - VM SKU selection
  - Multiple SKUs visible with checkboxes
  - 3-4 SKUs selected (different families)
  - Clear SKU names visible
  - Size: ~1000x800px

- [ ] **06-region-selection.png** - Region selection
  - Multiple regions visible with checkboxes
  - 2-3 regions selected (geographically diverse)
  - Clear region names visible
  - Size: ~1000x600px

### Action & Results
- [ ] **07-check-button.png** - Check button
  - Complete form with selections visible
  - Button highlighted or prominent
  - User info hidden
  - Size: ~1200x1000px

- [ ] **08-results.png** - Results page
  - Multiple result cards visible (6-9 results)
  - Variety of score values shown
  - Good layout visibility
  - User name hidden
  - Size: ~1200x1600px

- [ ] **09-result-card.png** - Single result card detail
  - Close-up of one card
  - SKU name clear
  - Region clear
  - Placement score visible
  - Size: ~600x400px

### Guides & Navigation
- [ ] **10-decision-guide.png** - Score interpretation guide
  - Visual showing score ranges
  - Color-coded zones (green=excellent, red=poor)
  - Easy to understand
  - Size: ~800x600px
  - Note: This may need to be created as a graphic

- [ ] **11-new-search.png** - New search button
  - Button at bottom of results visible
  - Context of surrounding results
  - User info hidden
  - Size: ~1000x800px

## Post-Capture Review

### Data Sanitization Check
For each screenshot, verify:
- [ ] No real email addresses visible
- [ ] No real subscription names visible
- [ ] No subscription IDs visible
- [ ] No real user names visible
- [ ] No tenant IDs visible
- [ ] No client IDs visible
- [ ] No API keys or secrets visible
- [ ] No internal URLs (except localhost)
- [ ] No proprietary information visible

### Quality Check
For each screenshot, verify:
- [ ] Image is clear and not blurry
- [ ] Text is readable at 100% zoom
- [ ] Colors are accurate and consistent
- [ ] Proper dimensions (see size guidelines)
- [ ] File size under 500KB
- [ ] Saved in PNG format
- [ ] Proper filename (matches expected name)
- [ ] Saved in correct location (`docs/images/`)

### Technical Optimization
- [ ] All images optimized for web
- [ ] Images compressed (but still readable)
- [ ] Total size of all images reasonable (<3MB combined)
- [ ] Images display correctly in markdown preview

## File Organization

Verify files are in correct location:
```
docs/
├── SCREENSHOT_GUIDE.md
├── images/
│   ├── README.md
│   ├── 01-signin.png ✓
│   ├── 02-dashboard.png ✓
│   ├── 03-subscription-dropdown.png ✓
│   ├── 04-availability-options.png ✓
│   ├── 05-sku-selection.png ✓
│   ├── 06-region-selection.png ✓
│   ├── 07-check-button.png ✓
│   ├── 08-results.png ✓
│   ├── 09-result-card.png ✓
│   ├── 10-decision-guide.png ✓
│   └── 11-new-search.png ✓
```

## Git Commit Preparation

Before committing:
- [ ] All 11 screenshots captured and saved
- [ ] All sensitive data removed/blurred
- [ ] All images optimized
- [ ] Images display correctly in README preview
- [ ] README.md image links work correctly
- [ ] Created meaningful commit message

### Suggested Commit Message
```
Add user guide screenshots to documentation

- Added 11 screenshots illustrating step-by-step usage
- All sensitive data sanitized (no real emails, subscriptions, or IDs)
- Images optimized for web display
- Updated README with comprehensive user guide
```

## Final Steps

- [ ] Test README in GitHub preview (or local markdown viewer)
- [ ] Verify all image links work
- [ ] Check that images display at appropriate sizes
- [ ] Commit changes to git
- [ ] Push to GitHub repository
- [ ] View on GitHub to confirm everything displays correctly
- [ ] Update or remove `docs/images/README.md` placeholder file

## Optional Enhancements

Consider these optional improvements:
- [ ] Add annotations (arrows, highlights) to key screenshots
- [ ] Create animated GIFs for key workflows
- [ ] Add mobile/responsive view screenshots
- [ ] Create video walkthrough (link in README)
- [ ] Add captions under images for accessibility

## Notes

Use this space to track any issues or special considerations:

```
[Your notes here]
```

---

**Date Started**: _______________
**Date Completed**: _______________
**Captured By**: _______________
