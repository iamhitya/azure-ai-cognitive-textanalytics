using Azure;
using Azure.AI.TextAnalytics;
using System.Text.Json;

var endpoint = Environment.GetEnvironmentVariable("AZURE_TEXT_ANALYTICS_ENDPOINT"); //e.g. https://<your-resource-name>.cognitiveservices.azure.com/
var apiKey = Environment.GetEnvironmentVariable("AZURE_TEXT_ANALYTICS_KEY");

if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine("Missing configuration. Set AZURE_TEXT_ANALYTICS_ENDPOINT and AZURE_TEXT_ANALYTICS_KEY environment variables.");
    Environment.Exit(1);
}

var client = new TextAnalyticsClient(new Uri(endpoint!), new AzureKeyCredential(apiKey!));

// Replace with real feedback source (file, DB, etc.)
string[] feedback =
{
    "Love the new noise-cancelling headphones, but the battery dies after 3 hours.",
    "Customer support resolved my issue quickly. Very satisfied!",
    "The app crashes when I try to upload photos. Please fix.",
    "Delivery was on time. Packaging was damaged though."
};

const string language = "en";

try
{
    // Get responses then access .Value collections
    var sentimentResponse = client.AnalyzeSentimentBatch(feedback, language);
    var sentimentResults = sentimentResponse.Value; // AnalyzeSentimentResultCollection
    var keyPhraseResponse = client.ExtractKeyPhrasesBatch(feedback, language);
    var keyPhraseResults = keyPhraseResponse.Value; // ExtractKeyPhrasesResultCollection

    var combinedResults = new List<object>(feedback.Length);

    Console.WriteLine("Customer Feedback Analysis");
    Console.WriteLine(new string('-', 32));

    for (int i = 0; i < feedback.Length; i++)
    {
        var s = sentimentResults[i];
        var k = keyPhraseResults[i];

        if (s.HasError || k.HasError)
        {
            var errorMsg = $"{(s.HasError ? s.Error.Message : string.Empty)} {(k.HasError ? k.Error.Message : string.Empty)}".Trim();
            Console.WriteLine($"[{i}] Error analyzing feedback: {errorMsg}");
            combinedResults.Add(new { Index = i, Text = feedback[i], Error = errorMsg });
            continue;
        }

        var docSentiment = s.DocumentSentiment;
        var scores = docSentiment.ConfidenceScores;

        Console.WriteLine($"[{i}] {feedback[i]}");
        Console.WriteLine($"    Sentiment: {docSentiment.Sentiment} (Pos {scores.Positive:F2}, Neu {scores.Neutral:F2}, Neg {scores.Negative:F2})");
        Console.WriteLine($"    Key Phrases: {string.Join(", ", k.KeyPhrases)}");

        combinedResults.Add(new
        {
            Index = i,
            Text = feedback[i],
            Sentiment = docSentiment.Sentiment.ToString(),
            Confidence = new { scores.Positive, scores.Neutral, scores.Negative },
            KeyPhrases = k.KeyPhrases
        });
    }

    var json = JsonSerializer.Serialize(combinedResults, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText("analysis-result.json", json);
    Console.WriteLine();
    Console.WriteLine("Wrote analysis-result.json");
}
catch (RequestFailedException ex)
{
    Console.Error.WriteLine($"Azure request failed: {ex.Status} {ex.ErrorCode} - {ex.Message}");
    Environment.Exit(2);
}
