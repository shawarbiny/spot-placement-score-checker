# Azure Spot VM Placement Score Website

This ASP.NET Core MVC web application allows users to authenticate with Microsoft Entra ID and check Azure Spot VM placement scores across different VM SKUs and regions.

## Features

- **Entra ID Authentication**: Secure authentication using Microsoft Entra ID (Azure AD)
- **Subscription Selection**: Dropdown to select from available Azure subscriptions
- **VM SKU Selection**: Checkboxes to select up to 5 VM SKUs
- **Region Selection**: Checkboxes to select up to 3 Azure regions
- **Spot Placement Score Check**: Query Azure Compute API to get spot placement scores
- **Results Display**: View placement scores and availability for each SKU/region combination

## Prerequisites

1. .NET 8.0 SDK or later
2. Azure subscription
3. Azure AD application registration

## Setup Instructions

### 1. Register an Application in Entra ID

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Microsoft Entra ID** > **App registrations** > **New registration**
3. Configure:
   - **Name**: Spot Placement Score Website
   - **Supported account types**: Accounts in this organizational directory only
   - **Redirect URI**: Web - `https://localhost:7001/signin-oidc`
4. After creation, note down:
   - **Application (client) ID**
   - **Directory (tenant) ID**
5. Go to **Certificates & secrets** > **New client secret**
   - Create a new secret and save the **Value**
6. Go to **API permissions**:
   - Add **Azure Service Management** > **user_impersonation**
   - Grant admin consent

### 2. Configure the Application

Edit `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

Replace:
- `YOUR_TENANT_ID` with your Directory (tenant) ID
- `YOUR_CLIENT_ID` with your Application (client) ID
- `YOUR_CLIENT_SECRET` with your client secret value

### 3. Grant Azure Permissions

Ensure the signed-in user has at least **Reader** role on the Azure subscriptions they want to query.

### 4. Run the Application

```powershell
dotnet restore
dotnet build
dotnet run
```

The application will start at `https://localhost:7001`

## Usage

1. Navigate to `https://localhost:7001`
2. Sign in with your Entra ID credentials
3. Select an Azure subscription from the dropdown
4. The page will load available VM SKUs and regions
5. Select up to 5 VM SKUs and up to 3 regions
6. Click "Check Spot Placement Scores"
7. View the results showing spot placement scores for each combination

## User Guide

> **Note**: This user guide includes references to screenshots that illustrate each step. To capture these screenshots, please follow the detailed instructions in [`docs/SCREENSHOT_GUIDE.md`](docs/SCREENSHOT_GUIDE.md). All screenshots must be sanitized to remove sensitive data (email addresses, subscription IDs, user names, etc.) before being added to the repository.

