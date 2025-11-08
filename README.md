# Azure Text Analytics: Customer Feedback Analyzer (Sentiment + Key Phrases)

This sample demonstrates how to use Azure Text Analytics to quickly understand customer satisfaction trends without manual effort. It:
- Detects sentiment (Positive, Neutral, Negative) for each feedback item.
- Extracts key phrases (e.g., product names, issues).
- Prints a concise console summary and writes an `analysis-result.json` report you can share in the repo.

## What it looks like

Example console output:
```
Customer Feedback Analysis
--------------------------------
[0] Love the new noise-cancelling headphones, but the battery dies after 3 hours.
    Sentiment: Mixed (Pos 0.64, Neu 0.06, Neg 0.30)
    Key Phrases: noise-cancelling headphones, battery

[1] Customer support resolved my issue quickly. Very satisfied!
    Sentiment: Positive (Pos 0.98, Neu 0.01, Neg 0.01)
    Key Phrases: Customer support, issue
```

And a shareable JSON file: `analysis-result.json`.

## Prerequisites

- Azure subscription with an Azure AI Language (Text Analytics) resource
  - Note the resource Endpoint and Key from the Azure portal.
- .NET 9 SDK

## Setup

1) Add the Azure Text Analytics SDK
```
dotnet add package Azure.AI.TextAnalytics
```

2) Configure credentials (do NOT hardcode secrets)

Set environment variables:
- `AZURE_TEXT_ANALYTICS_ENDPOINT` — e.g., `https://<your-resource>.cognitiveservices.azure.com/`
- `AZURE_TEXT_ANALYTICS_KEY` — your resource key

Examples:
- Windows (PowerShell):
```
[Environment]::SetEnvironmentVariable("AZURE_TEXT_ANALYTICS_ENDPOINT","https://<your-resource>.cognitiveservices.azure.com/","User")
[Environment]::SetEnvironmentVariable("AZURE_TEXT_ANALYTICS_KEY","<your-key>","User")
```
- macOS/Linux (bash):
```
export AZURE_TEXT_ANALYTICS_ENDPOINT="https://<your-resource>.cognitiveservices.azure.com/"
export AZURE_TEXT_ANALYTICS_KEY="<your-key>"
```

In Visual Studio you can set per-profile environment variables via: Project > Properties > Debug > Environment variables.

3) Build and run
```
dotnet build
dotnet run
```

You’ll see a per-feedback sentiment summary in the console and a generated `analysis-result.json` file.

## Customize

- Replace the `feedback` array in `Program.cs` with your real data source (CSV, database, etc.).
- Change `language` if your feedback isn’t English.
- For large volumes or multiple actions in one pass, consider long-running operations (LRO) with `StartAnalyzeActions`.

## Troubleshooting

- 401/403: check `AZURE_TEXT_ANALYTICS_KEY` and that you’re using the matching endpoint.
- 404: verify the endpoint URL (region and resource name).
- 429: you are being throttled; back off and retry.

## Repository contents

- `Program.cs` — main sample code.
- `analysis-result.json` — generated report.
- `README.md` — this file.
