# Screenshot Guide for User Documentation

This guide explains how to capture screenshots for the README without exposing sensitive data.

## Required Screenshots

### 1. Sign In Page (`01-signin.png`)
- **URL**: Capture the Microsoft Entra ID login page
- **What to show**: The login interface with email field
- **What to hide**: 
  - Do NOT enter your actual email address
  - Use a generic example like "user@example.com" or blur the email field
- **Recommended size**: 1200x800px

### 2. Main Dashboard (`02-dashboard.png`)
- **URL**: Main application page after login
- **What to show**: 
  - Subscription dropdown (closed state)
  - Availability zones checkbox
  - Desired count input field
  - VM SKU section
  - Region section
  - Submit button
- **What to hide**: 
  - User name in the header (blur or use generic name)
  - Specific subscription names (blur or use placeholders)
- **Recommended size**: 1200x1400px (full page)

### 3. Subscription Selection (`03-subscription-dropdown.png`)
- **URL**: Main page with subscription dropdown expanded
- **What to show**: Dropdown menu expanded with subscription list
- **What to hide**: 
  - Real subscription names - replace with:
    - "Production Subscription"
    - "Development Subscription"
    - "Testing Subscription"
  - Real subscription IDs - blur or hide
  - User name in header
- **Recommended size**: 1200x900px
- **Tip**: Use browser dev tools to modify the DOM and replace subscription names before taking screenshot

### 4. Availability Options (`04-availability-options.png`)
- **URL**: Main page, zoomed to show availability options area
- **What to show**: 
  - Availability zones checkbox (checked)
  - Desired count input field with a value (e.g., "5")
- **What to hide**: User information
- **Recommended size**: 800x400px
- **Tip**: Crop to just show the relevant section

### 5. VM SKU Selection (`05-sku-selection.png`)
- **URL**: Main page after subscription is selected
- **What to show**: 
  - List of VM SKUs with checkboxes
  - Some SKUs checked (e.g., Standard_D2s_v3, Standard_D4s_v3, Standard_E2s_v3)
  - Clear view of SKU names
- **What to hide**: User information, subscription details
- **Recommended size**: 1000x800px
- **Tip**: Select 3-4 representative SKUs from different families

### 6. Region Selection (`06-region-selection.png`)
- **URL**: Main page after subscription is selected
- **What to show**: 
  - List of regions with checkboxes
  - Some regions checked (e.g., East US, West Europe, Southeast Asia)
  - Clear view of region names
- **What to hide**: User information, subscription details
- **Recommended size**: 1000x600px
- **Tip**: Select 2-3 geographically diverse regions

### 7. Check Button (`07-check-button.png`)
- **URL**: Main page with selections made
- **What to show**: 
  - Complete form with selections
  - "Check Spot Placement Scores" button highlighted
- **What to hide**: 
  - User information
  - Subscription names (can be blurred)
- **Recommended size**: 1200x1000px
- **Tip**: Use a highlight or arrow to draw attention to the button

### 8. Results Page (`08-results.png`)
- **URL**: Results page after checking scores
- **What to show**: 
  - Multiple result cards showing different SKU-region combinations
  - Placement scores (use realistic values: 75-95 for good scores, 30-50 for poor scores)
  - Clear layout of results grid
- **What to hide**: 
  - User name
  - Real subscription name
  - Use generic subscription name like "Production Subscription"
- **Recommended size**: 1200x1600px (full results page)
- **Tip**: Ensure you have at least 6-9 results showing different score values

### 9. Result Card Details (`09-result-card.png`)
- **URL**: Results page, zoomed to one result card
- **What to show**: 
  - Close-up of a single result card
  - VM SKU name clearly visible (e.g., "Standard_D4s_v3")
  - Region clearly visible (e.g., "East US")
  - Placement score (e.g., "85")
  - Card styling and layout
- **What to hide**: Background information
- **Recommended size**: 600x400px
- **Tip**: Crop to show just one result card with all details clearly visible

### 10. Decision Guide (`10-decision-guide.png`)
- **What to create**: A visual guide showing score interpretation
- **Options**:
  - Option A: Create a simple graphic showing score ranges with color coding:
    - Green zone (90-100): Excellent
    - Blue zone (70-89): Good
    - Yellow zone (50-69): Moderate
    - Orange zone (30-49): Limited
    - Red zone (0-29): Very Limited
  - Option B: Screenshot of Excel/table showing the score interpretation
  - Option C: Screenshot of results page with annotations showing which scores are good/bad
- **Recommended size**: 800x600px
- **Tools**: PowerPoint, Draw.io, or any graphics editor

### 11. New Search (`11-new-search.png`)
- **URL**: Results page, bottom section
- **What to show**: 
  - "Check Another Configuration" button at bottom of results
  - Some result cards above for context
