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