### Quick Navigation
- [Step 1: Sign In](#step-1-sign-in)
- [Step 2: Main Dashboard](#step-2-main-dashboard)
- [Step 3: Select Your Azure Subscription](#step-3-select-your-azure-subscription)
- [Step 4: Configure Availability Options](#step-4-configure-availability-options)
- [Step 5: Select VM SKUs](#step-5-select-vm-skus)
- [Step 6: Select Azure Regions](#step-6-select-azure-regions)
- [Step 7: Check Spot Placement Scores](#step-7-check-spot-placement-scores)
- [Step 8: View Results](#step-8-view-results)
- [Step 9: Make Deployment Decisions](#step-9-make-deployment-decisions)
- [Step 10: Check Another Configuration](#step-10-check-another-configuration)
- [Common Workflows](#common-workflows)
- [Tips for Best Results](#tips-for-best-results)
- [Troubleshooting Common Issues](#troubleshooting-common-issues)

### Step 1: Sign In

When you first access the application, you'll be redirected to the Microsoft Entra ID login page.

![Sign In Page](docs/images/01-signin.png)

- Enter your organizational email address
- Click **Next** and complete authentication
- After successful authentication, you'll be redirected to the main application

### Step 2: Main Dashboard

Once authenticated, you'll see the main dashboard with all available options.

![Main Dashboard](docs/images/02-dashboard.png)

The dashboard includes:
- **Subscription Selector**: Dropdown showing your accessible Azure subscriptions
- **Availability Zones Option**: Checkbox to include availability zones in the placement score calculation
- **Desired Count**: Number of VMs you plan to deploy (1-100)
- **VM SKU Selection**: Checkboxes for available VM sizes (max 5 selections)
- **Region Selection**: Checkboxes for Azure regions (max 3 selections)

### Step 3: Select Your Azure Subscription

Click the **Select Subscription** dropdown to view all subscriptions you have access to.

![Subscription Selection](docs/images/03-subscription-dropdown.png)

- Choose the subscription where you plan to deploy Spot VMs
- The page will automatically load available VM SKUs and regions for that subscription

### Step 4: Configure Availability Options

Configure your deployment preferences:

![Availability Options](docs/images/04-availability-options.png)

- **Use Availability Zones**: Check this box if you want to distribute VMs across availability zones for higher availability
- **Desired Count**: Enter the number of VM instances you plan to deploy (between 1 and 100)

These parameters help Azure calculate more accurate placement scores based on your specific needs.

### Step 5: Select VM SKUs

Choose the VM SKUs you want to evaluate. You can select up to 5 different SKUs.

![VM SKU Selection](docs/images/05-sku-selection.png)

Popular SKU families include:
- **Standard_D series**: General purpose VMs (balanced CPU-to-memory ratio)
- **Standard_E series**: Memory-optimized VMs (high memory-to-CPU ratio)
- **Standard_F series**: Compute-optimized VMs (high CPU-to-memory ratio)
- **Standard_B series**: Burstable VMs (cost-effective for workloads with variable CPU usage)

**Tip**: Select SKUs that meet your application requirements. If unsure, start with Standard_D series VMs.

### Step 6: Select Azure Regions

Choose the regions where you want to check Spot VM availability. You can select up to 3 regions.

![Region Selection](docs/images/06-region-selection.png)

Consider these factors when selecting regions:
- **Latency**: Choose regions close to your users
- **Compliance**: Select regions that meet your data residency requirements
- **Cost**: Pricing varies by region
- **Availability**: Some SKUs may not be available in all regions

**Tip**: If you're flexible on location, select multiple regions to compare placement scores.

### Step 7: Check Spot Placement Scores

After making your selections, click the **Check Spot Placement Scores** button.

![Check Button](docs/images/07-check-button.png)

The form will validate:
- At least one VM SKU is selected (max 5)
- At least one region is selected (max 3)
- Desired count is within valid range (1-100)

### Step 8: View Results

The results page displays spot placement scores for each SKU and region combination.

![Results Page](docs/images/08-results.png)

#### Understanding the Results

Each result card shows:

![Result Card Details](docs/images/09-result-card.png)

1. **VM SKU Name**: The specific VM size evaluated
2. **Region**: Azure region where the score was calculated
3. **Placement Score**: A numerical value indicating spot capacity availability
4. **Score Interpretation**:
   - **Higher scores** (closer to 100): Better availability, lower eviction risk
   - **Lower scores**: Limited capacity, higher eviction risk
   - **No score available**: SKU may not support Spot VMs in this region

#### Placement Score Guide

| Score Range | Availability | Recommendation |
|-------------|-------------|----------------|
| 90-100 | Excellent | Highly recommended - Very low eviction risk |
| 70-89 | Good | Recommended - Low eviction risk |
| 50-69 | Moderate | Consider with caution - Moderate eviction risk |
| 30-49 | Limited | Not recommended - High eviction risk |
| 0-29 | Very Limited | Avoid - Very high eviction risk |
| N/A | Unavailable | SKU doesn't support Spot or no capacity |

### Step 9: Make Deployment Decisions

Based on the results:

![Decision Making](docs/images/10-decision-guide.png)

**Best Practices**:
- ✅ Choose combinations with scores above 70 for production workloads
- ✅ Consider multiple regions for better availability
- ✅ Have backup SKUs in case your first choice has low scores
- ✅ Re-check scores periodically as capacity changes
- ⚠️ Avoid SKUs with consistently low scores (<50)
- ⚠️ Test your application's behavior during VM evictions

### Step 10: Check Another Configuration

To evaluate different options:

![New Search](docs/images/11-new-search.png)

- Click **Check Another Configuration** button at the bottom of results
- You'll return to the main page with your subscription still selected
- Modify your SKU and region selections
- Run a new placement score check

### Common Workflows

#### Workflow 1: Find Best Region for a Specific SKU
1. Select your subscription
2. Choose **one VM SKU**
3. Select **multiple regions** (up to 3)
4. Compare scores across regions
5. Deploy in the region with the highest score

#### Workflow 2: Find Best SKU for a Specific Region
1. Select your subscription
2. Choose **multiple VM SKUs** (up to 5)
3. Select **one region**
4. Compare scores across SKUs
5. Choose the SKU with the best score that meets your requirements

#### Workflow 3: Comprehensive Evaluation
1. Select your subscription
2. Choose **multiple VM SKUs** (up to 5)
3. Select **multiple regions** (up to 3)
4. Review all combinations (up to 15 results)
5. Identify the best SKU-region pairs

### Tips for Best Results

- **Timing Matters**: Spot capacity fluctuates. Check scores before each deployment
- **Enable Availability Zones**: Improves fault tolerance and may affect scores
- **Adjust Desired Count**: Higher VM counts may result in lower scores
- **Geographic Flexibility**: Being flexible with region selection increases your chances of finding good capacity
- **SKU Alternatives**: Always have backup SKU options in case primary choices have low scores
- **Automation**: Consider integrating the API into your deployment pipelines for automated score checking

### Troubleshooting Common Issues

#### Issue: No VM SKUs or Regions Appear

**Solution**:
- Verify you selected a subscription from the dropdown
- Check that your account has Reader role on the subscription
- Try refreshing the page
- Check browser console for errors (F12)

#### Issue: "Access Denied" Error

**Solution**:
- Ensure you have at least Reader role on the selected subscription
- Verify your Azure AD account is active
- Contact your Azure administrator to grant appropriate permissions

#### Issue: All Scores Show "N/A"

**Possible Reasons**:
- Selected SKUs don't support Spot VMs
- No spot capacity currently available in selected regions
- API throttling (wait a few minutes and try again)
- Try different SKU or region combinations

#### Issue: Scores Are Very Low

**Considerations**:
- Spot capacity is limited at the current time
- Selected region may have high demand
- Try different regions or SKUs
- Consider deploying during off-peak hours
- Check Azure status page for any regional issues

## Architecture

### Security Features
- Uses **Microsoft.Identity.Web** for secure authentication
- Token-based authentication with Azure Management API
- No hardcoded credentials (uses delegated permissions)
- HTTPS enforcement
- Anti-forgery tokens on forms

### Key Components

- **Program.cs**: Configures Entra ID authentication and dependency injection
- **HomeController.cs**: Handles form submission and orchestrates service calls
- **AzureResourceService.cs**: Queries Azure Resource Manager for subscriptions, SKUs, and regions
- **SpotPlacementScoreService.cs**: Calls Azure Compute API to check spot placement scores
- **Views**: Razor views for form and results display
- **spotplacement.js**: Client-side validation and dynamic form behavior

### API Endpoints Used

- `GET /subscriptions` - List subscriptions
- `GET /subscriptions/{id}/providers/Microsoft.Compute/skus` - List VM SKUs
- `GET /subscriptions/{id}/locations` - List regions
- `POST /subscriptions/{id}/providers/Microsoft.Compute/locations/{region}/spotPlacementScores` - Check spot placement score

## Troubleshooting

### Authentication Issues
- Verify your Entra ID app registration settings
- Ensure redirect URIs match exactly
- Check that API permissions are granted and admin consent is provided

### API Errors
- Ensure your account has Reader role on subscriptions
- Check that the subscription is active
- Verify network connectivity to Azure endpoints

### No SKUs or Regions Showing
- Check browser console for JavaScript errors
- Verify the subscription has access to VM resources
- Try a different subscription

## Security Best Practices Implemented

✅ Uses delegated user authentication (no service principals hardcoded)  
✅ Secure token acquisition and caching  
✅ HTTPS enforcement  
✅ Anti-forgery token validation  
✅ Proper error handling and logging  
✅ Input validation (max 5 SKUs, max 3 regions)  
✅ Client-side and server-side validation  

## License

This application is provided as-is for demonstration purposes.