- **What to hide**: User information, subscription details
- **Recommended size**: 1000x800px
- **Tip**: Show the button with surrounding context

## General Screenshot Guidelines

### Preparation Steps

1. **Use a Clean Browser Profile**:
   - Open browser in incognito/private mode
   - Or create a test user profile
   - This prevents browser extensions and personal bookmarks from showing

2. **Clear Console and Network Tabs**:
   - Press F12 to open developer tools
   - Clear any error messages
   - Close dev tools before screenshot (unless specifically needed)

3. **Use Sample Data**:
   - Replace real subscription names in the code temporarily
   - Use generic usernames like "Demo User" or "Sample User"
   - Modify DOM elements using browser dev tools if needed

4. **Set Consistent Window Size**:
   - Use 1920x1080 browser window for consistency
   - Or use responsive mode in dev tools set to a specific size
   - Zoom level: 100%

### Data Sanitization Checklist

Before taking each screenshot, verify:
- ✅ No real email addresses visible
- ✅ No real subscription IDs visible
- ✅ No real user names visible
- ✅ No tenant IDs or client IDs visible
- ✅ No API keys or secrets visible
- ✅ No internal URLs or hostnames visible (use localhost or generic URLs)
- ✅ No proprietary subscription names visible

### Screenshot Tools

**Recommended Tools**:
- **Windows**: Snipping Tool (Windows + Shift + S) or Snip & Sketch
- **Browser Extensions**: Nimbus Screenshot, Awesome Screenshot
- **Full Page**: Use browser dev tools or extensions for full-page screenshots

**Post-Processing**:
- Use tools like Paint.NET, GIMP, or Photoshop to:
  - Blur sensitive information
  - Add annotations or highlights
  - Crop to desired size
  - Add arrows or callouts if needed

### Blur/Anonymization Techniques

1. **Browser Dev Tools Method**:
   ```javascript
   // In browser console, replace text before screenshot:
   document.querySelector('.subscription-name').textContent = 'Production Subscription';
   document.querySelector('.user-name').textContent = 'Demo User';
   ```

2. **CSS Blur Method**:
   ```javascript
   // Add blur to specific elements:
   document.querySelector('.sensitive-element').style.filter = 'blur(8px)';
   ```

3. **Post-Screenshot Editing**:
   - Use blur tool in image editor
   - Draw rectangles to hide information
   - Replace text with annotations

## Creating Mock Data

If you want to create completely sanitized screenshots, you can temporarily modify the application:

1. **Modify View to Show Sample Data**:
   - Edit `Views/Home/Index.cshtml` to hardcode sample subscription names
   - Edit `Views/Home/Results.cshtml` to show predefined results

2. **Use Browser Dev Tools**:
   - Modify DOM elements in real-time
   - Change text content before screenshot
   - Add/remove elements as needed

3. **Create a Test Branch**:
   - Create a `screenshots` branch
   - Modify data to be generic
   - Take all screenshots
   - Don't merge this branch

## Naming Convention

Save all screenshots with the exact names specified:
```
docs/images/01-signin.png
docs/images/02-dashboard.png
docs/images/03-subscription-dropdown.png
docs/images/04-availability-options.png
docs/images/05-sku-selection.png
docs/images/06-region-selection.png
docs/images/07-check-button.png
docs/images/08-results.png
docs/images/09-result-card.png
docs/images/10-decision-guide.png
docs/images/11-new-search.png
```

## Image Specifications

- **Format**: PNG (for better quality and transparency support)
- **Color Profile**: sRGB
- **Compression**: Optimize for web (but maintain readability)
- **Max File Size**: Keep under 500KB per image
- **Text Readability**: Ensure text is crisp and readable at 100% zoom

## Quality Checklist

Before finalizing screenshots:
- [ ] All images are clear and not blurry
- [ ] Text is readable at 100% zoom
- [ ] No sensitive data is visible
- [ ] Consistent styling across screenshots
- [ ] Proper dimensions for each image
- [ ] Images are optimized for web
- [ ] All 11 screenshots are captured
- [ ] Images are saved with correct filenames in `docs/images/` folder

## Next Steps

After capturing all screenshots:
1. Review each image for sensitive data
2. Optimize images for web (use tools like TinyPNG or ImageOptim)
3. Save images in `docs/images/` folder
4. Verify all image references in README.md work correctly
5. Commit images to repository
6. Push to GitHub

## Optional Enhancements

Consider adding:
- **Annotations**: Arrows, callouts, or highlights to draw attention to key features
- **Border/Shadow**: Add subtle borders or shadows to make screenshots stand out
- **Consistent Theme**: Use light or dark mode consistently across all screenshots
- **Mobile Screenshots**: If the app is responsive, consider adding mobile view screenshots
- **Animated GIFs**: Create short GIFs showing key workflows (sign in, select options, view results)
