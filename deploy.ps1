# Deployment script
$ErrorActionPreference = "Stop"

Write-Host "Cleaning previous build..."
Remove-Item -Path ./publish -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Publishing application..."
dotnet publish -c Release -o ./publish

Write-Host "Creating deployment package..."
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force

Write-Host "Deploying to Azure..."
az webapp deployment source config-zip --resource-group rg-spotplacementscore --name spotplacementscorechecker --src ./deploy.zip

Write-Host "Deployment complete!"
