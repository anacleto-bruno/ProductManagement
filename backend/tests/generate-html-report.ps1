# HTML Coverage Report Generator for Product Management API

Write-Host "Generating HTML Coverage Report - Product Management API" -ForegroundColor Green

# Navigate to backend directory
#Set-Location "C:\Users\anacl\source\repos\Product Management\backend"

# Create directories
$coverageDir = "coverage-results"
$htmlDir = "coverage-html"

if (Test-Path $coverageDir) { Remove-Item $coverageDir -Recurse -Force }
if (Test-Path $htmlDir) { Remove-Item $htmlDir -Recurse -Force }

New-Item -ItemType Directory -Path $coverageDir | Out-Null
New-Item -ItemType Directory -Path $htmlDir | Out-Null

Write-Host "Running tests with coverage collection..." -ForegroundColor Yellow

# Run tests with coverage
dotnet test ProductManagement.UnitTests.csproj `
    --collect:"XPlat Code Coverage" `
    --results-directory $coverageDir `
    --logger "console;verbosity=normal" `
    --configuration Release `
    --settings code-coverage.runsettings

if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Find coverage files
$coverageFiles = Get-ChildItem -Path $coverageDir -Recurse -Filter "coverage.cobertura.xml"

if ($coverageFiles.Count -eq 0) {
    Write-Host "No coverage files found!" -ForegroundColor Red
    exit 1
}

$coverageFile = $coverageFiles[0]
Write-Host "Found coverage file: $($coverageFile.Name)" -ForegroundColor Green

# Generate HTML report
Write-Host "Generating HTML report..." -ForegroundColor Yellow

reportgenerator `
    -reports:"$($coverageFile.FullName)" `
    -targetdir:"$htmlDir" `
    -reporttypes:"Html;HtmlSummary;Badges" `
    -sourcedirs:"..\api" `
    -filefilters:"-**/Migrations/**" `
    -assemblyfilters:"+ProductManagement" `
    -classfilters:"-ProductManagement.DirectFunctionExecutor;-ProductManagement.FunctionExecutorAutoStartup;-ProductManagement.FunctionExecutorHostBuilderExtensions;-ProductManagement.FunctionMetadataProviderAutoStartup;-ProductManagement.GeneratedFunctionMetadataProvider;-ProductManagement.WorkerExtensionStartupCodeExecutor;-ProductManagement.WorkerHostBuilderFunctionMetadataProviderExtension" `
    -title:"Product Management API - Test Coverage (Excluding Auto-Generated Code)"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to generate HTML report!" -ForegroundColor Red
    exit 1
}

# Parse coverage data
[xml]$coverage = Get-Content $coverageFile.FullName
$lineRate = [math]::Round([decimal]$coverage.coverage.'line-rate' * 100, 2)
$branchRate = [math]::Round([decimal]$coverage.coverage.'branch-rate' * 100, 2)

Write-Host "`nCoverage Summary:" -ForegroundColor Green
Write-Host "   Line Coverage:   $lineRate%" -ForegroundColor White
Write-Host "   Branch Coverage: $branchRate%" -ForegroundColor White

Write-Host "`nReport Locations:" -ForegroundColor Green
Write-Host "   HTML Report: $htmlDir\index.html" -ForegroundColor Cyan
Write-Host "   Summary:     $htmlDir\summary.html" -ForegroundColor Cyan

# Open report
Write-Host "`nOpening HTML report..." -ForegroundColor Green
$htmlReportPath = Join-Path $htmlDir "index.html"
Start-Process $htmlReportPath

Write-Host "HTML coverage report generated successfully!" -ForegroundColor Green